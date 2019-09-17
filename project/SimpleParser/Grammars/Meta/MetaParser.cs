namespace SimpleParser.Grammars.Meta
{
    internal class MetaParser : LexicalParser<MetaToken>
    {

        private enum ParseStep
        {
            Idle,
            WaitForSymbol,
        }

        private ParseStep Step;

        protected override void OnBegin()
        {
            Step = ParseStep.Idle;
        }

        protected override void OnEnd()
        {
        }

        protected override void Read()
        {
            if (ByteValue == -1)
            {
                EndSymbol();
                return;
            }
            var ch = (char) ByteValue;
            if (char.IsWhiteSpace(ch))
            {
                switch (Step)
                {
                    case ParseStep.Idle:
                        return;
                    case ParseStep.WaitForSymbol:
                        EndSymbol();
                        Step = ParseStep.Idle;
                        return;
                }
            }
            else if (ch == '(')
            {
                switch (Step)
                {
                    case ParseStep.Idle:
                        PushToken(MetaTokenType.LP);
                        return;
                    case ParseStep.WaitForSymbol:
                        EndSymbol();
                        PushToken(MetaTokenType.LP);
                        Step = ParseStep.Idle;
                        return;
                }
            }
            else if (ch == ')')
            {
                switch (Step)
                {
                    case ParseStep.Idle:
                        PushToken(MetaTokenType.RP);
                        return;
                    case ParseStep.WaitForSymbol:
                        EndSymbol();
                        PushToken(MetaTokenType.RP);
                        Step = ParseStep.Idle;
                        return;
                }
            }
            else if (ch == '+')
            {
                switch (Step)
                {
                    case ParseStep.Idle:
                        PushToken(MetaTokenType.OneOrMore);
                        return;
                    case ParseStep.WaitForSymbol:
                        EndSymbol();
                        PushToken(MetaTokenType.OneOrMore);
                        Step = ParseStep.Idle;
                        return;
                }
            }
            else if (ch == '|')
            {
                switch (Step)
                {
                    case ParseStep.Idle:
                        PushToken(MetaTokenType.Or);
                        return;
                    case ParseStep.WaitForSymbol:
                        EndSymbol();
                        PushToken(MetaTokenType.Or);
                        Step = ParseStep.Idle;
                        return;
                }
            }
            else if(ch == '*')
            {
                switch (Step)
                {
                    case ParseStep.Idle:
                        PushToken(MetaTokenType.ZeroOrMore);
                        return;
                    case ParseStep.WaitForSymbol:
                        EndSymbol();
                        PushToken(MetaTokenType.ZeroOrMore);
                        Step = ParseStep.Idle;
                        return;
                }
            }
            else
            {
                switch (Step)
                {
                    case ParseStep.Idle:
                        PushBuffer();
                        Step = ParseStep.WaitForSymbol;
                        break;
                    case ParseStep.WaitForSymbol:
                        PushBuffer();
                        break;
                }
            }
        }

        private void EndSymbol()
        {
            if (EndBuffer(out var value, out var line, out var column))
            {
                PushToken(new MetaToken(MetaTokenType.Symbol, value, line, column));
            }
        }

        private void PushToken(MetaTokenType type)
        {
            PushToken(new MetaToken(type, ((char)ByteValue).ToString(), LineNumber, ColumnNumber));
        }

    }
}