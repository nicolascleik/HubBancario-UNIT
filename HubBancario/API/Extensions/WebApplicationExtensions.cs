using Microsoft.AspNetCore.Builder;

namespace HubBancario.API.Extensions;

public static class WebApplicationExtensions
{
    public static WebApplication ConfigurePipeline(this WebApplication app)
    {

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.UseHttpsRedirection();


        app.UseCors("DefaultCorsPolicy");

        
        app.UseAuthorization();


        app.MapControllers();

        return app;
    }
}