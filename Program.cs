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
            double[,] tableOfKommiv =
            {
            { Double.MaxValue, 20, 18, 12, 8 },
            { 5, Double.MaxValue, 14, 7, 11 },
            { 12, 18, Double.MaxValue, 6, 11 },
            { 11, 17, 11, Double.MaxValue, 12 },
            { 5, 5, 5, 5, Double.MaxValue }
        };
            TSPSolver tspSolver = new TSPSolver();
            tspSolver.Solve(tableOfKommiv);
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
            int rows = matrix.GetLength(0);
            int columns = matrix.GetLength(1);
            double[,] result = new double[rows - 1, columns - 1];

            for (int i = 0, newi = 0; i < rows; i++)
            {
                if (i == rowToRemoveIndex) continue;

                for (int j = 0, newj = 0; j < columns; j++)
                {
                    if (j == columnToRemoveIndex) continue;

                    result[i, j] = matrix[i, j];
                    newj++;
                }
                newi++;
            }

            return result; 
        }
    }

    


}





