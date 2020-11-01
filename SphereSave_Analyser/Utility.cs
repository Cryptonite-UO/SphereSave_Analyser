namespace SphereSave_Analyser
{
        public static class Util
    {
        public static int StringHexToInt(string s)
        {
            return int.Parse(s, System.Globalization.NumberStyles.HexNumber);
        }
    }
}