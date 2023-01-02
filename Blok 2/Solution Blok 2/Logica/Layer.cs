using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Globals;

namespace Logica {
    public abstract class Layer {
        public abstract int Outputs { get; }

        public abstract Matrix Forward(Matrix input);
        public abstract Matrix Backward(Matrix gradient, double rate);

    }
}
