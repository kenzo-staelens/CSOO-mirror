using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Globals {
    public struct Matrix {
        
        public double[,] MatrixData { get; set; }

        public double this[int i, int j] {
            get { return MatrixData[i,j]; }
            set { MatrixData[i,j] = value; }
        }

        public int[] Dimensions {
            get {
                return new int[2] { MatrixData.GetLength(0), MatrixData.GetLength(1) };
            }
        }

        public Matrix(Matrix mat) {
            this.MatrixData = new double[mat.Dimensions[0], mat.Dimensions[1]];
            for (int i = 0; i < this.Dimensions[0]; i++) {
                for (int j = 0; j < this.Dimensions[1]; j++) {
                    this.MatrixData[i, j] = mat.MatrixData[i, j];
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

        public string Serialize() {
            var s = "{ ";
            for (int row = 0; row < Dimensions[0] - 1; row++) {
                s += $"{SerializeRow(GetRow(row))}, ";
            }
            return $"{s}{SerializeRow(GetRow(Dimensions[0] - 1))} }}";
        }

        private string SerializeRow(double[] row) {
            var s = "{ ";
            for (int r = 0; r < row.Length - 1; r++) {
                s += $"{row[r]}, ";
            }
            return $"{s}{row[row.Length - 1]} }}";
        }

        /// <summary>
        /// een enkele kolom krijgen van de matrix
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="columnNumber"></param>
        /// <returns></returns>
        /// <see cref="https://stackoverflow.com/questions/27427527/how-to-get-a-complete-row-or-column-from-2d-array-in-c-sharp"/>
        public double[] GetColumn(int columnNumber) {
            var temp = MatrixData; // cannot access MatrixData in struct from anonymous lamdba
            return Enumerable.Range(0, this.Dimensions[0])
                    .Select(x => temp[x, columnNumber])
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
            var temp = MatrixData; // cannot access MatrixData in struct from anonymous lamdba
            return Enumerable.Range(0, this.Dimensions[1])
                    .Select(x => temp[rowNumber, x])
                    .ToArray();
        }
    }
}
