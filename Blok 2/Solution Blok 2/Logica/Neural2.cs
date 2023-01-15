using Datalaag;
using ExtensionMethods;
using Globals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;

#pragma warning disable CS8618 
#pragma warning disable CS8600 
namespace Logica {
    public class Neural2 : MachineLearningModel, ISerializable {
        public List<Layer> LayerList;

        private IMatrixProvider _matrixProvider;
        private IMatrixOperator _matrixOperator;

        public int Inputs;
        public int Outputs {
            get {
                try {
                    return LayerList[LayerList.Count - 1].Outputs;
                }
                catch (ArgumentOutOfRangeException) {
                    return Inputs;
                }
            }
        }

        public Neural2() {
            this._matrixOperator = new MatrixOperator();
            this._matrixProvider = new MatrixProvider();
        }
        public Neural2(SerializationInfo info, StreamingContext context) {
            Inputs = info.GetInt32("Inputs");
            LayerList = (List<Layer>)info.GetValue("LayerList", typeof(List<Layer>)) ?? new List<Layer>();
        }

        public Neural2(int inputs, IMatrixOperator matrixOperator, IMatrixProvider matrixProvider) : this(inputs, 0.5, matrixOperator, matrixProvider) { }

        public Neural2(int inputs, double trainingRate, IMatrixOperator matrixOperator, IMatrixProvider matrixProvider) {
            if (inputs <= 0) throw new ArgumentException("expecting at least 1 input");
            Inputs = inputs;
            this.TrainingRate = trainingRate;
            this._matrixProvider = matrixProvider;

            this.LayerList = new List<Layer>();

            this._matrixOperator = matrixOperator;
        }

        public void AddDenseLayer(int nodes) {
            if (nodes <= 0) throw new ArgumentException("invalid number of nodes, expecting 1 or more");
            Matrix weights;
            if (LayerList.Count == 0) {
                weights = _matrixProvider.Random(nodes, Inputs);
            }
            else {
                weights = _matrixProvider.Random(nodes, Outputs);
            }
            Matrix biases = _matrixProvider.Random(nodes, 1);
            LayerList.Add(new DenseLayer(weights, biases, _matrixOperator));
        }

        public void AddConvolutionLayer(int[] inputShape, int kernelsize, int depth) {
            LayerList.Add(new ConvolutionLayer(inputShape, kernelsize, depth, _matrixOperator, _matrixProvider));
        }

        public void AddReshapeLayer(int[] inputShape, int[] outputShape) {
            this.LayerList.Add(new ReshapeLayer(inputShape, outputShape));
        }

        public void AddActivation(ActivationType type) {
            bool layered;
            try {
                layered = LayerList.Last().UsesList;
            }
            catch { layered = false; }
            switch (type) {
                case ActivationType.SIGMOID:
                    LayerList.Add(new SigmoidLayer(Outputs, _matrixOperator));
                    break;
                case ActivationType.TANH:
                    LayerList.Add(new TanhLayer(Outputs, _matrixOperator));
                    break;
                case ActivationType.CUSTOM:
                    throw new NotImplementedException();
            }
            ((ActivationLayer)LayerList.Last()).SetLayered(layered);
        }

        public override Matrix Predict(double[] inputObject) {
            if (LayerList.Count == 0) throw new MLProcessingException($"cannot process inputs with {LayerList.Count} layers in network");
            Matrix processMatrix = _matrixProvider.FromArray(inputObject);
            Object processObj = _matrixOperator.Transpose(processMatrix);

            for (int i = 0; i < LayerList.Count; i++) {
                if (LayerList[i].UsesList) {
                    List<Matrix> processList;
                    if (processObj.GetType().Name == "Matrix") {
                        processList = new List<Matrix> { (Matrix)processObj };
                    }
                    else { processList = (List<Matrix>)processObj; }

                    processObj = LayerList[i].Forward(processList);
                    bool flag = ((List<Matrix>)processObj)[0][0, 0] == 1; // de-encapsulatie van reshape layer output als nodig
                    ((List<Matrix>)processObj).RemoveAt(0);
                    if (flag) processObj = ((List<Matrix>)processObj)[0];
                }
                else {
                    processObj = LayerList[i].Forward((Matrix)processObj);
                }
            }
            if (processObj.GetType().Name == "Matrix") {
                return (Matrix)processObj;
            }
            else {
                return ((List<Matrix>)processObj)[0];
            }
        }

        public override void Train(List<double[]> trainingInput, List<double[]> trainingOutput) {
            throw new NotImplementedException();
        }

        public void Train(List<double[]> trainingInput, List<double[]> trainingOutput, Func<Matrix, Matrix, double> loss, Func<Matrix, Matrix, Matrix> lossPrime, int epochs) {
            Train(trainingInput, trainingOutput, loss, lossPrime, epochs, 0, true);
        }
        public void Train(List<double[]> trainingInput, List<double[]> trainingOutput, Func<Matrix, Matrix, double> loss, Func<Matrix, Matrix, Matrix> lossPrime, int epochs, double maxError) {
            Train(trainingInput, trainingOutput, loss, lossPrime, epochs, maxError, true);
        }
        public void Train(List<double[]> trainingInput, List<double[]> trainingOutput, Func<Matrix, Matrix, double> loss, Func<Matrix, Matrix, Matrix> lossPrime, int epochs, bool verbose) {
            Train(trainingInput, trainingOutput, loss, lossPrime, epochs, 0, verbose);
        }
        public void Train(List<double[]> trainingInput, List<double[]> trainingOutput, Func<Matrix, Matrix, double> loss, Func<Matrix, Matrix, Matrix> lossPrime, int epochs, double maxError, bool verbose) {
            if (trainingInput.Count != trainingOutput.Count) throw new MLProcessingException($"length of input list ({trainingInput.Count}) and target list ({trainingOutput.Count}) arrays must be equal");
            if (trainingInput[0].Length != Inputs) throw new MLProcessingException($"number of inputs ({trainingInput[0].Length}) input nodes({Inputs}) must be equal");
            if (trainingOutput[0].Length != Outputs) throw new MLProcessingException($"number of outputs ({trainingOutput[0].Length}) output nodes({Outputs}) must be equal");
            for (int epoch = 1; epoch <= epochs; epoch++) {

                double error = 0;
                for (int i = 0; i < trainingInput.Count; i++) {
                    var expected = _matrixProvider.FromArray(trainingOutput[i]);
                    expected = _matrixOperator.Transpose(expected);
                    var output = Predict(trainingInput[i]);
                    var lo = loss(expected, output);
                    error += lo;
                    Object gradientObj = lossPrime(expected, output);

                    for (int l = LayerList.Count - 1; l >= 0; l--) {
                        if (LayerList[l].UsesList) {
                            List<Matrix> gradientList;
                            if (gradientObj.GetType().Name == "Matrix") gradientList = new List<Matrix> { (Matrix)gradientObj };
                            else gradientList = (List<Matrix>)gradientObj;

                            gradientObj = LayerList[l].Backward(gradientList, TrainingRate);
                            bool flag = ((List<Matrix>)gradientObj)[0][0, 0] == 1;
                            ((List<Matrix>)gradientObj).RemoveAt(0);
                            if (flag) gradientObj = ((List<Matrix>)gradientObj)[0];
                        }
                        else gradientObj = LayerList[l].Backward((Matrix)gradientObj, TrainingRate);
                    }
                    
                    if (verbose) {
                        Console.WriteLine($"epoch: {epoch}({i}/{trainingInput.Count}) loss: {lo}");
                    }
                }
                Console.WriteLine($"epoch: {epoch}, error: {error / trainingInput.Count}");
                if (error / trainingInput.Count <= maxError) return;
            }
        }

        public void TrainRandom(List<double[]> trainingInput, List<double[]> trainingOutput, Func<Matrix, Matrix, Matrix> lossPrime, int iterations) {
            for (int i = 0; i < iterations; i++) {
                int random = new Random().Next(trainingInput.Count);
                Matrix expected = _matrixProvider.FromArray(trainingOutput[random]);
                expected = _matrixOperator.Transpose(expected);
                Matrix output = Predict(trainingInput[random]);

                Object gradientObj = lossPrime(expected, output);

                for (int l = LayerList.Count - 1; l >= 0; l--) {
                    if (LayerList[l].UsesList) {
                        List<Matrix> gradientList;
                        if (gradientObj.GetType().Name == "Matrix") gradientList = new List<Matrix> { (Matrix)gradientObj };
                        else gradientList = (List<Matrix>)gradientObj;

                        gradientObj = LayerList[l].Backward(gradientList, TrainingRate);
                        bool flag = ((List<Matrix>)gradientObj)[0][0, 0] == 1;
                        ((List<Matrix>)gradientObj).RemoveAt(0);
                        if (flag) gradientObj = ((List<Matrix>)gradientObj)[0];
                    }
                    else gradientObj = LayerList[l].Backward((Matrix)gradientObj, TrainingRate);
                }
            }
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context) {
            info.AddValue("Inputs", Inputs);
            info.AddValue("LayerList", LayerList);
        }
    }
}
