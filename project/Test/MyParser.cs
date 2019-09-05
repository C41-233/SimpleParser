using SimpleParser;

namespace Test
{

    public class MyParser : LexicalParser
    {

        protected override void OnBegin()
        {
            Step = LexStep.Init;
        }

        protected override void OnEnd()
        {
        }

        protected override bool Read()
        {
            var ch = (char)ByteValue;
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
                        PushToken(new LexToken(TokenDefine.Semicolon, ";", LineNumber, ColumnNumber));
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
                        PushToken(new LexToken(TokenDefine.OpenBrace, "{", LineNumber, ColumnNumber));
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
                        PushToken(new LexToken(TokenDefine.CloseBrace, "}", LineNumber, ColumnNumber));
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
                        PushToken(new LexToken(TokenDefine.Dot, ".", LineNumber, ColumnNumber));
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
                            PushBuffer();
                            Step = LexStep.WaitForToken;
                            break;
                        }
                    case LexStep.WaitForToken:
                        {
                            PushBuffer();
                            break;
                        }
                    case LexStep.Whitespace:
                        {
                            PushBuffer();
                            Step = LexStep.WaitForToken;
                            break;
                        }
                }
            }

            return true;
        }

        private void EndToken()
        {
            EndBuffer(out var value, out var line, out var column);
            PushToken(new LexToken(TokenDefine.Token, value, line, column));
        }

        private enum LexStep
        {
            Init,
            WaitForToken,
            Whitespace,
        }

        private LexStep Step;

    }

}
