using CookComputing.XmlRpc;
using System;
using System.Linq;

namespace ServerRpc
{
    [XmlRpcService]
    public class Server : XmlRpcServerProtocol
    {
        private ProcessMatrix processMatrix = new ProcessMatrix();
        [XmlRpcMethod("ServerUpdateMatrix")]
        public int[,] ServerUpdateMatrix(int[,] matrix)
        {
            return processMatrix.UpdateMatrix(matrix);
        }

        [XmlRpcMethod("SearchMinValueDiagonal")]
        public (int minValueMainDiagonal, int minValueSideDiagonal) SearchMinValueDiagonal(int[,] matrix)
        {
            return processMatrix.SearchMinValueDiagonal(matrix);
        }

    }

    public class ProcessMatrix
    {
        public int minValueMainDiagonal;
        public int minValueSideDiagonal;
        public enum MatrixDiagonal
        {
            Main,
            Side,
            None
        }

        public (int minValueMainDiagonal, int minValueSideDiagonal) SearchMinValueDiagonal(int[,] matrix)
        {
            minValueMainDiagonal = int.MaxValue;
            minValueSideDiagonal = int.MaxValue;
            for (int i = 0, j = 0; i < matrix.GetLength(0) && j < matrix.GetLength(1); i++, j++)
            {
                if (matrix[i, j] < minValueMainDiagonal) minValueMainDiagonal = matrix[i, j];
            }

            for (int i = 0, j = matrix.GetLength(1) - 1; i < matrix.GetLength(0) && j >= 0; i++, j--)
            {
                if (matrix[i, j] < minValueSideDiagonal) minValueSideDiagonal = matrix[i, j];
            }
            return (minValueMainDiagonal, minValueSideDiagonal);
        }
        public int[,] UpdateMatrix(int[,] matrix)
        {
            if (minValueMainDiagonal >= minValueSideDiagonal)
            {
                for (int i = matrix.GetLength(0) - 1, j = 0; i >= 0 && j < matrix.GetLength(1); i--, j++)
                {
                    matrix[i, j] = 0;
                }
                ReplaceElements(ref matrix, MatrixDiagonal.Main);
            }
            else
            {
                for (int i = 0, j = 0; i < matrix.GetLength(0) && j < matrix.GetLength(1); i++, j++)
                {
                    matrix[i, j] = 0;
                }
                ReplaceElements(ref matrix, MatrixDiagonal.Side);
            }
            return matrix;
        }

        private void ReplaceElements(ref int[,] matrix, MatrixDiagonal matrixDiagonal)
        {
            switch (matrixDiagonal)
            {
                case MatrixDiagonal.Main:
                    {
                        for (int i = 0; i < matrix.GetLength(0); i++)
                        {
                            for (int j = matrix.GetLength(1) - 1; j >= matrix.GetLength(0) - i; j--)
                            {
                                matrix[i, j] = matrix[i, j] * matrix[i, j];
                            }
                        }
                    }
                    break;
                case MatrixDiagonal.Side:
                    {
                        for (int i = 0; i < matrix.GetLength(0); i++)
                        {
                            for (int j = 0; j <= i; j++)
                            {
                                matrix[i, j] = matrix[i, j] * matrix[i, j];
                            }
                        }
                    }
                    break;
            }
        }
    }
}
