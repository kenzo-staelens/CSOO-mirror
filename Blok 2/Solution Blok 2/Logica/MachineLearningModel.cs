using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Globals;

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
        public abstract Matrix predict(double[] inputObject);

        public abstract void train(double[] trainingInput, double[] trainingOutput);
    }
}
