﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Globals;

namespace Logica {
    public abstract class Layer {
        private bool _usesList;

        public bool UsesList {
            get {
                return _usesList;
            }
            set { // zou normaal protected zijn; in plaats is public wegens serializatie
                _usesList = value;
            }
        }

        protected int _outputs;
        public abstract int Outputs { get; set; }

        public abstract Matrix Forward(Matrix input);
        public abstract Matrix Backward(Matrix gradient, double rate);

        public abstract List<Matrix> Forward(List<Matrix> input);
        public abstract List<Matrix> Backward(List<Matrix> gradient, double rate);

    }
}
