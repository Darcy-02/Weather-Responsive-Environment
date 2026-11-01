using System;


[Serializable]
public class LocationPreset
{
    public string displayName;
    public float latitude;
    public float longitude;

    public LocationPreset() { }

    public LocationPreset(string displayName, float latitude, float longitude)
    {
        this.displayName = displayName;
        this.latitude = latitude;
        this.longitude = longitude;
    }
}
