using System;

public interface IApiKeyProvider
{
    string GetApiKey();
}

public class EnvApiKeyProvider : IApiKeyProvider
{
    private const string KEY_NAME = "OPENWEATHER_API_KEY";
    private bool _envLoaded;

    public string GetApiKey()
    {
        if (!_envLoaded)
        {
            EnvLoader.Load();
            _envLoaded = true;
        }

        return Environment.GetEnvironmentVariable(KEY_NAME);
    }
}