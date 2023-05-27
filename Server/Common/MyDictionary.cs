using System.Collections.Generic;

namespace Server.Common
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
