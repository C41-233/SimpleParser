using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Runtime.Remoting.Messaging;

namespace SimpleParser
{
    public class Grammar_old
    {

        private class Terminal
        {
            public int Token;
            public Predicate<Token> Predicate;

            public bool Match(Token token)
            {
                if (token.Type != Token)
                {
                    return false;
                }

                if (Predicate != null && !Predicate(token))
                {
                    return false;
                }

                return true;
            }
        }


        private readonly Dictionary<string, Terminal> terminals = new Dictionary<string, Terminal>();
        private readonly Dictionary<string, List<string[]>> nonTerminals = new Dictionary<string, List<string[]>>();
        private readonly List<string[]> roots = new List<string[]>();

        public void DefineTerminal(string name, int token, Predicate<Token> predicate = null)
        {
            Console.WriteLine($"{name} -> {token}");
            terminals.Add(name, new Terminal
            {
                Token = token,
                Predicate = predicate,
            });
        }

        public void DefineNonTerminal(string name, params string[] symbols)
        {
            Console.WriteLine($"{name} -> {string.Join(" ", symbols)}");
            if (!nonTerminals.TryGetValue(name, out var list))
            {
                list = new List<string[]>();
                nonTerminals.Add(name, list);
            }
            list.Add(symbols);
        }

        public void DefineRoot(params string[] symbols)
        {
            Console.WriteLine($"S -> {string.Join(" ", symbols)}");
            roots.Add(symbols);
        }

        private enum NodeLife
        {
            Init,
            Open,
            Close,
        }

        private abstract class Node
        {

            public abstract void Clear();

            private static int indent;

            public string name { get; }

            public NodeLife life = NodeLife.Init;

            public abstract void Resolve();

            public bool Walk(Grammar_old grammarOld, TokenStream stream)
            {
                indent++;
                var offset = stream.Offset;
                var rst = Walk0(grammarOld, stream);
                if(!rst)
                {
                    stream.Reset(offset);
                }

                indent--;
                return rst;
            }

            public string pad
            {
                get
                {
                    string a = "";
                    for (var i=0; i<=indent-1; i++)
                    {
                        a += "----";
                    }

                    return a;
                }
            }

            public abstract bool Walk0(Grammar_old grammarOld, TokenStream stream);

            protected Node(string name)
            {
                this.name = name;
            }

        }

        private class RootNode : NonTerminalNode
        {

            public RootNode(List<string[]> symbols) : base("S", symbols)
            {
            }

        }

        private class NonTerminalNode : Node
        {
            private List<NonTerminalPath> definitions;

            private int index;

            private List<string[]> symbols;

            public NonTerminalNode(string name, List<string[]> symbols) : base(name)
            {
                this.symbols = symbols;
            }

            public override void Clear()
            {
                index = 0;
                life = NodeLife.Init;
                foreach (var definition in definitions)
                {
                    definition.Clear();
                }
            }

            public override void Resolve()
            {
                definitions[index].Resolve();
            }

            public override bool Walk0(Grammar_old grammarOld, TokenStream stream)
            {
                Debug.Assert(life != NodeLife.Close);
                Console.WriteLine($"{pad} Walk node {name} at {stream}");
                if (life == NodeLife.Init)
                {
                    definitions = new List<NonTerminalPath>();
                    for (var i = 0; i < symbols.Count; i++)
                    {
                        var path = symbols[i];
                        definitions.Add(new NonTerminalPath(name, path, i));
                    }

                    life = NodeLife.Open;
                }

                while (index < definitions.Count)
                {
                    var rst = definitions[index].Walk(grammarOld, stream);
                    if (definitions[index].life == NodeLife.Close)
                    {
                        index++;
                    }

                    if (index >= definitions.Count)
                    {
                        life = NodeLife.Close;
                    }
                    if (rst)
                    {
                        return true;
                    }
                }

                return false;
            }

        }

        private class NonTerminalPath : Node
        {

            private readonly string[] path;
            private int N;

            public NonTerminalPath(string name, string[] path, int i) : base(name)
            {
                this.path = path;
                this.N = i;
            }

            private List<Node> list = new List<Node>(); //元素栈
            private Stack<int> offsets = new Stack<int>();

            public override void Clear()
            {
                life = NodeLife.Init;
                list.Clear();
                offsets.Clear();
            }

            public override void Resolve()
            {
                Console.WriteLine($"{name} -> {string.Join(" ", path)}");
                foreach (var node in list)
                {
                    node.Resolve();
                }
            }

            public override bool Walk0(Grammar_old grammarOld, TokenStream stream)
            {
                Debug.Assert(life != NodeLife.Close);
                Console.WriteLine($"{pad} Walk path {name}[{N}]");
                if (life == NodeLife.Init)
                {
                    life = NodeLife.Open;
                    for (var i=0; i<path.Length; i++)
                    {
                        list.Add(CreateNode(grammarOld, i));
                    }

                    var n = 0;
                    while (n < list.Count)
                    {
                        offsets.Push(stream.Offset);
                        var rst = list[n].Walk(grammarOld, stream);
                        if (rst)
                        {
                            n++;
                            continue;
                        }

                        offsets.Pop();
                        n = Return(n-1, stream);
                        if (n < 0)
                        {
                            life = NodeLife.Close;
                            return false;
                        }

                        for (var i = n + 1; i < list.Count; i++)
                        {
                            list[i].Clear();
                        }
                    }

                    return true;
                }
                if (life == NodeLife.Open)
                {
                    var n = Return(list.Count - 1, stream);
                    if (n < 0)
                    {
                        life = NodeLife.Close;
                        return false;
                    }

                    for (var i = n + 1; i < list.Count; i++)
                    {
                        list[i].Clear();
                    }

                    while (n < list.Count)
                    {
                        offsets.Push(stream.Offset);
                        var rst = list[n].Walk(grammarOld, stream);
                        if (rst)
                        {
                            n++;
                            continue;
                        }

                        n = Return(n, stream);
                        if (n < 0)
                        {
                            life = NodeLife.Close;
                            return false;
                        }

                        for (var i = n + 1; i < list.Count; i++)
                        {
                            list[i].Clear();
                        }
                    }

                    return true;
                }

                return false;
            }

            private int Return(int n, TokenStream stream)
            {
                for (var i = n; i >= 0; i--)
                {
                    var offset = offsets.Pop();
                    if (list[i].life != NodeLife.Close)
                    {
                        stream.Reset(offset);
                        return i;
                    }
                }

                return -1;
            }

            private Node CreateNode(Grammar_old grammarOld, int i)
            {
                if (grammarOld.TryParseNonTerminal(path[i], out var symbols))
                {
                    return new NonTerminalNode(path[i], symbols);
                }

                if (grammarOld.TryParseTerminal(path[i], out var terminal))
                {
                    return new TerminalNode(path[i], terminal);
                }

                return null;
            }

        }

        private class TerminalNode : Node
        {

            private Terminal terminal;

            public TerminalNode(string name, Terminal terminal) : base(name)
            {
                this.terminal = terminal;
                this.life = NodeLife.Close;
            }

            public override void Clear()
            {
            }

            public override void Resolve()
            {
            }

            public override bool Walk0(Grammar_old grammarOld, TokenStream stream)
            {
                Console.WriteLine($"{pad} Walk terminal {name} at {stream}");
                var token = stream.Next(pad);
                if (token == null)
                {
                    return false;
                }

                return terminal.Match(token);
            }

        }

        public void Parse(List<Token> tokens)
        {
            Console.WriteLine();
            var stream = new TokenStream(tokens);
            var root = new RootNode(roots);
            var rst = root.Walk(this, stream);

            Console.WriteLine();
            Console.WriteLine(rst);

            Debug.Assert(stream.EOF);

            Console.WriteLine();
            root.Resolve();
        }


        private bool TryParseNonTerminal(string name, out List<string[]> symbols)
        {
            return nonTerminals.TryGetValue(name, out symbols);
        }

        private bool TryParseTerminal(string name, out Terminal terminal)
        {
            return terminals.TryGetValue(name, out terminal);
        }

        private class TokenStream
        {

            private List<Token> tokens;
            public bool EOF => Offset >= tokens.Count;

            public int Offset { get; private set; }

            public TokenStream(List<Token> tokens)
            {
                this.tokens = tokens;
            }

            public void Reset(int offset)
            {
               // Console.WriteLine($"reset {tokens[offset].Value}");
                this.Offset = offset;
            }

            public Token Next(string pad)
            {
                if (Offset >= tokens.Count)
                {
                    Console.WriteLine($"{pad} eat EOF");
                    return null;
                }

                Console.WriteLine($"{pad} eat {tokens[Offset].Value}");
                return tokens[Offset++];
            }

            public override string ToString()
            {
                return tokens[Offset].Value;
            }
        }

    }
}
