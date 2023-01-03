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

        Neural2 nn = new Neural2(9, mo, mp);
        nn.AddReshapeLayer(new int[] { 9, 1 }, new int[] { 3, 3, 1 });
        nn.AddConvolutionLayer(new int[] { 3, 3, 1 }, 3, 3);
        nn.addActivation(ActivationType.SIGMOID);
        nn.AddReshapeLayer(new int[] { 1, 1, 3 }, new int[] { 3, 1 });
        nn.AddDenseLayer(1);
        nn.addActivation(ActivationType.SIGMOID);

        //Console.WriteLine(nn.Predict(new double[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 }).Serialize());

        List<double[]> traindata = new List<double[]>() {
            new double[] { 9, 4, 8, 5, 3, 7, 6, 2, 1 }, //random
            new double[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 }, //sorted
            new double[] { 9, 8, 7, 6, 5, 4, 3, 2, 1 }, //sorted
            new double[] { 4, 8, 7, 1, 2, 3, 9, 5, 6 }  //random
        };
        List<double[]> trainout = new List<double[]>() {
            new double[] { 0},
            new double[] { 1},
            new double[] { 1},
            new double[] { 0}
        };

        nn.TrainingRate = 0.9;

        // binary cross entropy
        Func<Matrix, Matrix, double> loss = (Matrix expected, Matrix predicted) => {
            double sum = 0;
            for (int i = 0; i < expected.Rows; i++) {
                sum += (-expected[i, 0] * Math.Log10(predicted[i, 0])) - (1 - expected[i, 0]) * Math.Log10(1 - predicted[i, 0]);
            }
            return sum / expected.Rows;
        };
        //bce prime
        Func<Matrix, Matrix, Matrix> lossPrime = (Matrix expected, Matrix predicted) => {
            var result = new Matrix(expected.Rows, expected.Columns);
            for (int i = 0; i < result.Rows; i++) {
                for (int j = 0; j < result.Columns; j++) {
                    result[i, j] = ((1 - expected[i, j]) / (1 - predicted[i, j]) - (expected[i, j] / predicted[i, j])) / (result.Rows * result.Columns);
                }
            }
            return result;
        };
           
        nn.Train(traindata, trainout, loss, lossPrime, 80000,0.00005);
        
        Console.WriteLine(nn.Predict(new double[] { 9, 4, 8, 5, 3, 7, 6, 2, 1 }).Serialize());
        Console.WriteLine(nn.Predict(new double[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 }).Serialize());
        Console.WriteLine(nn.Predict(new double[] { 9, 8, 7, 6, 5, 4, 3, 2, 1 }).Serialize());
        Console.WriteLine(nn.Predict(new double[] { 4, 8, 7, 1, 2, 3, 9, 5, 6 }).Serialize());

        //Console.WriteLine("waarschuwing: data inladen kan soms lang duren hou het tot op zijn meest 10000 images");
        //var task = DataReader.ReadMnistAsync("./Resources/t10k-labels.idx3-ubyte");

        //byte[] t = task.Result;

        //Console.WriteLine(task.Result);

    }
}