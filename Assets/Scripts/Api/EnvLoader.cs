using System;
using System.IO;
using UnityEngine;

public static class EnvLoader
{
    public static void Load()
    {
        string filePath = Path.Combine(Application.dataPath, "../.env");

        if (!File.Exists(filePath))
        {
            Debug.LogWarning(".env file not found at project root!");
            return;
        }

        foreach (var line in File.ReadAllLines(filePath))
        {
            if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#"))
                continue;

            string[] parts = line.Split(new[] { '=' }, 2);
            if (parts.Length == 2)
            {
                string key = parts[0].Trim();
                string value = parts[1].Trim();

                Environment.SetEnvironmentVariable(key, value);
            }
        }
    }
}

