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

namespace Globals {
    [Serializable]
    public struct Matrix : IEnumerable {

        public double[,] MatrixData { get; set; }

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

        public Matrix(double[][] matrix) : this(convert(matrix)) { }

        public Matrix(double[,] matrix) {
            this.MatrixData = new double[matrix.GetLength(0), matrix.GetLength(1)];
            for (int i = 0; i < this.Rows; i++) {
                for (int j = 0; j < this.Columns; j++) {
                    this.MatrixData[i, j] = matrix[i, j];
                }
            }
        }

        public string Serialize() {
            MemoryStream ms = new MemoryStream();
            XmlTextWriter streamWriter = new XmlTextWriter(ms, Encoding.UTF8);
            streamWriter.Formatting = Formatting.Indented;
            XmlSerializer ser = new XmlSerializer(typeof(double[][]));
            var jagged = new double[Rows][];

            for (int i = 0; i < Rows; i++) {
                jagged[i] = new double[Columns];

                for (int j = 0; j < Columns; j++)
                    jagged[i][j] = this[i, j];
            }
            ser.Serialize(streamWriter, jagged);

            return Encoding.UTF8.GetString(ms.ToArray());
        }

        public static Matrix Deserialize(String xml) {
            double[][] result;
            XmlSerializer ser = new XmlSerializer(typeof(double[][]));
            using (TextReader reader = new StringReader(xml)) {
                Console.WriteLine("\n\n" + reader.ReadLine()); //weghalen ongeldige lijn
                result = (double[][])ser.Deserialize(reader);
            }
            return new Matrix(result);
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

        IEnumerator IEnumerable.GetEnumerator() {
            return MatrixData.GetEnumerator();
        }

        private static double[,] convert(double[][] doubleArr) {
            double[,] res = new double[doubleArr.Length, doubleArr[0].Length];
            for (int i = 0; i < doubleArr.Length; i++) {
                for (int j = 0; j < doubleArr[i].Length; j++) {
                    res[i, j] = doubleArr[i][j];
                }
            }
            return res;
        }
    }
}
