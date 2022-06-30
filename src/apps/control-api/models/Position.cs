namespace Control.Api.Models;

public class Position
{
    public Coordinate Coordinate { get; set; }
    public FacingDirections FacingDirection { get; set; }
    public bool IsBlocked { get; set; }
    public string RoverId { get; set; }
}