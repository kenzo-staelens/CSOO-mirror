using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Xml;
using System.Runtime.ConstrainedExecution;

#pragma warning disable CS8618 
#pragma warning disable CS8600
namespace Globals {
    public struct Matrix : ISerializable {

        [XmlIgnore]
        public double[,] MatrixData { get; set; }

        public double[][] JaggedMatrix {
            get {
                var jagged = new double[Rows][];

                for (int i = 0; i < Rows; i++) {
                    jagged[i] = new double[Columns];

                    for (int j = 0; j < Columns; j++)
                        jagged[i][j] = this[i, j];
                }
                return jagged;
            }
            set {
                double[,] res = new double[value.Length, value[0].Length];
                for (int i = 0; i < value.Length; i++) {
                    for (int j = 0; j < value[i].Length; j++) {
                        res[i, j] = value[i][j];
                    }
                }
                MatrixData = res;
            }
        }

        public int Rows {
            get {
                return MatrixData.GetLength(0);
            }
        }

        public int Columns {
            get {
                return MatrixData.GetLength(1);
            }
        }

        public double this[int i, int j] {
            get { return MatrixData[i, j]; }
            set { MatrixData[i, j] = value; }
        }

        public Matrix() { }
        public Matrix(SerializationInfo info, StreamingContext context) {
            JaggedMatrix = (double[][])info.GetValue("JaggedMatrix", typeof(double[][])) ?? new double[1][];
        }

        public Matrix(Matrix mat) {
            this.MatrixData = new double[mat.Rows, mat.Columns];
            for (int i = 0; i < this.Rows; i++) { // array is reference type
                for (int j = 0; j < this.Columns; j++) {
                    this.MatrixData[i, j] = mat.MatrixData[i, j];
                }
            }
        }

        public Matrix(int rows, int cols) {
            this.MatrixData = new double[rows, cols];
        }

        public Matrix(double[] singleDim) {
            this.MatrixData = new double[1, singleDim.Length];
            for (int i = 0; i < singleDim.Length; i++) {
                this.MatrixData[0, i] = singleDim[i];
            }
        }

        public Matrix(double[][] matrix) {
            JaggedMatrix = matrix;
        }

        public Matrix(double[,] matrix) {
            this.MatrixData = new double[matrix.GetLength(0), matrix.GetLength(1)];
            for (int i = 0; i < this.Rows; i++) {
                for (int j = 0; j < this.Columns; j++) {
                    this.MatrixData[i, j] = matrix[i, j];
                }
            }
        }

        /// <summary>
        /// een enkele kolom krijgen van de matrix
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="columnNumber"></param>
        /// <returns></returns>
        /// <see cref="https:// stackoverflow.com/questions/27427527/how-to-get-a-complete-row-or-column-from-2d-array-in-c-sharp"/>
        public double[] GetColumn(int columnNumber) {
            var temp = MatrixData; // cannot access MatrixData in struct from anonymous lamdba
            return Enumerable.Range(0, this.Rows)
                    .Select(x => temp[x, columnNumber])
                    .ToArray();
        }

        /// <summary>
        /// een enkele rij krijgen van de matrix
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="rowNumber"></param>
        /// <returns></returns>
        /// <see cref="https:// stackoverflow.com/questions/27427527/how-to-get-a-complete-row-or-column-from-2d-array-in-c-sharp"/>
        public double[] GetRow(int rowNumber) {
            var temp = MatrixData; // cannot access MatrixData in struct from anonymous lamdba
            return Enumerable.Range(0, this.Columns)
                    .Select(x => temp[rowNumber, x])
                    .ToArray();
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context) {
            info.AddValue("jaggedMatrix", JaggedMatrix);
        }
    }
}
