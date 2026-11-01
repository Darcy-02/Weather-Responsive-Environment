using TMPro;
using UnityEngine;


public class WeatherControllerBehaviour : MonoBehaviour
{
    [Header("Light")]
    [SerializeField] private Light sun;

    [Header("Skyboxes (Skybox/Procedural materials)")]
    [SerializeField] private Material clearDaySky;
    [SerializeField] private Material clearNightSky;
    [SerializeField] private Material cloudyDaySky;
    [SerializeField] private Material cloudyNightSky;

    [Header("Particle systems (turn Play On Awake OFF)")]
    [SerializeField] private ParticleSystem rain;
    [SerializeField] private ParticleSystem snow;
    [SerializeField] private ParticleSystem clouds;

    [Header("Optional")]
    [SerializeField] private TMP_Text statusText;

    private void Start()
    {
        RenderSettings.fog = true;
        RenderSettings.fogMode = FogMode.Exponential;
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
        SetParticles(snow, isSnow);
        SetClouds(isOvercast, isDay, isRain);


        RenderSettings.skybox = isOvercast
            ? (isDay ? cloudyDaySky : cloudyNightSky)
            : (isDay ? clearDaySky : clearNightSky);
        DynamicGI.UpdateEnvironment(); // refresh the ambient light from the new sky

        if (sun != null)
        {
            sun.intensity = (isDay ? 1.1f : 0.15f) * (isOvercast ? 0.5f : 1f);
            sun.color = isDay ? new Color(1f, 0.96f, 0.86f) : new Color(0.5f, 0.6f, 0.9f);
        }


        Color fog = isOvercast ? new Color(0.62f, 0.64f, 0.67f) : new Color(0.72f, 0.82f, 0.92f);
        if (!isDay) fog *= 0.12f;
        RenderSettings.fogColor = fog;
        RenderSettings.fogDensity = isMist ? 0.03f : isRain ? 0.012f : isSnow ? 0.01f : isCloudy ? 0.005f : 0.002f;

       
        if (statusText != null)
        {
            string kind = isRain ? "Rain" : isSnow ? "Snow" : isMist ? "Mist" : isCloudy ? "Cloudy" : code == 800 ? "Clear" : "Default";
            statusText.text = (isDay ? "Day" : "Night") + " | " + kind;
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

    private void SetClouds(bool on, bool isDay, bool isRain)
    {
        if (clouds == null) return;

        if (!on)
        {
            clouds.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            return;
        }


        Color color = isDay ? new Color(1f, 1f, 1f, 0.3f) : new Color(0.25f, 0.28f, 0.4f, 0.3f);
        if (isRain)
        {
            color = Color.Lerp(color, Color.black, 0.4f);
            color.a = 0.3f;
        }

        var main = clouds.main;
        main.startColor = color;

        clouds.Clear();
        clouds.Play();
    }


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