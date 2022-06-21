namespace Rover.Models;

public class Command {
    public Move[] Moves { get; set; }
    public Position StartingPosition { get; set; }
}