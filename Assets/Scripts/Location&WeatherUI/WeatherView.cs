using System;
using System.Globalization;
using System.Text;
using TMPro;
using UnityEngine;


public class WeatherView : MonoBehaviour
{
    private const string NOT_AVAILABLE = "N/A";

    [SerializeField] private TMP_Text weatherText;
    [SerializeField] private TMP_Text errorText;

    private void Awake()
    {
        weatherText.text = string.Empty;
        ClearError();
    }

    public void ShowLoading(string message)
    {
        ClearError();
        weatherText.text = message;
    }

    public void ShowWeather(WeatherData data, string title)
    {
        ClearError();

        if (data == null)
        {
            ShowRequestError("No weather data to display.");
            return;
        }

        MainData main = data.main ?? new MainData();
        Wind wind = data.wind ?? new Wind();
        Sys sys = data.sys ?? new Sys();

        string condition = (data.weather != null && data.weather.Length > 0)
            ? Capitalize(data.weather[0].description)
            : NOT_AVAILABLE;

        var text = new StringBuilder();
        text.AppendLine(string.IsNullOrEmpty(title) ? data.name : title);
        text.AppendLine($"Observed: {FormatLocalTime(data.dt, data.timezone)} local ({FormatUtcOffset(data.timezone)})");
        text.AppendLine($"Condition: {condition}");
        text.AppendLine($"Temperature: {OneDecimal(main.temp)} °C (feels like {OneDecimal(main.feels_like)} °C)");
        text.AppendLine($"Humidity: {main.humidity}%");
        text.AppendLine($"Pressure: {main.pressure} hPa");
        text.AppendLine($"Wind: {OneDecimal(wind.speed)} m/s");
        text.Append($"Sunrise: {FormatLocalTime(sys.sunrise, data.timezone)} | Sunset: {FormatLocalTime(sys.sunset, data.timezone)}");

        weatherText.text = text.ToString();
    }

    
    public void ShowValidationError(string message)
    {
        errorText.text = message;
    }

    
    public void ShowRequestError(string message)
    {
        weatherText.text = string.Empty;
        errorText.text = message;
    }

    public void ClearError()
    {
        errorText.text = string.Empty;
    }

    

    private static string OneDecimal(float value)
    {
        return value.ToString("0.0", CultureInfo.InvariantCulture);
    }

    private static string Capitalize(string text)
    {
        if (string.IsNullOrEmpty(text)) return NOT_AVAILABLE;
        return char.ToUpperInvariant(text[0]) + text.Substring(1);
    }

    
    private static string FormatLocalTime(long unixSeconds, int timezoneOffsetSeconds)
    {
        if (unixSeconds <= 0) return NOT_AVAILABLE;

        DateTime local = DateTimeOffset.FromUnixTimeSeconds(unixSeconds).UtcDateTime
                                       .AddSeconds(timezoneOffsetSeconds);
        return local.ToString("HH:mm", CultureInfo.InvariantCulture);
    }

    private static string FormatUtcOffset(int offsetSeconds)
    {
        TimeSpan offset = TimeSpan.FromSeconds(Math.Abs(offsetSeconds));
        string sign = offsetSeconds < 0 ? "-" : "+";

        return offset.Minutes == 0
            ? $"UTC{sign}{offset.Hours}"
            : $"UTC{sign}{offset.Hours}:{offset.Minutes:00}";
    }
}