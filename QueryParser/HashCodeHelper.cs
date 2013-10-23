using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HigginsThomas.QueryParser.Core
{
    class HashCodeHelper
    {
        public int Hash { get; private set; }

        public static HashCodeHelper HashCode() { return new HashCodeHelper(); }

        public HashCodeHelper()
        {
            Hash = 17;
        }

        public HashCodeHelper Add(Object obj)
        {
            unchecked
            {
                Hash = Hash * 31 + obj.GetHashCode();
            }
            return this;
        }
    }
}
