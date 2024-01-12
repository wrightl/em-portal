var builder = DistributedApplication.CreateBuilder(args);

var repos = builder.AddProject<Projects.Repos_API>("repos");

builder.AddProject<Projects.EmPortal_Client>("frontend")
    .WithReference(repos);

builder.Build().Run();
