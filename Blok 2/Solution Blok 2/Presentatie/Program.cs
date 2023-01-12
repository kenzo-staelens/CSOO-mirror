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
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using System.IO;
using System.Xml;
using System.Text.Json;

internal class Program {
    static void Main(string[] args) {
        const string trainPath = "./Resources/train-";
        const string testPath = "./Resources/t10k-";
        const string labelSuffix = "labels.idx1-ubyte";
        const string imageSuffix = "images.idx3-ubyte";

        IMatrixOperator mo = new MatrixOperator(500);
        IMatrixProvider mp = new MatrixProvider();

        /* Console.WriteLine("waarschuwing: data inladen kan soms lang duren");
         var trainImages = DataReader.ReadMnistFractionedAsync(trainPath+imageSuffix,0,10000);
         var trainLabels = DataReader.ReadMnistFractionedAsync(trainPath+labelSuffix,0,10000);
         var trainImageResult = trainImages.Result;
         var trainLabelResult = trainLabels.Result;

         List<double[]> trainFilteredImages = new List<double[]>();
         List<double[]> trainFilteredLabels = new List<double[]>();
         for (int i = 0; i < trainLabelResult.Count; i++) {
             if (trainLabelResult[i][0] == 0 || trainLabelResult[i][0] == 1) {
                 trainFilteredImages.Add(trainImageResult[i]);
                 trainFilteredLabels.Add(trainLabelResult[i]);
             }
         }

         trainImages = null; // cleanup van memory
         trainLabels = null;
         trainImageResult = null;
         trainLabelResult = null;

         // foreach (var val in testFilteredImages) sanitize(val, 0, 255);
         foreach (var val in trainFilteredImages) sanitize(val, 0, 255);

         // gebruik "sanitaire" waarden voor input (tussen 0 en 1) anders kan vaak NAN voorkomen

         Neural2 nn = new Neural2(784, mo, mp);
         nn.AddReshapeLayer(new int[] { 28 * 28, 1 }, new int[] { 28, 28, 1 });
         nn.AddConvolutionLayer(new int[] { 28, 28, 1 }, 3, 5);
         nn.addActivation(ActivationType.SIGMOID);
         nn.AddReshapeLayer(new int[] { 26, 26, 5 }, new int[] { 26 * 26 * 5, 1 });
         nn.AddDenseLayer(100);
         nn.addActivation(ActivationType.SIGMOID);
         nn.AddDenseLayer(1);
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

         nn.Train(trainFilteredImages, trainFilteredLabels, loss, lossPrime, 20, 0);


         var testImages = DataReader.ReadMnistAsync(testPath+imageSuffix);
         var testLabels = DataReader.ReadMnistAsync(testPath+labelSuffix);
         var testImageResult = testImages.Result;
         var testLabelResult = testLabels.Result;


         List<double[]> testFilteredImages = new List<double[]>();
         List<double[]> testFilteredLabels = new List<double[]>();
         for (int i = 0; i < testLabelResult.Count; i++) {
             if (testLabelResult[i][0] == 0 || testLabelResult[i][0] == 1) {
                 testFilteredImages.Add(testImageResult[i]);
                 testFilteredLabels.Add(testLabelResult[i]);
             }
         }


         for(int i=0;i<testFilteredImages.Count;i++) {
             var output = nn.Predict(testFilteredImages[i]);
             Console.WriteLine($"expected: {testFilteredLabels[i]}, result(rounded output): {Math.Round(output[0,0])}");
         }

         //foreach (var val in testImageresult) Console.WriteLine(printArray(val));
        */

        Matrix mat = mp.Random(2, 2);
        Console.WriteLine(mat.Serialize());
        var newmat = Matrix.Deserialize(mat.Serialize());
        Console.WriteLine(newmat.Serialize());
    }

    static double[] sanitize(double[] arr, double min, double max) {
        for (int i = 0; i < arr.Length; i++) {
            arr[i] /= (max - min);
        }
        return arr;
    }

    static string printMnistArray(double[] arr) {
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