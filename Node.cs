using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using KommivoyzherKuzmin;

public class Node
{
    public int CityFrom { get; set; }
    public int CityTo { get; set; }
    public double RootLocalLowerBoundary { get; set; }
    public bool IsStarred { get; set; }
    public double[,] Matrix { get; set; }

    public Node Parent { get; set; }
    public List<Node> Children { get; set; }

/*    public Node(int CityFrom, int CityTo, bool IsStarred, double[,] Matrix, Node Parent)
    {
        this.Matrix = MatrixUtilities.CopyMatrix(Matrix);
        this.Children = new List<Node>();
    }*/

    public Node(int CityFrom, int CityTo, double RootLocalLowerBoundary, bool IsStarred, double[,] Matrix, Node Parent, List<Node> Children)
    {
        this.CityFrom = CityFrom;
        this.CityTo = CityTo;
        this.RootLocalLowerBoundary = RootLocalLowerBoundary;
        this.IsStarred = IsStarred;
        this.Matrix = MatrixUtilities.CopyMatrix(Matrix);
        this.Parent = Parent;
        this.Children = Children;
    }

    public Node(int CityFrom, int CityTo, double RootLocalLowerBoundary, bool IsStarred, double[,] Matrix, Node Parent)
    {
        this.CityFrom = CityFrom;
        this.CityTo = CityTo;
        this.RootLocalLowerBoundary = RootLocalLowerBoundary;
        this.IsStarred = IsStarred;
        this.Matrix = MatrixUtilities.CopyMatrix(Matrix);
        this.Parent = Parent;
        this.Children = new List<Node>();
    }

    public bool IsBranchedOut()
    {
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