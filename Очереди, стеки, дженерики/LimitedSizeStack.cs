using System;
using System.Collections.Generic;

namespace LimitedSizeStack;
/*
 * В практике реализован стэк с ограниченным количеством элементов.
 * Для работы с разными типами данных используются дженерики
 */
public class LimitedSizeStack<T>
{
    //основная коллекция, которая хранит данные. Используется linkedlist,
    //т.к. удобно и позволяет реализовать операцию Push за O(1)

    private readonly LinkedList<T> list = new();
    
    // максимальная размерность стэка
    private readonly int MaxCount = 0;

    //хранит количество элементов в стеке
    public int Count { get; private set; }

    // конструктор, задает размерность стека, инициализирует объект
    public LimitedSizeStack(int undoLimit)
    {
        MaxCount = undoLimit;
    }

    //Операция добавления
    public void Push(T item)
    {
        // Частный случай: размер стека 0. Заверщает работу метода
        if (MaxCount == 0) return;

        // Общий случай добавления.

        // Если храниться больше или равное максимальному число элементов:
        if (list.Count >= MaxCount)
        {
            // убираем первый элемент, уменьшаем длину на 1
            list.RemoveFirst();
            Count--;
        }

        // добавляем входной элемент. Испольняет в любом случае.
        list.AddLast(item);
        Count++;
    }

    // Операция получения 
    public T Pop()
    {
        // Если стек не содержит элементов: ошибка
        if (list.Count <= 0)
            throw new InvalidOperationException();

        // запоминаем резульат
        var result = list.Last.Value;

        // убираем последний элемент из стека, уменьшаем длину, возвращаем значение
        list.RemoveLast();
        Count--;
        return result;
    }

}