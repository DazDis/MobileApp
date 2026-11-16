using System.IO;
using System.Text;
using System.Collections.Generic;
using UnityEngine;

public static class HabitExporter
{
    public static string ExportToCSV(HabitData habit, List<HabitLogData> logs)
    {
        // 1. Создаем имя файла (убираем недопустимые символы)
        string safeName = string.Join("_", habit.Name.Split(Path.GetInvalidFileNameChars()));
        string fileName = $"Stats_{safeName}_{System.DateTime.Now:yyyy-MM-dd_HH-mm}.csv";
        string filePath = Path.Combine(Application.persistentDataPath, fileName);

        // 2. Строим содержимое файла
        StringBuilder sb = new StringBuilder();

        // Заголовки колонок (разделитель - точка с запятой, так как Excel в русской локали ждет именно её)
        sb.AppendLine("День;Дата;Количество срывов;Статус");

        foreach (var log in logs)
        {
            // Переводим Unix время в читаемую дату
            System.DateTime dt = System.DateTimeOffset.FromUnixTimeSeconds(log.DateUnixTime).LocalDateTime;

            string status = log.RelapseCount == 0 ? "Без срывов" : "Был срыв";

            // Если в названии есть точки с запятой, Excel может сломать таблицу, поэтому экранируем кавычками
            sb.AppendLine($"{log.DayNumber};{dt:yyyy-MM-dd HH:mm};{log.RelapseCount};{status}");
        }

        // 3. Записываем в файл с UTF-8 BOM (чтобы Excel понял русский язык)
        try
        {
            File.WriteAllText(filePath, sb.ToString(), new UTF8Encoding(true));
            Debug.Log($"[Экспорт] Файл успешно сохранен: {filePath}");
            return filePath;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[Экспорт] Ошибка сохранения: {e.Message}");
            return null;
        }
    }
}