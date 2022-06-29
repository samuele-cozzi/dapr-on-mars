using Dapr;

var builder = WebApplication.CreateBuilder(args);

//builder.Host.UseSerilog();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks();
builder.Services.AddControllers().AddDapr();

builder.Services.Configure<RoverSettings>(builder.Configuration.GetSection(nameof(RoverSettings)));
//builder.Services.Configure<IntegrationSettings>(builder.Configuration.GetSection(nameof(IntegrationSettings)));
builder.Services.Configure<MarsSettings>(builder.Configuration.GetSection(nameof(MarsSettings)));
builder.Services.Configure<DaprSettings>(builder.Configuration.GetSection(nameof(DaprSettings)));

builder.Services.AddHostedService<InitMarsService>();
builder.Services.AddHostedService<RoverWorkerService>();
builder.Services.AddScoped<RoverService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();
app.MapHealthChecks("/");
app.UseCloudEvents();

//app.MapGet("/position", [Topic("rover-pubsub","command-topic")] async (IOptions<DaprSettings> daprSettings) =>
app.MapGet("/position", async (IOptions<DaprSettings> daprSettings) =>
{
    var daprClient = new DaprClientBuilder().Build();
    var result = await daprClient.GetStateAsync<Position>(
        daprSettings.Value.StateStoreName, daprSettings.Value.StateRoverPosition
    );
    return result;
})
.WithName("GetPosition");

app.MapPost("/move", async (Command command,IOptions<DaprSettings> daprSettings, RoverService roverService) =>
{
    var daprClient = new DaprClientBuilder().Build();

    Position actualPosition = await daprClient.GetStateAsync<Position>(
        daprSettings.Value.StateStoreName, daprSettings.Value.StateRoverPosition
    );
    Position startingPosition = command.StartingPosition;

    if (actualPosition == null){
        actualPosition = command.StartingPosition;
    }

    if (
        actualPosition?.Coordinate?.Latitude == startingPosition?.Coordinate?.Latitude 
        && actualPosition?.Coordinate?.Longitude == startingPosition?.Coordinate?.Longitude 
        && actualPosition?.FacingDirection == startingPosition?.FacingDirection 
        && actualPosition?.RoverId == startingPosition?.RoverId
    ) {
        var newPosition = roverService.Move(command);

        await daprClient.SaveStateAsync<Position>(
        daprSettings.Value.StateStoreName, daprSettings.Value.StateRoverPosition, newPosition);
    }    
})
.WithName("Move");

app.Logger.LogInformation ("The application started");

app.Run();