﻿using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace SimpleAPI.Extensions
{
  public class ConfigureSwaggerOptions(
      IApiVersionDescriptionProvider provider)
                  : IConfigureNamedOptions<SwaggerGenOptions>
  {
    private readonly IApiVersionDescriptionProvider provider = provider;

    public void Configure(SwaggerGenOptions options)
    {
      // add swagger document for every API version discovered
      foreach (var description in provider.ApiVersionDescriptions)
      {
        options.SwaggerDoc(
            description.GroupName,
            CreateVersionInfo(description));
      }
    }

    public void Configure(string? name, SwaggerGenOptions options)
    {
      Configure(options);
    }

    private static OpenApiInfo CreateVersionInfo(
            ApiVersionDescription description)
    {
      var info = new OpenApiInfo()
      {
        Title = "Simple API :)",
        Version = description.ApiVersion.ToString()
      };

      if (description.IsDeprecated)
      {
        info.Description += " old API :(";
      }

      return info;
    }
  }
}