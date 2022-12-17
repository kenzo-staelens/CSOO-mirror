using Globals;
using System.Globalization;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Logica {
    public class MatrixOperator : IMatrixOperator {
        public MatrixOperator() { } //async all this

        public Matrix Add(Matrix mat1, Matrix mat2) {

            if (mat1.Rows != mat2.Rows || mat1.Columns != mat2.Columns) throw new MatrixMismatchException();
            var result = new Matrix(mat1.Rows, mat1.Columns);
            for (int i = 0; i < result.Rows; i++) {
                for (int j = 0; j < result.Columns; j++) {
                    result[i, j] = mat1[i, j] + mat2[i, j];
                }
            }
            return result;
            
        }

        public Matrix Dot(Matrix mat1, Matrix mat2) {
            if (mat1.Columns != mat2.Rows) throw new MatrixMismatchException($"cannot dot matrixes with {mat1.Rows} columns and {mat2.Columns} rows");
            Matrix result = new Matrix(mat1.Rows, mat2.Columns);

            for(int row = 0; row < result.Rows; row++) {
                for(int col = 0; col < result.Columns; col++) {
                    double sum = 0;
                    for(int depth = 0; depth < mat1.Columns; depth++) {
                        sum += mat1[row, depth] * mat2[depth, col];
                    }
                    result[row, col] = sum;
                }
            }
            return result;
        }

        public Matrix Transpose(Matrix mat) {
            Matrix result = new Matrix(mat.Columns, mat.Rows);
            for(int row = 0; row < result.Rows; row++) {
                var temp = mat.GetColumn(row);// useful for later threading
                for(int i = 0; i < temp.Length; i++) {
                    result[row, i] = temp[i]; // result[row, i] = mat[i, row]
                }
            }
            return result;
        }
    }
}
