using System.Collections.Generic;
using System.Linq;
using Greedy.Architecture;

namespace Greedy;

// Класс NotGreedyPathFinder реализует интерфейс IPathFinder для нахождения оптимального пути по лабиринту
public class NotGreedyPathFinder : IPathFinder
{
    private DijkstraPathFinder Finder; // Экземпляр класса DijkstraPathFinder для нахождения путей
    private Stack<PathWithCost> Paths; // Стек для хранения путей, которые мы исследуем
    private HashSet<Point> Chests; // Множество сундуков, которые нужно собрать
    private Dictionary<(Point, Point), PathWithCost?> PathDict; // Кэш для хранения найденных путей между точками
    private List<PathWithCost>? BestPath; // Лучший найденный путь (максимальное количество собранных сундуков)
    private State State; // Состояние, содержащее информацию о текущем положении и энергии

    // Метод для нахождения пути к цели с максимальным количеством собранных сундуков
    public List<Point> FindPathToCompleteGoal(State state)
    {
        Finder = new(); // Инициализация DijkstraPathFinder
        Paths = new(); // Инициализация стека для путей
        PathDict = new(); // Инициализация кэша для путей
        BestPath = null; // Изначально лучший путь отсутствует
        Chests = new(state.Chests); // Копируем множество сундуков из состояния
        State = state; // Сохраняем текущее состояние

        // Проходим по всем сундукам и ищем путь через каждый из них
        foreach (var chest in state.Chests)
        {
            var result = GetPathThroughChest(state.Position, chest, 0); // Получаем путь через текущий сундук
            if (result is not null) // Если путь найден, возвращаем его
                return result; // происходит, если все сундуки собраны и стоимость в пределах энергии
        }

        // Если был найден лучший путь, возвращаем его
        if (BestPath is not null)
            return GetPath(BestPath);
        return new(); // Если нет путей, возвращаем пустой список
    }

    // Рекурсивный метод для поиска пути через сундук
    private List<Point>? GetPathThroughChest(Point position, Point chest, int currentCost)
    {
        var path = GetPathTo(position, chest); // Получаем путь к сундуку
        if (path is null) return null; // Если путь не найден, возвращаем null

        Paths.Push(path); // Добавляем найденный путь в стек
        Chests.Remove(chest); // Удаляем текущий сундук из множества
        currentCost += path.Cost; // Увеличиваем текущую стоимость на стоимость пути

        // Если текущая стоимость не превышает доступную энергию и есть еще сундуки для сбора
        if (currentCost <= State.Energy && Chests.Count > 0)
            foreach (var nextChest in Chests.ToList()) // Проходим по оставшимся сундукам
            {
                var result = GetPathThroughChest(chest, nextChest, currentCost); // Рекурсивно ищем путь через следующий сундук
                if (result is not null) return result; // Если путь найден, возвращаем его
            }
        else if (currentCost <= State.Energy && Chests.Count == 0) // Если все сундуки собраны и стоимость в пределах энергии
            return GetPath(Paths.ToList()); // Возвращаем собранный путь

        // Восстанавливаем состояние: добавляем сундук обратно и удаляем путь из стека
        Chests.Add(chest);
        Paths.Pop();
        UpdateBestPath(); // Обновляем лучший найденный путь
        return null; // Возвращаем null, так как путь не найден
    }

    // Метод для обновления лучшего пути, если найденный путь лучше
    private void UpdateBestPath()
    {
        if (BestPath is null || BestPath.Count < Paths.Count) // Если лучший путь не установлен или найденный путь длиннее
            BestPath = Paths.ToList(); // Сохраняем текущий путь как лучший
    }

    // Метод для получения полного пути из списка путей
    private static List<Point> GetPath(List<PathWithCost> Paths)
    {
        var result = new List<Point>(); // Список для хранения результата
        foreach (var path in Paths.AsEnumerable().Reverse()) // Проходим по путям в обратном порядке
            foreach (var pathCost in path.Path.Skip(1)) // Пропускаем начальную точку
                result.Add(pathCost); // Добавляем остальные точки в результат
        return result; // Возвращаем полный путь
    }

    // Метод для получения пути между двумя точками с использованием Dijkstra
    private PathWithCost? GetPathTo(Point start, Point end)
    {
        // Проверяем, есть ли уже сохраненный путь между этими точками
        if (PathDict.ContainsKey((start, end)))
            return PathDict[(start, end)]; // Возвращаем сохраненный путь

        // Находим путь с помощью Dijkstra
        var path = Finder.GetPathsByDijkstra(State, start, new[] { end })
            .FirstOrDefault(); // Получаем первый найденный путь
        PathDict[(start, end)] = path; // Сохраняем путь в кэше
        return path; // Возвращаем найденный путь
    }
}
