"use strict";

class ProjectsEventHandler {
    constructor() {
        this.connection = new signalR.HubConnectionBuilder()
            .withUrl("/projects")
            .build();

        this.connect();
    }

    connect = () => {
        this.connection
            .start()
            .catch(err => onError(err));
    }

    subscripe = () => {
        this.connection.on('CreateProject',				this.onProjectCreated);
        this.connection.on('CollaboratorJoinsProject',  this.updateProjectCollaboratorsLength);
        this.connection.on('CollaboratorLeavesProject', this.updateProjectCollaboratorsLength);
    }

    onProjectCreated = async project => {
        var projectTag = await this.createProjectTag(project);
        $('#projectsTable tr:last').after(projectTag);
    }

    createProjectTag = async project => '<tr id="tr_' + project.name + '"><td><a href="/Editor?projectName=' + project.name + '" target="_blank">' + project.name + '</a></td><td>' + project.collaborators.length + '</td></tr>';

    updateProjectCollaboratorsLength = async project => {
        let collaboratorsRow = await this.getCollaboratorsRow(project);
        collaboratorsRow.text(project.collaborators.length);
    };

    getCollaboratorsRow = async project => $('#tr_' + project.name).find("td").eq(1);

    onError = error => console.error(error.toString());
}