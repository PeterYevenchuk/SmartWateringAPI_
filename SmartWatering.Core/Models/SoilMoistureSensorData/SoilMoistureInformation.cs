namespace SmartWatering.Core.Models.SoilMoistureSensorData;

public class SoilMoistureInformation
{
    public double StartLevel { get; set; }

    public double EndLevel { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public int UserId { get; set; }

    public string NameId { get; set; }
}
