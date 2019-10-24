"use strict";
var connection = new signalR.HubConnectionBuilder()
    .withUrl("/liveeditor")
    .build();

connection.on('CodeChanged', async (projectName, delta) => await codeChanged(projectName, delta));
connection.on('ProjectCompiled', async (projectName, buildResult) => await projectCompiled(projectName, buildResult));

connection
    .start()
    .then(async () => await connectionEstablished())
    .catch(err => onError(err));

var silent = false;
var bp     = [];

var editor = null;
var output = null;

const init = () => {
    require(['vs/editor/editor.main'], createEditor);
    window.removeEventListener("load", init);
};

const dispose = async () => {
    connection.invoke('CollaboratorLeavesProject', project.name);
    window.removeEventListener("beforeunload", dispose);
};

window.addEventListener("load", init);
window.addEventListener("beforeunload", dispose);

const createEditor = () => {
    editor = monaco.editor.create(document.getElementById('editor'), {
        value: project.code,
        language: 'csharp'
    });

    editor.onDidChangeModelContent(async e => await textChanged(e));

    editor.addCommand(monaco.KeyCode.F5, () => buildProject());

    editor.layout();

    output = monaco.editor.create(document.getElementById('output'), {
        readOnly: false
    });
    output.layout();
};

const connectionEstablished = async () => {
    connection.invoke('CollaboratorJoinsProject', project.name);
};

const codeChanged = async (projectName, changes) => {
    silent = true;

    let model = editor.getModel();
    model.applyEdits(changes);

    silent = false;
};

const textChanged = async e => {
    if (!silent) {
        connection.invoke('CodeChanged', project.name, e.changes);
    }
};

const writeToOutput = text => {
    var line  = editor.getPosition();
    var range = new monaco.Range(line.lineNumber, 1, line.lineNumber, 1);
    var id    = { major: 1, minor: 1 };
    var op = { identifier: id, range: range, text: text + '\n', forceMoveMarkers: true };

    output.executeEdits("my-source", [op]);
};

const buildProject = () => {
    let code = editor.getValue();

    let buildContext = {
        projectName: project.name,
        sourceCode: code,
        breakpoints: bp
    };

    connection.invoke('CompileProject', buildContext);
}

const projectCompiled = async (projectName, buildResult) => {
    let buildMessage = buildResult.success
        ? 'Build project ' + projectName + ' successfully complete'
        : 'Build project ' + projectName + ' complete with errors';

    console.log(buildMessage);

    writeToOutput(buildMessage);
    writeToOutput(buildResult.resultMessage);

    if (buildResult.success) {
        console.log(buildResult.resultMessage);
    } else {
        console.error(buildResult.resultMessage);
    }
};

//const setBreakpoint = e => {
//    let target = e.domEvent.target;

//    if (target.className.indexOf("ace_gutter-cell") == -1) return;
//    if (!editor.isFocused()) return;

//    if (e.clientX > 25 + target.getBoundingClientRect().left) return;

//    var breakpoints = e.editor.session.getBreakpoints(row, 0);
//    var row         = e.getDocumentPosition().row;

//    if (typeof breakpoints[row] === typeof undefined) {
//        bp.push(row);
//        e.editor.session.setBreakpoint(row);
//    } else {
//        var index = bp.indexOf(row);
//        if (index > -1) {
//            bp.splice(index, 1);
//        }
//        e.editor.session.clearBreakpoint(row);
//    }

//    e.stop();
//};