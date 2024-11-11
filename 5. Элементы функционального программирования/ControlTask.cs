using System;

namespace func_rocket;

// Класс ControlTask отвечает за управление ракетой, определяя, как она должна поворачиваться в сторону цели
public class ControlTask
{
    // Метод ControlRocket принимает ракету и цель, и возвращает направление поворота
    public static Turn ControlRocket(Rocket rocket, Vector target)
    {
        // Вычисляем угол между текущим направлением ракеты и направлением на цель
        var difAngle = CalculateAngle(rocket, target);

        // Если угол равен нулю, ракета уже направлена на цель, возвращаем "Нет поворота"
        if (difAngle == 0) return Turn.None;

        // Если угол положительный, ракета должна поворачиваться вправо
        if (difAngle > 0) return Turn.Right;

        // Если угол отрицательный, ракета должна поворачиваться влево
        return Turn.Left;
    }

    // Приватный метод CalculateAngle вычисляет угол между направлением ракеты и вектором к цели
    private static double CalculateAngle(Rocket rocket, Vector target)
    {
        // Вычисляем расстояние между ракетой и целью
        var distance = target - rocket.Location;
        // Получаем угол текущей скорости ракеты
        var velocityAngle = rocket.Velocity.Angle;

        // Проверяем, находится ли угол между направлением на цель и углом скорости в пределах 45 градусов
        if (Math.Abs(distance.Angle - velocityAngle) < Math.PI / 4)
            // Если да, то возвращаем усредненный угол для более плавного управления
            return (distance.Angle * 2 - velocityAngle - rocket.Direction) / 2;

        // В противном случае возвращаем угол между направлением на цель и текущим направлением ракеты
        return distance.Angle - rocket.Direction;
    }
}
