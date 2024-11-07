using System.Collections.Generic;
namespace LimitedSizeStack;
// Класс ListModel управляет списком элементов и поддерживает функциональность Undo
public class ListModel<TItem>
{
    // Свойство, представляющее список элементов
    public List<TItem> Items { get; }

    // Хранилище для истории действий с использованием LimitedSizeStack
    private readonly LimitedSizeStack<Command<TItem>> story;

    // Конструктор, который инициализирует список элементов и историю действий
    public ListModel(int undoLimit) : this(new List<TItem>(), undoLimit)
    {
        // Вызов другого конструктора с пустым списком
    }

    // Основной конструктор, который принимает начальный список элементов и лимит истории
    public ListModel(List<TItem> items, int undoLimit)
    {
        Items = items; // Инициализация списка элементов
        story = new LimitedSizeStack<Command<TItem>>(undoLimit); // Инициализация стека с лимитом
    }

    // Метод для добавления нового элемента в список
    public void AddItem(TItem item)
    {
        // Сохраняем команду добавления в историю
        story.Push(new Command<TItem>(Commands.AddItem, Items.Count, item));
        Items.Add(item); // Добавляем элемент в список
    }

    // Метод для удаления элемента из списка по индексу
    public void RemoveItem(int index)
    {
        // Сохраняем команду удаления в историю
        story.Push(new Command<TItem>(Commands.RemoveItem, index, Items[index]));
        Items.RemoveAt(index); // Удаляем элемент из списка
    }

    // Метод для проверки возможности отмены последнего действия
    public bool CanUndo()
    {
        // Возвращаем true, если история действий не пуста
        return story.Count != 0;
    }

    // Метод для отмены последнего действия
    public void Undo()
    {
        // Извлекаем последнюю команду из истории
        var com = story.Pop();

        // Проверяем, была ли последняя команда удалением
        if (com.CommandName == Commands.RemoveItem)
            // Восстанавливаем удаленный элемент в список
            Items.Insert(com.Index, com.Value);

        // Проверяем, была ли последняя команда добавлением
        if (com.CommandName == Commands.AddItem)
            // Удаляем элемент из списка по индексу
            Items.RemoveAt(com.Index);
    }
}

// Класс, представляющий команду с информацией о действии
public record Command<TItem>(Commands CommandName, int Index, TItem Value) { }

// Перечисление для обозначения типов команд
public enum Commands
{
    AddItem,    // Команда добавления элемента
    RemoveItem  // Команда удаления элемента
}
