using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Globals;

namespace Datalaag {
    public interface IMatrixProvider {
        public Matrix Zero(int row, int col);
        public Matrix Identity(int size);

        public Matrix Random(int row, int col);

        public Matrix Random(int row, int col, double min, double max);

        public Matrix FromString(string str);
    }
}
