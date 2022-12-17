using Globals;

namespace ExtensionMethods {
    public static class Extensions {
        public static int Map(this int value, Func<int, int> mappingMethod) {
            return mappingMethod(value);
        }

        public static void Map(this Matrix value, Action<Matrix> mappingMethod) {
            mappingMethod(value);
        }


    }
}