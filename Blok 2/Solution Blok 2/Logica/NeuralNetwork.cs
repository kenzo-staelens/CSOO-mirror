﻿using System;
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

        private readonly IMatrixProvider _matrixProvider;
        
        private readonly List<Matrix> _matrixList;
        private readonly List<Matrix> _biasList;
        private readonly List<Matrix> _memoryList;
        private readonly List<Func<double, double>> _mappingFunctions;

        private readonly IMatrixOperator _matrixOperator;
        private readonly Func<double, double> _defaultMappingFunc;

        public int Inputs { get; }
        public int Outputs { get; private set; }
        

        public NeuralNetwork(int inputs, IMatrixOperator matrixOperator, IMatrixProvider matrixProvider) : this(inputs, 0.5, matrixOperator, matrixProvider) { }

        public NeuralNetwork(int inputs, double trainingRate, IMatrixOperator matrixOperator, IMatrixProvider matrixProvider) {
            if (inputs <= 0) throw new ArgumentException("expecting at least 1 input");
            Inputs = inputs;
            this.TrainingRate = trainingRate;
            this._matrixProvider = matrixProvider;

            this._matrixList = new List<Matrix>();
            this._biasList = new List<Matrix>();
            this._memoryList = new List<Matrix>();
            this._mappingFunctions = new List<Func<double, double>>();

            this._matrixOperator = matrixOperator;
            this._defaultMappingFunc = x => { return 1 / (1 + Math.Exp(-x)); };
        }

        public void AddLayer(double[,] layer, double[,] bias) {
            AddLayer(layer, bias, _defaultMappingFunc);
        }
        
        public void AddLayer(int nodes) {
            AddLayer(nodes, _defaultMappingFunc);
        }

        public void AddLayer(double[,] layer, double[,] bias, Func<double,double> mappingFunc) {
            var layerMatrix = _matrixProvider.FromArray(layer);
            var biasMatrix = _matrixProvider.FromArray(bias);
            if (layerMatrix.Rows != biasMatrix.Rows) throw new MatrixMismatchException($"mismatch in layer and bias row count {layerMatrix.Rows} and {biasMatrix.Rows}");
            if (layerMatrix.Columns != Outputs) throw new MatrixMismatchException($"mismatch in last layer's count and new layers's input count {layerMatrix.Columns} and {_matrixList.Last().Rows}");
            if (biasMatrix.Columns != 1) throw new MatrixMismatchException("bias must have exactly one column");
            _matrixList.Add(layerMatrix);
            _biasList.Add(biasMatrix);
            _mappingFunctions.Add(mappingFunc);
        }

        public void AddLayer(int nodes, Func<double,double> mappingFunc) {
            if (_matrixList.Count == 0) {
                _matrixList.Add(_matrixProvider.Random(nodes, Inputs));
            }
            else {
                _matrixList.Add(_matrixProvider.Random(nodes, _matrixList.Last().Rows));
            }
            _biasList.Add(_matrixProvider.Random(nodes, 1));
            _mappingFunctions.Add(mappingFunc);
        }

        private Matrix Predict(double[] inputObject, bool keepMemory) {
            if (_matrixList.Count == 0) throw new MLProcessingException();
            var inputMatrix = _matrixProvider.FromArray(inputObject);
            var processMatrix = _matrixOperator.Transpose(inputMatrix);
            for (int i = 0; i < _matrixList.Count; i++) {
                processMatrix = _matrixOperator.Dot(_matrixList[i], processMatrix);
                processMatrix = _matrixOperator.Add(processMatrix, _biasList[i]);
                processMatrix.Map((double x) => {// casting because of ambiguous Func<Matrix, Matrix> and Func<double,double>
                    return x.Map(_mappingFunctions[i]);
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
            Matrix currentOutput = _matrixProvider.FromArray(trainingInput[random]);
            currentOutput = _matrixOperator.Transpose(currentOutput);
            Matrix prediction = Predict(trainingInput[random], true);
            Matrix error = _matrixOperator.Add(currentOutput, prediction.Map((double x) => { return -x; }));

            // backpropagate -> create an error list back to front then list.Reverse();
            // then update weights (gradient descent)
            
            throw new NotImplementedException();
        }

        public void Initialize() {
            throw new NotImplementedException();
        }

        public void Load() {
            throw new NotImplementedException();
        }
    }
}
