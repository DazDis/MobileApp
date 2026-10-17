using System;

public static class TimeFormatter
{
    public static string Format(TimeSpan t)
    {
        if (t.TotalDays >= 1)
            return $"{(int)t.TotalDays} {Plural((int)t.TotalDays, "день", "дня", "дней")} {t.Hours} ч {t.Minutes} мин";
        if (t.TotalHours >= 1)
            return $"{(int)t.TotalHours} ч {t.Minutes} мин {t.Seconds} с";
        return $"{t.Minutes} мин {t.Seconds} с";
    }

    private static string Plural(int n, string one, string few, string many)
    {
        int m100 = n % 100, m10 = n % 10;
        if (m100 >= 11 && m100 <= 14) return many;
        if (m10 == 1) return one;
        if (m10 >= 2 && m10 <= 4) return few;
        return many;
    }
}