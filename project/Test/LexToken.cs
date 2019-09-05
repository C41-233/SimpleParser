using SimpleParser;

namespace Test
{
    public class LexToken : Token
    {

        public LexToken(TokenDefine type, string value, int line, int column) : base((int)type, value, line, column)
        {
        }

    }
}
