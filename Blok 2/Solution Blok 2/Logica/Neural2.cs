using Datalaag;
using Globals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Logica {
    public class Neural2 : MachineLearningModel {
        public List<Layer> LayerList;

        private IMatrixProvider _matrixProvider;
        private IMatrixOperator _matrixOperator;

        public int Inputs { get; }
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
            if (nodes<=0) throw new ArgumentException("invalid number of nodes, expecting 1 or more");
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

        private double tanh(double x) { return (Math.Exp(2 * x) - 1) / (Math.Exp(2 * x) + 1); }
        private double sigmoid(double x) { return 1 / (1 + Math.Exp(-x)); }

        public void addActivation(ActivationType type) {
            switch (type) {
                case ActivationType.SIGMOID:
                    LayerList.Add(new ActivationLayer(Outputs,
                        new ActivationFunction(
                            sigmoid,
                            y => { return sigmoid(y) * (1 - sigmoid(y)); }),
                        _matrixOperator));
                    break;
                case ActivationType.TANH:
                    LayerList.Add(new ActivationLayer(Outputs,
                    new ActivationFunction(
                            tanh,
                            y => { return 1 - Math.Pow(tanh(y),2); }),
                    _matrixOperator));
                    break;
                case ActivationType.CUSTOM:
                    throw new NotImplementedException();
                    break;
            }

        }

        public override Matrix Predict(double[] inputObject) {
            if (LayerList.Count == 0) throw new MLProcessingException($"cannot process inputs with {LayerList.Count} layers in network");
            var processMatrix = _matrixProvider.FromArray(inputObject);
            processMatrix = _matrixOperator.Transpose(processMatrix);

            for (int i = 0; i < LayerList.Count; i++) {
                processMatrix = LayerList[i].Forward(processMatrix);
            }
            return processMatrix;
        }

        public override void Train(List<double[]> trainingInput, List<double[]> trainingOutput) {
            throw new NotImplementedException();
        }

        public void Train(List<double[]> trainingInput, List<double[]> trainingOutput, Func<Matrix,Matrix,double> loss, Func<Matrix, Matrix,Matrix> lossPrime) {
            if (trainingInput.Count != trainingOutput.Count) throw new MLProcessingException($"length of input list ({trainingInput.Count}) and target list ({trainingOutput.Count}) arrays must be equal");
            if (trainingInput[0].Length != Inputs) throw new MLProcessingException($"number of inputs ({trainingInput[0].Length}) input nodes({Inputs}) must be equal");
            if (trainingOutput[0].Length != Outputs) throw new MLProcessingException($"number of outputs ({trainingOutput[0].Length}) output nodes({Outputs}) must be equal");

            double error = 0;
            for (int i = 0; i < trainingInput.Count; i++) {
                var expected = _matrixProvider.FromArray(trainingOutput[i]);
                expected = _matrixOperator.Transpose(expected);
                var output = Predict(trainingInput[i]);

                error += loss(expected, output);

                var gradient = lossPrime(expected,output);
                
                for (int l = LayerList.Count - 1; l >= 0; l--) {
                    gradient = LayerList[l].Backward(gradient, TrainingRate);
                }
            }
        }
    }
}
