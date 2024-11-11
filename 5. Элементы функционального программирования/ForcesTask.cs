using System;

namespace func_rocket
{
    public class ForcesTask
    {
        // Метод, который создает делегат, возвращающий вектор силы тяги ракеты.
        // Сила тяги направлена вдоль ракеты и равна значению forceValue.
        public static RocketForce GetThrustForce(double forceValue)
        {
            return r =>
                new Vector(forceValue * Math.Cos(r.Direction), forceValue * Math.Sin(r.Direction));
        }

        // Метод, который преобразует делегат силы гравитации в делегат силы, действующей на ракету.
        // Принимает делегат gravity, который рассчитывает силу в зависимости от размера пространства и местоположения ракеты.
        public static RocketForce ConvertGravityToForce(Gravity gravity, Vector spaceSize)
        {
            return r => gravity(spaceSize, r.Location);
        }

        // Метод, который суммирует все переданные силы, действующие на ракету, и возвращает суммарную силу.
        // Создает нулевой вектор и добавляет к нему векторы всех переданных сил.
        public static RocketForce Sum(params RocketForce[] forces)
        {
            return r => {
                var totalVector = new Vector(0, 0); // Инициализируем вектор суммарной силы
                foreach (var force in forces)
                {
                    // Суммируем векторы всех сил, применяя их к ракете
                    totalVector += force(r);
                }
                return totalVector; // Возвращаем итоговый вектор силы
            };
        }
    }
}
