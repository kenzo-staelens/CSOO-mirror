namespace Globals {
    public struct ActivationFunction {
        public Func<double, double> Forward { get; }
        public Func<double, double> Backward { get; }
        
        public ActivationFunction(Func<double, double> forward, Func<double, double> backward) {
            Forward = forward;
            Backward = backward;
        }
    }
}
