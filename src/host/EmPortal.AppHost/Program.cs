var builder = DistributedApplication.CreateBuilder(args);

// Add the locations database.
var locationsdb = builder.AddPostgres("sql").AddDatabase("TicketsDb");

// // Set in user-secrets
// var sqlpassword = builder.Configuration["sqlpwd"];

// var sql = builder.AddSqlServerContainer("sql", sqlpassword)
//     .WithVolumeMount("VolumeMount.sqlserver.data", "/var/opt/mssql", VolumeMountType.Named)
//     .AddDatabase("TicketsDb");

var cache = builder.AddRedis("cache");

builder.AddContainer("prometheus", "prom/prometheus")
       .WithVolumeMount("../prometheus", "/etc/prometheus")
       .WithEndpoint(9090, hostPort: 9090);

var repos = builder.AddProject<Projects.Repos_API>("repos")
    // .WithReference(sql)
    .WithReference(locationsdb)
    .WithReference(cache);

builder.AddProject<Projects.EmPortal_Client>("frontend")
    .WithReference(cache)
    .WithReference(repos)
    .WithLaunchProfile("https");

builder.Build().Run();
