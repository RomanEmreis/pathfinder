var connection   = new signalR.HubConnectionBuilder()
    .withUrl("/liveeditor")
    .build();

connection.on('CodeChanged', (projectName, delta) => codeChanged(projectName, delta));
connection.on('ProjectCompiled', (projectName, buildResult) => projectCompiled(projectName, buildResult));

connection
    .start()
    .then(() => connectionEstablished())
    .catch(err => onError(err));

var silent = false;
var editor = ace.edit("editor");

editor.setTheme("ace/theme/tomorrow");
editor.session.setMode("ace/mode/csharp");

editor.commands.addCommand({
    name: 'compileCommand',
    bindKey: { win: 'F5' },
    exec: async editor => {
        let projectName = await getCurrentProjectName();
        let code        = editor.getValue();

        connection.invoke('CompileProject', projectName, code);
    },
    readOnly: false
});

editor.session.on('change', delta => textChanged(delta));
editor.session.selection.on('changeSelection', e => selectionChanged(e));
editor.session.selection.on('changeCursor', e => cursorChanged(e));

var output = ace.edit("output");

output.setTheme("ace/theme/tomorrow");
output.session.setMode("ace/mode/text");
output.setReadOnly(true);

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