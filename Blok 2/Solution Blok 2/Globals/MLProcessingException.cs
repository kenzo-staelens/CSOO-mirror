using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Globals {
    [Serializable]
    public class MLProcessingException : Exception {

        public MLProcessingException() { }

        public MLProcessingException(string message)
            : base(message) { }

        public MLProcessingException(string message, Exception inner)
            : base(message, inner) { }
    }
}
