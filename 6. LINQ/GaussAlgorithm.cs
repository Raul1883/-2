using System;
using System.Linq;

namespace GaussAlgorithm
{
    public class Solver
    {
        private double[][] Matrix; // Матрица коэффициентов
        private double[] FreeMembers; // Массив свободных членов
        public int LinesCount => Matrix.Length; // Количество строк в матрице
        public int ColumnsCount => LinesCount > 0 ? Matrix[0].Length : 0; // Количество столбцов в матрице
        private int UsedColumnsCount; // Количество использованных столбцов
        private bool[][] DependentVars; // Массив, указывающий, какие переменные зависят от уравнений
        private int DependentVarsCount; // Количество зависимых переменных

        public double[] Solve(double[][] matrix, double[] freeMembers)
        {
            Matrix = matrix; // Инициализация матрицы коэффициентов
            FreeMembers = freeMembers; // Инициализация массива свободных членов

            // Инициализируем массив зависимых переменных
            DependentVars = new bool[LinesCount][];
            for (var line = 0; line < LinesCount; line++)
                DependentVars[line] = new bool[ColumnsCount];

            // Основной цикл, который использует столбцы матрицы
            while (UsedColumnsCount < ColumnsCount)
                UseColumn(DependentVarsCount, UsedColumnsCount);

            // Проверка на наличие решений
            for (int line = 0; line < LinesCount; line++)
                if (IsLineNull(line) && freeMembers[line] != 0)
                    throw new NoSolutionException("NoSolutionException");

            // Получаем ответ на систему уравнений
            return GetAnswer(matrix, freeMembers);
        }

        private double[] GetAnswer(double[][] matrix, double[] freeMembers)
        {
            var answer = new double[ColumnsCount]; // Массив для хранения решения
            var definedVars = new bool[ColumnsCount]; // Массив для отслеживания определенных переменных

            // Обратный проход по строкам для нахождения значений переменных
            for (var line = LinesCount - 1; line >= 0; line--)
            {
                if (IsLineNull(line))
                    continue; // Пропускаем нулевые строки

                var DependentFlag = false; // Флаг для отслеживания зависимых переменных
                for (var column = ColumnsCount - 1; column >= 0 && !DependentFlag; column--)
                {
                    if (DependentVars[line][column]) // Если переменная зависима
                    {
                        var sum = 0.0; // Сумма для вычисления текущей переменной
                        for (var i = column + 1; i < ColumnsCount; i++)
                            sum += matrix[line][i] * answer[i]; // Вычисляем сумму

                        // Находим значение переменной
                        var tmp = (freeMembers[line] - sum) / matrix[line][column];
                        answer[column] = tmp; // Сохраняем значение переменной

                        DependentFlag = true; // Устанавливаем флаг
                    }
                    else if (!definedVars[column]) // Если переменная не определена
                        answer[column] = 0; // Присваиваем 0

                    definedVars[column] = true; // Отмечаем переменную как определенную
                }
            }
            return answer; // Возвращаем массив значений переменных
        }

        private bool IsLineNull(int line) => Matrix[line].All(x => x == 0); // Проверка, является ли строка нулевой

        private void UseColumn(int lineIndex, int columnIndex)
        {
            if (UsedColumnsCount == ColumnsCount) // Если все столбцы использованы
                return;

            if (lineIndex >= LinesCount) // Если индекс строки выходит за пределы
            {
                UsedColumnsCount++;
                return;
            }

            var resolvingElement = Matrix[lineIndex][columnIndex]; // Находим разрешающий элемент
            if (resolvingElement == 0) // Если элемент равен нулю
            {
                UseColumn(lineIndex + 1, columnIndex); // Переходим к следующей строке
                return;
            }

            MultiplyLine(lineIndex, 1 / resolvingElement); // Нормализуем строку
            for (var row = 0; row < LinesCount; row++) // Обнуляем текущий столбец в других строках
            {
                if (row == lineIndex) continue; // Пропускаем текущую строку
                AddMultipliedLine(row, lineIndex, -Matrix[row][columnIndex]); // Обнуляем элемент
            }

            DependentVars[lineIndex][columnIndex] = true; // Отмечаем переменную как зависимую
            if (lineIndex != UsedColumnsCount) // Если строка не на месте
                SwitchLines(lineIndex, UsedColumnsCount); // Меняем строки местами

            UsedColumnsCount++; // Увеличиваем счетчик использованных столбцов
            DependentVarsCount++; // Увеличиваем счетчик зависимых переменных
        }

        public void MultiplyLine(int Index, double multiplier)
        {
            Matrix[Index] = Matrix[Index].Select(x => x * multiplier).ToArray(); // Умножаем строку на множитель
            FreeMembers[Index] *= multiplier; // Умножаем свободный член
        }

        public void AddMultipliedLine(int Index, int addIndex, double multiplier)
        {
            for (var i = 0; i < ColumnsCount; i++)
            {
                Matrix[Index][i] += multiplier * Matrix[addIndex][i]; // Обновляем строку
                if (Math.Abs(Matrix[Index][i]) < 1e-6) // Проверка на близость к нулю
                    Matrix[Index][i] = 0;
            }
            FreeMembers[Index] += multiplier * FreeMembers[addIndex]; // Обновляем свободный член
            if (Math.Abs(FreeMembers[Index]) < 1e-6) // Проверка на близость к нулю
                FreeMembers[Index] = 0;
        }

        public void SwitchLines(int i, int j)
        {
            if (i < 0 || i >= LinesCount || j < 0 || j >= LinesCount) // Проверка на корректность индексов
                return;

            (Matrix[j], Matrix[i]) = (Matrix[i], Matrix[j]); // Меняем строки местами
            (FreeMembers[j], FreeMembers[i]) = (FreeMembers[i], FreeMembers[j]); // Меняем свободные члены местами
            (DependentVars[j], DependentVars[i]) = (DependentVars[i], DependentVars[j]); // Меняем зависимости местами
        }
    }
}
