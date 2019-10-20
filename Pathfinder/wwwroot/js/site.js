var connection = new signalR.HubConnectionBuilder()
    .withUrl("/projects")
    .build();

connection
    .start()
    .catch(err => console.error(err.toString()));

connection.on('CreateProject', (project) => {
    $('#projectsTable tr:last')
        .after('<tr id="tr_' + project.name + '"><td><a href="/Editor?projectName=' + project.name + '" target="_blank">' + project.name + '</a></td><td>' + project.collaborators.length + '</td></tr>');
});

connection.on('CollaboratorJoinsProject', (project) => {
    $('#tr_' + project.name)
        .find("td")
        .eq(1)
        .text(project.collaborators.length);
});