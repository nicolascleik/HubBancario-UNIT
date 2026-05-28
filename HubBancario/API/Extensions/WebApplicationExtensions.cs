using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;

namespace HubBancario.API.Extensions
{
    public static class WebApplicationExtensions
    {
        public static WebApplication ConfigurePipeline(this WebApplication app)
        {
            // O Swagger fica disponível inclusive em ambiente de desenvolvimento local
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c => 
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Hub Bancário API v1");
                    c.RoutePrefix = "swagger"; // Acessível em http://localhost:porta/swagger
                });
            }

            app.UseHttpsRedirection();

            // Ativa o sistema de segurança JWT e roteamento
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            return app;
        }
    }
}