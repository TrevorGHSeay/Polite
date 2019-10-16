using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Polite
{
    public partial class Token
    {

        /// <summary>
        /// The arities that a <see cref="Token"/> may have in the <see cref="Polite"/> framework.
        /// </summary>
        public static class Arities
        {
            public const short Unary = 1;
            public const short Binary = 2;
            public const short Ternary = 3;
        }
    }
}
