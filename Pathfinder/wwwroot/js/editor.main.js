"use strict";
var eventHandler = new EditorEventHandler(project);

const init = () => {
    require(['vs/editor/editor.main'], eventHandler.subscribe);
    window.removeEventListener("load", init);
};

const dispose = async () => {
    eventHandler.dispose();
    window.removeEventListener("beforeunload", dispose);
};

window.addEventListener("load", init);
window.addEventListener("beforeunload", dispose);