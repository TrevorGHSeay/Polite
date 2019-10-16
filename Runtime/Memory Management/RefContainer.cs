using System;

namespace Polite
{
    /// <summary>
    /// The default class that defines an internal reference to an <see cref="object"/> for things like member access and indexers.
    /// </summary>
    public class RefContainer : Container
    {
        /// <summary>
        /// The <see cref="object"/> that this <see cref="Container"/> returns for operation purposes, such as internal method invocations and arithmetic.
        /// </summary>
        public override object Value { get => Get(); set => Set(value); }

        /// <summary>
        /// The delegate used to obtain the <see cref="RefContainer.Value"/>
        /// </summary>
        public Runtime.Operation Get { get; internal set; }

        /// <summary>
        /// The delegate used to assign the <see cref="RefContainer.Value"/>
        /// </summary>
        public Action<object> Set { get; internal set; }

        /// <summary>
        /// Creates a new instance of <see cref="RefContainer"/> with a getter and setter delegate.
        /// </summary>
        /// <param name="getter">The delegate used to obtain the <see cref="RefContainer.Value"/></param>
        /// <param name="setter">The delegate used to assign the <see cref="RefContainer.Value"/></param>
        public RefContainer(Runtime.Operation getter, Action<object> setter)
        {
            this.Get = getter;
            this.Set = setter;
        }

        /// <summary>
        /// Creates a new instance of <see cref="RefContainer"/>.
        /// </summary>
        public RefContainer() { }

    }
}
