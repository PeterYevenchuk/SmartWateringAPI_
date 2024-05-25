namespace SmartWatering.Core.WeatherSettings.Messages;

public class ResultMessage
{
    public int UserId { get; set; }

    public string DayTime { get; set; }

    public string DateTime { get; set; }

    public string Message { get; set; }

    public bool IsRead { get; set; } = false;
}
