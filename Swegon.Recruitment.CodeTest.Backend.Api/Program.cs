using Swegon.Recruitment.CodeTest.Backend.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Add application services
builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddApiDocumentation();
builder.Services.AddCorsPolicy();

var app = builder.Build();

// Configure the HTTP request pipeline
app.UseCustomMiddleware();
app.UseApiDocumentation(app.Environment);

app.UseHttpsRedirection();
app.UseCors("DefaultPolicy");
app.UseAuthorization();
app.MapControllers();

app.Run();
