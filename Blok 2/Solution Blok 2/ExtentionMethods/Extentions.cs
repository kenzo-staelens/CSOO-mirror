using Globals;

namespace ExtentionMethods {
    public static class Extentions {
        public static int map(this int value, Func<int, int> mappingMethod) {
            return mappingMethod(value);
        }

        public static Matrix map(this Matrix value, Func<Matrix, Matrix> mappingMethod) {
            return mappingMethod(value);
        }


    }
}