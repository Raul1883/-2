using System;
using System.Collections.Generic;

namespace func_rocket
{
    public class LevelsTask
    {
        // базовая ракета
        private static readonly Rocket defaultRocket = new(new Vector(200, 500), Vector.Zero, -0.5 * Math.PI);
        // базовая стартовая позиция
        private static readonly Vector defaultStartPosition = new(200, 500);
        // базовая точка выхода
        private static readonly Vector defaultTargetPosition = new(600, 200);
        // базовая гравитация
        private static readonly Gravity defaultGravity = new((size, v) => Vector.Zero);
        // базовая физика
        static readonly Physics standardPhysics = new();

        public static IEnumerable<Level> CreateLevels()
        {
            yield return CreateLevel("Zero"); // Нулевая гравитация

            yield return CreateLevel("Heavy", (size, v) => new Vector(0, 0.9)); // Постоянная гравитация 0.09 вниз

            yield return CreateLevel("Up", new Vector(700, 500),
                (size, v) => new Vector(0, -300 / (size.Y - v.Y + 300.0))); // Гравитация направлена вверх

            yield return CreateLevel("WhiteHole", (size, v) => GetWhiteHoleVector(v, defaultTargetPosition)); // Гравитация от белой дыры

            yield return CreateLevel("BlackHole",
                (size, v) => GetBlackHoleVector(v, defaultTargetPosition, defaultStartPosition)); // Гравитация к черной дыре

            yield return CreateLevel("BlackAndWhite",
                (size, v) => GetBlackAndWhiteVector(v, defaultTargetPosition, defaultStartPosition)); // черно белая дыра
        }

        // Далее перегрузки метода для создания уровней
        // Метод для создания уровня с именем
        public static Level CreateLevel(string name) =>
             new(name, defaultRocket, defaultTargetPosition, defaultGravity, standardPhysics);

        // Метод для создания уровня с именем и гравитацией
        public static Level CreateLevel(string name, Gravity gravity) =>
             new(name, defaultRocket, defaultTargetPosition, gravity, standardPhysics);

        // Метод для создания уровня с именем, целью и гравитацией 
        public static Level CreateLevel(string name, Vector target, Gravity gravity) =>
           new(name, defaultRocket, target, gravity, standardPhysics);

        // Метод для вычисления вектора гравитации от белой дыры
        public static Vector GetWhiteHoleVector(Vector v, Vector target)
        {
            var targetVector = (target - v);
            var d = targetVector.Length;
            var vectorAbs = 140 * d / (d * d + 1);
            return -1 * targetVector.Normalize() * vectorAbs;
        }

        // Метод для вычисления вектора гравитации от черной дыры
        public static Vector GetBlackHoleVector(Vector v, Vector target, Vector startPosition)
        {
            var blackHole = (target + startPosition) / 2; // Находим аномалию
            var targetVector = (blackHole - v);
            var d = targetVector.Length;
            var vectorAbs = 300 * d / (d * d + 1);
            return targetVector.Normalize() * vectorAbs;
        }

        // Метод для вычисления среднего значения гравитаций белой и черной дыр
        public static Vector GetBlackAndWhiteVector(Vector v, Vector target, Vector startPosition)
        {
            var summaryVector = GetWhiteHoleVector(v, target) + GetBlackHoleVector(v, target, startPosition);
            return summaryVector / 2; // Возвращаем среднее значение
        }
    }
}
