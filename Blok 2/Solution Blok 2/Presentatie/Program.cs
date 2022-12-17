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
        MatrixOperator mo = new MatrixOperator();
        IMatrixProvider mp = new MatrixProvider();
        NeuralNetwork nn = new NeuralNetwork(1, mo, mp);
        nn.AddLayer(1);
        nn.AddLayer(2);
        List<double[]> traindata = new List<double[]>() { new double[] { 1} };
        List<double[]> trainout = new List<double[]>() { new double[] { 1,0} };

        for (int i = 0; i < 100; i++) {
            nn.Train(traindata, trainout);
        }
        Console.WriteLine(nn.Predict(traindata[0]).Serialize());
    }
}