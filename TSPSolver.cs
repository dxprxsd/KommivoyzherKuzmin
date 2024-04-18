using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using KommivoyzherKuzmin;


// Класс для решения Travelling Salesman Problem (TSP) [Задача коммивояжера]
public class TSPSolver
{
    public void Solve(double[,] input)
    {
        List<Node> nodesList = new List<Node>();
        /*
                int cityFrom = -1;
                int cityTo = -1;
                double rootLocalLowerBoundary = -1;
                bool isStarred = false;
                double[,] currentMatrix = MatrixUtilities.CopyMatrix(input);
                Node parent = null;
                List<Node> children = new List<Node>();

                rootLocalLowerBoundary = CalculateRootLocalLowerBoundary(currentMatrix, n, m);
        */
        int n = input.GetLength(0); // Количество строк
        int m = input.GetLength(1); // Количество столбцов

        double[,] rootMatrix = MatrixUtilities.CopyMatrix(input); 
        double rootLocalLowerBoundary = CalculateRootLocalLowerBoundary(rootMatrix, n, m);

        Node root = new Node(-1, -1, rootLocalLowerBoundary, false, rootMatrix, null);
        nodesList.Add(root);

        Node currentNode;

        while (true)
        {
            currentNode = FindNodeWithLowestRootLocalLowerBoundary(nodesList);

            if (IsSolutionFound(currentNode.Matrix, n, m))
            {
                break;
            }

            double[,] grades = FindGradesForZeroes(currentNode.Matrix, n, m);

            var findElementWithMaxGradeResult = FindElementWithMaxGrade(currentNode.Matrix, n, m, grades);

            double[,] newMatrix = ConductMatrixReduction(currentNode.Matrix, n, m, findElementWithMaxGradeResult.Item1, findElementWithMaxGradeResult.Item2);

            Node newNode = new Node(CityFrom: findElementWithMaxGradeResult.Item1, CityTo: findElementWithMaxGradeResult.Item2, RootLocalLowerBoundary: -1, IsStarred: false, Matrix: newMatrix, currentNode);

            Node newStarredNode = new Node(CityFrom: findElementWithMaxGradeResult.Item1, CityTo: findElementWithMaxGradeResult.Item2, RootLocalLowerBoundary: -1, IsStarred: true, Matrix: currentNode.Matrix, currentNode);

            newNode.RootLocalLowerBoundary = CalculateRootLocalLowerBoundary(newNode.Matrix, n, m, ParentLocalLowerBoundary: currentNode.RootLocalLowerBoundary);
            newStarredNode.RootLocalLowerBoundary = CalculateRootLocalLowerBoundary(newStarredNode.Matrix, n, m);

            nodesList.Add(newNode);
            nodesList.Add(newStarredNode);

        }

        return;

        /*List<int> cities = Enumerable.Range(0, n).ToList();
        List<int> path = new List<int>();*/
    }

    public double[] FindMinimumsForRows(double[,] matrix, int n, int m)
    {
        double[] d_i = new double[n];

        for (int i = 0; i < n; i++)
        {
            d_i[i] = Double.MaxValue;

            for (int j = 0; j < m; j++)
            {
                if (matrix[i, j] < d_i[i])
                {
                    d_i[i] = matrix[i, j];
                }
            }
        }

        return d_i;
    }

    public double[] FindMinimumsForColumns(double[,] matrix, int n, int m)
    {
        double[] d_j = new double[m];

        for (int j = 0; j < m; j++)
        {
            d_j[j] = Double.MaxValue;

            for (int i = 0; i < n; i++)
            {
                if (matrix[i, j] < d_j[j])
                {
                    d_j[j] = matrix[i, j];
                }
            }
        }

        return d_j;
    }


    public void ConductRowsReduction(double[,] matrix, int n, int m, double[] d_i)
    {
        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < m; j++)
            {
                if (matrix[i, j] != Double.MaxValue)
                {
                    matrix[i, j] -= d_i[i];
                }
            }
        }
    }

    public void ConductColumnsReduction(double[,] matrix, int n, int m, double[] d_j)
    {
        for (int j = 0; j < m; j++)
        {
            for (int i = 0; i < n; i++)
            {
                if (matrix[i, j] != Double.MaxValue)
                {
                    matrix[i, j] -= d_j[j];
                }
            }
        }
    }

    public double CalculateRootLocalLowerBoundary(double[,] matrix, int n, int m, double ParentLocalLowerBoundary = 0)
    {
        double[] d_i = FindMinimumsForRows(matrix, n, m);
        ConductRowsReduction(matrix, n, m, d_i);

        double[] d_j = FindMinimumsForColumns(matrix, n, m);
        ConductColumnsReduction(matrix, n, m, d_j);

        double rootLocalLowerBoundary = ParentLocalLowerBoundary; // Ho или Hk родителя
        for (int i = 0; i < n; i++)
        {
            rootLocalLowerBoundary += d_i[i];
        }

        for (int j = 0; j < m; j++)
        {
            rootLocalLowerBoundary += d_j[j];
        }

        return rootLocalLowerBoundary;
    }

    public double FindGrade(double[,] matrix, int n, int m, int rowIndex, int columnIndex)
    {
        double minRow = Double.MaxValue;
        double minColumn = Double.MaxValue;

        for (int i = 0; i < n; i++)
        {
            if (i != rowIndex)
            {
                if (matrix[i, columnIndex] < minColumn)
                {
                    minColumn = matrix[i, columnIndex];
                }
            }
        }

        for (int j = 0; j < m; j++)
        {
            if (j != columnIndex)
            {
                if (matrix[rowIndex, j] < minRow)
                {
                    minRow = matrix[rowIndex, j];
                }
            }
        }

        double grade = minRow + minColumn;

        return grade;
    }

    // Метод для нахождения оценок у нулевых элементов матрицы
    public double[,] FindGradesForZeroes(double[,] matrix, int n, int m)
    {
        double[,] grades = new double[n, m];
        /*
                    Dictionary<int, double> rowsMinimums = new Dictionary<int, double>();
                    Dictionary<int, double> columnsMinimums = new Dictionary<int, double>();

                    for (int i = 0; i < n; i++)
                    {
                        rowsMinimums[i] = Double.MaxValue;
                    }
                    for (int j = 0; j < m; j++)
                    {
                        columnsMinimums[j] = Double.MaxValue;
                    }


                    for (int i = 0; i < n; i++)
                    {
                        for (int j = 0; j < m; j++)
                        {
                            if (matrix[i, j] < rowsMinimums[i])
                            {
                                rowsMinimums[i] = matrix[i, j]; 
                            }
                            if (matrix[i, j] < columnsMinimums[j])
                            {
                                columnsMinimums[j] = matrix[i, j];
                            }
                        }
                    }
        */

        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < m; j++)
            {
                if (matrix[i, j] != 0)
                {
                    grades[i, j] = Double.MinValue;
                }
                else
                {
                    grades[i, j] = FindGrade(matrix, n, m, i, j);
                }
            }
        }

        return grades;
    }


    public (int, int, double) FindElementWithMaxGrade(double[,] matrix, int n, int m, double[,] grades)
    {
        // Найти ячейку с максимальной оценкой
        int maxGradeRowIndex = 0;
        int maxGradeColumnIndex = 0;

        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < m; j++)
            {
                if (grades[maxGradeRowIndex, maxGradeColumnIndex] < grades[i, j])
                {
                    maxGradeRowIndex = i;
                    maxGradeColumnIndex = j;
                }
            }
        }

        double maxGradeValue = grades[maxGradeRowIndex, maxGradeColumnIndex];

        return (maxGradeRowIndex, maxGradeColumnIndex, maxGradeValue);
    }

    public double[,] ConductMatrixReduction(double[,] inputMatrix, int n, int m, int maxGradeRowIndex, int maxGradeColumnIndex)
    {
        double[,] matrix = MatrixUtilities.CopyMatrix(inputMatrix);

        // Редукция матрицы
        /*for (int i = 0; i < n; i++)
        {

            matrix[i, maxGradeColumnIndex] = Double.MaxValue; // ???????????? !
        }

        for (int j = 0; j < m; j++)
        {
            matrix[maxGradeRowIndex, j] = Double.MaxValue; // ???????????? !
        }*/

        matrix = MatrixUtilities.CreateNewMatrixWithRemovedRowAndColumn(matrix, maxGradeRowIndex, maxGradeColumnIndex);
            
        matrix[maxGradeColumnIndex, maxGradeRowIndex] = Double.MaxValue;


        return matrix;
    }


    // Нахождение неветвящегося узла с минимальным H_k
    public Node FindNodeWithLowestRootLocalLowerBoundary(List<Node> nodesList)
    {
        Node minNode = nodesList.ElementAt(0);


        foreach (Node node in nodesList)
        {
            if (!node.IsBranchedOut() && node.RootLocalLowerBoundary < minNode.RootLocalLowerBoundary)
            {
                minNode = node;
            }
        }

        return minNode;
    }

    public bool IsSolutionFound(double[,] matrix, int n, int m)
    {
        int realValuesAmount = 0;

        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < m; j++)
            {
                if (matrix[i, j] != Double.MaxValue)
                {
                    realValuesAmount += 1;
                }
            }
        }

        if (realValuesAmount == 0)
        {
            throw new Exception("realValuesAmount = 0, inside IsSolutionFound()");
        }
        else if (realValuesAmount == 1)
        {
            return true;
        }
        else
        {
            return false;
        }

    }
}