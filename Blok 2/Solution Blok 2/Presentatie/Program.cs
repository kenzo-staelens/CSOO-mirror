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
        NeuralNetwork nn = new NeuralNetwork(2, mo, mp);
        nn.AddLayer(2);
        nn.AddLayer(1);
        List<double[]> traindata = new List<double[]>() {
            new double[] { 0,0},
            new double[] { 0,1},
            new double[] { 1,0},
            new double[] { 1,1}
        };
        List<double[]> trainout = new List<double[]>() {
            new double[] { 0},
            new double[] { 1},
            new double[] { 1},
            new double[] { 0}
        };

        for (int i = 0; i < 10000; i++) {
            nn.Train(traindata, trainout);
        }
        Console.WriteLine(nn.Predict(traindata[0]).Serialize());
        Console.WriteLine(nn.Predict(traindata[1]).Serialize());
        Console.WriteLine(nn.Predict(traindata[2]).Serialize());
        Console.WriteLine(nn.Predict(traindata[3]).Serialize());
    }
}