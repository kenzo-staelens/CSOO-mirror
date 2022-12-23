using Globals;
using System.Globalization;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using System.Threading.Tasks;
using System;

namespace Logica {
    public class MatrixOperator : IMatrixOperator {
        private readonly MatrixThreadHelper _matrixThreadHelper;
        public MatrixOperator() {
            _matrixThreadHelper = new MatrixThreadHelper();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mat1"></param>
        /// <param name="mat2"></param>
        /// <returns>resultaat van optelling</returns>
        /// <exception cref="MatrixMismatchException"></exception>
        /// <see cref="https://stackoverflow.com/questions/4190949/create-multiple-threads-and-wait-for-all-of-them-to-complete"/>
        public Matrix Add(Matrix mat1, Matrix mat2) {
            if (mat1.Rows != mat2.Rows || mat1.Columns != mat2.Columns) throw new MatrixMismatchException($"can only add matrixes of the same size");
            var result = new Matrix(mat1.Rows, mat1.Columns);
            if (mat1.Rows * mat1.Columns > 100) {
                for (int i = 0; i < result.Rows; i++) {
                    for (int j = 0; j < result.Columns; j++) {
                        result[i, j] = mat1[i, j] + mat2[i, j];
                    }
                }
                return result;
            }
            WaitHandle[] waitHandles = new WaitHandle[result.Rows];
                        for (int row = 0; row < result.Rows; row++) {
                            var j = row; //ontkoppelen van for loop -> parameter
                            var handle = new EventWaitHandle(false, EventResetMode.ManualReset);
                            var thread = new Thread(() =>{
                                _matrixThreadHelper.AddRow(mat1, mat2, result, j);
                                handle.Set();
                            });
                            waitHandles[row] = handle;
                            thread.Start();
                        }
                        WaitHandle.WaitAll(waitHandles);
            return result;
        }

        public Matrix Subtract(Matrix mat1, Matrix mat2) {
            if (mat1.Rows != mat2.Rows || mat1.Columns != mat2.Columns) throw new MatrixMismatchException($"can only add matrixes of the same size");
            var result = new Matrix(mat1.Rows, mat1.Columns);
            if (mat1.Rows * mat1.Columns < 100) {
                for (int i = 0; i < result.Rows; i++) {
                    for (int j = 0; j < result.Columns; j++) {
                        result[i, j] = mat1[i, j] - mat2[i, j];
                    }
                }
                return result;
            }
            WaitHandle[] waitHandles = new WaitHandle[result.Rows];
            for (int row = 0; row < result.Rows; row++) {
                var j = row; //ontkoppelen van for loop -> parameter
                var handle = new EventWaitHandle(false, EventResetMode.ManualReset);
                var thread = new Thread(() => {
                    _matrixThreadHelper.SubtractRow(mat1, mat2, result, j);
                    handle.Set();
                });
                waitHandles[row] = handle;
                thread.Start();
            }
            WaitHandle.WaitAll(waitHandles);
            return result;
        }

        public Matrix Dot(Matrix mat1, Matrix mat2) {
            if (mat1.Columns != mat2.Rows) throw new MatrixMismatchException($"cannot dot matrixes with {mat1.Rows} columns and {mat2.Columns} rows");
            Matrix result = new Matrix(mat1.Rows, mat2.Columns);
            if (mat1.Rows * mat1.Columns * mat2.Columns * mat2.Rows < 100) {
                for (int row = 0; row < result.Rows; row++) {
                    for (int col = 0; col < result.Columns; col++) {
                        double sum = 0;
                        for (int depth = 0; depth < mat1.Columns; depth++) {
                            sum += mat1[row, depth] * mat2[depth, col];
                        }
                        result[row, col] = sum;
                    }
                }
                return result;
            }
            WaitHandle[] waitHandles = new WaitHandle[result.Rows];
            for (int row = 0; row < result.Rows; row++) {
                for(int col = 0; col < result.Columns; col++) {
                    var i = row;
                    var j = col;
                    var handle = new EventWaitHandle(false, EventResetMode.ManualReset);
                    var thread = new Thread(() => {
                        _matrixThreadHelper.DotCell(mat1, mat2, result, i, j);
                        handle.Set();
                    });
                    waitHandles[row] = handle;
                    thread.Start();
                }
            }
            WaitHandle.WaitAll(waitHandles);
            return result;
        }

        public Matrix Multiply(Matrix mat1, Matrix mat2) {
            if (mat1.Columns != mat2.Columns || mat1.Rows != mat2.Rows) throw new MatrixMismatchException($"can only add matrixes of the same size maybe you are looking for IMatrixProvider:Dot(Matrix, Matrix)");
            Matrix result = new Matrix(mat1.Rows, mat1.Columns);
            if (mat1.Rows * mat1.Columns < 100) {
                for (int i = 0; i < result.Rows; i++) {
                    for (int j = 0; j < result.Columns; j++) {
                        result[i, j] = mat1[i, j] * mat2[i, j];
                    }
                }
                return result;
            }
            WaitHandle[] waitHandles = new WaitHandle[result.Rows];
            for (int row = 0; row < result.Rows; row++) {
                var j = row; //ontkoppelen van for loop -> parameter
                var handle = new EventWaitHandle(false, EventResetMode.ManualReset);
                var thread = new Thread(() => {
                    _matrixThreadHelper.MultiplyRow(mat1, mat2, result, j);
                    handle.Set();
                });
                waitHandles[row] = handle;
                thread.Start();
            }
            WaitHandle.WaitAll(waitHandles);
            return result;
        }

        public Matrix Transpose(Matrix mat) {
            Matrix result = new Matrix(mat.Columns, mat.Rows);
            if (result.Columns * result.Rows < 100) {
                for (int row = 0; row < result.Rows; row++) {
                    var temp = mat.GetColumn(row);// useful for later threading
                    for (int i = 0; i < temp.Length; i++) {
                        result[row, i] = temp[i]; // result[row, i] = mat[i, row]
                    }
                }
                return result;
            }
            WaitHandle[] waitHandles = new WaitHandle[result.Rows];
            for (int row = 0; row < result.Rows; row++) {
                var j = row; //ontkoppelen van for loop -> parameter
                var handle = new EventWaitHandle(false, EventResetMode.ManualReset);
                var thread = new Thread(() => {
                    _matrixThreadHelper.TransposeRow(mat, result, j);
                    handle.Set();
                });
                waitHandles[row] = handle;
                thread.Start();
            }
            WaitHandle.WaitAll(waitHandles);
            return result;
        }
    }
}
