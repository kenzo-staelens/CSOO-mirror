using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Logica;
using Globals;
using ExtensionMethods;
using Datalaag;
using System.Security.Cryptography.X509Certificates;

internal class Program {
    private static void Main(string[] args) {
        IMatrixOperator mo = new MatrixOperator();
        IMatrixProvider mp = new MatrixProvider();

        Console.WriteLine("\nwaarschuwing: data inladen kan soms lang duren");
        var trainImages = DataReader.ReadMnistAsync("./Resources/t10k-images.idx3-ubyte");
        var trainLabels = DataReader.ReadMnistAsync("./Resources/t10k-labels.idx1-ubyte");
        var trainImageResult = trainImages.Result;
        var trainLabelResult = trainLabels.Result;

        var images = DataReader.ReadMnistAsync("./Resources/train-images.idx3-ubyte");
        var labels = DataReader.ReadMnistAsync("./Resources/train-labels.idx1-ubyte");
        var imageResult = images.Result;
        var labelResult = labels.Result;

        List<double[]> filteredImages = new List<double[]>();
        List<double[]> filteredLabels = new List<double[]>();
        for (int i = 0; i < labelResult.Count; i++) {
            if (labelResult[i][0] == 0 || labelResult[i][0] == 1) {
                filteredImages.Add(imageResult[i]);
                filteredLabels.Add(labelResult[i]);
            }
        }

        List<double[]> trainFilteredImages = new List<double[]>();
        List<double[]> trainFilteredLabels = new List<double[]>();
        for (int i = 0; i < labelResult.Count; i++) {
            if (trainLabelResult[i][0] == 0 || trainLabelResult[i][0] == 1) {
                trainFilteredImages.Add(trainImageResult[i]);
                trainFilteredLabels.Add(trainLabelResult[i]);
            }
        }

        foreach (var val in filteredImages) sanitize(val, 0, 255);
        foreach (var val in trainFilteredImages) sanitize(val, 0, 255);

        // gebruik "sanitaire" waarden voor input (tussen 0 en 1) anders kan vaak NAN voorkomen

        Neural2 nn = new Neural2(2, mo, mp);
        nn.AddReshapeLayer(new int[] { 28 * 28, 1 }, new int[] { 28, 28, 1 });
        nn.AddConvolutionLayer(new int[] { 28, 28, 1 }, 3, 5);
        nn.addActivation(ActivationType.SIGMOID);
        nn.AddReshapeLayer(new int[] { 5, 26, 26 }, new int[] { 5 * 26 * 26 * 1 });
        nn.AddDenseLayer(100);
        nn.addActivation(ActivationType.SIGMOID);
        nn.AddDenseLayer(2);
        nn.addActivation(ActivationType.SIGMOID);

        nn.TrainingRate = 0.1;

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

        nn.Train(trainFilteredImages, trainFilteredLabels, loss, lossPrime, 20, 0.0000001);

        for(int i=0;i<filteredImages.Count;i++) {
            var output = nn.Predict(filteredImages[i]);
            Console.WriteLine($"expected: {filteredLabels[i]}, result: {output}");
        }

        //foreach (var val in imageresult) Console.WriteLine(printArray(val));
        //foreach (var val in imageResult) sanitize(val, 0, 255);


    }

    static double[] sanitize(double[] arr, double min, double max) {
        for (int i = 0; i < arr.Length; i++) {
            arr[i] /= (max - min);
        }
        return arr;
    }

    static string printArray(double[] arr) {
        string txt = "";
        string format = "{1,3}";
        for (int i = 1; i < 28; i++) { format += ", {" + i.ToString() + ",3}"; }
        for (int i = 0; i < arr.Length; i += 28) {
            string formatted = String.Format(format,
                arr[0 + i], arr[1 + i], arr[2 + i], arr[3 + i], arr[4 + i], arr[5 + i], arr[6 + i], arr[7 + i],
                arr[8 + i], arr[9 + i], arr[10 + i], arr[11 + i], arr[12 + i], arr[13 + i], arr[14 + i], arr[15 + i],
                arr[16 + i], arr[17 + i], arr[18 + i], arr[19 + i], arr[20 + i], arr[21 + i], arr[22 + i], arr[23 + i],
                arr[24 + i], arr[25 + i], arr[26 + i], arr[27 + i]
                );
            txt += formatted + "\n";
        }
        return txt;
    }
}