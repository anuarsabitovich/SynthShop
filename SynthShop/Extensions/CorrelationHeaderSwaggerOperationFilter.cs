using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using SynthShop.Domain.Constants;

namespace SynthShop.Extensions
{
    public class CorrelationHeaderSwaggerOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            operation.Parameters ??= [];
            
            operation.Parameters.Add(new OpenApiParameter
            {
                Name = LogConstants.CorrelationHeader,
                In = ParameterLocation.Header,
                Required = false,
                Schema = new OpenApiSchema
                {
                    Type = "string",
                    Default = new OpenApiString("0ebc7df4-7949-4b1d-ab06-7864967eef25")
                }
            });
        }
    }
}
