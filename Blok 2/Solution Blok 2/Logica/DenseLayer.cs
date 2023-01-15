using Globals;
using ExtensionMethods;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

#pragma warning disable CS8618 
#pragma warning disable CS8605
namespace Logica {
    public class DenseLayer : Layer, ISerializable {
        public override int Outputs {
            get {
                return Weights.Rows;
            }
            set { /*void*/}
        }

        public Matrix Weights;
        public Matrix Biases;
        private Matrix _output;
        private Matrix _input;

        private IMatrixOperator _matrixOperator;

        public DenseLayer() {
            _matrixOperator = new MatrixOperator();
        }
        public DenseLayer(SerializationInfo info, StreamingContext context) {
            UsesList = info.GetBoolean("UsesList");
            Weights = (Matrix)info.GetValue("Weights", typeof(Matrix));
            Biases = (Matrix)info.GetValue("Biases", typeof(Matrix));
        }

        public DenseLayer(Matrix weights, Matrix biases) : this(weights, biases, new MatrixOperator()) { }
        public DenseLayer(Matrix weights, Matrix biases, IMatrixOperator matrixOperator) {
            this.Weights = weights;
            this.Biases = biases;
            this._matrixOperator = matrixOperator;
            this.UsesList = false;
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
            this.Weights = _matrixOperator.Subtract(this.Weights, weightGradient.MapCopy((double x) => { return x * rate; }));
            this.Biases = _matrixOperator.Subtract(this.Biases, outputGradient.MapCopy((double x) => { return x * rate; }));
            return inputGradient;
        }

        public override List<Matrix> Forward(List<Matrix> input) {
            throw new NotImplementedException();
        }

        public override List<Matrix> Backward(List<Matrix> gradient, double rate) {
            throw new NotImplementedException();
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context) {
            info.AddValue("UsesList", UsesList);
            info.AddValue("Weights", Weights);
            info.AddValue("Biases", Biases);
        }
    }
}
