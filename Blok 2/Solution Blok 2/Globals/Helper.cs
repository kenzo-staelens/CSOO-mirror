using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Globals {
    public class Helper {
        public static double Map(double value, double in_start, double in_end, double out_start, double out_end) {
            if (value < in_start || value > in_end) throw new ConstraintException($"value {value} out of bounds for set values in_start({in_start}) and in_end({in_end})");
            if (in_start == in_end) throw new InvalidConstraintException($"value in_start({in_start}) and in_end({in_end}) must be different");
            if (out_start == out_end) throw new InvalidConstraintException($"value out_start({out_start}) and out_end({out_end}) must be different");

            if(in_start==out_start && in_end==out_end) return value;
            return (value - in_start) * (out_end - out_start) / (in_end - in_start) + out_start;
        }
    }
}
