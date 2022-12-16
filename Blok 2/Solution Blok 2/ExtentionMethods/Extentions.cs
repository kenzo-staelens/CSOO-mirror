using Globals;

namespace ExtentionMethods {
    public static class Extentions {
        public static int Map(this int value, Func<int, int> mappingMethod) {
            return mappingMethod(value);
        }

        public static Matrix Map(this Matrix value, Func<Matrix, Matrix> mappingMethod) {
            return mappingMethod(value);
        }


    }
}