using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace Booking.API.Infrastructure
{
    public class BearerSecuritySchemeTransformer(IAuthenticationSchemeProvider authenticationSchemeProvider) 
        : IOpenApiDocumentTransformer
    {
        readonly private IAuthenticationSchemeProvider _authenticationSchemeProvider = 
            authenticationSchemeProvider;

        public async Task TransformAsync(
            OpenApiDocument apiDocument, 
            OpenApiDocumentTransformerContext context, 
            CancellationToken cancellationToken)
        {
            var authenticationSchemes = 
                await _authenticationSchemeProvider.GetAllSchemesAsync();

            if (authenticationSchemes.Any(authScheme => authScheme.Name == "Bearer"))
            {
                var securitySchemes = new Dictionary<string, IOpenApiSecurityScheme>
                {
                    ["Bearer"] = new OpenApiSecurityScheme
                    {
                        Type = SecuritySchemeType.Http,
                        Scheme = "bearer",
                        In = ParameterLocation.Header,
                        BearerFormat = "Json Web Token",
                        Description = "JWT Authorization header using the Bearer scheme. **Enter Bearer Token Only**. \r\n\r\nExample: \"Bearer eyJhbGciOiJIUzI1NiIsInR5cCI...\""
                    }
                };
                apiDocument.Components ??= new OpenApiComponents();
                apiDocument.Components.SecuritySchemes = securitySchemes;

                foreach (var operation in apiDocument.Paths.Values.SelectMany(path => path.Operations))
                {
                    operation.Value.Security ??= [];
                    operation.Value.Security.Add(new OpenApiSecurityRequirement
                    {
                        [new OpenApiSecuritySchemeReference("Bearer", apiDocument)] = []
                    });
                }
            }
        }
    }
}
