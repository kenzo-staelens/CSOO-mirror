using Datalaag;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Logica {
    public class NeuralNetwork : MachineLearningModel {

        public NeuralNetwork(IMatrixProvider matrixProvider) : this(0.5, matrixProvider) { }

        public NeuralNetwork(double trainingRate, IMatrixProvider matrixProvider) {
            this.TrainingRate = trainingRate;
        }

        public override string predict(double[] inputObject) {
            throw new NotImplementedException();
        }

        public override void train(double[] trainingInput, double[] trainingOutput) {
            throw new NotImplementedException();
        }

        public void Initialize() {
            throw new NotImplementedException();
        }

        public void Load() {
            throw new NotImplementedException();
        }
    }
}
