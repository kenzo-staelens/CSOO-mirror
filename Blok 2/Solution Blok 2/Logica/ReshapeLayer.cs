using Globals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Logica {
    /// <summary>
    /// omzettingen tussen types input voor convolution laag (List<Matrix>) en dense laag (Matrix)
    /// </summary>
    public class ReshapeLayer : Layer,ISerializable {
        private int[] _inputShape;
        private int[] _outputShape;

        public int[] InputShape {
            get {
                return _inputShape;
            }
            set {
                if (_inputShape != null) {
                    int[] depth = new int[2];
                    try { depth[0] = _inputShape[2]; }
                    catch { depth[0] = 1; }
                    try { depth[1] = value[2]; }
                    catch { depth[1] = 1; }
                    if (_inputShape[0] * _inputShape[1] * depth[0] == value[0] * value[1] * depth[1]) _inputShape = value;
                }
                else { _inputShape = value; }
            }
        }

        public int[] OutputShape {
            get {
                return _outputShape;
            }
            set {
                if (_outputShape != null) {
                    int[] depth = new int[2];
                    try { depth[0] = _inputShape[2]; }
                    catch { depth[0] = 1; }
                    try { depth[1] = value[2]; }
                    catch { depth[1] = 1; }
                    if (_outputShape[0] * _outputShape[1] * depth[0] == value[0] * value[1] * depth[1]) _outputShape = value;
                }
                else { _outputShape = value; }
            }
        }

        public override int Outputs {
            get {
                var temp = _outputShape[0] * _outputShape[1];
                if (_outputShape.Length == 3) temp *= _outputShape[2];
                return temp;
            }
        }
        public ReshapeLayer() { }
        public ReshapeLayer(SerializationInfo info, StreamingContext context) {

        }
        public ReshapeLayer(int[] inputShape, int[] outputShape) {
            if ((inputShape.Length != 2 && inputShape.Length != 3) || (outputShape.Length != 2 && outputShape.Length != 3))
                throw new ArgumentOutOfRangeException("shapes can only contain 2 or 3 dimensions");
            var mul1 = 1;
            var mul2 = 1;
            for(int i = 0; i < 3; i++) {
                try { mul1*= inputShape[i]; }
                catch (Exception) { }

                try {mul2*= outputShape[i]; }
                catch (Exception) { }
            }
            if (mul1 != mul2) throw new ArgumentException("input and output shapes must have contain number of elemens");
            this._inputShape = inputShape;
            this._outputShape = outputShape;
            this.UsesList = true;
        }

        public override Matrix Forward(Matrix input) {
            throw new NotImplementedException();
        }

        public override Matrix Backward(Matrix gradient, double rate) {
            throw new NotImplementedException();
        }

        private List<Matrix> reshape(List<Matrix> input, int[] inputshape, int[] outputshape) {
            if (inputshape.Length == 3) { if (input.Count != inputshape[2]) throw new ArgumentException("invalid number of matrixes"); }
            else if (input.Count != 1) throw new ArgumentException("invalid number of matrixes");

            for (int i = 0; i < input.Count; i++) if (input[i].Rows != inputshape[0] || input[i].Columns != inputshape[1])
                    throw new ArgumentException("unexpected input matrix, received matrix with non matching dimensions");

            List<Matrix> result = new List<Matrix>();
            var flag = new Matrix(1, 1); // 0 = keep, 1 = extract to Matrix; see Neural2 class
            if (outputshape.Length == 2) flag[0, 0] = 1;
            result.Add(flag);
            int ri = 0, rj = 0;
            Matrix outmat = new Matrix(outputshape[0], outputshape[1]);
            for (int i = 0; i < input.Count; i++) {
                for (int j = 0; j < input[i].Rows; j++) {
                    for (int k = 0; k < input[i].Columns; k++) {
                        outmat[ri, rj] = input[i][j, k];
                        rj++;
                        if (rj == outputshape[1]) { rj = 0; ri++; }
                        if (ri == outputshape[0]) { ri = 0; result.Add(outmat); outmat = new Matrix(outputshape[0], outputshape[1]); }
                    }
                }
            }
            return result;
        }

        public override List<Matrix> Forward(List<Matrix> input) {
            return reshape(input, _inputShape, _outputShape);
            throw new NotImplementedException();
        }

        public override List<Matrix> Backward(List<Matrix> gradient, double rate) {
            return reshape(gradient, _outputShape, _inputShape);
            throw new NotImplementedException();
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context) {
            info.AddValue("InputShape", InputShape);
            info.AddValue("OutputShape", OutputShape);
        }
    }
}
