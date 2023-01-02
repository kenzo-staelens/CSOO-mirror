using Globals;
using ExtensionMethods;
using System.Runtime.InteropServices;

namespace Logica {
    public class DenseLayer : Layer {
        public override int Outputs {
            get {
                return Weights.Rows;
            }
        }

        public Matrix Weights;
        public Matrix Biases;
        private Matrix _output;
        private Matrix _input;

        private IMatrixOperator _matrixOperator;

        public DenseLayer(Matrix weights, Matrix biases, IMatrixOperator matrixOperator) {
            this.Weights=weights;
            this.Biases=biases;
            this._matrixOperator=matrixOperator;
        }

        public override Matrix Forward(Matrix input) {
            try {
                _input = input;
                _output = _matrixOperator.Dot(this.Weights, input);
                _output = _matrixOperator.Add(_output, this.Biases);
                return _output;
            }
            catch (MatrixMismatchException e) {
                throw new MLProcessingException($"error while operating on matrixes\n{e.Message}\n{e.StackTrace}");
            }
            catch (Exception e) {
                throw new MLProcessingException($"general error occured\n{e.Message}\n{e.StackTrace} during processing of input");
            }
        }

        public override Matrix Backward(Matrix outputGradient, double rate) {
            var weightGradient = _matrixOperator.Dot(outputGradient, _matrixOperator.Transpose(_input));
            var inputGradient = _matrixOperator.Dot(_matrixOperator.Transpose(Weights), outputGradient);
            this.Weights = _matrixOperator.Subtract(this.Weights, weightGradient.Map((double x) => { return x * rate; }));
            this.Biases = _matrixOperator.Subtract(this.Biases, outputGradient.Map((double x) => { return x * rate; }));
            return inputGradient;
        }
    }
}
