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
        private Matrix _input;
        private IMatrixOperator _matrixOperator;

        public ActivationLayer(int nodes, ActivationFunction func, IMatrixOperator matrixOperator) {
            _outputs = nodes;
            activation = func;
            _matrixOperator = matrixOperator;
        }

        public override Matrix Forward(Matrix input) {
            _input = input;
            return input.MapCopy((double x) => {// casting because of ambiguous Func<Matrix, Matrix> and Func<double,double>
                return x.Map(activation.Forward);
            });
        }

        public override Matrix Backward(Matrix gradient, double rate) {
            
            var prime = _input.Map((double x) => {
                return x.Map(activation.Backward);
            });

            return _matrixOperator.Multiply(gradient, prime);
        }
    }
}
