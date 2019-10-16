using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Polite
{
    /// <summary>
    /// The default class within the <see cref="Polite"/> framework that defines object members, and a storage location, which a <see cref="Variable"/> may point to.
    /// </summary>
    public class Container
    {
        /// <summary>
        /// The <see cref="object"/> that this <see cref="Container"/> returns for operation purposes, such as native method invocations and arithmetic.
        /// </summary>
        public virtual dynamic Value { get;  set; }

        /// <summary>
        /// The <see cref="Members"/> attributed to this <see cref="Polite.Container"/>, as children, in source.
        /// </summary>
        public virtual Dictionary<int, Variable> Members { get; internal set; }

        /// <summary>
        /// Creates a new instance of <see cref="Container"/> with a null value.
        /// </summary>
        public Container() : this(null) { }

        /// <summary>
        /// Creates a new instance of <see cref="Container"/> with a value.
        /// </summary>
        /// <param name="value">The <see cref="object"/> used for operation purposes, such as native method invocations and arithmetic.</param>
        public Container(object value)
        {
            if (!(this is Function))
            {
                if (!(this is RefContainer))
                    this.Value = value;
                this.Members = new Dictionary<int, Variable>();
            }
        }

        public override string ToString() => this.Value?.ToString() ?? "null";
    }
}
