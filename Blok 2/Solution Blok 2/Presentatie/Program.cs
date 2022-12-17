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
        MatrixOperator mo = new MatrixOperator();
        IMatrixProvider mp = new MatrixProvider();
        NeuralNetwork nn = new NeuralNetwork(2, mo, mp);
        nn.AddLayer(4);
        nn.AddLayer(3);
        var pred = nn.predict(new double[] { 1, 0 });
        Console.WriteLine(pred.Serialize());
    }
}