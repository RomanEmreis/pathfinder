var connection   = new signalR.HubConnectionBuilder()
    .withUrl("/liveeditor")
    .build();

connection.on('CodeChanged', (projectName, delta) => codeChanged(projectName, delta));
connection.on('ProjectCompiled', (projectName, buildResult) => projectCompiled(projectName, buildResult));

connection
    .start()
    .then(() => connectionEstablished())
    .catch(err => onError(err));

var silent      = false;
var bp          = [];
var editor      = ace.edit("editor");

editor.setTheme("ace/theme/tomorrow");
editor.session.setMode("ace/mode/csharp");
document.getElementById('editor').style.fontSize = '14px';

editor.commands.addCommand({
    name: 'compileCommand',
    bindKey: { win: 'F5' },
    exec: async editor => {
        let projectName  = await getCurrentProjectName();
        let code         = editor.getValue();
        /*let breakpoints  = editor.session.getBreakpoints();*/
        
        let buildContext = {
            projectName: projectName,
            sourceCode: code,
            breakpoints: bp
        };

        connection.invoke('CompileProject', buildContext);
    },
    readOnly: false
});

editor.session.on('change', delta => textChanged(delta));
editor.on("guttermousedown", e => setBreakpoint(e));
editor.session.selection.on('changeSelection', e => selectionChanged(e));
editor.session.selection.on('changeCursor', e => cursorChanged(e));

var output = ace.edit("output");

output.setTheme("ace/theme/tomorrow");
output.session.setMode("ace/mode/text");
output.setReadOnly(true);
document.getElementById('output').style.fontSize = '14px';

$(window).on("beforeunload", () => editorClosed())

const connectionEstablished = async () => {
    let projectName = await getCurrentProjectName();
    connection.invoke('CollaboratorJoinsProject', projectName);
};

const editorClosed = async () => {
    let projectName = await getCurrentProjectName();
    connection.invoke('CollaboratorLeavesProject', projectName);
};

const getCurrentProjectName = async () => {
    let searchParams = new URLSearchParams(window.location.search);
    return searchParams.get('projectName');
};

const codeChanged = (projectName, delta) => {
    silent = true;
    editor.session.doc.applyDeltas([delta]);
    silent = false;
};

const projectCompiled = (projectName, buildResult) => {
    let buildMessage = buildResult.success
        ? 'Build project ' + projectName + ' successfully complete'
        : 'Build project ' + projectName + ' complete with errors';

    console.log(buildMessage);
    output.insert(buildMessage + '\n');
    output.insert(buildResult.resultMessage + '\n');

    if (buildResult.success) {
        console.log(buildResult.resultMessage);
    } else {
        console.error(buildResult.resultMessage);
    }
};

const textChanged = async delta => {
    if (!silent) {
        let projectName = await getCurrentProjectName();
        connection.invoke('CodeChanged', projectName, delta);
    }
};

const selectionChanged = e => { };

const cursorChanged = e => { };

const setBreakpoint = e => {
    let target = e.domEvent.target;

    if (target.className.indexOf("ace_gutter-cell") == -1) return;
    if (!editor.isFocused()) return;

    if (e.clientX > 25 + target.getBoundingClientRect().left) return;

    var breakpoints = e.editor.session.getBreakpoints(row, 0);
    var row         = e.getDocumentPosition().row;

    if (typeof breakpoints[row] === typeof undefined) {
        bp.push(row);
        e.editor.session.setBreakpoint(row);
    } else {
        var index = bp.indexOf(row);
        if (index > -1) {
            bp.splice(index, 1);
        }
        e.editor.session.clearBreakpoint(row);
    }

    e.stop();
};