using System.Collections.Generic;
using System.Globalization;


public readonly struct CoordinateValidationResult
{
    public bool IsValid { get; }
    public float Latitude { get; }
    public float Longitude { get; }
    public string ErrorMessage { get; }

    private CoordinateValidationResult(bool isValid, float latitude, float longitude, string errorMessage)
    {
        IsValid = isValid;
        Latitude = latitude;
        Longitude = longitude;
        ErrorMessage = errorMessage;
    }

    public static CoordinateValidationResult Success(float latitude, float longitude)
    {
        return new CoordinateValidationResult(true, latitude, longitude, null);
    }

    public static CoordinateValidationResult Failure(string errorMessage)
    {
        return new CoordinateValidationResult(false, 0f, 0f, errorMessage);
    }
}


public interface ICoordinateValidator
{
    CoordinateValidationResult Validate(string latitudeText, string longitudeText);
}

public class CoordinateValidator : ICoordinateValidator
{
    public const float MIN_LATITUDE = -90f;
    public const float MAX_LATITUDE = 90f;
    public const float MIN_LONGITUDE = -180f;
    public const float MAX_LONGITUDE = 180f;

    private const NumberStyles ALLOWED_STYLES = NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint;

    public CoordinateValidationResult Validate(string latitudeText, string longitudeText)
    {
        var errors = new List<string>();

        bool latitudeOk = TryReadValue(latitudeText, "Latitude", MIN_LATITUDE, MAX_LATITUDE,
            out float latitude, out string latitudeError);
        if (!latitudeOk) errors.Add(latitudeError);

        bool longitudeOk = TryReadValue(longitudeText, "Longitude", MIN_LONGITUDE, MAX_LONGITUDE,
            out float longitude, out string longitudeError);
        if (!longitudeOk) errors.Add(longitudeError);

       
        if (errors.Count > 0)
        {
            return CoordinateValidationResult.Failure(string.Join("\n", errors));
        }

        return CoordinateValidationResult.Success(latitude, longitude);
    }

    private static bool TryReadValue(
        string text,
        string fieldName,
        float min,
        float max,
        out float value,
        out string error)
    {
        value = 0f;
        error = null;

        // 1) Empty field
        if (string.IsNullOrWhiteSpace(text))
        {
            error = $"{fieldName} is required.";
            return false;
        }

       
        if (!float.TryParse(Normalize(text), ALLOWED_STYLES, CultureInfo.InvariantCulture, out float parsed)
            || float.IsNaN(parsed)
            || float.IsInfinity(parsed))
        {
            error = $"{fieldName} must be a valid number (for example 35.6764).";
            return false;
        }

        
        if (parsed < min || parsed > max)
        {
            error = $"{fieldName} must be between {Format(min)} and {Format(max)}.";
            return false;
        }

        value = parsed;
        return true;
    }

    private static string Normalize(string text)
    {
        return text.Trim()
                   .Replace('\u2212', '-') 
                   .Replace(',', '.');     
    }

    private static string Format(float limit)
    {
        return limit.ToString("0", CultureInfo.InvariantCulture);
    }
}
