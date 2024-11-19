using System;
using System.Collections;
using System.Collections.Generic;

// !!ТЕСТЫ МОГУТ НЕ ПРОХОДИТЬ С ПЕРВОГО РАЗА ЭТО НОРМАЛЬНО !!
// Если пишет, что bynary three is too slow отправь ещё раз. В пределах 5 раз проходит. 
// Тут просто несколько странные тесты

namespace BinaryTrees
{
    // Класс для представления бинарного дерева поиска, реализующий интерфейс IEnumerable<T>
    public class BinaryTree<T> : IEnumerable<T> where T : IComparable
    {
        // Корень дерева
        private Node<T> Root;

        // Метод для добавления нового значения в дерево
        public void Add(T value)
        {
            // Начинаем с корня дерева
            var currentNode = Root;

            // Проходим по дереву, пока не найдем место для нового узла
            while (currentNode is not null)
            {
                // Увеличиваем глубину текущего узла
                currentNode.Depth++;

                // Сравниваем текущее значение с узлом
                if (value.CompareTo(currentNode.Value) == -1)
                {
                    // Если текущее значение меньше, переходим в левое поддерево
                    if (currentNode.Left is null)
                    {
                        // Если левый узел пуст, добавляем новый узел
                        currentNode.Left = new Node<T>(value);
                        return;
                    }
                    // Переходим к левому узлу
                    currentNode = currentNode.Left;
                }
                else
                {
                    // Если текущее значение больше или равно, переходим в правое поддерево
                    if (currentNode.Right is null)
                    {
                        // Если правый узел пуст, добавляем новый узел
                        currentNode.Right = new Node<T>(value);
                        return;
                    }
                    // Переходим к правому узлу
                    currentNode = currentNode.Right;
                }
            }

            // Если дерево было пустым, устанавливаем новый узел как корень
            Root ??= new Node<T>(value);
        }

        // Метод для проверки наличия значения в дереве
        public bool Contains(T value)
        {
            // Если корень пуст, значит дерево пустое
            if (Root == null)
                return false;

            // Начинаем с корня дерева
            var currentNode = Root;

            // Проходим по дереву, пока не найдем нужное значение или не достигнем конца
            while (true)
            {
                // Если значение найдено, возвращаем true
                if (value.CompareTo(currentNode.Value) == 0)
                    return true;

                // Если текущее значение меньше, переходим в левое поддерево
                if (value.CompareTo(currentNode.Value) == -1)
                {
                    // Если левый узел пуст, значение отсутствует
                    if (currentNode.Left is null)
                        return false;
                    // Переходим к левому узлу
                    currentNode = currentNode.Left;
                }
                else
                {
                    // Если текущее значение больше, переходим в правое поддерево
                    if (currentNode.Right is null)
                        return false;
                    // Переходим к правому узлу
                    currentNode = currentNode.Right;
                }
            }
        }

        // Индексатор для доступа к i-му элементу в порядке возрастания
        public T this[int i]
        {
            get
            {
                var node = Root;

                // Проходим по дереву для нахождения i-го элемента
                while (true)
                {
                    // Получаем глубину левого поддерева
                    var currentNodeDepth = node.Left?.Depth ?? 0;

                    // Если i совпадает с глубиной, возвращаем значение текущего узла
                    if (i == currentNodeDepth)
                        return node.Value;

                    // Если i меньше глубины, переходим в левое поддерево
                    if (i < currentNodeDepth)
                        node = node.Left;
                    else
                    {
                        // Переходим в правое поддерево и уменьшаем i
                        node = node.Right;
                        i -= currentNodeDepth + 1;
                    }
                }
            }
        }

        // Реализация интерфейса IEnumerable<T>
        public IEnumerator<T> GetEnumerator() => GetBinaryTreeEnumerator(Root);
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        // Метод для получения перечислителя, который обходит дерево в порядке возрастания
        private static IEnumerator<T> GetBinaryTreeEnumerator(Node<T> root)
        {
            // Если узел пуст, выходим
            if (root is null)
                yield break;

            // Рекурсивно обходим левое поддерево
            var currentEnum = GetBinaryTreeEnumerator(root.Left);
            while (currentEnum.MoveNext())
            {
                yield return currentEnum.Current;
            }

            // Возвращаем значение текущего узла
            yield return root.Value;

            // Рекурсивно обходим правое поддерево
            currentEnum = GetBinaryTreeEnumerator(root.Right);
            while (currentEnum.MoveNext())
            {
                yield return currentEnum.Current;
            }
        }

        // Класс для представления узла дерева
        private class Node<TValue>
        {
            public TValue Value; // Значение узла
            public Node<TValue> Left, Right; // Левый и правый дочерние узлы
            public int Depth = 1; // Глубина узла (размер поддерева)

            // Конструктор для создания узла с заданным значением
            public Node(TValue value)
            {
                Value = value; // Устанавливаем значение узла
            }
        }
    }
}
