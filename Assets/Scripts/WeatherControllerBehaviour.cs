using TMPro;
using UnityEngine;


public class WeatherControllerBehaviour : MonoBehaviour
{
    [Header("Skybox materials")]


    [SerializeField] private AudioSource rainAudio;
    [SerializeField] private Material clearDaySky;
    [SerializeField] private Material clearNightSky;
    [SerializeField] private Material cloudyDaySky;
    [SerializeField] private Material cloudyNightSky;

    [Header("Light ")]
    [SerializeField] private Light sun;
    
    [SerializeField, Range(0f, 1f)] private float nightLightMultiplier = 0.15f;

    [Header("Particle systems)")]
    [SerializeField] private ParticleSystem rain;
    [SerializeField] private ParticleSystem snow;
    [SerializeField] private ParticleSystem clouds;

    [Header("Cloud colours")]
    [SerializeField] private Color cloudDayColor = new Color(1f, 1f, 1f, 0.3f);
    [SerializeField] private Color cloudNightColor = new Color(0.25f, 0.28f, 0.4f, 0.3f);



    private Material _sceneSky;            
    private float _sceneLightIntensity = 1f; 

    
    private Material DefaultSky => clearDaySky != null ? clearDaySky : _sceneSky;

    private void Awake()
    {
        _sceneSky = RenderSettings.skybox;

        if (sun != null)
        {
            _sceneLightIntensity = sun.intensity;
        }
    }

    private void Start()
    {
        ResetToDefault();
    }

  
    public void UpdateEnvironment(WeatherData data)
    {
        bool hasWeather = data != null && data.weather != null && data.weather.Length > 0;
        if (!hasWeather)
        {
            ResetToDefault();
            return;
        }

        Apply(data.weather[0].id, CheckIsDay(data));
    }

   
    public void ResetToDefault()
    {
        Apply(0, true);
    }

    private void Apply(int code, bool isDay)
    {
        
        bool isRain = code >= 200 && code < 600;    
        bool isSnow = code >= 600 && code < 700;
        bool isMist = code >= 700 && code < 800;
        bool isCloudy = code > 800 && code < 900;
        bool isOvercast = isRain || isSnow || isMist || isCloudy;

        SetParticles(rain, isRain);

        if (rainAudio != null)
        {
            if (isRain)
            {
                if (!rainAudio.isPlaying)
                {
                    rainAudio.Play();
                }
            }
            else
            {
                rainAudio.Stop();
            }
        }
        SetParticles(snow, isSnow);
        SetClouds(isOvercast, isDay);

        
        Material sky = isOvercast
            ? (isDay ? cloudyDaySky : cloudyNightSky)
            : (isDay ? clearDaySky : clearNightSky);
        if (sky == null) sky = DefaultSky;

        if (sky != null && RenderSettings.skybox != sky)
        {
            RenderSettings.skybox = sky;
            DynamicGI.UpdateEnvironment(); 
        }

        
        if (sun != null)
        {
            sun.intensity = _sceneLightIntensity * (isDay ? 1f : nightLightMultiplier);
        }

    }

    private void SetParticles(ParticleSystem system, bool on)
    {
        if (system == null) return;

        if (on && !system.isPlaying)
        {
            system.Play();
        }
        else if (!on)
        {
            system.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }
    }

    private void SetClouds(bool on, bool isDay)
    {
        if (clouds == null) return;

        if (!on)
        {
            clouds.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            return;
        }

        var main = clouds.main;
        main.startColor = isDay ? cloudDayColor : cloudNightColor;

        clouds.Clear();
        clouds.Play();
    }


    [ContextMenu("Test Clear Day")] private void TestClearDay() { Apply(800, true); }
    [ContextMenu("Test Clear Night")] private void TestClearNight() { Apply(800, false); }
    [ContextMenu("Test Cloudy Day")] private void TestCloudyDay() { Apply(803, true); }
    [ContextMenu("Test Cloudy Night")] private void TestCloudyNight() { Apply(803, false); }
    [ContextMenu("Test Rain Day")] private void TestRainDay() { Apply(501, true); }
    [ContextMenu("Test Rain Night")] private void TestRainNight() { Apply(501, false); }
    [ContextMenu("Test Snow Day")] private void TestSnowDay() { Apply(601, true); }
    [ContextMenu("Test Snow Night")] private void TestSnowNight() { Apply(601, false); }
    [ContextMenu("Test Mist Day")] private void TestMistDay() { Apply(741, true); }

   
    private static bool CheckIsDay(WeatherData data)
    {
        string icon = data.weather[0].icon;

        
        if (!string.IsNullOrEmpty(icon))
        {
            if (icon.EndsWith("n")) return false;
            if (icon.EndsWith("d")) return true;
        }

        
        if (data.sys != null && data.sys.sunrise > 0 && data.sys.sunset > data.sys.sunrise)
        {
            return data.dt >= data.sys.sunrise && data.dt < data.sys.sunset;
        }

        return true;
    }

}