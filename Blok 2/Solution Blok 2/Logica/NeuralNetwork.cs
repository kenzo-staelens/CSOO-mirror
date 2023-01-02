using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

using Datalaag;
using Globals;
using ExtensionMethods;
using System.Linq.Expressions;

namespace Logica {
    public class NeuralNetwork : MachineLearningModel {

        private IMatrixProvider _matrixProvider;

        private List<Matrix> _weightList;
        private List<Matrix> _biasList;
        private List<Matrix> _memoryList;
        private List<ActivationFunction> _activationFunctions;

        private IMatrixOperator _matrixOperator;
        private ActivationFunction _defaultActivation;

        public int Inputs { get; }
        public int Outputs {
            get {
                try {
                    return _weightList[_weightList.Count - 1].Rows;
                } catch(ArgumentOutOfRangeException) {
                    return Inputs;
                }
            }
        }


        public NeuralNetwork(int inputs, IMatrixOperator matrixOperator, IMatrixProvider matrixProvider) : this(inputs, 0.5, matrixOperator, matrixProvider) { }

        public NeuralNetwork(int inputs, double trainingRate, IMatrixOperator matrixOperator, IMatrixProvider matrixProvider) {
            if (inputs <= 0) throw new ArgumentException("expecting at least 1 input");
            Inputs = inputs;
            this.TrainingRate = trainingRate;
            this._matrixProvider = matrixProvider;

            this._weightList = new List<Matrix>();
            this._biasList = new List<Matrix>();
            this._memoryList = new List<Matrix>();
            this._activationFunctions = new List<ActivationFunction>();

            this._matrixOperator = matrixOperator;
            this._defaultActivation = new ActivationFunction(
                    x => { return 1 / (1 + Math.Exp(-x)); },
                    y => { return y * (1 - y); }
                 );
        }

        public void AddLayer(double[,] layer, double[,] bias) {
            AddLayer(layer, bias, _defaultActivation);
        }

        public void AddLayer(int nodes) {
            AddLayer(nodes, _defaultActivation);
        }

        public void AddLayer(double[,] layer, double[,] bias, ActivationFunction activation) {
            var layerMatrix = _matrixProvider.FromArray(layer);
            var biasMatrix = _matrixProvider.FromArray(bias);
            if (layerMatrix.Rows != biasMatrix.Rows) throw new MatrixMismatchException($"mismatch in layer and bias row count {layerMatrix.Rows} and {biasMatrix.Rows}");
            if (layerMatrix.Columns != Outputs) throw new MatrixMismatchException($"mismatch in last layer's count and new layers's input count {layerMatrix.Columns} and {_weightList.Last().Rows}");
            if (biasMatrix.Columns != 1) throw new MatrixMismatchException("bias must have exactly one column");
            _weightList.Add(layerMatrix);
            _biasList.Add(biasMatrix);
            _activationFunctions.Add(activation);
        }

        public void AddLayer(int nodes, ActivationFunction activation) {
            if (_weightList.Count == 0) {
                _weightList.Add(_matrixProvider.Random(nodes, Inputs));
            }
            else {
                _weightList.Add(_matrixProvider.Random(nodes, _weightList.Last().Rows));
            }
            _biasList.Add(_matrixProvider.Random(nodes, 1));
            _activationFunctions.Add(activation);
        }

        public override Matrix Predict(double[] inputObject) {
            return Predict(inputObject, false);
        }

        private Matrix Predict(double[] inputObject, bool keepMemory) {
            if (_weightList.Count == 0) throw new MLProcessingException($"cannot process inputs with {_weightList.Count} layers in network");
            var inputMatrix = _matrixProvider.FromArray(inputObject);
            var processMatrix = _matrixOperator.Transpose(inputMatrix);
            if (keepMemory) _memoryList.Add(processMatrix);
            try {
                for (int i = 0; i < _weightList.Count; i++) {
                    processMatrix = _matrixOperator.Dot(_weightList[i], processMatrix);
                    processMatrix = _matrixOperator.Add(processMatrix, _biasList[i]);
                    processMatrix.Map((double x) => {// casting because of ambiguous Func<Matrix, Matrix> and Func<double,double>
                        return x.Map(_activationFunctions[i].Forward);
                    });
                    if (keepMemory) _memoryList.Add(processMatrix);
                }
                return processMatrix;
            }
            catch(MatrixMismatchException e) {
                throw new MLProcessingException($"error while operating on matrixes\n{e.Message}\n{e.StackTrace}");
            }
            catch(Exception e) {
                throw new MLProcessingException($"general error occured\n{e.Message}\n{e.StackTrace} during processing of input");
            }
        }

        public override void Train(List<double[]> trainingInput, List<double[]> trainingOutput) {
            if (trainingInput.Count != trainingOutput.Count) throw new MLProcessingException($"length of input list ({trainingInput.Count}) and target list ({trainingOutput.Count}) arrays must be equal");
            if (trainingInput[0].Length != Inputs) throw new MLProcessingException($"number of inputs ({trainingInput[0].Length}) input nodes({Inputs}) must be equal");
            if (trainingOutput[0].Length != Outputs) throw new MLProcessingException($"number of outputs ({trainingOutput[0].Length}) output nodes({Outputs}) must be equal");
            
            int random = new Random().Next(trainingInput.Count);
            Matrix currentOutput = _matrixProvider.FromArray(trainingOutput[random]);
            currentOutput = _matrixOperator.Transpose(currentOutput);
            Matrix prediction = Predict(trainingInput[random], true);
            
            Matrix error = _matrixOperator.Subtract(currentOutput, prediction);
            
            // backpropagate
            Matrix[] errorList = new Matrix[_weightList.Count];
            errorList[errorList.Length - 1] = error;
            for (int i = errorList.Length - 2; i >= 0; i--)
                errorList[i] = _matrixOperator.Dot(_matrixOperator.Transpose(_weightList[i + 1]), errorList[i + 1]);

            // update values
            for (int i = 0; i < _weightList.Count; i++) {
                var gradient = _memoryList[i + 1].MapCopy(_activationFunctions[i].Backward);
                gradient = _matrixOperator.Multiply(gradient, errorList[i]);
                gradient = gradient.Map((double x) => { return x * TrainingRate; });
                
                _biasList[i] = _matrixOperator.Add(_biasList[i], gradient);
                
                var memTranspose = _matrixOperator.Transpose(_memoryList[i]);
                var delta = _matrixOperator.Dot(gradient, memTranspose);
                _weightList[i] = _matrixOperator.Add(_weightList[i], delta);
            }
            _memoryList = new List<Matrix>(); // memory clean
        }

        public void Load() {
            throw new NotImplementedException();
        }
    }
}
