using Globals;

namespace ExtensionMethods {
    public static class Extensions {
        public static double Map(this double value, Func<double, double> mappingMethod) {
            return mappingMethod(value);
        }

        public static Matrix Map(this Matrix value, Func<double, double> mappingMethod) {//each element independent
            for (int i = 0; i < value.Rows; i++) {
                for (int j = 0; j < value.Columns; j++) {
                    value[i, j] = value[i, j].Map(mappingMethod);
                }
            }
            return value;
        }

        public static Matrix Map(this Matrix value, Func<Matrix, Matrix> mappingMethod) {//for value calculation depending on multiple value
            return mappingMethod(value);
        }
    }
}