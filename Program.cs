using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KommivoyzherKuzmin
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //double[,] tableOfKommiv =
            //{
            //    { Double.MaxValue, 20, 18, 12, 8 },
            //    { 5, Double.MaxValue, 14, 7, 11 },
            //    { 12, 18, Double.MaxValue, 6, 11 },
            //    { 11, 17, 11, Double.MaxValue, 12 },
            //    { 5, 5, 5, 5, Double.MaxValue }
            //};

            //char[] rows = { 'A', 'B', 'C', 'D', 'E' };
            //char[] columns = { 'A', 'B', 'C', 'D', 'E' };
            char[] rows = { 'А', 'Б', 'В', 'Г' };
            char[] columns = { 'А', 'Б', 'В', 'Г' };


            double[,] tableOfKommiv =
            {
                { Double.MaxValue, 10, 15, 20 },
                { 10, Double.MaxValue, 35, 25 },
                { 15, 35, Double.MaxValue, 30 },
                { 20, 25, 30, Double.MaxValue }
            };


            TSPSolver tspSolver = new TSPSolver();
            (Dictionary<char, char>, double) solution = tspSolver.Solve(tableOfKommiv, rows, columns);
            PrintPath(solution.Item1);
            PrintLength(solution.Item2);

            // D-B (17), A-E (8), B-A (5), C-D (6) и E-C (5)

            // E -> C
            // C -> D
            // D -> B
            // B -> A
            // A -> E


            Console.ReadKey();
        }

        static void PrintPath(Dictionary<char, char> path)
        {
            Console.WriteLine("Полученный путь в ходе решения задачи коммивояжера:");

            char currentCity = 'А';  //тут русская буква!!!
            Console.Write($"{currentCity}");

            for (int i = 0; i < path.Count; i++)
            {
                currentCity = path[currentCity];
                Console.Write($" -> {currentCity}");
            }

            Console.WriteLine("\n");
        }

        static void PrintLength(double length)
        {
            Console.WriteLine($"Длина пути: {length:F2}");
        }
    }

    public static class MatrixUtilities
    {
        public static double[,] CopyMatrix(double[,] matrix)
        {
            int rows = matrix.GetLength(0); // Количество строк
            int columns = matrix.GetLength(1); // Количество столбцов

            double[,] copy = new double[rows, columns];

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    copy[i, j] = matrix[i, j];
                }
            }

            return copy;
        }

        public static double[,] CreateNewMatrixWithRemovedRowAndColumn(double[,] matrix, int rowToRemoveIndex, int columnToRemoveIndex)
        {
            // rowToRemoveIndex - индекс строки, которую мы удаляем
            // colToRemoveIndex - индекс столбца, который мы удаляем

            int rows = matrix.GetLength(0);
            int columns = matrix.GetLength(1);
            double[,] result = new double[rows - 1, columns - 1];

            // i, j - итераторы по старой матрице (по большей матрице)
            // newi, newj - итераторы по новой матрице (по меньшей матрице)
            for (int i = 0, newi = 0; i < rows; i++)
            {
                if (i == rowToRemoveIndex) continue;

                for (int j = 0, newj = 0; j < columns; j++)
                {
                    if (j == columnToRemoveIndex) continue;

                    result[newi, newj] = matrix[i, j];
                    newj++;
                }
                newi++;
            }

            return result;
        }
    }

}





