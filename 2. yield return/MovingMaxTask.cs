using System.Collections.Generic;

namespace yield;

public static class MovingMaxTask
{
    // Метод для вычисления максимума в скользящем окне
    public static IEnumerable<DataPoint> MovingMax(this IEnumerable<DataPoint> data, int windowWidth)
    {
        var queue = new Queue<double>(); // Очередь для хранения текущих значений Y в окне
        var potentialMax = new LinkedList<double>(); // Связный список для хранения потенциальных максимумов

        // Проходим по каждому элементу в коллекции данных
        foreach (var item in data)
        {
            queue.Enqueue(item.OriginalY); // Добавляем текущее значение Y в очередь

            // Если размер очереди превышает ширину окна, удаляем старую точку
            if (queue.Count > windowWidth)
            {
                // Если удаляемое значение является текущим максимумом, удаляем его из списка потенциальных максимумов
                if (queue.Dequeue() == potentialMax.First.Value)
                    potentialMax.RemoveFirst();
            }

            // Удаляем все значения из конца списка потенциальных максимумов, которые меньше текущего значения Y
            while (potentialMax.Count > 0 && item.OriginalY > potentialMax.Last.Value)
                potentialMax.RemoveLast();

            // Добавляем текущее значение Y в список потенциальных максимумов
            potentialMax.AddLast(item.OriginalY);

            // Возвращаем новую точку с максимальным значением Y из списка потенциальных максимумов
            yield return item.WithMaxY(potentialMax.First.Value);
        }
    }
}