using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace SimpleParser
{

    public abstract class LexicalParser
    {

        private readonly List<Token> tokens = new List<Token>(1);
        private StringBuilder buffer;

        private void Init()
        {
            buffer = new StringBuilder();
            LineNumber = 1;
            OnBegin();
        }

        private void Clean()
        {
            Debug.Assert(tokens.Count == 0);
            buffer = null;
            OnEnd();
        }

        protected abstract void OnBegin();

        protected abstract void OnEnd();

        public IEnumerable<Token> ParseFile(string file)
        {
            Init();
            using (var reader = new StreamReader(new FileStream(file, FileMode.Open, FileAccess.Read), Encoding.UTF8))
            {
                do
                {
                    ByteValue = reader.Read();
                    while (!Read())
                    {
                    }

                    if (tokens.Count > 0)
                    {
                        foreach (var token in tokens)
                        {
                            yield return token;
                        }
                        tokens.Clear();
                    }

                    ColumnNumber++;
                    if (ByteValue == '\n')
                    {
                        LineNumber++;
                        ColumnNumber = 1;
                    }
                } while (ByteValue != -1);
                Clean();
            }
        }

        public IEnumerable<Token> Parse(string content)
        {
            Init();
            foreach (var ch in content)
            {
                ByteValue = ch;
                while (!Read())
                {
                }

                if (tokens.Count > 0)
                {
                    foreach (var token in tokens)
                    {
                        yield return token;
                    }
                    tokens.Clear();
                }

                ColumnNumber++;
                if (ByteValue == '\n')
                {
                    LineNumber++;
                    ColumnNumber = 1;
                }
            }

            ByteValue = -1;
            while (!Read())
            {
            }

            if (tokens.Count > 0)
            {
                foreach (var token in tokens)
                {
                    yield return token;
                }
                tokens.Clear();
            }

            Clean();
        }

        protected int ByteValue { get; private set; }
        protected int LineNumber { get; private set; }
        protected int ColumnNumber { get; private set; }

        protected abstract bool Read();

        protected void PushBuffer()
        {
            if (buffer.Length == 0)
            {
                bufferLine = LineNumber;
                bufferColumn = ColumnNumber;
            }
            buffer.Append((char)ByteValue);
        }

        private int bufferLine;
        private int bufferColumn;

        protected void EndBuffer(out string value, out int line, out int column)
        {
            value = buffer.ToString();
            buffer.Clear();
            line = bufferLine;
            column = bufferColumn;
        }

        protected void PushToken(Token token)
        {
            tokens.Add(token);
        }

    }

}
