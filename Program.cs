using Asp.Versioning;
using CodeLab.ApiVersioning.Endpoints;
using CodeLab.ApiVersioning.OpenApi;

var builder = WebApplication.CreateBuilder(args);

// builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1,0); //this is the default api version
    options.ApiVersionReader = new UrlSegmentApiVersionReader(); //sets the type of versioning to the url, example: api/v1
})
.AddMvc() //this is only if we want to use controllers
.AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV"; //the type of version format
    options.SubstituteApiVersionInUrl = true; //will substitute all the api endpoints in the swagger endpoints
});

//here we add the swagger configuration for having the documentation for the multiple versions
builder.Services.ConfigureOptions<ConfigureSwaggerGenOptions>();

var app = builder.Build();

//we need to add these before configuring the swagger, this is because they will be retrieved when we run the api
var apiVersionSet = app.NewApiVersionSet()
    .HasApiVersion(new ApiVersion(1)) //here we define the version that the api supports, every version needs to use this
    .HasApiVersion(new ApiVersion(2)) 
    .ReportApiVersions() //here we say that in the response headers it will go all the versions supported by the api
    .Build();
        
//we set the apiVersion like this v{apiVersion:apiVersion}
var versionedGroup = app.MapGroup("api/v{apiVersion:apiVersion}")
    .WithApiVersionSet(apiVersionSet);

//here we use the versioned group so it can apply to all the endpoints
versionedGroup.MapApiEndpoints();

if (app.Environment.IsDevelopment())
{
    // app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        var descriptions = app.DescribeApiVersions();

        foreach (var description in descriptions)
        {
            //this will add the swagger version documentation to the Swagger UI
            string url = $"/swagger/{description.GroupName}/swagger.json";
            string name =  description.GroupName.ToUpperInvariant();
            
            options.SwaggerEndpoint(url, name);
        }
    });
}

app.UseHttpsRedirection();

app.Run();