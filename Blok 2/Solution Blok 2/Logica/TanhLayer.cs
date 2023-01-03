using Globals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logica {
    public class TanhLayer : ActivationLayer {
        private double tanh(double x) { return (Math.Exp(2 * x) - 1) / (Math.Exp(2 * x) + 1); }

        /// <summary>
        /// extentie van activatie laag om Tanh direct te implementeren
        /// </summary>
        /// <param name="nodes"></param>
        /// <param name="matrixOperator"></param>
        public TanhLayer(int nodes, IMatrixOperator matrixOperator) : base(nodes, new ActivationFunction(), matrixOperator) {
            ActivationFunction a = new ActivationFunction(
                            tanh,
                            y => { return 1 - Math.Pow(tanh(y), 2); });
            this.activation = a;
        }
    }
}
