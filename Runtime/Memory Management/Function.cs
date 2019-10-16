using System;
using System.Collections.Generic;
using System.Text;

namespace Polite
{

    public class Function : Container
    {
        /// <summary>
        /// Does this <see cref="Function"/> return a null value?
        /// </summary>
        public bool ReturnsNull { get; internal set; }

        /// <summary>
        /// The local parameters used to operate within the <see cref="Function"/>.
        /// </summary>
        public Variable[] Parameters { get; internal set; }

        /// <summary>
        /// The internal operations of the <see cref="Function"/>.
        /// </summary>
        public Runtime.Operation Operation { get; internal set; }

        /// <summary>
        /// The parent of this <see cref="Function"/>; in C#, this is accessed using the 'this' keyword.
        /// </summary>
        public Variable Parent { get; internal set; }

        internal bool Running = true;
        internal Variable LastResult;
        internal Variable Result {
            get
            {
                return this.LastResult = this.Operation.Invoke();
            }
            set
            {
                this.Running = false;
                this.LastResult = value;
            }
        }

        /// <summary>
        /// The result of the <see cref="Function"/> after having run through its <see cref="Runtime.Operation"/>
        /// </summary>
        public override object Value
        {
            get
            {
                return this.Result.Container.Value;
            }
            set
            {
                this.Result.Container.Value = value;
            }
        }

        /// <summary>
        /// The members of the <see cref="Function"/>'s resulting <see cref="Container"/>.
        /// </summary>
        public override Dictionary<int, Variable> Members
        {
            get
            {
                return this.LastResult.Container.Members;
            }
            internal set
            {
                this.LastResult = this.Operation.Invoke();
                this.LastResult.Container.Members = value;
            }
        }

        internal Function() : base(null)
        {
            this.LastResult = new Variable(Token.Types.Secondary.ToPUA(Token.Types.Secondary.Function.Result).ToString());
            this.Parameters = Array.Empty<Variable>();
            this.Operation = () => null;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder("function (" + (this.Parameters.Length > 0 ? this.Parameters[0].Name : string.Empty));
            for (int i = 1; i < this.Parameters.Length; i += 1)
                sb.Append(", " + this.Parameters[i].Name);
            sb.Append(")");
            return sb.ToString();
        }

    }
}
