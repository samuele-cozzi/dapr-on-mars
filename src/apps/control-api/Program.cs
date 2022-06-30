var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks();
builder.Services.AddControllers().AddDapr();


builder.Services.Configure<DaprSettings>(builder.Configuration.GetSection(nameof(DaprSettings)));


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

// app.MapGet("/position", async (IOptions<DaprSettings> daprSettings) =>
// {
//     var daprClient = new DaprClientBuilder().Build();
//     var result = await daprClient.GetStateAsync<Position>(
//         daprSettings.Value.StateStoreName, daprSettings.Value.StateRoverPosition
//     );
//     return result;
// })
// .WithName("GetPosition");

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

    await daprClient.PublishEventAsync<Command>(
        daprSettings.Value.PubSubName, 
        String.Format(daprSettings.Value.PubSubPositionTopicName, command.RoverId), 
        command);

     
})
.WithName("Move");



app.Run();