using System.Collections.Generic;
using System.Linq;

namespace rocket_bot;

// Класс Channel реализует потокобезопасный канал для хранения элементов типа T
public class Channel<T> where T : class
{
    private readonly List<T> TList = new(); // Список для хранения элементов типа T

    /// <summary>
    /// Возвращает элемент по индексу или null, если такого элемента нет.
    /// При присвоении удаляет все элементы после.
    /// Если индекс в точности равен размеру коллекции, работает как Append.
    /// </summary>
    public T this[int index]
    {
        get
        {
            lock (TList) // Блокируем доступ к списку для потокобезопасности
            {
                // Если индекс больше, чем размер списка, возвращаем null
                return index > TList.Count - 1 ? null : TList[index];
            }
        }
        set
        {
            lock (TList) // Блокируем доступ к списку для потокобезопасности
            {
                // Если индекс равен размеру списка, добавляем новый элемент
                if (TList.Count == index)
                    TList.Add(value);
                else
                {
                    // Удаляем все элементы после указанного индекса
                    TList.RemoveRange(index, TList.Count - index);
                    TList.Add(value); // Добавляем новый элемент
                }
            }
        }
    }

    /// <summary>
    /// Возвращает последний элемент или null, если такого элемента нет
    /// </summary>
    public T LastItem()
    {
        lock (TList) // Блокируем доступ к списку для потокобезопасности
        {
            // Возвращаем последний элемент списка или null, если список пуст
            return TList.LastOrDefault();
        }
    }

    /// <summary>
    /// Добавляет item в конец только если lastItem является последним элементом
    /// </summary>
    public void AppendIfLastItemIsUnchanged(T item, T knownLastItem)
    {
        lock (TList) // Блокируем доступ к списку для потокобезопасности
        {
            // Если известный последний элемент совпадает с текущим последним элементом списка
            if (knownLastItem == LastItem())
                TList.Add(item); // Добавляем новый элемент в конец
        }
    }

    /// <summary>
    /// Возвращает количество элементов в коллекции
    /// </summary>
    public int Count
    {
        get
        {
            lock (TList) // Блокируем доступ к списку для потокобезопасности
            {
                return TList.Count; // Возвращаем количество элементов в списке
            }
        }
    }
}
