using System;
using System.Collections.Generic;
using System.Linq;
namespace linq_slideviews;

public class StatisticsTask
{
    public static double GetMedianTimePerSlide(List<VisitRecord> visits, SlideType slideType)
    {
        // Определяем минимальное и максимальное время для фильтрации значений.
        var minTime = TimeSpan.FromMinutes(1); // Минимальное время: 1 минута.
        var maxTime = TimeSpan.FromHours(2);   // Максимальное время: 2 часа.

        // Начинаем основную логику метода.
        return visits.OrderBy(x => x.DateTime) // Сортируем записи посещений по времени.
            .GroupBy(x => x.UserId) // Группируем записи по идентификатору пользователя.
            .SelectMany(group => group.Bigrams() // Для каждой группы (пользователя) получаем пары посещений (биграммы).
            .Where(x => x.First.SlideType == slideType)) // Фильтруем только те пары, где первый слайд соответствует указанному типу.
            .Select(x => x.Second.DateTime.Subtract(x.First.DateTime)) // Вычисляем время между первым и вторым посещением в паре.
            .Where(x => x >= minTime && x <= maxTime) // Фильтруем результаты, оставляя только те, которые находятся в пределах от 1 минуты до 2 часов.
            .Select(x => x.TotalMinutes) // Преобразуем время в минуты.
            .DefaultIfEmpty(0) // Если после всех фильтров нет значений, возвращаем 0 (чтобы избежать исключений при расчете медианы).
            .Median(); // Вычисляем медиану полученных значений времени в минутах.
    }
}