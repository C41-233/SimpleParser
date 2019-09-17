using System;
using SimpleParser;

namespace Test
{

    public class MyParser : LexicalParser<LexToken>
    {

        protected override void OnBegin()
        {
            Step = LexStep.Init;
        }

        protected override void OnEnd()
        {
        }

        protected override void Read()
        {
            if (ByteValue < 0)
            {
                switch (Step)
                {
                    case LexStep.Init:
                        return;
                    case LexStep.WaitForToken:
                        EndToken();
                        return;
                    case LexStep.Whitespace:
                        return;
                }
            }
            var ch = (char)ByteValue;
            if (char.IsWhiteSpace(ch))
            {
                switch (Step)
                {
                    case LexStep.Init: return;
                    case LexStep.WaitForToken:
                        EndToken();
                        Step = LexStep.Whitespace;
                        return;
                    case LexStep.Whitespace: return;
                }
            }
            else if (ch == ';')
            {
                switch (Step)
                {
                    case LexStep.Init:
                        PushToken(new LexToken(TokenDefine.Semicolon, ";", LineNumber, ColumnNumber));
                        return;
                    case LexStep.WaitForToken:
                        EndToken();
                        Step = LexStep.Init;
                        PushToken(new LexToken(TokenDefine.Semicolon, ";", LineNumber, ColumnNumber));
                        return;
                    case LexStep.Whitespace:
                        Step = LexStep.Init;
                        PushToken(new LexToken(TokenDefine.Semicolon, ";", LineNumber, ColumnNumber));
                        return;
                }
            }
            else if (ch == '{')
            {
                switch (Step)
                {
                    case LexStep.Init:
                        PushToken(new LexToken(TokenDefine.OpenBrace, "{", LineNumber, ColumnNumber));
                        return;
                    case LexStep.WaitForToken:
                        EndToken();
                        Step = LexStep.Init;
                        PushToken(new LexToken(TokenDefine.OpenBrace, "{", LineNumber, ColumnNumber));
                        return;
                    case LexStep.Whitespace:
                        Step = LexStep.Init;
                        PushToken(new LexToken(TokenDefine.OpenBrace, "{", LineNumber, ColumnNumber));
                        return;
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
                        PushToken(new LexToken(TokenDefine.CloseBrace, "}", LineNumber, ColumnNumber));
                        return;
                    case LexStep.Whitespace:
                        Step = LexStep.Init;
                        PushToken(new LexToken(TokenDefine.CloseBrace, "}", LineNumber, ColumnNumber));
                        return;
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
                        PushToken(new LexToken(TokenDefine.Dot, ".", LineNumber, ColumnNumber));
                        return;
                    case LexStep.Whitespace:
                        Step = LexStep.Init;
                        PushToken(new LexToken(TokenDefine.Dot, ".", LineNumber, ColumnNumber));
                        return;
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
                            return;
                        }
                    case LexStep.WaitForToken:
                        {
                            PushBuffer();
                            return;
                        }
                    case LexStep.Whitespace:
                        {
                            PushBuffer();
                            Step = LexStep.WaitForToken;
                            return;
                        }
                }
            }
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
