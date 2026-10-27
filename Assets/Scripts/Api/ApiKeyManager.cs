using System;
using UnityEngine;

public class ApiKeyManager : MonoBehaviour
{
    void Awake()
    {
        EnvLoader.Load();

        string apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");

        if (!string.IsNullOrEmpty(apiKey))
        {
            Debug.Log("API Key loaded!");
        }
        else
        {
            Debug.LogError("Failed to load API Key.");
        }
    }
}
