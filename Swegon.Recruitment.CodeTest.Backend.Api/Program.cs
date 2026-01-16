using Swegon.Recruitment.CodeTest.Backend.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddApiDocumentation();
builder.Services.AddCorsPolicy();

var app = builder.Build();
app.UseCustomMiddleware();
app.UseApiDocumentation(app.Environment);

app.UseHttpsRedirection();
app.UseCors("DefaultPolicy");
app.UseAuthorization();
app.MapControllers();

app.Run();

public partial class Program { }
