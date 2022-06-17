namespace Rover.Models;

public class MarsSettings
{
    public int AngularPartition { get; set; }
    public double? ObstaclesPercentage { get; set; }
    public List<Coordinate> Obstacles { get; set; }
}