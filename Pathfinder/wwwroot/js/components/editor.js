"use strict";

class CodeEditor {
    static breakpointCss     = 'debugging-breakpoint';
    static debuggingLineCss  = 'debugging-line';
    static debuggingPointCss = 'debugging-line-point';

    constructor(connection, project) {
        this.connection = connection;
        this.project = project;

        this.editor = monaco.editor.create(document.getElementById('editor'), {
            value: this.project.code,
            language: 'csharp'
        });

        this.editor.onDidChangeModelContent(this.textChanged);
        this.editor.onMouseDown(this.tryHandleBreakpoint);

        this.editor.addCommand(monaco.KeyCode.F5,  this.buildProject);
        this.editor.addCommand(monaco.KeyCode.F9,  () => this.handleBreakpoint(null));
        this.editor.addCommand(monaco.KeyCode.F10, this.stepOver);
        this.editor.addCommand(monaco.KeyCode.F11, this.stepInto);

        this.editor.layout();

        this.isSilent = false;
        this.breakpoints = [];
        this.decorations = [];
        this.currentDebuggingLine = 0;
    }

    textChanged = e => {
        if (!this.isSilent) {
            this.connection.invoke('CodeChanged', this.project.name, e.changes);
        }
    }

    buildProject = () => {
        let buildContext = this.getBuildingContext(this.breakpoints.length > 0, true);
        this.connection.invoke('CompileProject', buildContext);
    }

    stepOver = () => {
        let buildContext = this.getBuildingContext(true, false);
        this.connection.invoke('StepOver', buildContext);
    }

    stepInto = () => {
        let buildContext = this.getBuildingContext(true, false);
        this.connection.invoke('StepInto', buildContext);
    }

    getBuildingContext = (isDebugging, isRunToNextBreakpoint) => {
        let code = this.editor.getValue();
        return {
            projectName: this.project.name,
            sourceCode: code,
            breakpoints: this.breakpoints,
            currentLine: this.currentDebuggingLine,
            isDebugging: isDebugging,
            isRunToNextBreakpoint: isRunToNextBreakpoint
        };
    }

    tryHandleBreakpoint = e => {
        let targetName = e.target.element.className;
        if (targetName === 'line-numbers') {
            this.handleBreakpoint(e.target.range);
        }
    }

    handleBreakpoint = range => {
        let model = this.editor.getModel();

        if (!range) {
            let position = this.editor.getPosition();
            range = this.createRangeByLine(position.lineNumber);
        }

        var decorationsInRange = model.getDecorationsInRange(range);
        var existedBreakpoint  = decorationsInRange.find(this.isBreakpoint);

        if (existedBreakpoint) {
            this.removeBreakpoint(range, existedBreakpoint)
        } else {
            this.setBreakpoint(range);
        }
    }

    isBreakpoint = (element, index, array) => element.options.linesDecorationsClassName === CodeEditor.breakpointCss;

    setBreakpoint = range => {
        this.onSetBreakpoint(range);
        this.connection.invoke('SetBreakpoint', this.project.name, range);
    };

    onSetBreakpoint = range => {
        this.decorations = this.editor.deltaDecorations([], [this.createBreakpoint(range)]);
        this.breakpoints.push(range.startLineNumber);
    };

    createBreakpoint = range => {
        return {
            range: range,
            options: {
                isWholeLine: true,
                linesDecorationsClassName: CodeEditor.breakpointCss
            }
        };
    };

    removeBreakpoint = (range, breakpoint) => {
        this.onRemoveBreakpoint(range, breakpoint)
        this.connection.invoke('RemoveBreakpoint', this.project.name, range, breakpoint);
    };

    onRemoveBreakpoint = (range, breakpoint) => {
        var newBreakpoints  = [...this.decorations];
        var breakpointIndex = newBreakpoints.indexOf(breakpoint.id);
        if (breakpointIndex > -1) {
            newBreakpoints.splice(breakpointIndex, 1);
        }

        this.decorations = this.editor.deltaDecorations(this.decorations, newBreakpoints);

        var indexOfBp = this.breakpoints.indexOf(range.startLineNumber);
        if (indexOfBp > -1) {
            this.breakpoints.splice(indexOfBp, 1);
        }
    };

    removeDebuggingLine = () => {
        this.decorations = this.editor.deltaDecorations(this.decorations, []);
    };

    isDebuggingLine = (element, index, array) => element.options.className === CodeEditor.debuggingLineCss;

    setDebuggingLine = range => {
        this.decorations = this.editor.deltaDecorations([], [this.createDebuggingLine(range)]);
    };

    createDebuggingLine = range => {
        return {
            range: range,
            options: {
                isWholeLine: true,
                className: CodeEditor.debuggingLineCss,
                linesDecorationsClassName: CodeEditor.debuggingPointCss
            }
        };
    };

    getCurrentDebuggingLineRange = () => this.createRangeByLine(this.currentDebuggingLine);

    createRangeByLine = line => new monaco.Range(line, 1, line, 1);

    applyChanges = changes => {
        this.isSilent = true;

        let model = this.editor.getModel();
        model.applyEdits(changes);

        this.isSilent = false;
    }
}