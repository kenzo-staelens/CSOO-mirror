using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

using Datalaag;
using Globals;

namespace Logica {
    public class NeuralNetwork : MachineLearningModel {

        private IMatrixProvider _matrixProvider;
        private List<Matrix> _matrixList = new List<Matrix>();
        private List<Matrix> _biasList = new List<Matrix>();

        public int Inputs { get; }
        public int Outputs { get; private set; }
        

        public NeuralNetwork(int inputs, IMatrixProvider matrixProvider) : this(inputs, 0.5, matrixProvider) { }

        public NeuralNetwork(int inputs, double trainingRate, IMatrixProvider matrixProvider) {
            if (inputs <= 0) throw new ArgumentException("expecting at least 1 input");
            Inputs = inputs;
            this.TrainingRate = trainingRate;
            this._matrixProvider = matrixProvider;
        }

        public void AddLayer(double[,] layer, double[,] bias) {
            var layerMatrix = new Matrix(layer);
            var biasMatrix = new Matrix(bias);
            if (layerMatrix.Rows != biasMatrix.Rows) throw new MatrixMismatchException($"mismatch in layer and bias row count {layerMatrix.Rows} and {biasMatrix.Rows}");
            if (layerMatrix.Columns != Outputs) throw new MatrixMismatchException($"mismatch in last layer's count and new layers's input count {layerMatrix.Columns} and {_matrixList.Last().Rows}");
            if (biasMatrix.Columns != 1) throw new MatrixMismatchException("bias must have exactly one column");
            this._matrixList.Add(layerMatrix);
            this._biasList.Add(biasMatrix);
        }

        public void AddLayer(int nodes) {
            if (_matrixList.Count == 0) {
                _matrixList.Add(_matrixProvider.Random(nodes, Inputs));
            }
            else {
                _matrixList.Add(_matrixProvider.Random(nodes, _matrixList.Last().Rows));
            }
            _biasList.Add(new Matrix(nodes, 1));
        }

        public override string predict(double[] inputObject) {
            if (_matrixList.Count == 0) throw new MLProcessingException();
            throw new NotImplementedException();
        }

        public override void train(double[] trainingInput, double[] trainingOutput) {
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
