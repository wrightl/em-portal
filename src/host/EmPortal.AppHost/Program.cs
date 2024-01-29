var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedis("cache");

builder.AddContainer("prometheus", "prom/prometheus")
       .WithVolumeMount("../prometheus", "/etc/prometheus")
       .WithServiceBinding(9090, hostPort: 9090);

var repos = builder.AddProject<Projects.Repos_API>("repos")
    .WithReference(cache);

builder.AddProject<Projects.EmPortal_Client>("frontend")
    .WithReference(cache)
    .WithReference(repos)
    .WithLaunchProfile("https");

builder.Build().Run();
