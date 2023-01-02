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
        IMatrixOperator mo = new MatrixOperator();
        IMatrixProvider mp = new MatrixProvider();
        //NeuralNetwork nn = new NeuralNetwork(2, mo, mp);
        //nn.AddLayer(2);
        //nn.AddLayer(1);

        Neural2 nn = new Neural2(2, mo, mp);
        nn.AddDenseLayer(2);
        nn.addActivation(ActivationType.SIGMOID);
        nn.AddDenseLayer(1);
        nn.addActivation(ActivationType.SIGMOID);

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

        nn.TrainingRate = 0.9;

        Func<Matrix, Matrix, double> loss = (Matrix expected, Matrix predicted) => Math.Pow(expected[0, 0] - predicted[0, 0], 2);
        Func<Matrix, Matrix, Matrix> lossPrime = (Matrix expected, Matrix predicted) =>
            new Matrix(
                new double[,] { {2 * (predicted[0, 0] - expected[0, 0]) } }
            );

        for (int i = 0; i < 10000; i++) {
            if (i % 100 == 0) Console.WriteLine(i);
            //nn.Train(traindata, trainout);
            nn.Train(traindata, trainout, loss, lossPrime);
        }


        Console.WriteLine(nn.Predict(new double[] { 0, 0 }).Serialize());
        Console.WriteLine(nn.Predict(new double[] { 0, 1 }).Serialize());
        Console.WriteLine(nn.Predict(new double[] { 1, 0 }).Serialize());
        Console.WriteLine(nn.Predict(new double[] { 1, 1 }).Serialize());

        //Console.WriteLine("waarschuwing: data inladen kan soms lang duren hou het tot op zijn meest 10000 images");
        //var task = DataReader.ReadMnistAsync("./Resources/t10k-labels.idx3-ubyte");

        //byte[] t = task.Result;

        //Console.WriteLine(task.Result);

    }
}