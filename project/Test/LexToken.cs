using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleParser;

namespace Test
{
    public class LexToken : Token
    {

        public LexToken(TokenDefine type, string value) : base((int)type, value)
        {
        }

    }
}
