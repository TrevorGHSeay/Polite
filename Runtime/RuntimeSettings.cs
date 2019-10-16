using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Polite
{
    public static class RuntimeSettings
    {

        public static class ValueTypes
        {
            public const int PassByRef = 1 << 0;
            public const int AssignByRef = 1 << 1;
        }
        public static class ObjectTypes
        {
            public const int PassByRef = 1 << 2;
            public const int AssignByRef = 1 << 2;
        }

        /// <summary>
        /// Does mask contain field?
        /// </summary>
        public static bool Contains(int mask, int field)
        {
            return ((mask | field) == mask);
        }

        /// <summary>
        /// Converts the following settings into a usable <see cref="Runtime.RuntimeSettings"/> bitmask.
        /// </summary>
        /// <param name="pass_value_by_ref"></param>
        /// <param name="assign_value_by_ref"></param>
        /// <param name="pass_object_by_ref"></param>
        /// <param name="assign_object_by_ref"></param>
        /// <returns></returns>
        public static int ToBitMask(bool pass_value_by_ref, bool assign_value_by_ref, bool pass_object_by_ref, bool assign_object_by_ref)
        {
            return (Convert.ToInt32(pass_value_by_ref) << 0) | Convert.ToInt32(assign_value_by_ref) << 1 | Convert.ToInt32(pass_object_by_ref) << 2 | Convert.ToInt32(assign_object_by_ref) << 3;
        }
    }
}
