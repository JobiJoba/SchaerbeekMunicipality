var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
    .WithDataVolume()
    .AddDatabase("schaerbeek");

builder.AddProject<Projects.SchaerbeekMunicipality_Web>("web")
    .WithReference(postgres)
    .WaitFor(postgres);

builder.Build().Run();
