using System;
using System.Collections.Generic;
using System.Linq;

namespace DiskTree
{
    // Класс, содержащий логику для восстановления структуры каталогов
    public class DiskTreeTask
    {
        // Основной метод, решающий задачу
        public static List<string> Solve(List<string> input)
        {
            // Создаем корневой каталог (Dir) без имени
            var root = new Dir(null);

            // Проходим по каждому пути из входного списка
            foreach (var path in input)
            {
                var currentDir = root; // Начинаем с корня
                // Разбиваем путь на отдельные каталоги
                foreach (var dirName in path.Split(@"\"))
                {
                    // Если текущий каталог не содержит подкаталог с таким именем
                    if (!currentDir.Contains(dirName))
                        // Добавляем новый каталог в список дочерних каталогов
                        currentDir.Childs.Add(new Dir(dirName));

                    // Переходим к дочернему каталогу с заданным именем
                    currentDir = currentDir.Childs.Where(x => x.Name == dirName).FirstOrDefault();
                }
            }
            // Получаем отформатированное дерево каталогов и возвращаем его в виде списка строк
            return GetTree(root).ToList();
        }

        // Рекурсивный метод для получения строкового представления дерева каталогов
        private static IEnumerable<string> GetTree(Dir root, int depth = 0)
        {
            // Сортируем дочерние каталоги в лексикографическом порядке
            foreach (var child in root.Childs.OrderBy(x => x.Name, StringComparer.Ordinal))
            {
                // Возвращаем имя текущего каталога с отступами в зависимости от глубины
                yield return new string(' ', depth) + child.Name;
                // Рекурсивно обходим дочерние каталоги, увеличивая глубину на 1
                foreach (var dirNames in GetTree(child, depth + 1))
                {
                    yield return dirNames; // Возвращаем имена подкаталогов
                }
            }
        }

        // Вложенный класс для представления каталога
        private class Dir
        {
            public string Name; // Имя каталога
            public List<Dir> Childs = new(); // Список дочерних каталогов

            // Метод для проверки наличия подкаталога с заданным именем
            public bool Contains(string name) => Childs.Where(x => x.Name == name).Any();

            // Конструктор для создания каталога с заданным именем
            public Dir(string name)
            {
                Name = name; // Устанавливаем имя каталога
            }
        }
    }
}
