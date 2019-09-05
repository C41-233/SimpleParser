using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SimpleParser
{

    public abstract class LexicalParser
    {

        private List<Token> tokens;
        private StringBuilder buffer;

        private void Init()
        {
            tokens = new List<Token>();
            buffer = new StringBuilder();
            LineNumber = 1;
            OnBegin();
        }

        private void Clean()
        {
            tokens = null;
            buffer = null;
            OnEnd();
        }

        protected abstract void OnBegin();

        protected abstract void OnEnd();

        public List<Token> Parse(string file)
        {
            Init();
            using (var reader = new StreamReader(new FileStream(file, FileMode.Open), Encoding.UTF8))
            {
                do
                {
                    ByteValue = reader.Read();
                    while (!Read())
                    {
                    }

                    ColumnNumber++;
                    if (ByteValue == '\n')
                    {
                        LineNumber++;
                        ColumnNumber = 1;
                    }
                } while (ByteValue != -1);
                var result = tokens;
                Clean();
                return result;
            }
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
