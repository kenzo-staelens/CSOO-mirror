using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Globals;

namespace Logica {
    public abstract class Layer {
        private bool _usesList;
        public bool UsesList { get {
                return _usesList;
            }
            protected set {
                _usesList = value;
            }
        }
        public abstract int Outputs { get; }

        public abstract Matrix Forward(Matrix input);
        public abstract Matrix Backward(Matrix gradient, double rate);

        public abstract List<Matrix> Forward(List<Matrix> input);
        public abstract List<Matrix> Backward(List<Matrix> gradient, double rate);

    }
}
