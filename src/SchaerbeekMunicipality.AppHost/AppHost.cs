var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
    .WithDataVolume()
    .AddDatabase("schaerbeek");

var api = builder.AddProject<Projects.SchaerbeekMunicipality_Api>("api")
    .WithReference(postgres)
    .WaitFor(postgres);

builder.AddProject<Projects.SchaerbeekMunicipality_Web>("web")
    .WithReference(postgres)
    .WithReference(api)
    .WaitFor(api);

builder.Build().Run();
