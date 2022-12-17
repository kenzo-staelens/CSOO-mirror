using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Logica;
using Globals;
using ExtensionMethods;
using Datalaag;

internal class Program {
    private static void Main(string[] args) {
        Matrix matrix= new Matrix(3,2);
        matrix.MatrixData[0, 0] = 0;
        matrix.MatrixData[0, 1] = 1;
        //MatrixOperator Operator = new MatrixOperator();
        //var r = Operator.Transpose(matrix);
        MatrixProvider mp = new MatrixProvider();
        NeuralNetwork nn = new NeuralNetwork(2, mp);
        nn.AddLayer(4);
        nn.AddLayer(3);
        var pred = nn.predict(new double[] { 1, 0 });
        Console.WriteLine(pred.Serialize());
    }
}