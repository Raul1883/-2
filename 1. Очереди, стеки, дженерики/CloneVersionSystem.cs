using System.Collections.Generic;
namespace Clones
{
    // Интерфейс для системы контроля клонов
    public class CloneVersionSystem : ICloneVersionSystem
    {
        // Список, хранящий всех клонов
        private readonly List<Clone> CloneList = new();

        // Метод для выполнения команды, переданной в виде строки
        public string Execute(string query)
        {
            // Если список клонов пуст, добавляем базового клона
            if (CloneList.Count == 0) 
                CloneList.Add(new Clone());

            // Разделяем команду на части
            var commandList = query.Split(' ');
            var cloneNumber = int.Parse(commandList[1]) - 1; // Номер клона (уменьшаем на 1 для индексации)
            var commandName = commandList[0]; // Имя команды
            var programmName = commandList.Length > 2 ? commandList[2] : null; // Имя программы (если есть)

            // Получаем клона по номеру
            var clone = CloneList[cloneNumber];

            // Выполняем команду для данного клона и возвращаем результат
            return clone.ExecuteCommand(commandName, cloneNumber, CloneList, programmName);
        }
    }
    // Класс, представляющий клона
    public class Clone
    {
        // Стек для хранения усвоенных программ
        private readonly StoryStack LearnStack;

        // Стек для хранения истории откатов
        private readonly StoryStack RollBackStack;

        // Конструктор для создания нового клона с базовыми знаниями
        public Clone()
        {
            LearnStack = new StoryStack(); // Инициализируем стек усвоенных программ
            RollBackStack = new StoryStack(); // Инициализируем стек откатов
        }

        // Конструктор для клонирования существующего клона
        public Clone(Clone baseClone)
        {
            // Клонируем стеки усвоенных программ и откатов
            LearnStack = new StoryStack(baseClone.LearnStack);
            RollBackStack = new StoryStack(baseClone.RollBackStack);
        }

        // Метод для выполнения команды, переданной клону
        public string ExecuteCommand(string commandName, int cloneNumber, List<Clone> CloneList,
            string programmName = null)
        {
            // Обработка различных команд
            switch (commandName)
            {
                case "learn":
                    LearnStack.Push(programmName); // Добавляем программу в стек усвоенных программ
                    RollBackStack.Clear(); // Очищаем стек откатов
                    return null;
                case "rollback":
                    if (LearnStack.IsEmpty()) 
                        return null; // Если нет усвоенных программ, ничего не делаем
                    RollBackStack.Push(LearnStack.Pop()); // Перемещаем последнюю усвоенную программу в стек откатов
                    return null;
                case "relearn":
                    if (RollBackStack.IsEmpty())
                        return null; // Если нет истории откатов, ничего не делаем
                    LearnStack.Push(RollBackStack.Pop()); // Перемещаем последнюю откатную программу обратно в стек усвоенных
                    return null;
                case "clone":
                    CloneList.Add(new Clone(CloneList[cloneNumber])); // Клонируем текущего клона и добавляем в список
                    return null;
                case "check":
                    if (LearnStack.IsEmpty()) 
                        return "basic"; // Если клон не усвоил никаких программ, возвращаем "basic"
                    return LearnStack.GetValue(); // Возвращаем последнюю усвоенную программу
            }
            // Если команда не распознана, возвращаем null
            return null; 
        }
    }

    // Класс, представляющий элемент стека истории
    public class StoryStackItem
    {
        public string Value; // Значение программы
        public StoryStackItem Next; // Ссылка на следующий элемент

        // Конструктор для создания нового элемента стека
        public StoryStackItem(string value, StoryStackItem next)
        {
            Value = value; // Инициализируем значение
            Next = next; // Устанавливаем ссылку на следующий элемент
        }
    }

    // Класс для стека истории
    public class StoryStack
    {
        private StoryStackItem Head; // Указатель на верхний элемент стека
        // Конструктор для создания пустого стека

        public StoryStack()
        {
        }

        // Конструктор для клонирования стека
        public StoryStack(StoryStack baseStack)
        {
            Head = baseStack.Head; // Клонируем указатель на верхний элемент
        }

        // Метод для добавления элемента в стек
        public void Push(string item)
        {
            Head = new StoryStackItem(item, Head); // Создаем новый элемент и устанавливаем его как верхний
        }
        // Метод для удаления верхнего элемента из стека
        public string Pop()
        {
            var result = Head.Value; // Сохраняем значение верхнего элемента
            Head = Head.Next; // Заменяем верхний элемент
            return result; // Возвращаем значение
        }

        // Метод для очистки стека
        public void Clear()
        {
            Head = null; // Устанавливаем указатель на null, чтобы очистить стек
        }

        // Метод для проверки, пуст ли стек
        public bool IsEmpty()
        {
            return Head == null; // Возвращаем true, если указатель на верхний элемент равен null
        }

        // Метод для получения значения верхнего элемента стека
        public string GetValue()
        {
            return Head.Value; // Возвращаем значение верхнего элемента
        }
    }
}