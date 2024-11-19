using System;

namespace BinaryTrees
{
    // Класс для представления бинарного дерева поиска
    public class BinaryTree<T> where T : IComparable
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
            Root = new Node<T>(value);
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
    }

    // Класс для представления узла дерева
    public class Node<T>
    {
        public T Value; // Значение узла
        public Node<T> Left, Right; // Левый и правый дочерние узлы

        // Конструктор для создания узла с заданным значением
        public Node(T value)
        {
            Value = value; // Устанавливаем значение узла
        }
    }
}
