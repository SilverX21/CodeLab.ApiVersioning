using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CodeLab.ApiVersioning.OpenApi;

public class ConfigureSwaggerGenOptions(IApiVersionDescriptionProvider provider) : IConfigureNamedOptions<SwaggerGenOptions>
{
    /// <summary>
    /// Here we will get the versions we have available and add them to the swagger documentation
    /// </summary>
    /// <param name="options"></param>
    public void Configure(SwaggerGenOptions options)
    {
        foreach (var description in provider.ApiVersionDescriptions)
        {
            var openApiInfo = new OpenApiInfo
            {
                Title = $"CodeLab.ApiVersioning v{description.ApiVersion}",
                Version = description.ApiVersion.ToString()
            };
            
            options.SwaggerDoc(description.GroupName, openApiInfo);
        }
    }

    public void Configure(string? name, SwaggerGenOptions options)
    {
        Configure(options);
    }
}