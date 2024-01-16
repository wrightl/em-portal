var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedis("cache");

var repos = builder.AddProject<Projects.Repos_API>("repos")
    .WithReference(cache);

builder.AddProject<Projects.EmPortal_Client>("frontend")
    .WithReference(cache)
    .WithReference(repos);

builder.Build().Run();
