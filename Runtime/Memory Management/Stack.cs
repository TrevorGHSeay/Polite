using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Polite
{
    /// <summary>
    /// The class that handles stack memory within the <see cref="Polite.Runtime"/>.
    /// </summary>
    public class Stack
    {

        private Stack<Frame> Frames;
        
        /// <summary>
        /// The <see cref="Frame"/> currently at the top of the <see cref="Stack"/>.
        /// </summary>
        public Frame Top { get; private set; }

        /// <summary>
        /// The persistent <see cref="Frame"/> at the bottom of the <see cref="Stack"/>. This is what Polite refers to as its Heap.
        /// </summary>
        public Frame Bottom { get; private set; }

        /// <summary>
        /// Creates a new instance of <see cref="Stack"/>.
        /// </summary>
        public Stack()
        {
            this.Frames = new Stack<Frame>();
            this.Reset();
        }

        /// <summary>
        /// Pushes a new <see cref="Frame"/> onto the <see cref="Stack"/>.
        /// </summary>
        public void Push()
        {
            Frames.Push(this.Top = new Frame());
        }

        /// <summary>
        /// Removes the top <see cref="Frame"/> from the top of the <see cref="Stack"/> and returns it.
        /// </summary>
        public Frame Pop()
        {
            Frames.Pop();
            return Top = Frames.Count == 0 ? null : Frames.Peek();
        }

        /// <summary>
        /// Resets the <see cref="Stack"/>.
        /// </summary>
        public void Reset()
        {
            this.Frames.Clear();
            this.Frames.Push(this.Top = this.Bottom = new Frame());
        }

    }

}
