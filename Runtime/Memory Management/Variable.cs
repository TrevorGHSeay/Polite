using System;
using System.Collections.Generic;

namespace Polite
{

    /// <summary>
    /// The process by which a particular <see cref="Variable"/> is initialized at <see cref="Runtime"/>.
    /// </summary>
    public delegate void Initializer();

    /// <summary>
    /// The class that is used to represent a variable at <see cref="Runtime"/>.
    /// </summary>
    public class Variable
    {
        /// <summary>
        /// The name of the <see cref="Variable"/>.
        /// </summary>
        public string Name { get; internal set; }

        /// <summary>
        /// The alias used to refer to this <see cref="Variable"/> on the <see cref="Stack"/>.
        /// </summary>
        public int Alias { get; internal set; }

        /// <summary>
        /// The <see cref="Polite.Container"/> that this <see cref="Variable"/> points to.
        /// </summary>
        public Container Container { get; set; }

        /// <summary>
        /// Defines the initialization process for this <see cref="Variable"/>.
        /// </summary>
        public Initializer Initialize { get; internal set; }

        /// <summary>
        /// Creates a new instance of <see cref="Variable"/>.
        /// </summary>
        /// <param name="name">The name of the <see cref="Variable"/>, as referenced in source code.</param>
        public Variable(string name)
        {
            this.Name = name;
            this.Alias = name.GetHashCode();
            this.Container = new Container();
        }

        public override string ToString()
        {
            return this.ToString(-1);
        }

        internal string ToString(int depth)
        {
            // Safeguard
            if (depth > 1000)
            {
                return "Continued . . .";
            }
            
            string result = (this.Container is Function f ? f.ToString() : ("var " + this.Name + " = " + (object)this.Container.Value?.ToString()?? "null").ToString()).Indented(depth);

            if (this.Container.Members.Count > 0)
            {
                foreach (KeyValuePair<int, Variable> varPair in this.Container.Members)
                {
                    result += Environment.NewLine + varPair.Value.ToString(depth + 1);
                }
                if (!char.IsWhiteSpace(result[result.Length - 1]))
                    result += Environment.NewLine;
            }
            return result;
        }
    }
}
