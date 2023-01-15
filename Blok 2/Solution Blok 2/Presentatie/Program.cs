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
        while(choice==0){
            answer = Console.ReadLine() ?? "";
            try {
                choice = Int16.Parse(answer);
                if (choice != 1 && choice != 2) choice = 0;
            }
            catch {
                Console.WriteLine("geef een nummer 1 of 2");
            }
        }

        IMatrixOperator mo = new MatrixOperator(500);
        IMatrixProvider mp = new MatrixProvider();

        // binary cross entropy
        Func<Matrix, Matrix, double> loss = (Matrix expected, Matrix predicted) => {
            var predcp1 = predicted.MapCopy(x => -Math.Log(x));
            var expcp1 = expected.MapCopy(y => 1 - y);
            var predcp2 = predicted.MapCopy(x => -Math.Log(1 - x));
            var mul1 = mo.Multiply(expected, predcp1);
            var mul2 = mo.Multiply(expcp1, predcp2);
            return mo.Average(mo.Add(mul1,mul2));
        };
        //bce prime
        Func<Matrix, Matrix, Matrix> lossPrime = (Matrix expected, Matrix predicted) => {
            return mo.Subtract(
                    mo.Multiply(
                        expected.MapCopy(y => 1 - y),
                        predicted.MapCopy(x => 1 / (1 - x))
                    ),
                    mo.Multiply(expected, predicted.MapCopy(x => 1 / x))
            ).Map(x => x / (expected.Columns * expected.Rows));
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
            bool fromScratch = true;
            Console.WriteLine("gebruik pre-trained model? (geef een pad of laat leeg)");
            answer = Console.ReadLine() ?? "";
            if (answer.Length > 0) {
                fromScratch = false;
                try {
                    serializedXml = XML_IO.Read(answer);
                    using (StringReader stringReader = new StringReader(serializedXml)) {
                        nn = (Neural2)serializer.Deserialize(stringReader);
                    }
                }
                catch (ArgumentException) { Console.WriteLine("ongeldig bestand"); fromScratch = true; }
                catch { Console.WriteLine("bestand kon niet worden gevonden"); fromScratch = true; }
                var labeledTrainImages = LoadPreparedTrainData();
                nn.Train(labeledTrainImages[0], labeledTrainImages[1], loss, lossPrime, 20, 0.05);
            }
            if (fromScratch) {
                nn = new Neural2(784, mo, mp);
                nn.AddReshapeLayer(new int[] { 28 * 28, 1 }, new int[] { 28, 28, 1 });
                nn.AddConvolutionLayer(new int[] { 28, 28, 1 }, 3, 5);
                nn.AddActivation(ActivationType.SIGMOID);
                nn.AddReshapeLayer(new int[] { 26, 26, 5 }, new int[] { 26 * 26 * 5, 1 });
                nn.AddDenseLayer(100);
                nn.AddActivation(ActivationType.SIGMOID);
                nn.AddDenseLayer(2);
                nn.AddActivation(ActivationType.SIGMOID);
                nn.TrainingRate = 0.1;

                Console.WriteLine("trainingdata inladen...");
                List<List<double[]>> labeledTrainImages = LoadPreparedTrainData();
                nn.Train(labeledTrainImages[0], labeledTrainImages[1], loss, lossPrime, 20, 0.05);
            }

            Console.WriteLine("\ntestdata inladen...");
            List<List<double[]>> labeledTestImages = LoadPreparedTestData();
            for(int i = 0; i < labeledTestImages[0].Count; i++) {
                double[][] printable = new double[28][];
                for (int j = 0; j < 28; j++) {
                    double[] inner = new double[28];
                    for(int k = 0; k < 28; k++) {
                        inner[k] = labeledTestImages[0][i][j * 28 + k];
                    }
                    printable[j] = inner;
                }
                PrintGreyscaleArray(printable,0,1);
                var predicted = nn.Predict(labeledTestImages[0][i]);
                Console.WriteLine($"expected: {labeledTestImages[1][i][1]} predicted: {((predicted[0, 0] > predicted[1,0])?0:1)}");
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
                return mo.Multiply(mo.Subtract(predicted, expected),
                    mp.Zero(expected.Rows, expected.Columns).Map(x => 2 / (expected.Rows * expected.Columns))
                    );
            };

            nn.Train(trainInput, trainLabels, loss, lossPrime, 10000, 0, false);
            double[][] gs = new double[28][];

            for (double i = 0; i < 28; i++) {
                double[] gs2 = new double[28];
                for (double j = 0; j < 28; j++) {
                    gs2[(int)j] = nn.Predict(new double[] { i / 27, j / 27 })[0, 0];
                }
                gs[(int)i] = gs2;
            }
            Console.WriteLine();
            PrintGreyscaleArray(gs,-1,1);
        }
        Save(serializer, nn);
    }

    static double[] Sanitize(double[] arr, double min, double max) {
        for (int i = 0; i < arr.Length; i++) {
            arr[i] /= (max - min);
        }
        return arr;
    }

    static void PrintGreyscaleArray(double[][] arr, int min, int max) {//232 - 255
        for (int i = 0; i < arr.Length; i++) {
            for (int j = 0; j < arr[i].Length; j++) {
                Console.Write($"\x1b[48;5;" + (int)Math.Floor(Helper.Scale(arr[i][j], min, max, 232, 256)) + "m  ");
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
        for (int i = 0; i < trainLabelResult.Count; i++) {
            trainLabelResult[i] = new double[] { Math.Abs(trainLabelResult[i][0] - 1), trainLabelResult[i][0]};
        }

        List<double[]> trainFilteredImages = new List<double[]>();
        List<double[]> trainFilteredLabels = new List<double[]>();
        for (int i = 0; i < trainLabelResult.Count; i++) {
            if (trainLabelResult[i][1] == 0 || trainLabelResult[i][1] == 1) {
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

        for (int i = 0; i < testLabelResult.Count; i++) {
            testLabelResult[i] = new double[] { Math.Abs(testLabelResult[i][0] - 1), testLabelResult[i][0] };
        }

        List<double[]> testFilteredImages = new List<double[]>();
        List<double[]> testFilteredLabels = new List<double[]>();
        for (int i = 0; i < testLabelResult.Count; i++) {
            if (testLabelResult[i][1] == 0 || testLabelResult[i][1] == 1) {
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

    static void Save(XmlSerializer serializer, Neural2? nn) {
        string serializedXml;
        using (StringWriter stringWriter = new()) {
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