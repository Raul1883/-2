using System.Collections.Generic;
using System.Linq;

namespace Rivals;

public class RivalsTask
{
    /// <summary>
    /// Определяет, какие клетки карты будут принадлежать каждому игроку.
    /// </summary>
    /// <param name="map">Карта, на которой играют игроки.</param>
    /// <returns>Перечисление объектов OwnedLocation, представляющих клетки, принадлежащие игрокам.</returns>
    public static IEnumerable<OwnedLocation> AssignOwners(Map map)
    {
        // Очередь для хранения клеток, которые будут обрабатываться.
        var queue = new Queue<OwnedLocation>();
        // Множество для отслеживания посещенных клеток.
        var visited = new HashSet<Point>();

        // Инициализация очереди с начальными позициями игроков.
        for (int i = 0; i < map.Players.Length; i++)
        {
            // Добавляем начальную позицию игрока в множество посещенных клеток.
            visited.Add(map.Players[i]);
            // Добавляем начальную позицию игрока в очередь с расстоянием 0.
            queue.Enqueue(new OwnedLocation(i, map.Players[i], 0));
        }

        // Обработка очереди, пока в ней есть элементы.
        while (queue.Count > 0)
        {
            // Извлекаем текущую клетку из очереди.
            var location = queue.Dequeue();
            // Получаем соседние клетки, доступные для текущей клетки.
            var nearPoints = GetNearPointsArray(location.Location, map, visited);

            // Обрабатываем каждую соседнюю клетку.
            foreach (var p in nearPoints)
            {
                // Добавляем соседнюю клетку в множество посещенных клеток.
                visited.Add(p);
                // Добавляем соседнюю клетку в очередь с увеличенным расстоянием.
                queue.Enqueue(new OwnedLocation(location.Owner, p, location.Distance + 1));
            }
            // Возвращаем текущую клетку с информацией о владельце и расстоянии.
            yield return location;
        }
    }

    /// <summary>
    /// Получает массив соседних клеток для заданной клетки.
    /// </summary>
    /// <param name="point">Текущая клетка.</param>
    /// <param name="map">Карта, на которой осуществляется поиск.</param>
    /// <param name="visited">Множество посещенных клеток.</param>
    /// <returns>Массив соседних клеток, доступных для перемещения.</returns>
    private static Point[] GetNearPointsArray(Point point, Map map, HashSet<Point> visited)
    {
        // Проверяем, является ли текущая клетка сундуком.
        if (map.Chests.Any(x => x == point))
        {
            // Если это сундук, возвращаем пустой массив, так как игроки остаются "охранять" сундук.
            return System.Array.Empty<Point>();
        }

        // Возвращаем массив соседних клеток, которые находятся в пределах карты,
        // не являются стенами и еще не были посещены.
        return new Point[]
        {
            new Point(point.X, point.Y + 1), // Вниз
            new Point(point.X, point.Y - 1), // Вверх
            new Point(point.X + 1, point.Y), // Вправо
            new Point(point.X - 1, point.Y)  // Влево
        }
        .Where(x => map.InBounds(x)) // Проверка на границы карты
        .Where(x => map.Maze[x.X, x.Y] != MapCell.Wall) // Проверка на стену
        .Where(x => !visited.Contains(x)) // Проверка на посещенные клетки
        .ToArray();
    }
}
