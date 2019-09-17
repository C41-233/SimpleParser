using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace SimpleParser
{

    public abstract class LexicalParser<T> where T : Token
    {

        private readonly List<T> tokens = new List<T>(1);
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

        public IEnumerable<T> ParseFile(string file)
        {
            Init();
            using (var reader = new StreamReader(new FileStream(file, FileMode.Open, FileAccess.Read), Encoding.UTF8))
            {
                do
                {
                    ByteValue = reader.Read();
                    Read();

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

        public IEnumerable<T> Parse(string content)
        {
            Init();
            foreach (var ch in content)
            {
                ByteValue = ch;
                Read();
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
            Read();

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

        protected abstract void Read();

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

        protected bool EndBuffer(out string value, out int line, out int column)
        {
            if (buffer.Length > 0)
            {
                value = buffer.ToString();
                buffer.Clear();
                line = bufferLine;
                column = bufferColumn;
                return true;
            }

            value = null;
            line = 0;
            column = 0;
            return false;
        }

        protected void PushToken(T token)
        {
            tokens.Add(token);
        }

    }

}
