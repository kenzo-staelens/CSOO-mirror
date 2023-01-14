using Globals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Logica {
    public class SigmoidLayer : ActivationLayer {
        private double sigmoid(double x) { return 1 / (1 + Math.Exp(-x)); }
        public override int Outputs => _outputs;
        public SigmoidLayer() {
            ActivationFunction a = new ActivationFunction(
                            sigmoid,
                            y => { return sigmoid(y) * (1 - sigmoid(y)); });
            this.activation = a;
        }

        /// <summary>
        /// extentie van activatie laag om Sigmoid direct te implementeren
        /// </summary>
        /// <param name="nodes"></param>
        /// <param name="matrixOperator"></param>
        public SigmoidLayer(int nodes, IMatrixOperator matrixOperator) : base(nodes, new ActivationFunction(), matrixOperator) {
            ActivationFunction a = new ActivationFunction(
                            sigmoid,
                            y => { return sigmoid(y) * (1 - sigmoid(y)); });
            this.activation = a;
        }
    }
}
