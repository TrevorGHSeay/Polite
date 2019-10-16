using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Polite
{
    public class Object
    {

        /// <summary>
        /// The <see cref="Polite.Container"/> that the <see cref="Object"/> currently references.
        /// </summary>
        public Container Container { get; internal set; }

        /// <summary>
        /// The <see cref="Members"/> attributed to this <see cref="Polite.Container"/>, as children, in source.
        /// </summary>
        public Dictionary<int, Variable> Members { get; internal set; }
        
        public Object()
        {
            this.Members = new Dictionary<int, Variable>();
            this.Container = new Container();
        }
    }
}
