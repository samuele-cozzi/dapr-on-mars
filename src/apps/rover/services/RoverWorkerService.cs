namespace Rover.Services;

public class RoverWorkerService : IHostedService, IDisposable
{
    private readonly ILogger<InitMarsService> _logger;
    private readonly MarsSettings _marsSettings;
    private readonly DaprSettings _daprSettings;
    private Timer? _timer = null;

    public RoverWorkerService(
        ILogger<InitMarsService> logger,
        IOptions<MarsSettings> marsSettings,
        IOptions<DaprSettings> daprSettings)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _marsSettings = marsSettings?.Value ?? throw new ArgumentNullException(nameof(marsSettings));
        _daprSettings = daprSettings?.Value ?? throw new ArgumentNullException(nameof(daprSettings));
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Begin Start Rover Worker");

        _timer = new Timer(SendPosition, null, TimeSpan.Zero, TimeSpan.FromSeconds(10));
        
        _logger.LogInformation($"End Start Rover Worker");

        return Task.CompletedTask;
    }

    private async void SendPosition(object? state){

        _logger.LogInformation($"Send Rover Position Start");

        var daprClient = new DaprClientBuilder().Build();

        Position actualPosition = await daprClient.GetStateAsync<Position>(
            _daprSettings.StateManagement.StoreName, _daprSettings.StateManagement.RoverPosition
        );

        if (actualPosition != null){
            //TODO
            await daprClient.PublishEventAsync<Position>("rover-pubsub", "position-topic", actualPosition);
        }

        _logger.LogInformation($"Send Rover Position End");
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Finish Rover Worker");

        _timer?.Change(Timeout.Infinite, 0);

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }
}