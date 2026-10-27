using UnityEngine;

public class WeatherTester : MonoBehaviour
{

    [SerializeField] private MonoBehaviour weatherService;

    private IWeatherService _weatherService;

    private void Start()
    {
        _weatherService = ResolveService();

        if (_weatherService == null)
        {
            Debug.LogError("[WeatherTest] No IWeatherService found in the scene.");
            return;
        }

        float testLat = -1.9500f;
        float testLon = 30.0589f;

        Debug.Log($"[WeatherTest] Sending request for coordinates: {testLat}, {testLon}...");

        _weatherService.GetCurrentWeather(testLat, testLon,
            onSuccess: (data) =>
            {
                string condition = (data.weather != null && data.weather.Length > 0)
                    ? data.weather[0].description
                    : "No description available";

                Debug.Log($"<color=green>[Coordinate Test] SUCCESS!</color>\n" +
                          $"Resolved Location Name: {data.name}\n" +
                          $"Temp: {data.main.temp}°C\n" +
                          $"Condition: {condition}\n" +
                          $"Sunrise (UTC): {data.sys.SunriseUtc}, Sunset (UTC): {data.sys.SunsetUtc}");
            },
            onFailure: (errorMessage) =>
            {
                Debug.LogError($"<color=red>[Coordinate Test] FAILED!</color>\nError: {errorMessage}");
            }
        );
    }

    private IWeatherService ResolveService()
    {
        if (weatherService is IWeatherService assigned)
        {
            return assigned;
        }

        foreach (MonoBehaviour behaviour in FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None))
        {
            if (behaviour is IWeatherService found)
            {
                return found;
            }
        }

        return null;
    }
}