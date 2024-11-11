using System;
using System.Collections;
using System.Collections.Generic;

namespace hashes
{
    // Класс-обёртка для массива байт, позволяющий сравнивать массивы по содержимому
    public class ReadonlyBytes : IEnumerable<byte>
    {
        private readonly byte[] BytesArray; // Хранит массив байт

        public int Length => BytesArray.Length; // Свойство для получения длины массива
        private readonly int Hash; // Хранит предрассчитанный хэш-код

        // Конструктор, принимающий массив байт
        public ReadonlyBytes(params byte[] array)
        {
            BytesArray = array ?? throw new ArgumentNullException(); // Проверка на null
            Hash = CalculateHash(); // Вычисление хэш-кода
        }

        // Индексатор для доступа к элементам массива
        public byte this[int index] =>
            index < 0 || index >= BytesArray.Length ?
            throw new IndexOutOfRangeException() : BytesArray[index];

        // Реализация интерфейса IEnumerable для перебора элементов массива
        public IEnumerator<byte> GetEnumerator()
        {
            for (int i = 0; i < Length; i++)
                yield return BytesArray[i];
        }

        // Необязательная реализация для IEnumerable
        IEnumerator IEnumerable.GetEnumerator() =>
            BytesArray.GetEnumerator();

        // Переопределение ToString для удобного отображения массива
        public override string ToString() =>
            "[" + String.Join(", ", BytesArray) + "]";

        // Переопределение метода Equals для сравнения объектов по содержимому
        public override bool Equals(object obj)
        {
            var array = obj as ReadonlyBytes; // Приведение объекта к типу ReadonlyBytes
            if ((array == null) || array.GetType() != typeof(ReadonlyBytes)
                || (array.Length != Length))
                return false; // Если не совпадают типы или длины, возвращаем false
            // Сравнение элементов массивов
            for (int i = 0; i < Length; i++)
                if (array[i] != BytesArray[i])
                    return false;
            return true; // Все элементы совпадают
        }

        // Метод для вычисления хэш-кода на основе содержимого массива
        private int CalculateHash()
        {
            var hash = 1234; // Начальное значение хэша
            unchecked // Игнорируем переполнение
            {
                var primeNumber = 397; // Простое число для улучшения распределения хэша
                foreach (byte b in BytesArray)
                {
                    hash *= primeNumber; // Умножаем на простое число
                    hash ^= b; // Используем XOR с текущим байтом
                }
            }
            return hash; // Возвращаем вычисленный хэш
        }

        // Переопределение метода GetHashCode для получения хэш-кода
        public override int GetHashCode() => Hash;
    }
}
