var connection   = new signalR.HubConnectionBuilder()
    .withUrl("/liveeditor")
    .build();

connection.on('CodeChanged', (projectName, delta) => codeChanged(projectName, delta));

connection
    .start()
    .then(() => connectionEstablished())
    .catch(err => onError(err));

var silent = false;
var editor = ace.edit("editor");

editor.setTheme("ace/theme/tomorrow");
editor.session.setMode("ace/mode/csharp");

editor.session.on('change', delta => textChanged(delta));
editor.session.selection.on('changeSelection', e => selectionChanged(e));
editor.session.selection.on('changeCursor', e => cursorChanged(e));

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

const textChanged = async delta => {
    if (!silent) {
        let projectName = await getCurrentProjectName();
        connection.invoke('CodeChanged', projectName, delta);
    }
};

const selectionChanged = e => { };

const cursorChanged = e => { };