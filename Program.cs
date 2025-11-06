using System;
using System.Diagnostics;
using System.Threading;

namespace VectorNumberAddition
{
    // Клас для додавання числа до кожного елемента вектора
    class VectorNumberAdder
    {
        private int[] vector;
        private int numberToAdd;
        private int size;

        public VectorNumberAdder(int size, int numberToAdd)
        {
            this.size = size;
            this.numberToAdd = numberToAdd;
            this.vector = new int[size];
            InitializeVector();
        }

        // Ініціалізація вектора випадковими значеннями
        private void InitializeVector()
        {
            Random rand = new Random();
            for (int i = 0; i < size; i++)
            {
                vector[i] = rand.Next(1, 100);
            }
        }

        // Отримання копії вектора (Цей метод був відсутній)
        public int[] GetVector()
        {
            return (int[])vector.Clone();
        }

        // Виведення вектора (лише для невеликих розмірів)
        public void PrintVector(int[] arr, string message)
        {
            if (arr.Length <= 20)
            {
                Console.WriteLine(message);
                Console.WriteLine("[" + string.Join(", ", arr) + "]");
                Console.WriteLine();
            }
        }

        // ОДНОПОТОКОВА ВЕРСІЯ
        public int[] SingleThreadAdd()
        {
            int[] result = new int[size];

            // Додаємо число до кожного елемента
            for (int i = 0; i < size; i++)
            {
                result[i] = vector[i] + numberToAdd;
            }

            return result;
        }

        // БАГАТОПОТОКОВА ВЕРСІЯ
        public int[] MultiThreadAdd(int threadCount)
        {
            int[] result = new int[size];
            Thread[] threads = new Thread[threadCount];

            // Розмір блоку для кожного потоку
            int blockSize = size / threadCount;

            for (int i = 0; i < threadCount; i++)
            {
                int threadIndex = i;
                int startIndex = threadIndex * blockSize;
                // Останній потік бере на себе залишок елементів
                int endIndex = (threadIndex == threadCount - 1)
                    ? size
                    : startIndex + blockSize;

                threads[i] = new Thread(() =>
                {
                    // Кожен потік обробляє свій блок
                    AddBlock(vector, result, startIndex, endIndex, numberToAdd);
                });

                threads[i].Start();
            }

            // Очікування завершення всіх потоків
            foreach (Thread thread in threads)
            {
                thread.Join();
            }

            return result;
        }

        // Метод для додавання числа до блоку елементів (виконується в окремому потоці)
        private void AddBlock(int[] source, int[] destination, int start, int end, int number)
        {
            for (int i = start; i < end; i++)
            {
                destination[i] = source[i] + number;
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine("Лабораторна робота №1: Складання вектора з числом (Завдання 5)");
            Console.WriteLine();

            // Параметри тестування
            int[] testSizes = { 1000, 10000, 100000, 1000000, 10000000 };
            int[] threadCounts = { 1, 2, 4, 8, 16 };
            int numberToAdd = 10; // Число, яке додаємо до вектора

            Console.WriteLine($"Число для додавання: {numberToAdd}");
            Console.WriteLine($"Кількість ядер процесора: {Environment.ProcessorCount}");
            Console.WriteLine();

            // Тестування на різних розмірах
            foreach (int testSize in testSizes)
            {
                Console.WriteLine($"--- РОЗМІР ВЕКТОРУ: {testSize:N0} елементів ---");

                VectorNumberAdder adder = new VectorNumberAdder(testSize, numberToAdd);

                // Демонстрація для малих векторів
                if (testSize <= 20)
                {
                    // Цей рядок тепер працюватиме коректно
                    int[] original = adder.GetVector();
                    adder.PrintVector(original, "Оригінальний вектор:");
                }

                // Однопотокова версія
                Stopwatch sw = Stopwatch.StartNew();
                int[] singleResult = adder.SingleThreadAdd();
                sw.Stop();
                double singleThreadTime = sw.Elapsed.TotalMilliseconds;

                Console.WriteLine($"\nОднопотокова версія:");
                Console.WriteLine($"Час виконання: {singleThreadTime:F2} мс");

                if (testSize <= 20)
                {
                    adder.PrintVector(singleResult, "Результат (однопотоковий):");
                }

                // Багатопотокові версії
                Console.WriteLine($"\nБагатопотокові версії:");
                Console.WriteLine($"{"Потоків",-10} {"Час (мс)",-15} {"Прискорення",-15} {"Ефективність",-15}");
                Console.WriteLine(new string('-', 55));

                foreach (int threadCount in threadCounts)
                {
                    sw = Stopwatch.StartNew();
                    int[] multiResult = adder.MultiThreadAdd(threadCount);
                    sw.Stop();
                    double multiThreadTime = sw.Elapsed.TotalMilliseconds;

                    double speedup = singleThreadTime / multiThreadTime;
                    double efficiency = speedup / threadCount * 100;

                    Console.WriteLine($"{threadCount,-10} {multiThreadTime,-15:F2} {speedup,-15:F2} {efficiency,-15:F1}%");
                }
                Console.WriteLine();
            }

            Console.WriteLine("\nТестування завершено.");
            Console.WriteLine("\nНатисніть будь-яку клавішу для виходу...");
            Console.ReadKey();
        }
    }
}