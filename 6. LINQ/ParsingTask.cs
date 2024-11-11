using System;
using System.Collections.Generic;
using System.Linq;

namespace linq_slideviews;

public class ParsingTask
{
    /// <param name="lines">все строки файла, которые нужно распарсить. Первая строка заголовочная.</param>
    /// <returns>Словарь: ключ — идентификатор слайда, значение — информация о слайде</returns>
    /// <remarks>Метод должен пропускать некорректные строки, игнорируя их</remarks>
    public static IDictionary<int, SlideRecord> ParseSlideRecords(IEnumerable<string> lines)
    {
        return lines
            .Select(x => x.Split(';')) //делим на ячейки 
            .Where(x => x.Length == 3 && x[1].Length > 0 &&
            Enum.TryParse(
                string.Concat(x[1][..1].ToUpper(), x[1].AsSpan(1)),
                out SlideType _)) // отбираем только строки длинной 3, содержащие название темы и входяшие в Enum
            .Select(x => ParseSlideRecord(x)) // формируем список Records
            .Where(x => x != null) // убираем нулевые значения
            .ToDictionary(x => x.SlideId, x => x); // конвертируем в словарь
    }

    public static SlideRecord ParseSlideRecord(string[] line)
    {
        if (!int.TryParse(line[0], out int slideId)) // если не получается получить число - возвращаем null
            return null;
        _ = Enum.TryParse(string.Concat(line[1][..1].ToUpper(), line[1].AsSpan(1)),
            out SlideType slideType); // создаем SlydeType
        var unitTitle = line[2]; 
        return new SlideRecord(slideId, slideType, unitTitle);
    }

    /// <param name="lines">все строки файла, которые нужно распарсить. Первая строка — заголовочная.</param>
    /// <param name="slides">Словарь информации о слайдах по идентификатору слайда. 
    /// Такой словарь можно получить методом ParseSlideRecords</param>
    /// <returns>Список информации о посещениях</returns>
    /// <exception cref="FormatException">Если среди строк есть некорректные</exception>
    public static IEnumerable<VisitRecord> ParseVisitRecords
    (IEnumerable<string> lines, IDictionary<int, SlideRecord> slides)
    {
        return lines.Skip(1)
        .Select(s => ParseVisit(s, slides)); //пропускаем первую строку - заголовоки,
                                             //Возвращает список информации о посещениях.
    }

    // Метод для парсинга одной строки посещений.
    // Возвращает объект VisitRecord или выбрасывает исключение, если строка некорректна.
    private static VisitRecord ParseVisit(string line, IDictionary<int, SlideRecord> slides)
    {
        var str = line.Split(';');


        //проверка на корректность строки
        if (!(str.Length == 4) ||
            !int.TryParse(str[0], out int userId) ||
            !int.TryParse(str[1], out int slideId) ||
            !DateTime.TryParse(str[2], out DateTime _) ||
            !DateTime.TryParse(str[2], out DateTime _) ||
            !DateTime.TryParse(str[3], out DateTime _) ||
            !slides.ContainsKey(slideId))
            throw new FormatException("Wrong line [" + line + "]");

        return new VisitRecord(userId,
                slideId, DateTime.Parse($"{str[2]} {str[3]}"),
                slides[slideId].SlideType);
    }
}