namespace SmartWatering.Core.WeatherSettings.Messages;

public static class RecomendationMessages
{
    public static string M0000 => "Unknown weather conditions for the specified day.";

    public static string M0001 => "The temperature is high, air humidity is low, soil humidity is low. Intensive watering is recommended.";

    public static string M0002 => "It's raining, and the ground is already wet enough. Watering is not needed now.";

    public static string M0003 => "The temperature is low, the soil moisture is low. Wet watering is recommended.";

    public static string M0004 => "Snowing. Watering is not required, but monitor the condition of the plants in winter.";

    public static string M0005 => "A storm is coming. It is recommended to turn off watering to avoid electric shock to plants.";

    public static string M0006 => "The temperature is comfortable, the air is humid. Watering plants according to the usual regime.";

    public static string M0007 => "The temperature and humidity of the soil is low. A little watering is recommended, while being careful to protect the plants from frost.";

    public static string M0008 => "Bad weather conditions are approaching. It is recommended to delay watering to avoid water spray.";

    public static string M0009 => "Air humidity is high, but the soil is already wet. Delay watering to avoid waterlogging.";

    public static string M0010 => "The temperature is high, but the soil moisture is low. Intensive watering is recommended.";

    public static string M0011 => "The heat continues. Keep plants out of direct sunlight and use protective equipment.";

    public static string M0012 => "The temperature is comfortable, but the soil is dry. It is recommended to water the plants.";

    public static string M0013 => "The temperature is low, but the soil is moist. Plants can withstand a little frost.";

    public static string M0014 => "It is drizzling, the soil moisture is fine. Watering is not required.";

    public static string M0015 => "Bad weather conditions are approaching, but the humidity of the soil is low. It is recommended to turn on irrigation, but you need to make sure that splashes do not form.";

    public static string M0016 => "Soil moisture is critically low. You need to water the plants!";

    // tomorrow
    public static string M0101 => "It will be quite hot, watch the soil moisture!";

    public static string M0102 => "A comfortable temperature is expected.";

    public static string M0103 => "It will be cold, keep an eye on the plants.";

    public static string M0104 => "It will be windy, watch the plants.";

    public static string M0105 => "Unfavorable conditions are expected for your plants, keep an eye on them and soil moisture!";

    public static string M0106 => "Stay tuned for further posts and take care of your plants.";
}
