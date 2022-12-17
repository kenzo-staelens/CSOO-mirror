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
        
        private List<Matrix> _matrixList;
        private List<Matrix> _biasList;
        private List<Matrix> _memoryList;
        private List<ActivationFunction> _activationFunctions;

        private IMatrixOperator _matrixOperator;
        private ActivationFunction _defaultActivation;

        public int Inputs { get; }
        public int Outputs { get; private set; }
        

        public NeuralNetwork(int inputs, IMatrixOperator matrixOperator, IMatrixProvider matrixProvider) : this(inputs, 0.9, matrixOperator, matrixProvider) { }

        public NeuralNetwork(int inputs, double trainingRate, IMatrixOperator matrixOperator, IMatrixProvider matrixProvider) {
            if (inputs <= 0) throw new ArgumentException("expecting at least 1 input");
            Inputs = inputs;
            this.TrainingRate = trainingRate;
            this._matrixProvider = matrixProvider;

            this._matrixList = new List<Matrix>();
            this._biasList = new List<Matrix>();
            this._memoryList = new List<Matrix>();
            this._activationFunctions = new List<ActivationFunction>();

            this._matrixOperator = matrixOperator;
            this._defaultActivation = new ActivationFunction(
                    x => { return 1 / (1 + Math.Exp(-x)); },
                    y => { return y * (1 - y);
                });
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
            if (layerMatrix.Columns != Outputs) throw new MatrixMismatchException($"mismatch in last layer's count and new layers's input count {layerMatrix.Columns} and {_matrixList.Last().Rows}");
            if (biasMatrix.Columns != 1) throw new MatrixMismatchException("bias must have exactly one column");
            _matrixList.Add(layerMatrix);
            _biasList.Add(biasMatrix);
            _activationFunctions.Add(activation);
        }

        public void AddLayer(int nodes, ActivationFunction activation) {
            if (_matrixList.Count == 0) {
                _matrixList.Add(_matrixProvider.Random(nodes, Inputs));
            }
            else {
                _matrixList.Add(_matrixProvider.Random(nodes, _matrixList.Last().Rows));
            }
            _biasList.Add(_matrixProvider.Random(nodes, 1));
            _activationFunctions.Add(activation);
        }

        private Matrix Predict(double[] inputObject, bool keepMemory) {
            if (_matrixList.Count == 0) throw new MLProcessingException();
            var inputMatrix = _matrixProvider.FromArray(inputObject);
            var processMatrix = _matrixOperator.Transpose(inputMatrix);
            if (keepMemory) _memoryList.Add(processMatrix);
            for (int i = 0; i < _matrixList.Count; i++) {
                processMatrix = _matrixOperator.Dot(_matrixList[i], processMatrix);
                processMatrix = _matrixOperator.Add(processMatrix, _biasList[i]);
                processMatrix.Map((double x) => {// casting because of ambiguous Func<Matrix, Matrix> and Func<double,double>
                    return x.Map(_activationFunctions[i].Forward);
                });
                if (keepMemory) _memoryList.Add(processMatrix);
            }
            return processMatrix;
        }

        public override Matrix Predict(double[] inputObject) {
            return Predict(inputObject, false);
        }

        public override void Train(List<double[]> trainingInput, List<double[]> trainingOutput) {
            if (trainingInput.Count != trainingOutput.Count) throw new MLProcessingException($"length of input({trainingInput.Count}) and target({trainingOutput.Count}) arrays must be equal");
            int random = new Random().Next(trainingInput.Count);
            Matrix currentOutput = _matrixProvider.FromArray(trainingOutput[random]);
            currentOutput = _matrixOperator.Transpose(currentOutput);
            Matrix prediction = Predict(trainingInput[random], true);
            Matrix error = _matrixOperator.Add(currentOutput, _matrixProvider.Copy(prediction).Map((double x) => { return -x; }));
            
            // backpropagate
            List<Matrix> errorList = new List<Matrix>(_matrixList.Count);
            for (int i = 0; i < _matrixList.Count; i++) errorList.Add(_matrixProvider.Zero(1, 1));
            errorList[errorList.Count-1] = error;
            for(int i=errorList.Count-2;i>=0;i--) {
                errorList[i] = _matrixOperator.Dot(_matrixOperator.Transpose(_matrixList[i + 1]), errorList[i + 1]);
            }

            // update values
            for(int i=0;i<_matrixList.Count;i++) {
                if (i == 1) Console.WriteLine(_memoryList[i + 1].Serialize());
                var gradient = _memoryList[i + 1].Map(_activationFunctions[i].Backward);
                if (i==1)Console.WriteLine(gradient.Serialize());
                if(i==1)Console.WriteLine("");
                gradient = _matrixOperator.Multiply(gradient, errorList[i]);
                
                gradient = gradient.Map((double x) => { return x * TrainingRate; });
                // ik heb geen enkel idee waarom dingen werken met -TrainingRate dat +TrainingRate zou moeten zijn

                _biasList[i] = _matrixOperator.Add(_biasList[i], gradient);

                var memTranspose = _matrixOperator.Transpose(_memoryList[i]);
                var delta = _matrixOperator.Dot(gradient, memTranspose);
                _matrixList[i] = _matrixOperator.Add(_matrixList[i], delta);
            }
            _memoryList = new List<Matrix>(); // memory clean
            //Console.WriteLine(errorList[errorList.Count - 1].Serialize());
        }

        public void Initialize() {
            throw new NotImplementedException();
        }

        public void Load() {
            throw new NotImplementedException();
        }
    }
}
