using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using KommivoyzherKuzmin;


// Класс для решения Travelling Salesman Problem (TSP) [Задача коммивояжера]
public class TSPSolver
{
    public (Dictionary<char, char>, double) Solve(double[,] input, char[] rowLetters, char[] colLetters)
    {
        List<Node> nodesList = new List<Node>(); // список для всех узлов
        int n = input.GetLength(0); // Количество строк
        int m = input.GetLength(1); // Количество столбцов

        double[,] rootMatrix = MatrixUtilities.CopyMatrix(input); // копируем матрицу ввода, чтобы матрица ввода не изменялась далее
        double rootLocalLowerBoundary = CalculateRootLocalLowerBoundary(rootMatrix); // подсчитываем границу + ВНУТРИ ПРОИСХОДИТ редукция строк и столбцов

        // создаем узел корня дерева
        Node root = new Node(CityFrom: '-', CityTo: '-', RootLocalLowerBoundary: rootLocalLowerBoundary, IsStarred: false, Matrix: rootMatrix,
            RowsIndexLetterMap: new BidirectionalDictionary(rowLetters), ColsIndexLetterMap: new BidirectionalDictionary(colLetters), Parent: null);
       
        nodesList.Add(root); // добавляем корень в список узлов

        Node currentNode; // текущий узел 

        while (true)
        {
            currentNode = FindNodeWithLowestRootLocalLowerBoundary(nodesList);

            // проверка на нахождение решения
            if (IsSolutionFound(currentNode.Matrix))
            {
                break;
            }


            // копируем матрицу узла, с которым мы сейчас будем работать
            double[,] newMatrix = MatrixUtilities.CopyMatrix(currentNode.Matrix);

            // если текущий узел - это узел со звездочкой, то тогда перед дальнейшими шагами нужно сделать редукцию строк и столбцов
            if (currentNode.IsStarred)
            {
                ConductRowsAndColumnsReduction(newMatrix); // тут изменяется newMatrix
            }

            // создаем матрицу оценок и заполняем ее результатом метода FindGradesForZeroes
            double[,] grades = FindGradesForZeroes(newMatrix);

            // findElementWithMaxGradeResult
            // return (maxGradeRowIndex, maxGradeColumnIndex, maxGradeValue);

            // Этот метод возвращает Tuple из трех значений:
            // Item 1) индекс (по строке) для максимальной оценки в матрице,
            // Item 2) индекс (по столбцу) для максимальной оценки в матрице,
            // Item 3) значение максимальной оценки в матрице
            var findElementWithMaxGradeResult = FindElementWithMaxGrade(newMatrix, grades);

            // Item1 - это индекс города "откуда"
            // Item2 - это индекс города "куда"
            char cityFrom = currentNode.rowsIndexLetterMap.GetLetter(findElementWithMaxGradeResult.Item1);
            char cityTo = currentNode.colsIndexLetterMap.GetLetter(findElementWithMaxGradeResult.Item2);


            // ConductMatrixReduction - производит редукцию матрицы и обновляет связь (двустороннюю карту) между индексами и буквами (названия строк/столбцов и их индексы)
            // return (matrix, newRowsIndexLetterMap, newColsIndexLetterMap);
            // Item 1) новая матрица
            // Item 2) новая связь индексов строк с буквами строк (и в обратном направлении: буква -> индекс)
            // Item 3) новая связь индексов столбцов с буквами столбцов (и в обратном направлении: буква -> индекс)
            var conductMatrixReductionResult = ConductMatrixReduction(newMatrix, findElementWithMaxGradeResult.Item1, findElementWithMaxGradeResult.Item2, 
                currentNode.rowsIndexLetterMap, currentNode.colsIndexLetterMap);

            // удалили строку + столбец
            // узел без звездочки

            // передаем в Matrix редуцированную матрицу (Item1 после ConductMatrixReduction) и, соответственно, обновленные связи RowsIndexLetterMap и ColsIndexLetterMap
            Node newNode = new Node(CityFrom: cityFrom, CityTo: cityTo, 
                RootLocalLowerBoundary: -1, IsStarred: false, Matrix: conductMatrixReductionResult.Item1,
                RowsIndexLetterMap: conductMatrixReductionResult.Item2,
                ColsIndexLetterMap: conductMatrixReductionResult.Item3,
                Parent: currentNode);

            // Получаем индексы города-откуда и города-куда по их букве через связь rowsIndexLetterMap и colsIndexLetterMap
            int cityFromIndex = currentNode.rowsIndexLetterMap.GetIndex(cityFrom);
            int cityToIndex = currentNode.colsIndexLetterMap.GetIndex(cityTo);

            // оставили строку + столбец
            // узел со звездочкой

            // передаем в Matrix только ту матрицу, которую ранее получили после редукции СТРОК и СТОЛЬЦОВ, не ту матрицу, что после ConductMatrixReduction
            // соответственно, сохраняем старую связь индексов и букв, поэтому нужно сделать копию 
            Node newStarredNode = new Node(CityFrom: cityFrom, CityTo: cityTo, 
                RootLocalLowerBoundary: -1, IsStarred: true, Matrix: newMatrix,
                RowsIndexLetterMap: BidirectionalDictionary.CreateNewCopy(currentNode.rowsIndexLetterMap), // копия связи индексов и букв для строк
                ColsIndexLetterMap: BidirectionalDictionary.CreateNewCopy(currentNode.colsIndexLetterMap), // копия связи индексов и букв для столбцов
                Parent: currentNode);
            newStarredNode.Matrix[cityFromIndex, cityToIndex] = Double.MaxValue; // ставим М на этот путь

            // Считаем корневые локальные нижние границы
            newNode.RootLocalLowerBoundary = CalculateRootLocalLowerBoundary(newNode.Matrix, ParentLocalLowerBoundary: currentNode.RootLocalLowerBoundary);
            
            // Для узла со звездочкой корневая локальная нижняя граница просто = корневная локальная нижняя граница родителя + текущая максимальная оценка 
            newStarredNode.RootLocalLowerBoundary = currentNode.RootLocalLowerBoundary + findElementWithMaxGradeResult.Item3;

            // Добавляем полученные узлы в список nodesList
            nodesList.Add(newNode);
            nodesList.Add(newStarredNode);

            // Добавляем полученные узлы как детей для текущего узла
            currentNode.AddChild(newNode);
            currentNode.AddChild(newStarredNode);
        }


        // Если в связи индексов и букв для столбцов или строк почему-то оказалось по итогу решения больше, чем только 1 строка и 1 столбец
        if (currentNode.colsIndexLetterMap.GetCount() != 1 || currentNode.rowsIndexLetterMap.GetCount() != 1)
        {
            throw new ArgumentException("Решение не было найдено, в конце матрица размером больше, чем 1 на 1."); 
        }

        Dictionary<char, char> path = new Dictionary<char, char>();
        char lastCityFrom = currentNode.rowsIndexLetterMap.GetLetter(0); // 0 столбец, так как матрица должна быть 1 на 1
        char lastCityTo = currentNode.colsIndexLetterMap.GetLetter(0); // 0 строка, так как матрица должна быть 1 на 1


        path[lastCityFrom] = lastCityTo; // добавляем это значение в словарь

        Node lastNode = currentNode; 

        // В цикле восстанавливаем путь (идем снизу вверх) до того момента, пока количество сегментов в пути не будет равно нужному (n)
        while (path.Count != n)
        {
            path[currentNode.CityFrom] = currentNode.CityTo; // записали пару городов: откуда-куда
            currentNode = currentNode.Parent; // переключились на родителя этого узла 

            if (currentNode == null) // если currentNode == null, значит мы как-то пришли к корню, а такого быть не должно 
            {
                throw new ArgumentException("Решение не было найдено, дошли обратно до корня.");
            }
        }

        return (path, lastNode.RootLocalLowerBoundary);
    }


    // Метод для нахождения минимумов среди строк, возвращает d_i
    public double[] FindMinimumsForRows(double[,] matrix)
    {
        int n = matrix.GetLength(0);
        int m = matrix.GetLength(1);

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

    // Метод для нахождения минимумов среди столбцов, возвращает d_j
    public double[] FindMinimumsForColumns(double[,] matrix)
    {
        int n = matrix.GetLength(0);
        int m = matrix.GetLength(1);

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


    // Метод для проведения редукции строк, изменяет matrix
    public void ConductRowsReduction(double[,] matrix, double[] d_i)
    {
        int n = matrix.GetLength(0);
        int m = matrix.GetLength(1);

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

    // Метод для проведения редукции столбцов, изменяет matrix
    public void ConductColumnsReduction(double[,] matrix, double[] d_j)
    {
        int n = matrix.GetLength(0);
        int m = matrix.GetLength(1);

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

    // Метод для проведения редукции строк и столбцов, используя нахождение минимумов по строкам и столбцам
    // Метод изменяет matrix
    public (double[], double[]) ConductRowsAndColumnsReduction(double[,] matrix)
    {
        int n = matrix.GetLength(0);
        int m = matrix.GetLength(1);

        //double[] d_i = FindMinimumsForRows(matrix, n, m);
        double[] d_i = FindMinimumsForRows(matrix);
        //ConductRowsReduction(matrix, n, m, d_i);
        ConductRowsReduction(matrix, d_i);

        //double[] d_j = FindMinimumsForColumns(matrix, n, m);
        double[] d_j = FindMinimumsForColumns(matrix);
        //ConductColumnsReduction(matrix, n, m, d_j);
        ConductColumnsReduction(matrix, d_j);

        return (d_i, d_j);
    }

    // Метод для нахождения локальной корневой границы для узлов, не помеченных звездочкой
    public double CalculateRootLocalLowerBoundary(double[,] matrix, double ParentLocalLowerBoundary = 0)
    {
        int n = matrix.GetLength(0);
        int m = matrix.GetLength(1);

        var conductRowsAndColumnsReductionResult = ConductRowsAndColumnsReduction(matrix); // внутри рассчета МЫ ДЕЛАЕМ РЕДУКЦИЮ СТРОК И СТОЛБЦОВ
        double[] d_i = conductRowsAndColumnsReductionResult.Item1;
        double[] d_j = conductRowsAndColumnsReductionResult.Item2;


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


    // Метод для нахождения оценки для текущего элемента (rowIndex, columnIndex) в матрице matrix
    public double FindGrade(double[,] matrix, int rowIndex, int columnIndex)
    {
        int n = matrix.GetLength(0);
        int m = matrix.GetLength(1);

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
    public double[,] FindGradesForZeroes(double[,] matrix)
    {
        int n = matrix.GetLength(0);
        int m = matrix.GetLength(1);

        double[,] grades = new double[n, m];

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
                    //grades[i, j] = FindGrade(matrix, n, m, i, j);
                    grades[i, j] = FindGrade(matrix, i, j);
                }
            }
        }

        return grades;
    }

    // Нахождение элемента с наибольшей оценкой
    // Возвращает его индекс по строке, индекс по столбцу, его значение
    public (int, int, double) FindElementWithMaxGrade(double[,] matrix, double[,] grades)
    {
        int n = matrix.GetLength(0);
        int m = matrix.GetLength(1);

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

    // Метод для проведения редукции матрицы
    // Возвращает матрицу, новую связь индексов и букв по строкам, новую связь индексов и букв по столбцам
    public (double[,], BidirectionalDictionary, BidirectionalDictionary) ConductMatrixReduction(double[,] inputMatrix, int maxGradeRowIndex, int maxGradeColumnIndex, BidirectionalDictionary rowsIndexLetterMap, BidirectionalDictionary colsIndexLetterMap)
    {
        int n = inputMatrix.GetLength(0);
        int m = inputMatrix.GetLength(1);

        double[,] matrix = MatrixUtilities.CopyMatrix(inputMatrix);

        // rowLetter = 'B', maxGradeRowIndex = 1
        char rowLetter = rowsIndexLetterMap.GetLetter(maxGradeRowIndex);
        // colLetter = 'D', maxGradeColumnIndex = 2
        char colLetter = colsIndexLetterMap.GetLetter(maxGradeColumnIndex);


        // rowToLetter - содержит двустороннюю связь (BidirectionalDictionary) между индексами и буквами строк
        // colToLetter - содержит двустороннюю связь (BidirectionalDictionary) между индексами и буквами столбцов

        // Проверка, что обратный путь существует
        if (rowsIndexLetterMap.CheckIfLettersExists(colLetter) && colsIndexLetterMap.CheckIfLettersExists(rowLetter))
        {
            // обратный путь
            int rowIndex = rowsIndexLetterMap.GetIndex(colLetter); // индекс строки получаем по букве столбца
            int colIndex = colsIndexLetterMap.GetIndex(rowLetter); // индекс столбца получаем по букве строки
        
            matrix[rowIndex, colIndex] = Double.MaxValue; // ставим М на обратный путь
        }


        // maxGradeRowIndex - индекс (по строке) элемента с максимальной оценкой из всей матрицы
        // maxGradeColIndex - индекс (по столбцу) элемента с максимальной оценкой из всей матрицы
        matrix = MatrixUtilities.CreateNewMatrixWithRemovedRowAndColumn(matrix, maxGradeRowIndex, maxGradeColumnIndex);

        // Так как были удалены строка и столбец, то теперь связь между индексами и буквами была нарушена, значит нужно ее поправить:
        BidirectionalDictionary newRowsIndexLetterMap = rowsIndexLetterMap.RemoveByLetterWithAdjustments(rowLetter);
        BidirectionalDictionary newColsIndexLetterMap = colsIndexLetterMap.RemoveByLetterWithAdjustments(colLetter);

        return (matrix, newRowsIndexLetterMap, newColsIndexLetterMap);
    }


    // Нахождение неветвящегося узла с минимальным H_k
    public Node FindNodeWithLowestRootLocalLowerBoundary(List<Node> nodesList)
    {
        Node minNode = null;


        foreach (Node node in nodesList)
        {
            if (!node.IsBranchedOut() && (minNode == null || node.RootLocalLowerBoundary < minNode.RootLocalLowerBoundary))
            {
                minNode = node;
            }
        }

        return minNode;
    }

    // Проверка, было ли найдено решение
    public bool IsSolutionFound(double[,] matrix)
    {
        int n = matrix.GetLength(0);
        int m = matrix.GetLength(1);

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