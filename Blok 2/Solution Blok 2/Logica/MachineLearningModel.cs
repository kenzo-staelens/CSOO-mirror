using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logica {
    public abstract class MachineLearningModel {
        private double _trainingRate;

        public double TrainingRate {
            get { return _trainingRate; }
            protected set {
                if (value > 1 || value < 0) throw new ArgumentException($"training rate {value} out of bounds [0,1]");
                _trainingRate = value;
            }
        }
        public abstract string predict(double[] inputObject); // serialized prediction

        public abstract void train(double[] trainingInput, double[] trainingOutput);
    }
}
