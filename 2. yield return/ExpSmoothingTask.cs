using System.Collections.Generic;
namespace yield;

// Статический класс для экспоненциального сглаживания данных
public static class ExpSmoothingTask
{

    // Метод для выполнения экспоненциального сглаживания
    public static IEnumerable<DataPoint> SmoothExponentialy(this IEnumerable<DataPoint> data, double alpha)
    {
        DataPoint previousPoint = null; // Переменная для хранения предыдущей сглаженной точки

        // Проходим по каждому элементу в коллекции данных
        foreach (var point in data)
        {
            // Если предыдущая точка не задана (это первая итерация)
            if (previousPoint == null)
            {
                // Устанавливаем первую сглаженную точку равной оригинальному значению
                previousPoint = point.WithExpSmoothedY(point.OriginalY);
                yield return previousPoint; // Возвращаем первую сглаженную точку
            }
            else
            {
                // Вычисляем новое сглаженное значение по формуле экспоненциального сглаживания
                var smoth = alpha * point.OriginalY + ((1 - alpha) * previousPoint.ExpSmoothedY);

                // Создаем новую точку с сглаженным значением, обновляем предыдущую сглаженную точку
                previousPoint = point.WithExpSmoothedY(smoth);
                yield return previousPoint; // Возвращаем новую сглаженную точку
            }
        }
    }
}