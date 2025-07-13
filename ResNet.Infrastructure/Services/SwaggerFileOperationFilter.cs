using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

public class SwaggerFileOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        // Список свойств IFormFile из всех параметров метода (DTO или сам IFormFile)
        var fileProperties = new Dictionary<string, OpenApiSchema>();

        foreach (var parameter in context.MethodInfo.GetParameters())
        {
            var paramType = parameter.ParameterType;

            // Если параметр - IFormFile напрямую
            if (paramType == typeof(Microsoft.AspNetCore.Http.IFormFile))
            {
                fileProperties[parameter.Name] = new OpenApiSchema { Type = "string", Format = "binary" };
            }
            else
            {
                // Если параметр - DTO, ищем свойства типа IFormFile внутри него
                var props = paramType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Where(p => p.PropertyType == typeof(Microsoft.AspNetCore.Http.IFormFile));

                foreach (var prop in props)
                {
                    fileProperties[prop.Name] = new OpenApiSchema { Type = "string", Format = "binary" };
                }
            }
        }

        if (!fileProperties.Any())
            return;

        operation.RequestBody = new OpenApiRequestBody
        {
            Content =
            {
                ["multipart/form-data"] = new OpenApiMediaType
                {
                    Schema = new OpenApiSchema
                    {
                        Type = "object",
                        Properties = fileProperties,
                        Required = fileProperties.Keys.ToHashSet()
                    }
                }
            }
        };
    }
}
