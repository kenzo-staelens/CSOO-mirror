using Datalaag;
using ExtensionMethods;
using Globals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

#pragma warning disable CS8618 
#pragma warning disable CS8600 
namespace Logica {
    /// <summary>
    /// class that handles convolution layer logic
    /// </summary>
    /// <see cref="https://www.youtube.com/watch?v=Lakz2MoHy6o"/>
    public class ConvolutionLayer : Layer,ISerializable {
        public int[] Shape { get; set; }
        
        public int[] KernelShape {
            get {
                try {
                    return new int[]{KernelList[0][0].Rows, KernelList[0][0].Columns};
                }
                catch {
                    return new int[] { 0, 0 };
                }
            }
        }
        private int _depth; // output depth

        public CorrelationModes Mode;
        private IMatrixOperator _matrixOperator;
        private IMatrixProvider _matrixProvider;

        public List<List<Matrix>> KernelList;
        public List<Matrix> Biases;

        private List<Matrix> _inputList;
        private List<Matrix> _outputList;

        public override int Outputs {
            get {
                return (Shape[0] - KernelShape[0] + 1) * (Shape[1]-KernelShape[1]+1);
            }
            set { /*void*/}
        }
        public int Depth {
            get {
                return _depth;
            }
            set {
                if (value <= 0) { throw new ArgumentException("invalid depth, expecting a minimum of 1 new channel"); }
                _depth = value;
            }
        }

        public ConvolutionLayer() {
            this._matrixOperator = new MatrixOperator();
            this._matrixProvider = new MatrixProvider();
        }
        public ConvolutionLayer(SerializationInfo info, StreamingContext context) {
            Depth = info.GetInt16("Depth");
            Mode = (CorrelationModes?)info.GetValue("Mode", typeof(CorrelationModes)) ?? CorrelationModes.VALID;
            KernelList = (List<List<Matrix>>)info.GetValue("KernelList", typeof(List<List<Matrix>>)) ?? new List<List<Matrix>>();
            Biases = (List<Matrix>)info.GetValue("Biases", typeof(List<Matrix>)) ?? new List<Matrix>();
        }

        public ConvolutionLayer(int[] shape, int kernelsize, int depth) : this(shape, kernelsize, depth, new MatrixOperator(), new MatrixProvider()) { }
        public ConvolutionLayer(int[] shape, int kernelsize, int depth, IMatrixOperator matrixOperator, IMatrixProvider matrixProvider) :
            this(shape, kernelsize, depth, CorrelationModes.VALID, matrixOperator, matrixProvider) { }

        public ConvolutionLayer(int[] shape, int kernelsize, int depth, CorrelationModes mode, IMatrixOperator matrixOperator, IMatrixProvider matrixProvider) {
            if (shape == null) throw new ArgumentNullException("shape");
            if (shape.Length != 3) throw new ArgumentException("invalid shape, expecting 3 values in order, rows, colums, channels");
            this.Shape = shape;
            this._depth = depth;
            this.Mode = mode;
            this._matrixOperator = matrixOperator;
            this._matrixProvider = matrixProvider;
            this.UsesList = true;
            KernelList = new List<List<Matrix>>();
            Biases = new List<Matrix>();

            for (int i = 0; i < depth; i++) {
                List<Matrix> internalKernelList = new List<Matrix>();
                for (int j = 0; j < shape[2]; j++) {
                    internalKernelList.Add(_matrixProvider.Random(kernelsize, kernelsize));
                }
                KernelList.Add(internalKernelList);
                Biases.Add(_matrixProvider.Random(shape[0] - kernelsize + 1, shape[1] - kernelsize + 1));
            }
        }

        public override List<Matrix> Forward(List<Matrix> inputList) {
            _inputList = new List<Matrix>(inputList);
            _outputList = new List<Matrix>(Biases);
            for (int i = 0; i < Depth; i++) {
                for (int j = 0; j < Shape[2]; j++) {
                    Matrix input = inputList[j];
                    if (Mode == CorrelationModes.SAME) input = _matrixOperator.Pad(inputList[j], 1, 1, 1, 1);
                    var correlated = _matrixOperator.Correlate(input, KernelList[i][j]);
                    _outputList[i] = _matrixOperator.Add(_outputList[i], correlated);
                }
            }
            _outputList.Insert(0, new Matrix(1, 1)); // never de-encapsulate
            return _outputList;
        }

        public override List<Matrix> Backward(List<Matrix> outputGradient, double rate) {
            var inputGradient = new List<Matrix>();
            for (int i = 0; i < Shape[2]; i++) inputGradient.Add(_matrixProvider.Zero(Shape[0], Shape[1]));

            for (int i = 0; i < Depth; i++) {
                for (int j = 0; j < Shape[2]; j++) { // TODO: uitzoeken hoe BACKWARD werkt met full convolution
                    KernelList[i][j] = _matrixOperator.Subtract(KernelList[i][j],
                        _matrixOperator.Correlate(_inputList[j], outputGradient[i])
                        .MapCopy((x) => { return x * rate; }));
                    inputGradient[j] = _matrixOperator.Add(inputGradient[j],
                        _matrixOperator.Convolve(
                            _matrixOperator.Pad(outputGradient[i], KernelList[i][j].Rows - 1, KernelList[i][j].Rows - 1,
                            KernelList[i][j].Columns - 1, KernelList[i][j].Columns - 1),
                            KernelList[i][j]));
                }
            }

            for (int i = 0; i < Biases.Count; i++) Biases[i] = _matrixOperator.Subtract(Biases[i], outputGradient[i].MapCopy(x=>x*rate));
            inputGradient.Insert(0, new Matrix(1, 1)); // never de-encapsulate
            return inputGradient;
        }

        public override Matrix Forward(Matrix input) { // heeft geen implementatie
            throw new NotImplementedException();
        }

        public override Matrix Backward(Matrix gradient, double rate) { // heeft geen implementatie
            throw new NotImplementedException();
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context) {
            info.AddValue("Depth", Depth);
            info.AddValue("Mode", Mode);
            info.AddValue("KernelList", KernelList);
            info.AddValue("Biases", Biases);
        }
    }
}
