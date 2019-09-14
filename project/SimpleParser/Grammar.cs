using System;
using System.Collections;
using System.Collections.Generic;

namespace SimpleParser
{
    public partial class Grammar
    {
        private readonly Dictionary<string, Terminal> terminals = new Dictionary<string, Terminal>();
        private readonly Dictionary<string, List<string[]>> nonTerminals = new Dictionary<string, List<string[]>>();
        private readonly List<string[]> roots = new List<string[]>();

        public void DefineTerminal(string name, int token, Predicate<Token> predicate = null)
        {
            terminals.Add(name, new Terminal
            {
                Token = token,
                Predicate = predicate,
            });
        }

        public void DefineNonTerminal(string name, params string[] symbols)
        {
            if (!nonTerminals.TryGetValue(name, out var list))
            {
                list = new List<string[]>();
                nonTerminals.Add(name, list);
            }
            list.Add((string[]) symbols.Clone());
        }

        public void DefineRoot(params string[] symbols)
        {
            var root = new string[symbols.Length + 1];
            symbols.CopyTo(root, 0);
            roots.Add(root);
        }

        private bool rst;

        public void Parse(IEnumerable<Token> tokens, IASTVisitor visitor)
        {
            var root = new RootTerminalNode(roots);
            using (var stream = new TokenStream(tokens))
            {
                var stack = new Stack<IEnumerator>();
                var offsets = new Stack<int>();
                stack.Push(root.Parse(this, stream));
                offsets.Push(stream.Offset);
                while (stack.Count > 0)
                {
                    var top = stack.Peek();
                    if (top.MoveNext())
                    {
                        var obj = top.Current;
                        if (obj is IEnumerator enumerator)
                        {
                            stack.Push(enumerator);
                            offsets.Push(stream.Offset);
                        }
                        else if (obj is bool b)
                        {
                            rst = b;
                            if (!b)
                            {
                                var offset = offsets.Peek();
                                stream.Reset(offset);
                            }

                            stack.Pop();
                            offsets.Pop();
                            if (top is IDisposable disposable)
                            {
                                disposable.Dispose();
                            }
                        }
                    }
                    else
                    {
                        stack.Pop();
                        offsets.Pop();
                        if (top is IDisposable disposable)
                        {
                            disposable.Dispose();
                        }
                    }
                }

                if (!rst)
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

        private bool TryParseNonTerminal(string name, out List<string[]> symbols)
        {
            return nonTerminals.TryGetValue(name, out symbols);
        }

        private bool TryParseTerminal(string name, out Terminal terminal)
        {
            return terminals.TryGetValue(name, out terminal);
        }
    }
}
