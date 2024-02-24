using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace WebApplication1.middleware
{

    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly string _secretKey = "300046c07bc4876a8596e83cbb34744d6462d8f7238660ad2815c1a433f90e934934c";

        public JwtMiddleware(RequestDelegate next)
        {
            _next = next;

        }

        public async Task InvokeAsync(HttpContext context)
        {
            string path = context.Request.Path.Value;
            if (/*path.StartsWith("/Chat") || */path.StartsWith("/Messages"))
            {
                var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
                if (string.IsNullOrEmpty(token))
                {
                    context.Response.StatusCode = 401; // Unauthorized
                    await context.Response.WriteAsync("No token has been provided");
                    return;
                }

                    var tokenHandler = new JwtSecurityTokenHandler();
                    var key = Encoding.ASCII.GetBytes(_secretKey);

                    TokenValidationParameters validationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ClockSkew = TimeSpan.Zero
                    };

                    try
                    {
                        tokenHandler.ValidateToken(token, validationParameters, out _);
                    }
                    catch (Exception)
                    {
                        context.Response.StatusCode = 401; // Unauthorized
                        return;
                    }
            }
            await _next.Invoke(context);

        }
    }
}

