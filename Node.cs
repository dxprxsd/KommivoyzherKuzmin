using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using KommivoyzherKuzmin;
 
public class Node // Node - узел на дереве (элемент)
{
    public char CityFrom { get; set; } // откуда
    public char CityTo { get; set; } // куда
    public double RootLocalLowerBoundary { get; set; } // Hk, корневая локальная нижняя граница
    public bool IsStarred { get; set; } // помечен ли звездочкой
    public double[,] Matrix { get; set; } 
    public BidirectionalDictionary rowsIndexLetterMap { get; set; }
    public BidirectionalDictionary colsIndexLetterMap { get; set; }


    public Node Parent { get; set; }
    private List<Node> Children { get; set; }

/*    public Node(int CityFrom, int CityTo, bool IsStarred, double[,] Matrix, Node Parent)
    {
        this.Matrix = MatrixUtilities.CopyMatrix(Matrix);
        this.Children = new List<Node>();
    }*/

/*    // конструктор (перегрузка №1)
    public Node(char CityFrom, char CityTo, double RootLocalLowerBoundary, bool IsStarred, double[,] Matrix, 
        BidirectionalDictionary RowsIndexLetterMap, BidirectionalDictionary ColsIndexLetterMap, Node Parent, List<Node> Children)
    {
        this.CityFrom = CityFrom;
        this.CityTo = CityTo;
        this.RootLocalLowerBoundary = RootLocalLowerBoundary;
        this.IsStarred = IsStarred;
        this.Matrix = MatrixUtilities.CopyMatrix(Matrix); 
        this.rowsIndexLetterMap = RowsIndexLetterMap; // TODO COPY !?
        this.colsIndexLetterMap = ColsIndexLetterMap;
        this.Parent = Parent;
        this.Children = Children;
    }*/

    // конструктор (перегрузка №1)
    public Node(char CityFrom, char CityTo, double RootLocalLowerBoundary, bool IsStarred, double[,] Matrix, 
        BidirectionalDictionary RowsIndexLetterMap, BidirectionalDictionary ColsIndexLetterMap, Node Parent)
    {
        this.CityFrom = CityFrom;
        this.CityTo = CityTo;
        this.RootLocalLowerBoundary = RootLocalLowerBoundary;
        this.IsStarred = IsStarred;
        this.Matrix = MatrixUtilities.CopyMatrix(Matrix);
        this.rowsIndexLetterMap = RowsIndexLetterMap; // TODO COPY !??
        this.colsIndexLetterMap = ColsIndexLetterMap;
        this.Parent = Parent;
        this.Children = new List<Node>();
    }

    // Ветвится ли этот узел (есть ли от него вниз другие узлы)
    public bool IsBranchedOut()
    {
        // Узел ветвится вниз => у него есть дети
        if (this.Children.Count == 0)
        {
            return false;
        }
        return true;
    }

    public void AddChild(Node node)
    {
        this.Children.Add(node);
    }

    
}