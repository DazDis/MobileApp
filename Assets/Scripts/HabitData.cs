using System;

[Serializable]
public class HabitData
{
    public string Id;
    public string Name;
    public string Description;
    public long StartUnixTime;  

    public TimeSpan Elapsed
    {
        get
        {
            long now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            long diff = now - StartUnixTime;
            return TimeSpan.FromSeconds(diff > 0 ? diff : 0);
        }
    }
}