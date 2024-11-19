using System;
using System.Collections.Generic;
using System.Linq;
using Greedy.Architecture;

namespace Greedy
{
    public class DijkstraPathFinder
    {
        // Метод для получения путей до сундуков с использованием алгоритма Дейкстры
        public IEnumerable<PathWithCost> GetPathsByDijkstra(State state, Point start, IEnumerable<Point> targets)
        {
            // Создаем множество сундуков из переданного списка целей
            var chests = new HashSet<Point>(targets);
            // Множество клеток, которые мы можем открыть (начинаем с начальной позиции)
            var ableToOpen = new HashSet<Point> { start };
            // Множество посещенных клеток
            var visited = new HashSet<Point>();

            // Словарь для хранения путей и стоимости до каждой клетки, начинаем с начальной клетки
            var path = new Dictionary<Point, Cell> { [start] = new Cell(null, 0) };

            // Основной цикл, который будет выполняться до тех пор, пока есть клетки для открытия
            while (true)
            {
                // Получаем следующую клетку, которую можно открыть
                Point? openPoint = GetPointToOpen(path, ableToOpen);
                // Если нет доступных клеток, выходим из метода
                if (openPoint == null) yield break;
                var value = openPoint.Value;

                // Если открытая клетка является сундуком, возвращаем путь к ней
                if (chests.Contains(value))
                    yield return GetPathWithCost(path, value);

                // Открываем клетку и обновляем информацию о соседних клетках
                OpenPoint(openPoint, path, state, visited, ableToOpen);
            }
        }

        // Метод для открытия клетки и обновления соседних клеток
        private void OpenPoint(Point? openPoint, Dictionary<Point, Cell> path, State state, HashSet<Point> visited, HashSet<Point> ableToOpen)
        {
            var value = openPoint.Value;
            // Обходим все соседние клетки
            foreach (var p in GetNearPoints(value, state, visited))
            {
                // Рассчитываем стоимость пути до соседней клетки
                var price = path[value].Price + state.CellCost[p.X, p.Y];

                // Если путь до соседней клетки дешевле, обновляем информацию о ней
                if (!path.ContainsKey(p) || path[p].Price > price)
                    path[p] = new Cell(openPoint, price);
                // Добавляем соседнюю клетку в список доступных для открытия
                ableToOpen.Add(p);
            }

            // Убираем открытую клетку из доступных и добавляем в посещенные
            ableToOpen.Remove(value);
            visited.Add(value);
        }

        // Метод для нахождения клетки с минимальной стоимостью из доступных
        private Point? GetPointToOpen(Dictionary<Point, Cell> path, HashSet<Point> ableToOpen)
        {
            Point? openPoint = null;
            var minPrice = int.MaxValue;
            // Ищем клетку с минимальной стоимостью среди доступных
            foreach (var p in ableToOpen)
                if (path[p].Price < minPrice)
                {
                    minPrice = path[p].Price;
                    openPoint = p;
                }
            return openPoint;
        }

        // Метод для получения пути с учетом стоимости до конечной клетки
        private static PathWithCost GetPathWithCost(Dictionary<Point, Cell> path, Point end)
        {
            var result = new List<Point>();
            Point? current = end;
            // Восстанавливаем путь от конечной клетки до начальной
            while (current != null)
            {
                result.Add(current.Value);
                current = path[current.Value].Previous;
            }

            // Создаем объект PathWithCost с общей стоимостью и маршрутом
            PathWithCost pathResult = new(path[end].Price, result
                .AsEnumerable()
                .Reverse()
                .ToArray());
            return pathResult;
        }

        // Метод для получения соседних клеток
        private static IEnumerable<Point> GetNearPoints(Point point, State state, HashSet<Point> visited)
        {
            // Определяем соседние клетки
            Point point1 = new(point.X, point.Y + 1);
            return new Point[]
            {
                point1, // Соседняя клетка сверху
                new(point.X, point.Y - 1), // Соседняя клетка снизу
                new(point.X + 1, point.Y), // Соседняя клетка справа
                new(point.X - 1, point.Y) // Соседняя клетка слева
            }
            .Where(x => state.InsideMap(x)) // Проверяем, что клетка внутри лабиринта
            .Where(x => !state.IsWallAt(x)) // Проверяем, что клетка не стена
            .Where(x => !visited.Contains(x)); // Проверяем, что клетка не была посещена
        }

        // Класс для хранения информации о клетке (предыдущая клетка и стоимость)
        private class Cell
        {
            public Point? Previous; // Предыдущая клетка на пути
            public int Price; // Стоимость пути до этой клетки

            // Конструктор для класса Cell
            public Cell(Point? previous, int price)
            {
                Previous = previous;
                Price = price;
            }
        }
    }
}
