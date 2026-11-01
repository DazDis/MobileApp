using SQLite;
using System;

public class HabitLogData
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [Indexed] 
    public int HabitId { get; set; }

    public int DayNumber { get; set; } 

    public int RelapseCount { get; set; } 

    public string PhotoPath { get; set; }

    public long DateUnixTime { get; set; } 
}