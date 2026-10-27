using System;
using UnityEngine;


public class WeatherAPIService : MonoBehaviour, IWeatherService
{
    private IApiKeyProvider _apiKeyProvider;
    private IWebRequestSender _requestSender;
    private IWeatherUrlBuilder _urlBuilder;
    private IWeatherResponseParser _responseParser;

    private void Awake()
    {
        EnsureDefaults();
    }

    public void Initialize(
        IApiKeyProvider apiKeyProvider,
        IWebRequestSender requestSender,
        IWeatherUrlBuilder urlBuilder,
        IWeatherResponseParser responseParser)
    {
        _apiKeyProvider = apiKeyProvider ?? throw new ArgumentNullException(nameof(apiKeyProvider));
        _requestSender = requestSender ?? throw new ArgumentNullException(nameof(requestSender));
        _urlBuilder = urlBuilder ?? throw new ArgumentNullException(nameof(urlBuilder));
        _responseParser = responseParser ?? throw new ArgumentNullException(nameof(responseParser));
    }

    public void GetCurrentWeather(
        float latitude,
        float longitude,
        Action<WeatherData> onSuccess,
        Action<string> onFailure)
    {
        EnsureDefaults();

        if (!AreCoordinatesValid(latitude, longitude))
        {
            onFailure?.Invoke($"Invalid coordinates (lat: {latitude}, lon: {longitude}). " +
                              "Latitude must be -90 to 90 and longitude must be -180 to 180.");
            return;
        }

        string apiKey = _apiKeyProvider.GetApiKey();
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            onFailure?.Invoke("OpenWeatherMap API key is missing. Check your .env file.");
            return;
        }

        string url = _urlBuilder.BuildByCoordinates(latitude, longitude, apiKey);

        StartCoroutine(_requestSender.Get(url, response => HandleResponse(response, onSuccess, onFailure)));
    }

    private void HandleResponse(WebResponse response, Action<WeatherData> onSuccess, Action<string> onFailure)
    {
        if (response == null || !response.IsSuccess)
        {
            onFailure?.Invoke(_responseParser.BuildErrorMessage(response));
            return;
        }

        if (_responseParser.TryParse(response.Body, out WeatherData data, out string error))
        {
            onSuccess?.Invoke(data);
        }
        else
        {
            onFailure?.Invoke(error);
        }
    }

    private static bool AreCoordinatesValid(float latitude, float longitude)
    {
        return !float.IsNaN(latitude) && !float.IsNaN(longitude)
               && latitude >= -90f && latitude <= 90f
               && longitude >= -180f && longitude <= 180f;
    }

    private void EnsureDefaults()
    {
        if (_apiKeyProvider == null) _apiKeyProvider = new EnvApiKeyProvider();
        if (_requestSender == null) _requestSender = new UnityWebRequestSender();
        if (_urlBuilder == null) _urlBuilder = new OpenWeatherUrlBuilder();
        if (_responseParser == null) _responseParser = new OpenWeatherResponseParser();
    }
}