"use strict";

class Output {
    constructor() {
        this.output = monaco.editor.create(document.getElementById('output'), {
            readOnly: false
        });
        this.output.layout();
    }

    writeResult = async buildResult => {
        if (!buildResult.resultMessage) return;

        this.writeToOutput(buildResult.resultMessage);

        if (buildResult.success) {
            console.log(buildResult.resultMessage);
        } else {
            console.error(buildResult.resultMessage);
        }
    }

    writeToOutput = text => {
        var line  = this.output.getPosition();
        var range = new monaco.Range(line.lineNumber, 1, line.lineNumber, 1);
        var id    = { major: 1, minor: 1 };
        var op    = { identifier: id, range: range, text: text + '\n', forceMoveMarkers: true };

        this.output.executeEdits("my-source", [op]);
    };
}