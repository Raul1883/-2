using System;
using System.Collections.Generic;

namespace func.brainfuck
{
    // Интерфейс для виртуальной машины
    public class VirtualMachine : IVirtualMachine
    {
        // Свойство, которое хранит инструкции программы в виде строки
        public string Instructions { get; }

        // Указатель на текущую инструкцию (индекс в строке Instructions)
        public int InstructionPointer { get; set; }

        // Массив памяти, который хранит байты
        public byte[] Memory { get; }

        // Указатель на текущую ячейку памяти (индекс в массиве Memory)
        public int MemoryPointer { get; set; }

        // Словарь, который связывает символы команд с соответствующими действиями (делегатами)
        private readonly Dictionary<char, Action<IVirtualMachine>> CommandDictionary = new();

        // Конструктор класса VirtualMachine
        // Принимает программу (строку инструкций) и размер памяти
        public VirtualMachine(string program, int memorySize)
        {
            Instructions = program; // Сохраняем инструкции
            Memory = new byte[memorySize]; // Инициализируем память заданного размера
        }

        // Метод для регистрации новой команды
        // Принимает символ команды и действие, которое будет выполняться при встрече этой команды
        public void RegisterCommand(char symbol, Action<IVirtualMachine> execute)
        {
            // Добавляем символ и соответствующее действие в словарь команд
            CommandDictionary.TryAdd(symbol, execute);
        }

        // Метод для выполнения программы
        public void Run()
        {
            // Цикл, который продолжается, пока указатель на инструкцию меньше длины инструкций
            while (InstructionPointer < Instructions.Length)
            {
                // Проверяем, существует ли команда для текущего символа
                if (CommandDictionary.ContainsKey(Instructions[InstructionPointer]))
                {
                    // Выполняем команду, передавая текущую виртуальную машину как аргумент
                    CommandDictionary[Instructions[InstructionPointer]](this);
                }
                // Увеличиваем указатель на инструкцию для перехода к следующей
                InstructionPointer++;
            }
        }
    }
}
