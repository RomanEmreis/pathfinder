var connection = new signalR.HubConnectionBuilder()
    .withUrl("/liveeditor")
    .build();

connection
    .start()
    .catch(err => console.error(err.toString()));

var silent = false;
var editor = ace.edit("editor");

editor.setTheme("ace/theme/tomorrow");
editor.session.setMode("ace/mode/csharp");

connection.on('CodeChanged', (delta) => {
    silent = true;

    let deltas = [delta];
    editor.session.doc.applyDeltas(deltas);

    silent = false;
});

editor.session.on('change', (delta) => {
    if (!silent) {
        connection.invoke('CodeChanged', delta);
    }
});

editor.session.selection.on('changeSelection', (e) => {
});

editor.session.selection.on('changeCursor', (e) => {
});