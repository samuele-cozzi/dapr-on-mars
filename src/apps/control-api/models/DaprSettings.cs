namespace Control.Api.Models;

public class DaprSettings {
    public string StateStoreName { get; set; }
    public string StateRoverPosition { get; set; }
    public string PubSubName  { get; set; }
    public string PubSubPositionTopicName { get; set; }
}