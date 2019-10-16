using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Polite
{

    /// <summary>
    /// Used as a base-class for <see cref="Heap"/>, and all layers of a <see cref="Stack"/>.
    /// </summary>
    public class Frame
    {

        /// <summary>
        /// Every <see cref="Variable"/> of the <see cref="Frame"/> indexed by their <see cref="Variable.Alias"/>
        /// </summary>
        public Dictionary<int, Variable> Variables { get; internal set; }

        /// <summary>
        /// Creates a new instance of <see cref="Frame"/>.
        /// </summary>
        public Frame()
        {
            Variables = new Dictionary<int, Variable>();
        }

        /// <summary>
        /// Clears all members of the <see cref="Frame"/>. (Short-hand for <see cref="Members"/>.Clear()).
        /// </summary>
        public void Clear() => Variables.Clear();

        /// <summary>
        /// Gets the <see cref="Container"/> in <see cref="Members"/> with key 'alias', or adds 'if_unfound' and returns that.
        /// </summary>
        /// <param name="alias">The key to try to get.</param>
        /// <param name="if_unfound">The <see cref="Container"/> to add and return if 'alias' is not found in <see cref="Members"/>.</param>
        /// <returns>The <see cref="Container"/> found in <see cref="Members"/>, or the 'if_unfound' argument's value if newly added.</returns>
        public Variable GetOrSet(int alias, Variable if_unfound)
        {
            if (Variables.ContainsKey(alias))
                return this.Variables[alias];

            if (!(if_unfound is null))
                this.Variables.Add(alias, if_unfound);
            return if_unfound;
        }

        /// <summary>
        /// Sets or adds the key 'alias' to 'value'.
        /// </summary>
        /// <param name="alias">The key to set or add.</param>
        /// <param name="value">The <see cref="Variable"/> to set in, or add to, <see cref="Variables"/> under key 'alias'.</param>
        public void Set(int alias, Variable value)
        {
            if (Variables.ContainsKey(alias))
                Variables[alias] = value;
            else
                Variables.Add(alias, value);
        }
    }
}
