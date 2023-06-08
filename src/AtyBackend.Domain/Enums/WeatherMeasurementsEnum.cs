namespace AtyBackend.Domain.Enums
{
    public enum WeatherMeasurementsEnum
    {
        Others = 0,
        AirTemperature = 1,
        RelativeHumidity = 2,
        AtmosphericPressure = 3,
        WindSpeed = 4,
        WindDirection = 5,
        Precipitation = 6,
        SolarRadiation = 7,
        UltravioletRadiation = 8,
        InfraredRadiation = 9,
        DewPoint = 10,
        Clouds = 11,
        AirQualityIndex = 12,
        Evapotranspiration = 13,
        SoilTemperature = 14,
        SoilMoisture = 15,
        LongwaveRadiation = 16,
        SeaSurfaceTemperature = 17,
        WaveHeight = 18,
        WaveDirection = 19,
        Visibility = 20,
        HeatIndex = 21,
        CloudCover = 22,
        PrecipitabilityIndex = 23,
        UltravioletIndex = 24,
        ThermalComfortIndex = 25
    }

    public static class WeatherMeasurementsExtensions
    {
        public static string GetMeasurementName(this WeatherMeasurementsEnum measurement)
        {
            return Enum.GetName(typeof(WeatherMeasurementsEnum), measurement);
        }
    }
}
