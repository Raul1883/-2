using System;
using System.Collections.Generic;
using DocumentTokens = System.Collections.Generic.List<string>;

namespace Antiplagiarism
{
    public class LevenshteinCalculator
    {
        // Метод для попарного сравнения документов
        public List<ComparisonResult> CompareDocumentsPairwise(List<DocumentTokens> documents)
        {
            // Список для хранения результатов сравнения
            var result = new List<ComparisonResult>();

            // Внешний цикл по всем документам
            for (var i = 0; i < documents.Count; i++)
                // Внутренний цикл для сравнения текущего документа с остальными
                for (var j = i + 1; j < documents.Count; j++)
                    // Добавляем результат сравнения двух документов в список
                    result.Add(CompareToken(documents[i], documents[j]));

            // Возвращаем список результатов
            return result;
        }

        // Метод для сравнения двух документов на основе токенов
        private static ComparisonResult CompareToken(DocumentTokens firstToken, DocumentTokens secondToken)
        {
            // Создаем матрицу для хранения значений расстояния
            var opt = new double[firstToken.Count + 1, secondToken.Count + 1];

            // Инициализация первой строки и первого столбца матрицы
            for (var i = 0; i <= firstToken.Count; i++)
                opt[i, 0] = i; // Стоимость удаления токенов
            for (var i = 0; i <= secondToken.Count; i++)
                opt[0, i] = i; // Стоимость добавления токенов

            // Заполняем матрицу по алгоритму Левенштейна
            for (var i = 1; i <= firstToken.Count; i++)
                for (var j = 1; j <= secondToken.Count; j++)
                {
                    var distance = 0.0;

                    // Если токены различаются, вычисляем стоимость замены
                    if (firstToken[i - 1] != secondToken[j - 1])
                        distance = TokenDistanceCalculator
                            .GetTokenDistance(firstToken[i - 1], secondToken[j - 1]);

                    // Вычисляем минимальную стоимость операций (удаление, добавление, замена)
                    opt[i, j] = Math.Min(Math.Min(opt[i - 1, j] + 1, // Удаление
                                                     opt[i, j - 1] + 1), // Добавление
                                         opt[i - 1, j - 1] + distance); // Замена
                }

            // Возвращаем результат сравнения, содержащий два документа и их расстояние
            return new ComparisonResult(firstToken, secondToken, opt[firstToken.Count, secondToken.Count]);
        }
    }
}
