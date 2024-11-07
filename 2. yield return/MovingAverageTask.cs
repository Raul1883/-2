using System.Collections.Generic;
namespace yield;

// Статический класс для вычисления скользящего среднего
public static class MovingAverageTask
{
    // Метод для вычисления скользящего среднего
    public static IEnumerable<DataPoint> MovingAverage(this IEnumerable<DataPoint> data, int windowWidth)
    {
        var queue = new Queue<DataPoint>(); // Очередь для хранения текущих точек в окне
        var sum = 0.0; // Переменная для хранения суммы значений Y в окне

        // Проходим по каждому элементу в коллекции данных
        foreach (var point in data)
        {
            queue.Enqueue(point); // Добавляем текущую точку в очередь
            sum += point.OriginalY; // Добавляем значение Y текущей точки к сумме

            // Если размер очереди превышает ширину окна, удаляем старую точку
            if (queue.Count > windowWidth)
            {
                sum -= queue.Dequeue().OriginalY; // Уменьшаем сумму на значение Y удаленной точки
            }

            // Возвращаем новую точку с усредненным значением Y
            yield return point.WithAvgSmoothedY(sum / queue.Count);
        }
    }
}