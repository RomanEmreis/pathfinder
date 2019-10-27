"use strict";

class EditorEventHandler {
    constructor(project) {
        this.project    = project;
        this.connection = new signalR.HubConnectionBuilder()
            .withUrl("/liveeditor")
            .build();

        this.connect();

        this.editor = new CodeEditor(this.connection, this.project);
        this.output = new Output();
    }

    connect = () => {
        this.connection
            .start()
            .then(async () => await this.connectionEstablished())
            .catch(err => projectsEventHandler.onError(err));
    }

    connectionEstablished = async () => this.connection.invoke('CollaboratorJoinsProject', this.project.name);

    dispose = () => this.connection.invoke('CollaboratorLeavesProject', this.project.name);

    subscribe = () => {
        this.connection.on('CodeChanged',      async (projectName, delta)       => await this.onCodeChanged(projectName, delta));
        this.connection.on('ProjectCompiled',  async (projectName, buildResult) => await this.onProjectCompiled(projectName, buildResult));
        this.connection.on('StepOver',         async buildContext               => await this.onStepOver(buildContext));
        this.connection.on('StepInto',         async buildContext               => await this.onStepInto(buildContext));
        this.connection.on('SetBreakpoint',    async range                      => await this.editor.onSetBreakpoint(range));
        this.connection.on('RemoveBreakpoint', async (range, breakpoint)        => await this.editor.onRemoveBreakpoint(range, breakpoint));
    }

    onCodeChanged = async (projectName, changes) => this.editor.applyChanges(changes);

    onStepOver = async buildContext => {
        this.editor.currentDebuggingLine = ++buildContext.currentLine;
        let currentRange                 = this.editor.getCurrentDebuggingLineRange();

        this.editor.removeDebuggingLine()
        this.editor.setDebuggingLine(currentRange);

        await this.output.writeResult(buildContext);
    };

    onStepInto = async buildContext => { };

    onProjectCompiled = async (projectName, buildResult) => {
        let buildMessage = buildResult.success
            ? 'Build project ' + projectName + ' successfully complete'
            : 'Build project ' + projectName + ' complete with errors';

        console.log(buildMessage);
        this.output.writeToOutput(buildMessage);

        await this.output.writeResult(buildResult);
    };
}