var connection = new signalR.HubConnectionBuilder()
    .withUrl("/projects")
    .build();

connection.on('CreateProject', project => projectCreated(project));
connection.on('CollaboratorJoinsProject', project => updateProjectCollaboratorsLength(project));
connection.on('CollaboratorLeavesProject', project => updateProjectCollaboratorsLength(project));

connection
    .start()
    .catch(err => onError(err));

const projectCreated = async project => {
    var projectTag = await createProjectTag(project);
    $('#projectsTable tr:last').after(projectTag);
};

const createProjectTag = async project => '<tr id="tr_' + project.name + '"><td><a href="/Editor?projectName=' + project.name + '" target="_blank">' + project.name + '</a></td><td>' + project.collaborators.length + '</td></tr>';

const updateProjectCollaboratorsLength = async project => {
    let collaboratorsRow = await getCollaboratorsRow(project);
    collaboratorsRow.text(project.collaborators.length);
};

const getCollaboratorsRow = async project => $('#tr_' + project.name).find("td").eq(1);

const onError = error => console.error(error.toString());