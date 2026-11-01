using System;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class WeatherUiController : MonoBehaviour
{
    private const string PLACEHOLDER_OPTION = "Select a location...";

    [Header("Weather service (any component that implements IWeatherService)")]
    [SerializeField] private MonoBehaviour weatherServiceSource;

    [Header("Display")]
    [SerializeField] private WeatherView weatherView;

    [Header("Predefined locations")]
    [SerializeField] private TMP_Dropdown locationDropdown;

    
    [SerializeField]
    private List<LocationPreset> presets = new List<LocationPreset>
    {
        new LocationPreset("Kigali, Rwanda",        -1.9441f,   30.0619f),  // UTC+2, tropical highland
        new LocationPreset("Tokyo, Japan",          35.6764f,  139.6500f),  // UTC+9, humid temperate
        new LocationPreset("Reykjavik, Iceland",    64.1466f,  -21.9426f),  // UTC+0, cold and windy
        new LocationPreset("Dubai, UAE",            25.2048f,   55.2708f),  // UTC+4, hot desert
        new LocationPreset("New York, USA",         40.7128f,  -74.0060f),  // UTC-5/-4, continental
        new LocationPreset("Sydney, Australia",    -33.8688f,  151.2093f)   // UTC+10/+11, southern hemisphere
    };

    [Header("Custom coordinates")]
    [SerializeField] private TMP_InputField latitudeInput;
    [SerializeField] private TMP_InputField longitudeInput;
    [SerializeField] private Button getWeatherButton;

    private IWeatherService _weatherService;
    private ICoordinateValidator _validator;
    private int _requestId;

  
    public void Initialize(IWeatherService weatherService, ICoordinateValidator validator)
    {
        _weatherService = weatherService ?? throw new ArgumentNullException(nameof(weatherService));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
    }

    private void Start()
    {
        if (!AllReferencesAssigned())
        {
            Debug.LogError("[WeatherUI] Some fields are not assigned in the Inspector.", this);
            return;
        }

        if (_weatherService == null) _weatherService = ResolveWeatherService();
        if (_validator == null) _validator = new CoordinateValidator();

        PopulateDropdown();

        locationDropdown.onValueChanged.AddListener(OnLocationSelected);
        getWeatherButton.onClick.AddListener(OnGetWeatherClicked);

        if (_weatherService == null)
        {
            weatherView.ShowRequestError("No weather service was found in the scene.");
        }
    }

    private void OnDestroy()
    {
        _requestId++; 

        if (locationDropdown != null) locationDropdown.onValueChanged.RemoveListener(OnLocationSelected);
        if (getWeatherButton != null) getWeatherButton.onClick.RemoveListener(OnGetWeatherClicked);
    }



    private void PopulateDropdown()
    {
        
        var options = new List<string> { PLACEHOLDER_OPTION };
        foreach (LocationPreset preset in presets)
        {
            options.Add(preset.displayName);
        }

        locationDropdown.ClearOptions();
        locationDropdown.AddOptions(options);
        locationDropdown.SetValueWithoutNotify(0);
        locationDropdown.RefreshShownValue();
    }

    private void OnLocationSelected(int optionIndex)
    {
        int presetIndex = optionIndex - 1; 
        if (presetIndex < 0 || presetIndex >= presets.Count) return;

        LocationPreset preset = presets[presetIndex];

        weatherView.ClearError();
        RequestWeather(preset.latitude, preset.longitude, preset.displayName);
    }



    private void OnGetWeatherClicked()
    {
        CoordinateValidationResult result = _validator.Validate(latitudeInput.text, longitudeInput.text);

        if (!result.IsValid)
        {
            weatherView.ShowValidationError(result.ErrorMessage);
            return;
        }

        weatherView.ClearError();

      
        locationDropdown.SetValueWithoutNotify(0);

        RequestWeather(result.Latitude, result.Longitude, null);
    }



    private void RequestWeather(float latitude, float longitude, string fixedTitle)
    {
        if (_weatherService == null)
        {
            weatherView.ShowRequestError("No weather service was found in the scene.");
            return;
        }

   
        int requestId = ++_requestId;

        weatherView.ShowLoading("Fetching weather...");

        _weatherService.GetCurrentWeather(latitude, longitude,
            onSuccess: data =>
            {
                if (requestId != _requestId) return;

                string title = fixedTitle ?? BuildCustomTitle(data, latitude, longitude);
                weatherView.ShowWeather(data, title);
            },
            onFailure: error =>
            {
                if (requestId != _requestId) return;

                weatherView.ShowRequestError(error);
            });
    }

    private static string BuildCustomTitle(WeatherData data, float latitude, float longitude)
    {
        return $"{data.name} ({FormatCoordinate(latitude)}, {FormatCoordinate(longitude)})";
    }

    private static string FormatCoordinate(float value)
    {
        return value.ToString("0.####", CultureInfo.InvariantCulture);
    }



    private IWeatherService ResolveWeatherService()
    {
        if (weatherServiceSource is IWeatherService assigned)
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

    private bool AllReferencesAssigned()
    {
        return weatherView != null
               && locationDropdown != null
               && latitudeInput != null
               && longitudeInput != null
               && getWeatherButton != null;
    }
}
