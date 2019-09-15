using System.Collections;
using System.Collections.Generic;
using SimpleParser.Grammars.Parser.LLn.Nodes;
using SimpleParser.Grammars.Parser.LLn.Tokens;

namespace SimpleParser.Grammars.Parser.LLn
{
    internal partial class LLnParser : IGrammarParser
    {

        private readonly RootToken rootToken;

        public LLnParser(RootToken root)
        {
            this.rootToken = root;
        }

        private struct Frame
        {
            public IEnumerator<ParseReport> Enumerator;
            public int Offset;

            public bool MoveNext() => Enumerator.MoveNext();
            public IEnumerator<ParseReport> Current => Enumerator.Current.Next;

            public void Dispose()
            {
                Enumerator.Dispose();
            }
        }

        public void Parse(IEnumerable<Token> tokens, IASTVisitor visitor)
        {
            var ctx = new ParserContext(this);
            var stack = new Stack<Frame>();
            var root = new RootNode(rootToken);
            using (var stream = new TokenStream(tokens))
            {
                stack.Push(new Frame
                {
                    Enumerator = root.Generate(ctx, stream),
                    Offset = stream.Offset,
                });

                while (stack.Count > 0)
                {
                    var frame = stack.Peek();
                    if (frame.MoveNext())
                    {
                        var next = frame.Current;
                        stack.Push(new Frame
                        {
                            Enumerator = next,
                            Offset = stream.Offset,
                        });
                    }
                    else
                    {
                        if (!ctx.PeekResult())
                        {
                            stream.Reset(frame.Offset);
                        }
                        stack.Pop();
                        frame.Dispose();
                    }
                }

                if (!ctx.PopResult())
                {
                    var token = stream.MostInputToken;
                    if (token == null)
                    {
                        throw new ParseException("unexpected token <eof>");
                    }
                    throw new ParseException($"unexpected token {token.Value} at ({token.Line},{token.Column})");
                }
            }

            root.Visit(visitor);
        }
    }

}
