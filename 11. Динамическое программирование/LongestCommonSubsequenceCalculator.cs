using System;
using System.Collections.Generic;

namespace Antiplagiarism
{
    public static class LongestCommonSubsequenceCalculator
    {
        // Метод для вычисления наибольшей общей подпоследовательности между двумя списками токенов
        public static List<string> Calculate(List<string> first, List<string> second) =>
            RestoreAnswer(CreateOptimizationTable(first, second), first, second);

        // Метод для создания таблицы оптимизации, которая хранит длины наибольших общих подпоследовательностей
        private static int[,] CreateOptimizationTable(List<string> first, List<string> second)
        {
            // Создаем двумерный массив для хранения длины наибольшей общей подпоследовательности
            var opt = new int[first.Count + 1, second.Count + 1];

            // Заполняем таблицу оптимизации начиная с конца списков
            for (var i = first.Count - 1; i >= 0; i--)
                for (var j = second.Count - 1; j >= 0; j--)
                    // Если токены совпадают, увеличиваем длину на 1
                    if (first[i] == second[j])
                        opt[i, j] = opt[i + 1, j + 1] + 1;
                    else
                        // Иначе берем максимальное значение из соседних ячеек
                        opt[i, j] = Math.Max(opt[i + 1, j], opt[i, j + 1]);

            // Возвращаем заполненную таблицу
            return opt;
        }

        // Метод для восстановления самой наибольшей общей подпоследовательности из таблицы оптимизации
        private static List<string> RestoreAnswer(int[,] opt, List<string> first, List<string> second)
        {
            var result = new List<string>(); // Список для хранения результата
            var x = 0; // Индекс для первого списка
            var y = 0; // Индекс для второго списка

            // Проходим по таблице, пока не достигнем конца одного из списков
            while (x != first.Count && y != second.Count)
                // Если токены совпадают, добавляем токен в результат
                if (first[x] == second[y])
                {
                    result.Add(first[x]); // Добавляем токен в результат
                    x++; y++; // Переходим к следующему токену в обоих списках
                }
                // Если значения в текущей ячейке равны значению ниже, двигаемся по первому списку
                else if (opt[x, y] == opt[x + 1, y]) x++;
                // Иначе двигаемся по второму списку
                else y++;

            // Возвращаем найденную подпоследовательность
            return result;
        }
    }
}
