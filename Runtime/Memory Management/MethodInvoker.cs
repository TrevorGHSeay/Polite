using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Polite
{
    /// <summary>
    /// Wrapper for the <see cref="Function"/> class to help with internal method calls.
    /// </summary>
    public class MethodInvoker : Function
    {
        public override object Value { get => this.Operation.Invoke(); }

        public MethodInvoker() { }
    }
}
