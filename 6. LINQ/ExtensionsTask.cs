using System;
using System.Collections.Generic;
using System.Linq;

namespace linq_slideviews;

public static class ExtensionsTask
{
    /// <summary>
    /// Медиана списка из нечетного количества элементов — это серединный элемент списка после сортировки.
    /// Медиана списка из четного количества элементов — это среднее арифметическое 
    /// двух серединных элементов списка после сортировки.
    /// </summary>
    /// <exception cref="InvalidOperationException">Если последовательность не содержит элементов</exception>
    public static double Median(this IEnumerable<double> items)
    {
        // Преобразование входной последовательности в список для дальнейших операций
        var valueList = items.ToList();

        // Проверка на наличие элементов в последовательности
        if (valueList.Count == 0)
            throw new InvalidOperationException("The sequence contains no elements.");

        // Сортировка списка и вычисление медианы
        if (valueList.Count % 2 != 0)
            // Для нечетного количества элементов возвращаем серединный элемент
            return valueList.OrderBy(x => x)
                .ElementAt(valueList.Count / 2);

        // Для четного количества элементов возвращаем среднее арифметическое двух серединных элементов
        return valueList.OrderBy(x => x)
            .Skip(valueList.Count / 2 - 1)
            .Take(2)
            .Average();
    }

    /// <returns>
    /// Возвращает последовательность, состоящую из пар соседних элементов.
    /// Например, по последовательности {1,2,3} метод должен вернуть две пары: (1,2) и (2,3).
    /// </returns>
    public static IEnumerable<(T First, T Second)> Bigrams<T>(this IEnumerable<T> items)
    {
        // Переменная для хранения предыдущего элемента
        var prevItem = default(T);
        var flag = true; // Флаг для отслеживания первого элемента

        // Перебор элементов в последовательности
        foreach (var item in items)
        {
            if (flag)
            {
                prevItem = item; // Сохраняем первый элемент
                flag = false; // Устанавливаем флаг в false
                continue; // Переходим к следующему элементу
            }

            // Возвращаем пару (предыдущий, текущий элемент)
            yield return (prevItem, item);
            prevItem = item; // Обновляем предыдущий элемент
        }
    }
}
