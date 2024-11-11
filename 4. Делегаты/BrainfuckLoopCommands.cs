using System.Collections.Generic;

namespace func.brainfuck
{
    // Класс BrainfuckLoopCommands отвечает за регистрацию команд циклов '[' и ']' в виртуальной машине
    public class BrainfuckLoopCommands
    {
        // Метод Find находит соответствия между началом и концом циклов
        private static Dictionary<int, int> Find(IVirtualMachine vm)
        {
            var CycleIndexes = new Dictionary<int, int>(); // Словарь для хранения пар индексов '[' и ']'
            var Stack = new Stack<int>(); // Стек для отслеживания вложенности циклов

            // Проходим по всем инструкциям в виртуальной машине
            for (int i = 0; i < vm.Instructions.Length; i++)
            {
                // Если встречаем '[', добавляем индекс в стек
                if (vm.Instructions[i] == '[')
                    Stack.Push(i);

                // Если встречаем ']', извлекаем индекс из стека
                if (vm.Instructions[i] == ']')
                {
                    var index = Stack.Pop(); // Получаем индекс соответствующего '['
                    CycleIndexes[index] = i; // Сохраняем соответствие
                    CycleIndexes[i] = index; // Сохраняем обратное соответствие
                }
            }

            return CycleIndexes; // Возвращаем словарь с соответствиями
        }

        // Метод RegisterTo регистрирует команды циклов в виртуальной машине
        public static void RegisterTo(IVirtualMachine vm)
        {
            // Находим соответствия для циклов
            var CycleIndexes = Find(vm);

            // Регистрация команды '['
            vm.RegisterCommand('[', b =>
            {
                // Если текущий байт памяти равен нулю, перескакиваем к соответствующему ']'
                if (b.Memory[b.MemoryPointer] == 0)
                    b.InstructionPointer = CycleIndexes[b.InstructionPointer];
            });

            // Регистрация команды ']'
            vm.RegisterCommand(']', b =>
            {
                // Если текущий байт памяти не равен нулю, перескакиваем к соответствующему '['
                if (b.Memory[b.MemoryPointer] != 0)
                    b.InstructionPointer = CycleIndexes[b.InstructionPointer];
            });
        }
    }
}
