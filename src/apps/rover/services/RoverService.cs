using System.Reflection;

namespace Rover.Services;

public class RoverService 
{

    private readonly ILogger<InitMarsService> _logger;
    private readonly MarsSettings _marsSettings;
    private readonly double angularStep;

    public RoverService(
        ILogger<InitMarsService> logger,
        IOptions<MarsSettings> marsSettings)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _marsSettings = marsSettings?.Value ?? throw new ArgumentNullException(nameof(marsSettings));
        angularStep = (_marsSettings.AngularPartition != 0) ? 360 / _marsSettings.AngularPartition : 0;
    }

    public Position Move(Command command)
    {
        _logger.LogInformation($"Start Move");

        bool isBlocked = false;
        double latitude = command.StartingPosition.Coordinate.Latitude;
        double longitude = command.StartingPosition.Coordinate.Longitude;
        FacingDirections direction = command.StartingPosition.FacingDirection;

        var position = new Position()
        {
            FacingDirection = direction,
            Coordinate = new Coordinate()
            {
                Latitude = latitude,
                Longitude = longitude
            }
        };

        foreach (var move in command.Moves)
        {
            switch (move)
            {
                case Moves.f:
                    position.MoveFarward(angularStep);
                    break;
                case Moves.b:
                    position.MoveBackward(angularStep);
                    break;
                case Moves.r:
                    position.TurnRight();
                    break;
                case Moves.l:
                    position.TurnLeft();
                    break;
            }

            isBlocked = CheckObstacles(position);
            if (!isBlocked)
            {
                latitude = position.Coordinate.Latitude;
                longitude = position.Coordinate.Longitude;
                direction = position.FacingDirection;
            }
            else
            {
                break;
            }
            
        }

        _logger.LogInformation($"Finish Move");

        return new Position(){
            FacingDirection = direction,
            Coordinate = new Coordinate()
            {
                Latitude = latitude,
                Longitude = longitude
            },
            IsBlocked = isBlocked
        };

        
    }

    private bool CheckObstacles(Position position)
    {
        if(_marsSettings.Obstacles.Count(
            x=> x.Latitude == position.Coordinate.Latitude &&
            x.Longitude == position.Coordinate.Longitude) > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}