using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace JardiTips.WebApi.Swagger;

public class LowercaseSwaggerDocumentFilter : IDocumentFilter
{
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        var originalPaths = swaggerDoc.Paths;

        //	generate the new keys
        var newPaths = new Dictionary<string, IOpenApiPathItem>();
        var removeKeys = new List<string>();
        foreach (var path in originalPaths)
        {
            var newKey = LowercaseEverythingButParameters(path.Key);
            if (newKey == path.Key)
            {
                continue;
            }

            removeKeys.Add(path.Key);
            newPaths.Add(newKey, path.Value);
        }

        //	add the new keys
        foreach (var path in newPaths)
        {
            swaggerDoc.Paths.Add(path.Key, path.Value);
        }

        //	remove the old keys
        foreach (var key in removeKeys)
        {
            swaggerDoc.Paths.Remove(key);
        }
    }

    private static string LowercaseEverythingButParameters(string key) =>
        string.Join("/", key.Split('/')
            .Select(x => x.Contains("{") ? x : x.ToLower()));
}