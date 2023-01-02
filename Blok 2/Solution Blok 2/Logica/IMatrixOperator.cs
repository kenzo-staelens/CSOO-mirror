using Globals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logica {
    public interface IMatrixOperator {
        public abstract Matrix Add(Matrix mat1, Matrix mat2);
        public abstract Matrix Subtract(Matrix mat1, Matrix mat2);
        public abstract Matrix Dot(Matrix mat1, Matrix mat2);
        public abstract Matrix Multiply(Matrix mat1, Matrix mat2);
        public abstract Matrix Transpose(Matrix matrix);

        public abstract Matrix Correlate(Matrix mat1, Matrix mat2);
        public abstract Matrix Convolve(Matrix mat1, Matrix mat2);
        public abstract Matrix Rotate180(Matrix mat);
        public abstract Matrix Pad(Matrix mat, int top, int bottom, int left, int right);
    }
}
