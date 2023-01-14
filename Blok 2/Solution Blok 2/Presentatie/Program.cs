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
using System.Diagnostics;
using System.Runtime.Serialization;

#pragma warning disable CS8600
#pragma warning disable CS8602
#pragma warning disable CS0162
#pragma warning disable IDE0035

internal class Program {
    static void Main(string[] args) {
        Console.WriteLine(
            @"dit is proof of concept aangezien deze applicatie wordt uitgebouwd als een library;
opties voor uitvoeren van code:
1. handgeschreven cijfers(0 of 1) classificeren (zeer traag)
2. xor ""problem"" (vrij snel)");
        string answer;
        int choice = 0;
        do {
            answer = Console.ReadLine() ?? "";
            try {
                choice = Int16.Parse(answer);
                if (choice != 1 && choice != 2) choice = 0;
            }
            catch {
                Console.WriteLine("geef een nummer 1 of 2");
            }
        } while (choice == 0);

        IMatrixOperator mo = new MatrixOperator(500);
        IMatrixProvider mp = new MatrixProvider();

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
        Neural2? nn = null;
        string serializedXml;
        XmlSerializer serializer = new XmlSerializer(typeof(Neural2), new Type[] {
            typeof(DenseLayer),
            typeof(ConvolutionLayer),
            typeof(ReshapeLayer),
            typeof(ActivationLayer),
            typeof(SigmoidLayer),
            typeof(TanhLayer),
        });

        if (choice == 1) {// mnist
            Console.WriteLine("gebruik pre-trained model? (geef een pad of laat leeg)");
            answer = Console.ReadLine() ?? "";
            if (answer.Length > 0) {
                serializedXml = XML_IO.Read(answer);
                using (StringReader stringReader = new StringReader(serializedXml)) {
                    nn = (Neural2)serializer.Deserialize(stringReader);
                }
            }
            else {
                nn = new Neural2(784, mo, mp);
                nn.AddReshapeLayer(new int[] { 28 * 28, 1 }, new int[] { 28, 28, 1 });
                nn.AddConvolutionLayer(new int[] { 28, 28, 1 }, 3, 5);
                nn.AddActivation(ActivationType.SIGMOID);
                nn.AddReshapeLayer(new int[] { 26, 26, 5 }, new int[] { 26 * 26 * 5, 1 });
                nn.AddDenseLayer(100);
                nn.AddActivation(ActivationType.SIGMOID);
                nn.AddDenseLayer(1);
                nn.AddActivation(ActivationType.SIGMOID);
                nn.TrainingRate = 0.1;

                Console.WriteLine("trainingdata inladen...");
                List<List<double[]>> labeledTrainImages = LoadPreparedTrainData();

                nn.Train(labeledTrainImages[0], labeledTrainImages[1], loss, lossPrime, 20, 0);
            }
            Console.WriteLine("\ntestdata inladen...");
            List<List<double[]>> labeledTestImages = LoadPreparedTestData();

            for (int i = 0; i < labeledTestImages[0].Count; i++) {
                var output = nn.Predict(labeledTestImages[0][i]);
                Console.WriteLine($"expected: {labeledTestImages[1][i][0]}, result(rounded output): {Math.Round(output[0, 0])}");
            }
        }
        else if (choice == 2) {
            nn = new Neural2(2, mo, mp);
            nn.AddDenseLayer(3);
            nn.AddActivation(ActivationType.SIGMOID);
            nn.AddDenseLayer(1);
            nn.AddActivation(ActivationType.TANH);
            nn.TrainingRate = 0.1;

            List<double[]> trainInput = new List<double[]>() {
                new double[]{0,0},
                new double[]{0,1},
                new double[]{1,0},
                new double[]{1,1}
            };

            List<double[]> trainLabels = new List<double[]>() {
                new double[]{-1},
                new double[]{ 1},
                new double[]{ 1},
                new double[]{-1}
            };

            loss = (Matrix expected, Matrix predicted) => {
                var sub = mo.Subtract(expected, predicted);
                sub = mo.Multiply(sub, sub);
                return mo.Average(sub);
            };

            lossPrime = (Matrix expected, Matrix predicted) => {
                return mo.Multiply(mo.Subtract(expected, predicted),
                    mp.Zero(expected.Rows, expected.Columns).Map(x => 2 / (expected.Rows * expected.Columns))
                    );
            };

            nn.Train(trainInput, trainLabels, loss, lossPrime, 10000, 0, false);
            double[][] gs = new double[28][];
            
            for (double i = 0; i < 28; i++) {
                double[] gs2 = new double[28];
                for (double j = 0; j < 28; j++) {
                    gs2[(int)j] = nn.Predict(new double[] { i / 27, j / 27 })[0,0];
                }
                gs[(int)i] = gs2;
            }
            Console.WriteLine();
            PrintGreyscaleArray(gs);
        }
        save(serializer, nn ?? new Neural2(1, mo, mp));
    }

    static double[] Sanitize(double[] arr, double min, double max) {
        for (int i = 0; i < arr.Length; i++) {
            arr[i] /= (max - min);
        }
        return arr;
    }

    static void PrintGreyscaleArray(double[][] arr) {//232 - 255
        for (int i = 0; i < arr.Length; i++) {
            for(int j = 0; j < arr[i].Length;j++) {
                Console.Write($"\x1b[48;5;" + (int)Math.Floor(Helper.Scale(arr[i][j],-1,1,232,256))+"m  ");
            }
            Console.WriteLine("\x1b[48;5;232m");
        }
        Console.WriteLine();
    }

    static List<List<double[]>> LoadPreparedTrainData() {
        const string trainPath = "./Resources/train-";
        const string labelSuffix = "labels.idx1-ubyte";
        const string imageSuffix = "images.idx3-ubyte";

        var trainImages = DataReader.ReadMnistFractionedAsync(trainPath + imageSuffix, 0, 10000);
        var trainLabels = DataReader.ReadMnistFractionedAsync(trainPath + labelSuffix, 0, 10000);
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

        foreach (var val in trainFilteredImages) Sanitize(val, 0, 255);

        List<List<double[]>> result = new List<List<double[]>> {
            trainFilteredImages,
            trainFilteredLabels
        };
        return result;
    }

    static List<List<double[]>> LoadPreparedTestData() {
        const string testPath = "./Resources/t10k-";
        const string labelSuffix = "labels.idx1-ubyte";
        const string imageSuffix = "images.idx3-ubyte";
        var testImages = DataReader.ReadMnistAsync(testPath + imageSuffix);
        var testLabels = DataReader.ReadMnistAsync(testPath + labelSuffix);
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

        foreach (var val in testFilteredImages) Sanitize(val, 0, 255);

        List<List<double[]>> result = new List<List<double[]>> {
            testFilteredImages,
            testFilteredLabels
        };
        return result;
    }

    static void save(XmlSerializer serializer, Neural2 nn) {
        string serializedXml;
        using (StringWriter stringWriter = new StringWriter()) {
            serializer.Serialize(stringWriter, nn);
            serializedXml = stringWriter.ToString();
        }
        try {
            Console.WriteLine("path to save to (geef een pad op of laat leeg): ");
            string? path = Console.ReadLine();
            XML_IO.Write(path ?? "", serializedXml);
        }
        catch { }
    }
}