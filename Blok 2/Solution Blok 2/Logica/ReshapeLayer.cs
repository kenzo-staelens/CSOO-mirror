using Globals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logica {
    /// <summary>
    /// omzettingen tussen types input voor convolution laag (List<Matrix>) en dense laag (Matrix)
    /// </summary>
    internal class ReshapeLayer : Layer {
        public override int Outputs => throw new NotImplementedException();

        public override Matrix Backward(Matrix gradient, double rate) {
            throw new NotImplementedException();
        }

        public override Matrix Forward(Matrix input) {
            throw new NotImplementedException();
        }
    }
}
