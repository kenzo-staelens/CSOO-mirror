using Globals;
using ExtensionMethods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

#pragma warning disable CS8618 
namespace Logica {
    public class ActivationLayer : Layer, ISerializable {
        public override int Outputs { get { return _outputs; } set { _outputs = value; } }
        protected ActivationFunction activation;
        private Object _input;
        private IMatrixOperator _matrixOperator;

        public ActivationLayer() {
            _matrixOperator = new MatrixOperator();
        }

        public ActivationLayer(int nodes, ActivationFunction func, IMatrixOperator matrixOperator) {
            this._outputs = nodes;
            this.activation = func;
            this._matrixOperator = matrixOperator;
            this.UsesList = false; // for now
        }

        public void SetLayered(bool uselist) {
            this.UsesList = uselist;
        }

        public override Matrix Forward(Matrix input) {
            _input = new Matrix(input);
            return input.MapCopy((double x) => {// casting because of ambiguous Func<Matrix, Matrix> and Func<double,double>
                return x.Map(activation.Forward);
            });
        }

        public override Matrix Backward(Matrix gradient, double rate) {
            var prime = ((Matrix)_input).MapCopy((double x) => {
                return x.Map(activation.Backward);
            });
            
            return _matrixOperator.Multiply(gradient, prime);
        }

        public override List<Matrix> Forward(List<Matrix> input) {
            _input = new List<Matrix>();
            foreach (Matrix m in input) ((List<Matrix>)_input).Add(new Matrix(m));
            for(int i=0;i<input.Count;i++) {
                input[i].Map((double x) => {
                    return x.Map(activation.Forward);
                });
            }
            input.Insert(0, new Matrix(1, 1)); // never de-encapsulate
            return input;
        }

        public override List<Matrix> Backward(List<Matrix> gradient, double rate) {
            var primes = new List<Matrix>();
            for (int i = 0; i < gradient.Count; i++) {
                var prime = ((List<Matrix>)_input)[i].MapCopy((double x) => {
                    return x.Map(activation.Backward);
                });
                
                primes.Add(_matrixOperator.Multiply(gradient[i],prime));
            }
            primes.Insert(0, new Matrix(1, 1)); // never de-encapsulate
            return primes;
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context) {
            info.AddValue("Outputs", Outputs);
        }
    }
}
