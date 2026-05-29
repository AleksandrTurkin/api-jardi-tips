var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.JardiTips_WebApi>("jarditips-webapi");

builder.Build().Run();
