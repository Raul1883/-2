using System.Numerics;

namespace Tickets
{

    /* Задача тупо на математику и алгоритмы, синтаксически здесь ничего сложного нет.
    Вот тут https://www.ega-math.narod.ru/Quant/Tickets.htm можно посмотреть на её объяснение. 
    Там и табличка есть для решения
     */
    internal class TicketsTask
    {
        // Метод для вычисления количества "счастливых" билетов с заданной суммой цифр
        public static BigInteger Solve(int halfLen, int totalSum)
        {
            // Если общая сумма нечетная, то не может быть "счастливых" билетов
            if (totalSum % 2 != 0) return 0;

            // Находим количество способов распределить сумму среди первой половины
            var sqrtResult = FindCount(halfLen, totalSum / 2);
            // Возвращаем квадрат этого количества, так как обе половины должны быть равны
            return sqrtResult * sqrtResult;
        }

        // Метод для нахождения количества способов распределить сумму среди halfLen разрядов
        private static BigInteger FindCount(int halfLen, int halfSum)
        {
            // Создаем двумерный массив для хранения количества способов для каждой длины и суммы
            var opt = new BigInteger[halfLen + 1, halfSum + 1];

            // Инициализируем базовые случаи: 1 способ для суммы 0 с любой длиной
            for (var i = 1; i <= halfLen; i++)
                opt[i, 0] = 1;

            // Инициализация: 0 способов для любой ненулевой суммы с длиной 0
            for (var i = 0; i <= halfSum; i++)
                opt[0, i] = 0;

            // Заполняем таблицу оптимизации
            for (var l = 1; l <= halfLen; l++)
                for (var s = 1; s <= halfSum; s++)
                {
                    // Если сумма больше максимальной возможной (l * 9), то 0 способов
                    if (s > l * 9)
                        opt[l, s] = 0;
                    else
                    {
                        // Количество способов без добавления новой цифры + количество способов с добавлением новой цифры
                        opt[l, s] = opt[l - 1, s] + opt[l, s - 1];
                        // Если сумма больше 9, вычитаем случаи, когда последняя цифра больше 9
                        if (s > 9)
                            opt[l, s] -= opt[l - 1, s - 10];
                    }
                }

            // Возвращаем количество способов для заданной длины и суммы
            return opt[halfLen, halfSum];
        }
    }
}
