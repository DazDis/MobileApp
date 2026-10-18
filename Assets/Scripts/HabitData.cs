using SQLite;
using System;
using UnityEngine;

public class HabitData
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public string Name { get; set; }
    public string Description { get; set; }
    public long StartUnixTime { get; set; }

    [Ignore]
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