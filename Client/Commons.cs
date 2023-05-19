using System;
using System.Collections.Generic;
using System.Text;

namespace Client.Commons
{
    public class MyDictionary<Tkey, Tvalue> : Dictionary<Tkey, Tvalue>
    {
        public Tvalue Get(Tkey key, Tvalue defaultValue)
        {
            return TryGetValue(key, out Tvalue value) ? value : defaultValue;
        }
        public Tvalue Get(Tkey key)
        {
            return TryGetValue(key, out Tvalue value) ? value : default;
        }
    }
}
