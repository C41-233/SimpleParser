using System;
using System.Collections.Generic;

namespace SimpleParser.Grammars.Parser
{
    internal class TokenStream : IDisposable
    {

        private readonly List<Token> buffers = new List<Token>();
        private IEnumerator<Token> enumerator;

        public int Offset { get; private set; }

        public Token MostInputToken { get; private set; }

        private int maxOffset;

        public TokenStream(IEnumerable<Token> tokens)
        {
            enumerator = tokens.GetEnumerator();
        }

        public void Reset(int offset)
        {
            Offset = offset;
        }

        public Token Next()
        {
            var token = DoNext();
            if (Offset >= maxOffset)
            {
                maxOffset = Offset;
                MostInputToken = token;
            }
            return token;
        }

        private Token DoNext()
        {
            if (enumerator == null)
            {
                return null;
            }

            if (Offset < buffers.Count)
            {
                return buffers[Offset++];
            }

            if (enumerator.MoveNext())
            {
                Offset++;
                var token = enumerator.Current;
                buffers.Add(token);
                return token;
            }

            if (enumerator is IDisposable disposable)
            {
                disposable.Dispose();
            }

            enumerator = null;

            return null;
        }

        void IDisposable.Dispose()
        {
            enumerator?.Dispose();
        }
    }
}