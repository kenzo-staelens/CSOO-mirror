using Logica;

internal class Program {
    private static void Main(string[] args) {
        Matrix matrix= new Matrix(2,2);
        Matrix matrix2= new Matrix(1, 2);
        matrix.MatrixData[0, 0] = 1;
        matrix.MatrixData[0, 1] = 0;
        matrix.MatrixData[1, 0] = 0;
        matrix.MatrixData[1, 1] = 1;
        matrix2.MatrixData[0, 0] = 3;
        matrix2.MatrixData[0, 1] = 4;
        var r = matrix2.dot(matrix);
        Console.WriteLine(r.Serialize());
    }
}