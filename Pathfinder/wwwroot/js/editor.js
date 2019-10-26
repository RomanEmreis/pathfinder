"use strict";
var connection = new signalR.HubConnectionBuilder()
    .withUrl("/liveeditor")
    .build();

connection.on('CodeChanged', async (projectName, delta) => await codeChanged(projectName, delta));
connection.on('ProjectCompiled', async (projectName, buildResult) => await projectCompiled(projectName, buildResult));
connection.on('StepOver', async buildContext => await onStepOver(buildContext));
connection.on('StepInto', async buildContext => await onStepInto(buildContext));
connection.on('SetBreakpoint', async range => await onSetBreakpoint(range));
connection.on('RemoveBreakpoint', async (range, breakpoint) => await onRemoveBreakpoint(range, breakpoint));

connection
    .start()
    .then(async () => await connectionEstablished())
    .catch(err => onError(err));

var silent = false;
var bp     = [];

var editor = null;
var output = null;
var decorations = [];
var currentDebuggingLine = 0;

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

    editor.onDidChangeModelContent(textChanged);
    editor.onMouseDown(tryHandleBreakpoint);

    editor.addCommand(monaco.KeyCode.F5, buildProject);
    editor.addCommand(monaco.KeyCode.F10, stepOver);
    editor.addCommand(monaco.KeyCode.F11, stepInto);

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

const textChanged = e => {
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
        breakpoints: bp,
        currentLine: currentDebuggingLine,
        isDebugging: bp.length > 0,
        isRunToNextBreakpoint: true
    };

    connection.invoke('CompileProject', buildContext);
}

const stepOver = () => {
    let code = editor.getValue();
    
    let buildContext = {
        projectName: project.name,
        sourceCode: code,
        breakpoints: bp,
        currentLine: currentDebuggingLine,
        isDebugging: true,
        isRunToNextBreakpoint: false
    };

    connection.invoke('StepOver', buildContext);
}

const onStepOver = async buildContext => {
    currentDebuggingLine = ++buildContext.currentLine;
    let currentRange = new monaco.Range(currentDebuggingLine, 1, currentDebuggingLine, 1);

    removeDebuggingLine()
    setDebuggingLine(currentRange);

    await writeResult(buildContext);
};

const stepInto = () => {
    let code = editor.getValue();
    
    let buildContext = {
        projectName: project.name,
        sourceCode: code,
        breakpoints: bp,
        currentLine: currentDebuggingLine,
        isDebugging: true,
        isRunToNextBreakpoint: false
    };

    connection.invoke('StepInto', buildContext);
}

const onStepInto = async buildContext => {

};

const projectCompiled = async (projectName, buildResult) => {
    let buildMessage = buildResult.success
        ? 'Build project ' + projectName + ' successfully complete'
        : 'Build project ' + projectName + ' complete with errors';

    console.log(buildMessage);
    writeToOutput(buildMessage);

    await writeResult(buildResult);
};

const writeResult = async buildResult => {
    if (!buildResult.resultMessage) return;

    writeToOutput(buildResult.resultMessage);

    if (buildResult.success) {
        console.log(buildResult.resultMessage);
    } else {
        console.error(buildResult.resultMessage);
    }
};

const tryHandleBreakpoint = e => {
    let targetName = e.target.element.className;
    if (targetName === 'line-numbers') {
        let currentRange = e.target.range;
        let model        = editor.getModel();

        var decorationsInRange = model.getDecorationsInRange(currentRange);
        var existedBreakpoint  = decorationsInRange.find(isBreakpoint);

        if (existedBreakpoint) {
            removeBreakpoint(currentRange, existedBreakpoint)
        } else {
            setBreakpoint(currentRange);
        }
    }
};

const isBreakpoint = (element, index, array) => element.options.linesDecorationsClassName === 'debugging-breakpoint';

const isDebuggingLine = (element, index, array) => element.options.linesDecorationsClassName === 'debugging-line';

const setBreakpoint = range => {
    onSetBreakpoint(range);
    connection.invoke('SetBreakpoint', project.name, range);
};

const onSetBreakpoint = range => {
    decorations = editor.deltaDecorations([], [createBreakpoint(range)]);
    bp.push(range.startLineNumber);
};

const setDebuggingLine = range => {
    decorations = editor.deltaDecorations([], [createDebuggingLine(range)]);
};

const removeBreakpoint = (range, breakpoint) => {
    onRemoveBreakpoint(range, breakpoint)
    connection.invoke('RemoveBreakpoint', project.name, range, breakpoint);
};

const onRemoveBreakpoint = (range, breakpoint) => {
    var breakpoints     = [...decorations];
    var breakpointIndex = breakpoints.indexOf(breakpoint.id);
    if (breakpointIndex > -1) {
        breakpoints.splice(breakpointIndex, 1);
    }

    decorations = editor.deltaDecorations(decorations, breakpoints);

    var indexOfBp = bp.indexOf(range.startLineNumber);
    if (indexOfBp > -1) {
        bp.splice(indexOfBp, 1);
    }
};

const removeDebuggingLine = () => {
    decorations = editor.deltaDecorations(decorations, []);
};

const createBreakpoint = range => {
    return {
        range: range,
        options: {
            isWholeLine: false,
            linesDecorationsClassName: 'debugging-breakpoint'
        }
    };
};

const createDebuggingLine = range => {
    return {
        range: range,
        options: {
            isWholeLine: true,
            className: 'debugging-line',
            linesDecorationsClassName: 'debugging-line-point'
        }
    };
};