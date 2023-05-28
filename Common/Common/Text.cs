namespace Common
{
    public static class Text
    {
        private static readonly System.Text.Encoding _defaultEncoding = System.Text.Encoding.UTF8;

        public static string GetString(byte[] bytes)
        {
            return _defaultEncoding.GetString(bytes);
        }

        public static byte[] GetBytes(string str)
        {
            return _defaultEncoding.GetBytes(str);
        }
    }
}
