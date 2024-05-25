using SmartWatering.Core.Models.SoilMoistureSensorData;

namespace SmartWatering.Core.UserInfo;

public class UserDTO
{
    public int Id { get; set; }

    public string Name { get; set; }

    public string SurName { get; set; }

    public string Email { get; set; }

    public bool AutoMode { get; set; }

    public string City { get; set; }

    public WateringInformation WateringInformation { get; set; }
}
