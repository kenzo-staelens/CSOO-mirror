using Globals;
using ExtensionMethods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logica {
    public class ActivationLayer : Layer {
        private int _outputs;
        public override int Outputs => _outputs;
        public ActivationFunction activation;
        private Object _input;
        private IMatrixOperator _matrixOperator;

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
            _input = input;
            return input.MapCopy((double x) => {// casting because of ambiguous Func<Matrix, Matrix> and Func<double,double>
                return x.Map(activation.Forward);
            });
        }

        public override Matrix Backward(Matrix gradient, double rate) {
            var prime = ((Matrix)_input).Map((double x) => {
                return x.Map(activation.Backward);
            });

            return _matrixOperator.Multiply(gradient, prime);
        }

        public override List<Matrix> Forward(List<Matrix> input) {
            _input = input;
            for(int i=0;i<input.Count;i++) {
                input[i] = input[i].MapCopy((double x) => {
                    return x.Map(activation.Forward);
                });
            }
            input.Insert(0, new Matrix(1, 1)); // never de-encapsulate
            return input;
        }

        public override List<Matrix> Backward(List<Matrix> gradient, double rate) {
            var primes = new List<Matrix>();
            for (int i = 0; i < gradient.Count; i++) {
                var prime = ((List<Matrix>)_input)[i].Map((double x) => {
                    return x.Map(activation.Backward);
                });
                primes.Add(_matrixOperator.Multiply(gradient[i],prime));
            }
            primes.Insert(0, new Matrix(1, 1)); // never de-encapsulate
            return primes;
        }
    }
}
