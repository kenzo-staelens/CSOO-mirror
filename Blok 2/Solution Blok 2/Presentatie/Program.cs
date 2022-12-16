using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Logica;
using Globals;

internal class Program {
    private static void Main(string[] args) {
        Matrix matrix= new Matrix(3,2);
        matrix.MatrixData[0, 0] = 0;
        matrix.MatrixData[0, 1] = 1;
        matrix.MatrixData[1, 0] = 2;
        matrix.MatrixData[1, 1] = 4;
        matrix.MatrixData[2, 0] = 5;
        matrix.MatrixData[2, 1] = 6;
        MatrixOperator Operator = new MatrixOperator();
        var r = Operator.transpose(matrix);
        Console.WriteLine(r.Serialize());
    }
}