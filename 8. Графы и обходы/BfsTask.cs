using System.Collections.Generic;
using System.Linq;

namespace Dungeon;

public class BfsTask
{
    /// <summary>
    /// Находит пути от стартовой точки до всех сундуков на карте, используя поиск в ширину (BFS).
    /// Возвращает коллекцию путей, каждый из которых представлен односвязным списком точек.
    /// </summary>
    /// <param name="map">Карта подземелья.</param>
    /// <param name="start">Стартовая точка.</param>
    /// <param name="chests">Массив точек, представляющих расположение сундуков.</param>
    /// <returns>Коллекция путей от стартовой точки до каждого доступного сундука.  Если путь до сундука не существует, он не включается в результат.</returns>

    public static IEnumerable<SinglyLinkedList<Point>> FindPaths(Map map, Point start, Chest[] chests)
    {
        // Словарь для хранения путей от стартовой точки до всех посещенных точек.
        // Ключ - точка, значение - путь до этой точки (односвязный список).
        var routeFrom = new Dictionary<Point, SinglyLinkedList<Point>>
        {
            // Инициализация словаря: путь до стартовой точки - это сама стартовая точка.
            [start] = new SinglyLinkedList<Point>(start)
        };

        // Очередь для алгоритма поиска в ширину.
        var queue = new Queue<SinglyLinkedList<Point>>();
        queue.Enqueue(routeFrom[start]); // Добавляем начальный путь в очередь.

        // Перебираем все сундуки.
        foreach (var chest in chests)
        {
            // Проверяем, найден ли уже путь до этого сундука.
            if (routeFrom.ContainsKey(chest.Location))
            {
                // Если путь найден, возвращаем его.
                yield return routeFrom[chest.Location];
                continue; // Переходим к следующему сундуку.
            }

            // Если путь не найден, ищем его с помощью вспомогательной функции.
            var path = FindPathFrom(routeFrom, queue, map, chest.Location);
            if (path != null)
                // Если путь найден, возвращаем его.
                yield return path;
        }
    }

    /// <summary>
    /// Вспомогательная функция для поиска пути от стартовой точки до заданной целевой точки (сундука) с помощью BFS.
    /// </summary>
    /// <param name="routeFrom">Словарь путей (передается по ссылке).</param>
    /// <param name="queue">Очередь BFS (передается по ссылке).</param>
    /// <param name="map">Карта подземелья.</param>
    /// <param name="end">Целевая точка (сундук).</param>
    /// <returns>Путь от стартовой точки до целевой точки, или null, если путь не существует.</returns>
    static SinglyLinkedList<Point>? FindPathFrom(
        Dictionary<Point, SinglyLinkedList<Point>> routeFrom,
        Queue<SinglyLinkedList<Point>> queue, Map map, Point end)
    {
        // Продолжаем поиск, пока очередь не пуста.
        while (queue.Count != 0)
        {
            // Извлекаем путь из очереди.
            var currentPosition = queue.Dequeue();
            // Текущая точка на пути.
            var point = currentPosition.Value;

            // Проверка на выход за границы карты или на наличие стены.
            if (!(map.InBounds(point) && map.Dungeon[point.X, point.Y] != MapCell.Wall))
                continue; // Если точка недоступна, переходим к следующей.

            // Перебираем все возможные направления движения.
            foreach (var nextPoint in Walker.PossibleDirections
                .Select(direction => currentPosition.Value + direction))
            {
                // Проверяем, не посещали ли мы уже эту точку.
                if (routeFrom.ContainsKey(nextPoint)) continue; // Если посещали, пропускаем.

                // Создаем новый путь и добавляем его в словарь и очередь.
                routeFrom[nextPoint] = new SinglyLinkedList<Point>(nextPoint, currentPosition);
                queue.Enqueue(routeFrom[nextPoint]);
            }

            // Проверяем, достигли ли мы целевой точки.
            if (routeFrom.ContainsKey(end)) return routeFrom[end]; // Если да, возвращаем путь.
        }
        // Если очередь опустела, а целевая точка не найдена, путь не существует.
        return default;
    }
}