namespace Control.Api.Models;

public class Command {
    public string RoverId {get; set; }
    public Moves[] Moves { get; set; }
    public Position StartingPosition { get; set; }
}