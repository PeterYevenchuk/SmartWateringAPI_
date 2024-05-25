namespace SmartWatering.Core.SensorInfo;

public class SensorInfoDTO
{
    public int Id { get; set; }

    public double StartLevel { get; set; }

    public double EndLevel { get; set; }

    public string StartDate { get; set; }

    public string EndDate { get; set; }

    public TimeOnly WastedTime { get; set; }

    public int UserId { get; set; }
}
