using System.Globalization;
using UnityEngine.Networking;
public interface IWeatherUrlBuilder
{
    string BuildByCoordinates(float latitude, float longitude, string apiKey);
}

public class OpenWeatherUrlBuilder : IWeatherUrlBuilder
{
    private const string BASE_URL = "https://api.openweathermap.org/data/2.5/weather";

    public string BuildByCoordinates(float latitude, float longitude, string apiKey)
    {

        string lat = latitude.ToString(CultureInfo.InvariantCulture);
        string lon = longitude.ToString(CultureInfo.InvariantCulture);

        return $"{BASE_URL}?lat={lat}&lon={lon}&appid={UnityWebRequest.EscapeURL(apiKey)}&units=metric";
    }
}