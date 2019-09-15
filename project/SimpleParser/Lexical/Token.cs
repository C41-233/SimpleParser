namespace SimpleParser
{

    public class Token
    {

        public int Type { get; }
        public string Value { get; }
        public int Line { get; }
        public int Column { get; }

        public Token(int type, string value, int line, int column)
        {
            Type = type;
            Value = value;
            Line = line;
            Column = column;
        }

    }
}
