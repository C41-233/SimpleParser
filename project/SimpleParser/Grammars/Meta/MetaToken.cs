namespace SimpleParser.Grammars.Meta
{

    internal enum MetaTokenType
    {
        Symbol,
        Optional,
        Or,
        ZeroOrMore,
        OneOrMore,
        LP,
        RP,
    }

    internal class MetaToken : Token
    {

        public MetaToken(MetaTokenType type, string value, int line, int column) : base((int)type, value, line, column)
        {
        }

    }
}