using Globals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datalaag {
    internal class MatrixProvider : IMatrixProvider {
        public Matrix FromString(string str) {
            throw new NotImplementedException();
        }

        public Matrix Identity(int row, int col) {
            if (row <= 0 || col <= 0) throw new ArgumentException($"arguments row({row}) and column({col}) cannot be less than or equal to 0");
            if (row!=col) throw new ArgumentException($"row({row}) and column({col}) must be equal for identity matrixes");
            throw new NotImplementedException();
        }

        public Matrix Random(int row, int col) {
            if (row <= 0 || col <= 0) throw new ArgumentException($"arguments row({row}) and column({col}) cannot be less than or equal to 0");
            return Random(row, col, -1,1);
        }

        public Matrix Random(int row, int col, double min, double max) {
            if (row <= 0 || col <= 0) throw new ArgumentException($"arguments row({row}) and column({col}) cannot be less than or equal to 0");
            var result = Zero(row, col);
            for(int r = 0; r < row; r++) {
                for(int c = 0; c < col; c++) {
                    var boundedRandom = (new Random()).NextDouble();
                    result[r, c] = Helper.Map(boundedRandom, 0, 1, min, max);
                }
            }
            return result;
        }

        public Matrix Zero(int row, int col) {
            if (row <= 0 || col <= 0) throw new ArgumentException($"arguments row({row}) and column({col}) cannot be less than or equal to 0");
            return new Matrix(row, col);
        }
    }
}
