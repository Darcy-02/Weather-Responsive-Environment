using System;
using System.Collections.Generic;
using UnityEngine;


public interface IWeatherResponseParser
{
    bool TryParse(string json, out WeatherData data, out string error);
    string BuildErrorMessage(WebResponse response);
}

public class OpenWeatherResponseParser : IWeatherResponseParser
{
   
    private static readonly Dictionary<long, string> StatusHints = new Dictionary<long, string>
    {
        { 401, "Check that your API key is correct and active." },
        { 404, "Location not found." },
        { 429, "Too many requests. Try again later." }
    };

    public bool TryParse(string json, out WeatherData data, out string error)
    {
        data = null;
        error = null;

        if (string.IsNullOrWhiteSpace(json))
        {
            error = "The weather service returned an empty response.";
            return false;
        }

        try
        {
            WeatherData parsed = JsonUtility.FromJson<WeatherData>(json);


            if (parsed == null || parsed.weather == null || parsed.weather.Length == 0)
            {
                error = "The response did not contain usable weather data.";
                return false;
            }

            FillMissingFields(parsed);
            data = parsed;
            return true;
        }
        catch (Exception ex)
        {
            error = $"Failed to parse weather JSON: {ex.Message}";
            return false;
        }
    }

    public string BuildErrorMessage(WebResponse response)
    {
        if (response == null)
        {
            return "No response received from the weather service.";
        }

        
        string detail = ExtractApiMessage(response.Body);
        if (string.IsNullOrEmpty(detail))
        {
            detail = response.Error;
        }

        string message = $"Error fetching weather (HTTP {response.StatusCode}): {detail}";

        if (StatusHints.TryGetValue(response.StatusCode, out string hint))
        {
            message += $" {hint}";
        }

        return message;
    }

    private static string ExtractApiMessage(string body)
    {
        if (string.IsNullOrEmpty(body))
        {
            return null;
        }

        try
        {
            ApiError apiError = JsonUtility.FromJson<ApiError>(body);
            return apiError != null ? apiError.message : null;
        }
        catch (Exception)
        {
            return null; 
        }
    }

    private static void FillMissingFields(WeatherData data)
    {
        if (string.IsNullOrEmpty(data.name)) data.name = "Unknown location";
        if (data.coord == null) data.coord = new Coord();
        if (data.main == null) data.main = new MainData();
        if (data.wind == null) data.wind = new Wind();
        if (data.sys == null) data.sys = new Sys();

        for (int i = 0; i < data.weather.Length; i++)
        {
            if (data.weather[i] == null)
            {
                data.weather[i] = new Weather();
            }
        }
    }
}
