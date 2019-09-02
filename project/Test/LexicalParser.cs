using System.Collections.Generic;
using System.IO;
using System.Text;
using SimpleParser;

namespace Test
{

    public class LexicalParser
    {

        private enum LexStep
        {
            Init,
            WaitForToken,
            Whitespace,
        }

        private List<Token> Tokens;
        private StringBuilder Buffer;
        private LexStep Step;

        private void Init()
        {
            Tokens = new List<Token>();
            Buffer = new StringBuilder();
            Step = LexStep.Init;
        }

        private void Clean()
        {
            Tokens = null;
            Buffer = null;
        }

        public List<Token> Parse(string file)
        {
            Init();
            using (var reader = new StreamReader(new FileStream(file, FileMode.Open), Encoding.UTF8))
            {
                int b;
                do
                {
                    b = reader.Read();
                    while (!Eat(b))
                    {
                    }
                } while (b != -1);
            }
            var tokens = Tokens;
            Clean();
            return tokens;
        }

        private bool Eat(int value)
        {
            var ch = (char)value;
            if (char.IsWhiteSpace(ch))
            {
                switch (Step)
                {
                    case LexStep.Init: break;
                    case LexStep.WaitForToken:
                        EndToken();
                        Step = LexStep.Whitespace;
                        break;
                    case LexStep.Whitespace: break;
                }
            }
            else if (ch == ';')
            {
                switch (Step)
                {
                    case LexStep.Init:
                        Tokens.Add(new Token
                        {
                            Type = (int)TokenDefine.Semicolon,
                            Value = ";",
                        });
                        break;
                    case LexStep.WaitForToken:
                        EndToken();
                        Step = LexStep.Init;
                        return false;
                    case LexStep.Whitespace:
                        Step = LexStep.Init;
                        return false;
                }
            }
            else if (ch == '{')
            {
                switch (Step)
                {
                    case LexStep.Init:
                        Tokens.Add(new Token
                        {
                            Type = (int)TokenDefine.OpenBrace,
                            Value = "{",
                        });
                        break;
                    case LexStep.WaitForToken:
                        EndToken();
                        Step = LexStep.Init;
                        return false;
                    case LexStep.Whitespace:
                        Step = LexStep.Init;
                        return false;
                }
            }
            else if (ch == '}')
            {
                switch (Step)
                {
                    case LexStep.Init:
                        Tokens.Add(new Token
                        {
                            Type = (int)TokenDefine.CloseBrace,
                            Value = "}",
                        });
                        break;
                    case LexStep.WaitForToken:
                        EndToken();
                        Step = LexStep.Init;
                        return false;
                    case LexStep.Whitespace:
                        Step = LexStep.Init;
                        return false;
                }
            }
            else if (ch == '.')
            {
                switch (Step)
                {
                    case LexStep.Init:
                        Tokens.Add(new Token
                        {
                            Type = (int)TokenDefine.Dot,
                            Value = ".",
                        });
                        break;
                    case LexStep.WaitForToken:
                        EndToken();
                        Step = LexStep.Init;
                        return false;
                    case LexStep.Whitespace:
                        Step = LexStep.Init;
                        return false;
                }
            }
            else
            {
                switch (Step)
                {
                    case LexStep.Init:
                        {
                            Buffer.Append(ch);
                            Step = LexStep.WaitForToken;
                            break;
                        }
                    case LexStep.WaitForToken:
                        {
                            Buffer.Append(ch);
                            break;
                        }
                    case LexStep.Whitespace:
                        {
                            Buffer.Append(ch);
                            Step = LexStep.WaitForToken;
                            break;
                        }
                }
            }

            return true;
        }

        private void EndToken()
        {
            Tokens.Add(new Token
            {
                Type = (int)TokenDefine.Token,
                Value = Buffer.ToString(),
            });
            Buffer.Clear();
        }

    }

}
