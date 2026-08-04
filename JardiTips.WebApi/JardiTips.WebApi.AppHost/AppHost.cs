var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.JardiTips_WebApi>("jarditips-webapi");

builder.AddProject<Projects.JardiTips_Client>("jarditips-client");

builder.Build().Run();
