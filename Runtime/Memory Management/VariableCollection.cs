using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Polite
{
    internal class VariableCollection
    {

        private int VariableIndex { get; set; }
        private int ParameterIndex { get; set; }
        
        private Dictionary<int, Variable> Locals;

        public Variable this[int index] { get => Locals[index]; }

        internal VariableCollection()
        {
            this.VariableIndex = 0;
            this.ParameterIndex = -1;
            this.Locals = new Dictionary<int, Variable>();
        }

        public void AddVariable(Variable variable) => this.Locals.Add(this.VariableIndex++, variable);

        public void AddParameter(Variable variable) => this.Locals.Add(this.ParameterIndex--, variable);

        public bool TryGetVariable(string name, out Variable destination)
        {
            if (this.TryFind(name, out int index))
            {
                destination = this.Locals[index];
                return true;
            }
            destination = null;
            return false;
        }

        public bool Contains(string name) => this.TryFind(name, out int index);

        public bool TryFind(string name, out int index)
        {
            for (int i = this.ParameterIndex + 1; i < this.VariableIndex; i += 1)
            {
                if (this.Locals[i].Name == name)
                {
                    index = i;
                    return true;
                }
            }
            index = 0;
            return false;
        }
    }
}
