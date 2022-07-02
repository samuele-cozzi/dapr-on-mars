var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks();
builder.Services.AddControllers().AddDapr();


builder.Services.Configure<DaprSettings>(builder.Configuration.GetSection(nameof(DaprSettings)));
builder.Services.Configure<RoverSettings>(builder.Configuration.GetSection(nameof(RoverSettings)));


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

app.MapGet("/rovers", async (IOptions<RoverSettings> roverSettings) =>
{
    return roverSettings.Value.RoverIds;
})
.WithName("GetRovers");

app.MapGet("/position", async (string roverId, IOptions<DaprSettings> daprSettings) =>
{
    var daprClient = new DaprClientBuilder().Build();
    var result = await daprClient.GetStateAsync<Position>(
        daprSettings.Value.StateStoreName, String.Format(daprSettings.Value.StateRoverPosition, roverId)
    );
    return result;
})
.WithName("GetPosition");

app.MapPost("/position", async (Position position,IOptions<DaprSettings> daprSettings) =>
{
    var daprClient = new DaprClientBuilder().Build();

    await daprClient.SaveStateAsync<Position>(
        daprSettings.Value.StateStoreName, String.Format(daprSettings.Value.StateRoverPosition, position.RoverId), position);
     
})
.WithName("PostPosition");

app.MapPost("/move", async (Command command,IOptions<DaprSettings> daprSettings) =>
{
    var daprClient = new DaprClientBuilder().Build();

    if (command.StartingPosition == null)
    {
        Position actualPosition = await daprClient.GetStateAsync<Position>(
            daprSettings.Value.StateStoreName, String.Format(daprSettings.Value.StateRoverPosition, command.RoverId)
        );
        
        command.StartingPosition = actualPosition;
    }
    else
    {
        command.StartingPosition.RoverId = command.RoverId;
    }

    await daprClient.PublishEventAsync<Command>(
        daprSettings.Value.PubSubName, 
        String.Format(daprSettings.Value.PubSubPositionTopicName, command.RoverId), 
        command);

     
})
.WithName("Move");



app.Run();