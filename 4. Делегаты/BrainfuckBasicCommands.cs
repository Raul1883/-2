using System;
using System.Linq;

namespace func.brainfuck
{
    // Класс BrainfuckBasicCommands отвечает за регистрацию базовых команд языка Brainfuck в виртуальной машине
    public class BrainfuckBasicCommands
    {
        // Метод RegisterTo регистрирует команды в виртуальной машине
        // Принимает виртуальную машину (vm), функцию для чтения символов (read) и действие для записи символов (write)
        public static void RegisterTo(IVirtualMachine vm, Func<int> read, Action<char> write)
        {
            // Регистрация команды '.', которая выводит байт памяти как символ
            vm.RegisterCommand('.', b => { write(Convert.ToChar(b.Memory[b.MemoryPointer])); });

            // Регистрация команды '+', которая увеличивает значение текущей ячейки памяти
            vm.RegisterCommand('+', PlusAction);

            // Регистрация команды '-', которая уменьшает значение текущей ячейки памяти
            vm.RegisterCommand('-', MinusAction);

            // Регистрация команды ',', которая считывает символ и сохраняет его ASCII-код в текущую ячейку памяти
            vm.RegisterCommand(',', b => { b.Memory[b.MemoryPointer] = Convert.ToByte(read()); });

            // Регистрация команды '>', которая сдвигает указатель памяти вправо
            vm.RegisterCommand('>', RightShiftAction);

            // Регистрация команды '<', которая сдвигает указатель памяти влево
            vm.RegisterCommand('<', LeftShiftAction);

            // Генерация символов от 'a' до 'z', 'A' до 'Z' и '0' до '9'
            var charCollection = Enumerable.Range('a', 26) // Символы 'a' - 'z'
                 .Concat(Enumerable.Range('A', 26)) // Символы 'A' - 'Z'
                 .Concat(Enumerable.Range('0', 10)) // Символы '0' - '9'
                 .Select(x => Convert.ToChar(x)); // Преобразование чисел в символы

            // Регистрация символов в виртуальной машине
            foreach (var symbol in charCollection)
                vm.RegisterCommand(symbol, b =>
                {
                    // Сохранение ASCII-кода символа в текущую ячейку памяти
                    b.Memory[b.MemoryPointer] = Convert.ToByte(symbol);
                });
        }

        // Статическое действие для увеличения значения текущей ячейки памяти
        private static readonly Action<IVirtualMachine> PlusAction = b =>
        {
            // Увеличиваем значение ячейки памяти, обнуляем, если оно равно 255
            b.Memory[b.MemoryPointer] =
            (byte)(b.Memory[b.MemoryPointer] == 255 ?
            0 : b.Memory[b.MemoryPointer] + 1);
        };

        // Статическое действие для уменьшения значения текущей ячейки памяти
        private static readonly Action<IVirtualMachine> MinusAction = b =>
        {
            // Уменьшаем значение ячейки памяти, устанавливаем 255, если оно равно 0
            b.Memory[b.MemoryPointer] =
            (byte)(b.Memory[b.MemoryPointer] == 0 ?
            255 : b.Memory[b.MemoryPointer] - 1);
        };

        // Статическое действие для сдвига указателя памяти вправо
        private static readonly Action<IVirtualMachine> RightShiftAction = b =>
        {
            // Сдвигаем указатель вправо, обнуляем, если он достиг конца памяти
            b.MemoryPointer = b.MemoryPointer == b.Memory.Length - 1 ?
            0 : b.MemoryPointer + 1;
        };

        // Статическое действие для сдвига указателя памяти влево
        private static readonly Action<IVirtualMachine> LeftShiftAction = b =>
        {
            // Сдвигаем указатель влево, устанавливаем на последний элемент, если он на нуле
            b.MemoryPointer = b.MemoryPointer == 0 ?
            b.Memory.Length - 1 : b.MemoryPointer - 1;
        };
    }
}
