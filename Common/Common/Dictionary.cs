namespace Common
{
    public class Dictionary<Tkey, Tvalue> : System.Collections.Generic.Dictionary<Tkey, Tvalue>
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
