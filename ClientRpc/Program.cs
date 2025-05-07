using CookComputing.XmlRpc;
using System;

public interface IMathService : IXmlRpcProxy
{
    [XmlRpcMethod("ServerUpdateMatrix")]
    int[,] ServerUpdateMatrix(int[,] matrix);

    [XmlRpcMethod("SearchMinValueDiagonal")]
    (int minValueMainDiagonal, int minValueSideDiagonal) ServerSearchMinValueDiagonal(int[,] matrix);
}

class Client
{
    static void Main()
    {
        Matrix matrix = new Matrix();
        IMathService proxy = XmlRpcProxyGen.Create<IMathService>();
        proxy.Url = "http://localhost:8080/";

        Console.Write("Размер матрицы: ");
        int.TryParse(Console.ReadLine(), out int size);

        int[,] matrixArray = new Matrix().RandomMatrix(size);
        Console.WriteLine("Отправляемая матрица:");
        matrix.PrintMatrix(matrixArray, enableChangeColor:true);

        var data = proxy.ServerSearchMinValueDiagonal(matrixArray);
        int minValueMainDiagonal = data.minValueMainDiagonal;
        int minValueSideDiagonal = data.minValueSideDiagonal;
        Console.WriteLine($"\nМинимальное значение основной диагонали: {minValueMainDiagonal}");
        Console.WriteLine($"Минимальное значение побочной диагонали: {minValueSideDiagonal}");

        int[,] result = proxy.ServerUpdateMatrix(matrixArray);
        Console.WriteLine("\nРезультирующая матрица:");
        matrix.PrintMatrix(result);

    }
}

public class Matrix
{
    public int[,] RandomMatrix(int size)
    {
        Random random = new Random();
        int[,] array = new int[size, size];

        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                array[i, j] = random.Next(10, 100);
            }
        }
        return array;
    }

    public void PrintMatrix(int[,] matrix, bool enableChangeColor = false)
    {
        for (int i = 0; i < matrix.GetLength(0); i++)
        {
            for (int j = 0; j < matrix.GetLength(1); j++)
            {
                if ((i == j || i == matrix.GetLength(1) - j - 1) && enableChangeColor) Console.ForegroundColor = ConsoleColor.Green;
                else Console.ForegroundColor = ConsoleColor.White;
                Console.Write(matrix[i, j] + "\t");
            }
            Console.WriteLine();
        }
        Console.ResetColor();
    }

}
