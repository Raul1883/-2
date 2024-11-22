using System.Collections.Generic;
using System.Linq;

namespace Dungeon
{
    public class DungeonTask
    {
        public static MoveDirection[] FindShortestPath(Map map)
        {
            // Находим пути от начальной позиции до всех сундуков
            var pathsFromStart = BfsTask.FindPaths(map, map.InitialPosition, map.Chests);

            // Собираем уникальные позиции сундуков
            var uniqueChestLocations = map.Chests.Select(chest => chest.Location).ToHashSet();
            var emptyChestArray = uniqueChestLocations.Select(location => new EmptyChest(location)).ToArray();

            // Находим пути от выхода до всех сундуков
            var pathsFromExit = BfsTask.FindPaths(map, map.Exit, emptyChestArray);

            // Объединяем пути
            var combinedShortestPaths = CombinePaths(pathsFromStart, pathsFromExit);

            // Если найдены кратчайшие пути, выбираем из них путь с самым "тяжелым" сундуком
            if (combinedShortestPaths != null && combinedShortestPaths.Any())
            {
                var optimalPath = GetMaxValueChestPath(combinedShortestPaths, map.Chests);
                if (optimalPath != null)
                    return ConvertPathToDirections(optimalPath).ToArray();
            }

            // Если кратчайший путь до выхода без сундуков
            var pathToExit = BfsTask.FindPaths(map, 
                map.InitialPosition,
                new Chest[] { new EmptyChest(map.Exit) }).FirstOrDefault(); 

            if (pathToExit != null)
                return ConvertPathToDirections(pathToExit.Reverse()).ToArray();

            // Если не найдено ни одного пути
            return System.Array.Empty<MoveDirection>();
        }


        private static IEnumerable<IEnumerable<Point>>? CombinePaths(
            IEnumerable<SinglyLinkedList<Point>> startToChestsPaths,
            IEnumerable<SinglyLinkedList<Point>> exitToChestsPaths)
        {
            // Объединяем пути от начальной позиции к сундукам и от выхода к сундукам

            // см пояснения в пояснения.txt
            return startToChestsPaths
                .Join(exitToChestsPaths, startPath => startPath.Value, exitPath => exitPath.Value,
                    (startPath, exitPath) => startPath.Reverse().Concat(exitPath.Skip(1)))
                .GroupBy(path => path.Count())
                .MinBy(group => group.Key);
        }

        private static IEnumerable<Point>? GetMaxValueChestPath(
            IEnumerable<IEnumerable<Point>> paths,
            IEnumerable<Chest> chests)
        {
            // Получаем значения сундуков в словарь
            var chestValues = chests.ToDictionary(chest => chest.Location, chest => chest.Value);
            var maxChestValue = byte.MinValue; // минимальное возможно значение
            IEnumerable<Point>? bestPath = null; // пока не нашли, то null

            // Ищем путь с максимальным значением сундука
            foreach (var path in paths)
            {
                if (path != null)
                {
                    var chestLocation = path.FirstOrDefault(cell => chestValues.ContainsKey(cell));
                    if (chestLocation != null)
                    {
                        var chestValue = chestValues[chestLocation];
                        if (chestValue >= maxChestValue)
                        {
                            maxChestValue = chestValue;
                            bestPath = path;
                        }
                    }
                }
            }

            return bestPath;
        }

        private static MoveDirection[] ConvertPathToDirections(IEnumerable<Point> path)
        {
            // результирующий лист, со временем добавления O(1)
            var directions = new LinkedList<MoveDirection>();

            // Конвертируем путь в направления движения
            Point? previousPoint = null; // предыдущая точка, изначально null
            foreach (var currentPoint in path)
            {
                if (previousPoint != null) // если предыдущая точка существует
                    // используем статический готовый метод, для получения направления
                    directions.AddLast(Walker
                        .ConvertOffsetToDirection(currentPoint - previousPoint));
                previousPoint = currentPoint;
            }
            return directions.ToArray();
        }
    }
}
