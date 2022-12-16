using Logica;

internal class Program {
    private static void Main(string[] args) {
        Matrix matrix= new Matrix(2,2);
        Matrix matrix2= new Matrix(2, 1);
        matrix.MatrixData[0, 0] = 1;
        matrix.MatrixData[0, 1] = 0;
        matrix.MatrixData[1, 0] = 0;
        matrix.MatrixData[1, 1] = 1;
        matrix2.MatrixData[0, 0] = 3;
        matrix2.MatrixData[1, 0] = 4;
        var r = matrix.dot(matrix2);
        Console.WriteLine(r.Serialize());
    }
}