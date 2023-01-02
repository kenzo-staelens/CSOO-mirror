using Datalaag;
using ExtensionMethods;
using Globals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;


namespace Logica {
    /// <summary>
    /// class that handles convolution layer logic
    /// </summary>
    /// <see cref="https://www.youtube.com/watch?v=Lakz2MoHy6o"/>
    public class ConvolutionLayer : Layer {
        private int[] _shape;
        private int[] _kernelShape;
        private int _depth; // output depth
        private CorrelationModes _mode;
        private IMatrixOperator _matrixOperator;
        private IMatrixProvider _matrixProvider;

        public List<List<Matrix>> KernelList;
        public List<Matrix> Biases;

        private List<Matrix>? _inputList;
        private List<Matrix>? _outputList;

        public override int Outputs {
            get {
                return _shape[0] * _shape[1];
            }
        }
        public int Depth {
            get {
                return _depth;
            }
                private set {
                if (value <= 0) { throw new ArgumentException("invalid depth, expecting a minimum of 1 new channel"); }
                _depth = value;
            }
        }

        public ConvolutionLayer(int[] shape, int kernelsize, int depth, IMatrixOperator matrixOperator, IMatrixProvider matrixProvider) :
            this(shape, kernelsize, depth, CorrelationModes.VALID, matrixOperator, matrixProvider) { }

        public ConvolutionLayer(int[] shape, int kernelsize, int depth, CorrelationModes mode, IMatrixOperator matrixOperator, IMatrixProvider matrixProvider) {
            if (shape == null) throw new ArgumentNullException("shape");
            if (shape.Length != 3) throw new ArgumentException("invalid shape, expecting 3 values in order, rows, colums, channels");
            this._shape = shape;
            this._depth = depth;
            this._mode = mode;
            this._matrixOperator= matrixOperator;
            this._matrixProvider= matrixProvider;
            this._kernelShape = new int[] { kernelsize, kernelsize };
            KernelList = new List<List<Matrix>>();
            Biases = new List<Matrix>();
            
            for(int i = 0; i < depth; i++) {
                List<Matrix> internalKernelList = new List<Matrix>();
                for(int j = 0; j < shape[2]; j++) {
                    internalKernelList.Add(_matrixProvider.Random(kernelsize, kernelsize));
                }
                KernelList.Add(internalKernelList);
                Biases.Add(_matrixProvider.Random(shape[0] - kernelsize + 1, shape[1] - kernelsize + 1));
            }
        }
        
        public List<Matrix> Forward(List<Matrix> inputList) {
            _inputList = new List<Matrix>(inputList);
            _outputList = new List<Matrix>(Biases);
            for (int i=0;i<Depth;i++) {
                for(int j = 0; j < _shape[2]; j++) {
                    Matrix input = inputList[j];
                    if (_mode == CorrelationModes.SAME) input = _matrixOperator.Pad(inputList[j],1,1,1,1);
                    var correlated = _matrixOperator.Correlate(input, KernelList[i][j]);
                    _outputList[i] = _matrixOperator.Add(_outputList[i], correlated);
                }
            }
            return _outputList;
        }

        public List<Matrix> Backward(List<Matrix> outputGradient, double rate) {
            var inputGradient = new List<Matrix>();
            for(int i = 0; i < _shape[2]; i++) inputGradient.Add(_matrixProvider.Zero(_shape[0], _shape[1]));

            for(int i = 0; i < Depth; i++) {
                for(int j = 0; j < _shape[2]; j++) { // TODO: uitzoeken hoe BACKWARD werkt met full convolution
                    KernelList[i][j] = _matrixOperator.Subtract(KernelList[i][j],
                        _matrixOperator.Correlate(_inputList[i], outputGradient[j]).Map((x) => { return x * rate; }));
                    inputGradient[j] = _matrixOperator.Add(inputGradient[j],
                        _matrixOperator.Convolve(
                            _matrixOperator.Pad(outputGradient[j],1,1,1,1),
                            KernelList[i][j]));
                }
            }
            for (int i = 0; i < Biases.Count; i++) Biases[i] = _matrixOperator.Subtract(Biases[i], outputGradient[i]);
            // outputGradient -> map al eerder uitgevoerd; Map (zonder copy) voert aanpassing uit op bestaande matrix, niet nogmaals nodig

            return inputGradient;

        }

        public override Matrix Forward(Matrix input) { // heeft geen implementatie
            throw new NotImplementedException();
        }

        public override Matrix Backward(Matrix gradient, double rate) { // heeft geen implementatie
            throw new NotImplementedException();
        }
    }
}
