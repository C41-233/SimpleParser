namespace SimpleParser
{

    public class Token
    {

        public int Type { get; }
        public string Value { get; }

        public Token(int type, string value)
        {
            Type = type;
            Value = value;
        }

    }
}
