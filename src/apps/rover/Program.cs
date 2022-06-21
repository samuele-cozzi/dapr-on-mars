var builder = WebApplication.CreateBuilder(args);

//builder.Host.UseSerilog();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks();

builder.Services.Configure<RoverSettings>(builder.Configuration.GetSection(nameof(RoverSettings)));
//builder.Services.Configure<IntegrationSettings>(builder.Configuration.GetSection(nameof(IntegrationSettings)));
builder.Services.Configure<MarsSettings>(builder.Configuration.GetSection(nameof(MarsSettings)));
builder.Services.Configure<DaprSettings>(builder.Configuration.GetSection(nameof(DaprSettings)));

builder.Services.AddHostedService<InitMarsService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();
app.MapHealthChecks("/");

app.MapGet("/position", async (IOptions<DaprSettings> daprSettings) =>
{
    var daprClient = new DaprClientBuilder().Build();
    var result = await daprClient.GetStateAsync<Command>(
        daprSettings.Value.StateManagement.StoreName, daprSettings.Value.StateManagement.RoverPosition
    );
    return result;
})
.WithName("GetPosition");

app.MapPost("/move", async (Command command,IOptions<DaprSettings> daprSettings) =>
{
    var daprClient = new DaprClientBuilder().Build();
    await daprClient.SaveStateAsync<Command>(
        daprSettings.Value.StateManagement.StoreName, daprSettings.Value.StateManagement.RoverPosition, command);
})
.WithName("Move");

app.Logger.LogInformation ("The application started");

app.Run();