using Globals;
using System.Globalization;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Logica {
    public class Matrix : ISerializable {
        public double[,] MatrixData { get; set; }
        public int[] Dimensions { get {
                return new int[2] { MatrixData.GetLength(0), MatrixData.GetLength(1)};
            }
        }
        public Matrix() {
            this.MatrixData = new double[1,1];
            this.MatrixData[0,0] = 0;
        }

        public Matrix(Matrix mat) {
            this.MatrixData = new double[mat.Dimensions[0], mat.Dimensions[1]];
            for(int i=0; i < this.Dimensions[0]; i++) {
                for (int j = 0; j < this.Dimensions[1]; j++) {
                    this.MatrixData[i, j] = mat.MatrixData[i,j];
                }
            }
        }

        public Matrix(int rows, int cols) {
            this.MatrixData = new double[rows, cols];
        }

        public Matrix(double[,] matrix) {
            this.MatrixData = new double[matrix.GetLength(0), matrix.GetLength(1)];
            for (int i = 0; i < this.Dimensions[0]; i++) {
                for (int j = 0; j < this.Dimensions[1]; j++) {
                    this.MatrixData[i, j] = matrix[i, j];
                }
            }
        }

        public void add(Matrix mat) {
            if (!Enumerable.SequenceEqual(this.Dimensions, mat.Dimensions)) throw new MatrixMismatchException();

            for (int i = 0; i < this.Dimensions[0]; i++) {
                for (int j = 0; j < this.Dimensions[1]; j++) {
                    double v = this.MatrixData[i, j] + mat.MatrixData[i, j];
                    this.MatrixData[i, j] = v;
                }
            }
            
        }

        public Matrix dot(Matrix mat) {
            if (this.Dimensions[1] != mat.Dimensions[0]) throw new MatrixMismatchException($"cannot dot matrixes with {this.Dimensions[0]} columns and {mat.Dimensions[1]} rows");
            Matrix result = new Matrix(mat.Dimensions[0], this.Dimensions[1]);

            for(int row = 0; row < result.Dimensions[0]; row++) {
                for(int col = 0; col < result.Dimensions[1]; col++) {
                    double sum = 0;
                    for(int depth = 0; depth < this.Dimensions[1]; depth++) {
                        sum =+ this.MatrixData[row, depth] * mat.MatrixData[depth, col];
                    }
                    result.MatrixData[row, col] = sum;
                }
            }
            return result;
        }

        public string Serialize() {
            var s = "{ ";
            for (int row = 0; row < Dimensions[1]-1; row++) {
                s += $"{SerializeRow(GetRow(row))}, ";
            }
            return $"{s} {SerializeRow(GetRow(Dimensions[1] - 1))} }}";
        }

        private string SerializeRow(double[] row) {
            var s = "{ ";
            for(int r=0;r<row.Length-1;r++) {
                s += $"{row[r]}, ";
            }
            return $"{s} {row[row.Length-1]} }}";
        }

        /// <summary>
        /// een enkele kolom krijgen van de matrix
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="columnNumber"></param>
        /// <returns></returns>
        /// <see cref="https://stackoverflow.com/questions/27427527/how-to-get-a-complete-row-or-column-from-2d-array-in-c-sharp"/>
        public double[] GetColumn(int columnNumber) {
            return Enumerable.Range(0, this.Dimensions[0])
                    .Select(x => this.MatrixData[x, columnNumber])
                    .ToArray();
        }

        /// <summary>
        /// een enkele rij krijgen van de matrix
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="rowNumber"></param>
        /// <returns></returns>
        /// <see cref="https://stackoverflow.com/questions/27427527/how-to-get-a-complete-row-or-column-from-2d-array-in-c-sharp"/>
        public double[] GetRow(int rowNumber) {
            return Enumerable.Range(0, this.Dimensions[1])
                    .Select(x => this.MatrixData[rowNumber, x])
                    .ToArray();
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context) {
            throw new NotImplementedException();
        }
    }
}
