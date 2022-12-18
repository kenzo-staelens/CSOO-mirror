using Globals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datalaag {
    public class MatrixProvider : IMatrixProvider {
        public Matrix FromString(string str) {
            throw new NotImplementedException();
        }

        public Matrix Identity(int size) {
            if (size <= 0) throw new ArgumentException($"argument size({size}) cannot be less than or equal to 0");
            var result = Zero(size, size);
            for(int i = 0; i < size; i++) { result[i, i] = 1; }
            return result;
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
                    var boundedRandom = new Random().NextDouble();
                    result[r, c] = Helper.Scale(boundedRandom, 0, 1, min, max);
                }
            }
            return result;
        }

        public Matrix Zero(int row, int col) {
            if (row <= 0 || col <= 0) throw new ArgumentException($"arguments row({row}) and column({col}) cannot be less than or equal to 0");
            return new Matrix(row, col);
        }

        public Matrix FromArray(double[] array) {
            return new Matrix(array);
        }

        public Matrix FromArray(double[,] array) {
            return new Matrix(array);
        }

        public Matrix Copy(Matrix matrix) {
            return new Matrix(matrix);
        }
    }
}
