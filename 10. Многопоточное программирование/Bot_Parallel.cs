using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace rocket_bot;

// Частичный класс Bot, который содержит логику для нахождения следующего хода ракеты
public partial class Bot
{
    // Метод для получения следующего хода ракеты
    public Rocket GetNextMove(Rocket rocket)
    {
        var tasks = CreateTasks(rocket); // получаем список тасков
        (Turn, double) bestMove = default; // инициализируем лучший ход

        tasks.ForEach(x => x.Start()); // запускаем все таски

        Task.WhenAll(tasks).ContinueWith(task =>
        {
            //обработка выполнения результатов по выполнении всех тасков
            bestMove = tasks
           .Select(x => x.Result) // Получаем результаты каждой задачи
           .Where(x => x.Item2 == tasks.Select(x => x.Result) // Находим максимальный результат
           .Max(m => m.Item2)).First(); // Выбираем первый ход с максимальным результатом
        }).Wait();  // ждем когда таски выполняться 

        return rocket.Move(bestMove.Item1, level);
    }

    // возвращает список тасков размером с количество потоков
    public List<Task<(Turn, double)>> CreateTasks(Rocket rocket)
    {
        return new Task<(Turn, double)>[threadsCount] //создаем пустой массив Тасков и инициализируем его
            .Select(x => new Task<(Turn, double)>(
            () => SearchBestMove(
                rocket, // ракета
                new Random(random.Next()), // у каждого потока должен быть свой рандом, он не потокобезопасный 
                iterationsCount / threadsCount // на каждом потоке выполниться число итераций деленное на число потоков
                ))).ToList();
    }
}
