using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Globals {
    /// <summary>
    /// klasse voor excepties te maken met matrix dimenties
    /// </summary>
    /// <see cref="https://learn.microsoft.com/en-us/dotnet/standard/exceptions/how-to-create-localized-exception-messages"/>
    [Serializable]
    public class MatrixMismatchException : Exception {

        public MatrixMismatchException() { }

        public MatrixMismatchException(string message)
            : base(message) { }

        public MatrixMismatchException(string message, Exception inner)
            : base(message, inner) { }
    }
}
