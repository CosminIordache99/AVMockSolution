internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);


        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowReactClient", policy =>
            {
                policy
                  .WithOrigins("http://localhost:3000")
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
            });
        });

        builder.Services.AddControllers();
        builder.Services.AddOpenApi();

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            //app.UseSwaggerUI(options =>
            //{
            //    options.SwaggerEndpoint("/openapi/v1.json", "AV API V1");
            //});
            //app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseCors("AllowReactClient");

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}