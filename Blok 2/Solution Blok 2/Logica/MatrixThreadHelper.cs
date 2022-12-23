using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Globals;

namespace Logica {
    internal class MatrixThreadHelper {
        public void AddRow(Matrix matrix, Matrix matrix2, Matrix result, int row) {
            for (int j = 0; j < result.Columns; j++) {
                result[row, j] = matrix[row, j] + matrix2[row, j];
            }
        }
        public void SubtractRow(Matrix matrix, Matrix matrix2, Matrix result, int row) {
            for (int j = 0; j < result.Columns; j++) {
                result[row, j] = matrix[row, j] - matrix2[row, j];
            }
        }
        public void MultiplyRow(Matrix matrix, Matrix matrix2, Matrix result, int row) {
            for (int j = 0; j < result.Columns; j++) {
                result[row, j] = matrix[row, j] * matrix2[row, j];
            }
        }
        public void DotCell(Matrix matrix, Matrix matrix2, Matrix result, int row, int col) {
            double sum = 0;
            for (int depth = 0; depth < matrix.Columns; depth++) {
                sum += matrix[row,depth] * matrix2[depth,col];
            }
            result[row, col] = sum;
        }
        public void TransposeRow(Matrix matrix, Matrix result, int row) {
            for (int i = 0; i < result.Columns; i++) {
                result[row, i] = matrix[i, row];
            }
        }
    }
}
