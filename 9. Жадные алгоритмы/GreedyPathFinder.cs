using System.Collections.Generic;
using System.Linq;
using Greedy.Architecture;

namespace Greedy
{
    public class GreedyPathFinder : IPathFinder
    {
        public List<Point> FindPathToCompleteGoal(State state)
        {
            // Создаем список для хранения пути, который будет возвращен
            var path = new List<Point>();
            // Создаем множество сундуков для быстрого доступа и модификации
            var chests = new HashSet<Point>(state.Chests);
            // Переменная для отслеживания затрат энергии на пути
            var price = 0;
            // Текущая позиция Жадины
            var position = state.Position;
            // Создаем экземпляр DijkstraPathFinder для поиска путей
            var finder = new DijkstraPathFinder();

            // Цикл для сбора N сундуков
            for (int i = 0; i < state.Goal; i++)
            {
                // Если больше нет сундуков, возвращаем пустой список
                if (chests.Count == 0) return new();

                // Получаем путь к ближайшему сундуку с помощью алгоритма Дейкстры
                var chestPath = finder.GetPathsByDijkstra(state, position, chests).FirstOrDefault();
                // Если путь не найден, возвращаем пустой список
                if (chestPath == null) return new();

                // Увеличиваем общую стоимость на стоимость этого пути
                price += chestPath.Cost;
                // Обновляем текущую позицию на конец найденного пути
                position = chestPath.End;
                // Проверяем, не превышает ли стоимость пути доступную энергию
                if (price > state.Energy) return new();

                // Удаляем сундук из множества, так как он уже собран
                chests.Remove(chestPath.End);

                // Добавляем все точки пути, кроме начальной, в список пути
                for (var j = 1; j < chestPath.Path.Count; j++)
                    path.Add(chestPath.Path[j]);
            }
            // Возвращаем собранный путь
            return path;
        }
    }
}
