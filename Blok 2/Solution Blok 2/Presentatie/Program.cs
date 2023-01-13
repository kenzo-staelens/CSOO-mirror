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

#pragma warning disable CS8600
#pragma warning disable CS8602
internal class Program {
    static void Main(string[] args) {
        Console.WriteLine(
            @"dit is proof of concept aangezien deze applicatie wordt uitgebouwd als een library;
opties voor uitvoeren van code:
1. handgeschreven cijfers(0 of 1) classificeren (zeer traag)
2. xor ""problem"" (vrij snel)");
        string answer = Console.ReadLine() ?? "";
        int choice = 2;
        do {
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

        if (choice == 1) {// mnist
            Neural2 nn;
            Console.WriteLine("gebruik pre-trained model? (geef een pad of laat leeg)");
            answer = Console.ReadLine() ?? "";
            string serializedXml;
            XmlSerializer serializer = new XmlSerializer(typeof(Neural2), new Type[] {
                typeof(DenseLayer),
                typeof(ConvolutionLayer),
                typeof(ReshapeLayer),
                typeof(ActivationLayer),
                typeof(SigmoidLayer),
                typeof(TanhLayer),
            });

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
                nn.addActivation(ActivationType.SIGMOID);
                nn.AddReshapeLayer(new int[] { 26, 26, 5 }, new int[] { 26 * 26 * 5, 1 });
                nn.AddDenseLayer(100);
                nn.addActivation(ActivationType.SIGMOID);
                nn.AddDenseLayer(1);
                nn.addActivation(ActivationType.SIGMOID);

                nn.TrainingRate = 0.1;

                List<List<double[]>> labeledTrainImages = LoadPreparedTrainData();

                nn.Train(labeledTrainImages[0], labeledTrainImages[1], loss, lossPrime, 20, 0);
            }
            List<List<double[]>> labeledTestImages = LoadPreparedTestData();
            
            for (int i = 0; i < labeledTestImages[0].Count; i++) {
                var output = nn.Predict(labeledTestImages[0][i]);
                Console.WriteLine($"expected: {labeledTestImages[1][i][0]}, result(rounded output): {Math.Round(output[0, 0])}");
            }

            using (StringWriter stringWriter = new StringWriter()) {
                serializer.Serialize(stringWriter, nn);
                serializedXml = stringWriter.ToString();
            }
            Console.WriteLine("path to save to (geef een pad op of laat leeg): ");
            string? path = Console.ReadLine();
            XML_IO.Write(path ?? "", serializedXml);
        }
        else if (choice == 2) {
            Console.Write("extra notitie: soms blijft deze hangen in een \"verkeerde vallei\" en soms is dan soms geen correcte output");
            NeuralNetwork nn = new NeuralNetwork(2, mo, mp);
            nn.AddLayer(2);
            nn.AddLayer(1);

            nn.TrainingRate = 0.1;

            List<double[]> trainInput = new List<double[]>() {
                new double[]{0,0},
                new double[]{0,1},
                new double[]{1,0},
                new double[]{1,1}
            };

            List<double[]> trainLabels = new List<double[]>() {
                new double[]{0},
                new double[]{1},
                new double[]{1},
                new double[]{0}
            };

            for (int i = 0; i < 100000; i++) nn.Train(trainInput, trainLabels);
            double[] temp = new double[28 * 28];

            for (double i = 0; i < 28; i++) {
                for (double j = 0; j < 28; j++) {
                    temp[(int)i * 28 + (int)j] = (Math.Floor(nn.Predict(new double[] { i / 27, j / 27 })[0, 0] * 1000));
                }
            }
            Console.WriteLine(
                FormatMnistArray(temp)  //duidelijk dat dit geen mnist array is, maar deze functie bestond nog sinds testen
            );

        }
    }

    static double[] Sanitize(double[] arr, double min, double max) {
        for (int i = 0; i < arr.Length; i++) {
            arr[i] /= (max - min);
        }
        return arr;
    }

    static string FormatMnistArray(double[] arr) {
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

        List<List<double[]>> result = new List<List<double[]>>();
        result.Add(trainFilteredImages);
        result.Add(trainFilteredLabels);
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

        List<List<double[]>> result = new List<List<double[]>>();
        result.Add(testFilteredImages);
        result.Add(testFilteredLabels);
        return result;
    }
}