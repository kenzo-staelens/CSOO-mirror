using Globals;
using System.Globalization;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Logica {
    public class MatrixOperator {
        public MatrixOperator() { }

        public void add(Matrix mat1, Matrix mat2) {

            if (mat1.Rows != mat2.Rows || mat1.Columns != mat2.Columns) throw new MatrixMismatchException();

            for (int i = 0; i < mat1.Rows; i++) {
                for (int j = 0; j < mat1.Columns; j++) {
                    double v = mat1.MatrixData[i, j] + mat2.MatrixData[i, j];
                    mat1.MatrixData[i, j] = v;
                }
            }
            
        }

        public Matrix dot(Matrix mat1, Matrix mat2) {// async this
            if (mat1.Columns != mat2.Rows) throw new MatrixMismatchException($"cannot dot matrixes with {mat1.Rows} columns and {mat2.Columns} rows");
            Matrix result = new Matrix(mat1.Rows, mat2.Columns);

            for(int row = 0; row < result.Rows; row++) {
                for(int col = 0; col < result.Columns; col++) {
                    double sum = 0;
                    for(int depth = 0; depth < mat1.Columns; depth++) {
                        sum += mat1.MatrixData[row, depth] * mat2.MatrixData[depth, col];
                    }
                    result.MatrixData[row, col] = sum;
                }
            }
            return result;
        }

        public Matrix transpose(Matrix mat) {
            Matrix result = new Matrix(mat.Columns, mat.Rows);
            for(int row = 0; row < result.Rows; row++) {
                var temp = mat.GetColumn(row);
                for(int i = 0; i < temp.Length; i++) {
                    result.MatrixData[row, i] = temp[i];
                }
            }
            return result;
        }
    }
}
