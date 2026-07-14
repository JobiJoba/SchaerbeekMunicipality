using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
    .WithDataVolume()
    .AddDatabase("schaerbeek");

var api = builder.AddProject<SchaerbeekMunicipality_Api>("api")
    .WithReference(postgres)
    .WaitFor(postgres);

builder.AddProject<SchaerbeekMunicipality_Web>("web")
    .WithReference(api)
    .WithDeveloperCertificateTrust(true)
    .WaitFor(api);

builder.Build().Run();