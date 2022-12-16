using Globals;
using System.Globalization;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Logica {
    public class MatrixOperator {
        public MatrixOperator() { }

        public void add(Matrix mat1, Matrix mat2) {
            if (!Enumerable.SequenceEqual(mat1.Dimensions, mat2.Dimensions)) throw new MatrixMismatchException();

            for (int i = 0; i < mat1.Dimensions[0]; i++) {
                for (int j = 0; j < mat1.Dimensions[1]; j++) {
                    double v = mat1.MatrixData[i, j] + mat2.MatrixData[i, j];
                    mat1.MatrixData[i, j] = v;
                }
            }
            
        }

        public Matrix dot(Matrix mat1, Matrix mat2) {// async this
            if (mat1.Dimensions[1] != mat2.Dimensions[0]) throw new MatrixMismatchException($"cannot dot matrixes with {mat1.Dimensions[0]} columns and {mat2.Dimensions[1]} rows");
            Matrix result = new Matrix(mat1.Dimensions[0], mat2.Dimensions[1]);

            for(int row = 0; row < result.Dimensions[0]; row++) {
                for(int col = 0; col < result.Dimensions[1]; col++) {
                    double sum = 0;
                    for(int depth = 0; depth < mat1.Dimensions[1]; depth++) {
                        sum += mat1.MatrixData[row, depth] * mat2.MatrixData[depth, col];
                    }
                    result.MatrixData[row, col] = sum;
                }
            }
            return result;
        }

        public Matrix transpose(Matrix mat) {
            Matrix result = new Matrix(mat.Dimensions[1], mat.Dimensions[0]);
            for(int row = 0; row < result.Dimensions[0]; row++) {
                var temp = mat.GetColumn(row);
                for(int i = 0; i < temp.Length; i++) {
                    result.MatrixData[row, i] = temp[i];
                }
            }
            return result;
        }
    }
}
