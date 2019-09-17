using System;
using System.Collections.Generic;
using SimpleParser.Grammars.Meta;
using SimpleParser.Grammars.Parser;
using SimpleParser.Grammars.Parser.LLn;
using SimpleParser.Grammars.Parser.LLn.Tokens;

namespace SimpleParser.Grammars
{
    public sealed partial class GrammarBuilder
    {
        private struct Terminal
        {
            public int Id;
            public int Token;
            public string Pattern;
            public bool Explicit;
        }

        private struct NonTerminalPath
        {
            public int Id;
            public bool Explicit;
            public string[] Path;
        }

        private struct NonTerminalPair
        {
            public NonTerminalToken Token;
            public List<NonTerminalPath> Paths;
        }

        private readonly Dictionary<string, Terminal> terminals = new Dictionary<string, Terminal>();
        private readonly Dictionary<string, List<NonTerminalPath>> nonTerminals = new Dictionary<string, List<NonTerminalPath>>();
        private readonly List<NonTerminalPath> roots = new List<NonTerminalPath>();

        private int id;

        public int DefineTerminal(string name, int token, string pattern = null)
        {
            return DefineTerminal(name, true, token, pattern);
        }

        public int DefineTerminal(string name, bool isExplicit, int token, string pattern = null)
        {
            if (terminals.ContainsKey(name))
            {
                throw GrammarDefineException.DuplicateTerminalDefinition(name);
            }
            terminals.Add(name, new Terminal
            {
                Id = id,
                Token = token,
                Pattern = pattern,
                Explicit = isExplicit,
            });
            return id++;
        }

        public int DefineNonTerminal(string name, params string[] symbols)
        {
            return DefineNonTerminal(name, true, symbols);
        }

        public int DefineNonTerminal(string name, bool isExplicit, params string[] symbols)
        {
            if (!nonTerminals.TryGetValue(name, out var list))
            {
                list = new List<NonTerminalPath>();
                nonTerminals.Add(name, list);
            }
            list.Add(new NonTerminalPath
            {
                Id = id,
                Explicit = isExplicit,
                Path = (string[])symbols.Clone(),
            });
            return id++;
        }

        public int ParseNonTerminal(string name, bool isExplicit, string expression)
        {
            var parser = new MetaParser();
            foreach (var token in parser.Parse(expression))
            {
                Console.WriteLine(token.Value);
            }
            Console.WriteLine();
            return 0;
        }

        public int DefineRoot(params string[] symbols)
        {
            roots.Add(new NonTerminalPath
            {
                Id = id,
                Path = (string[])symbols.Clone(),
            });
            return id++;
        }

        public IGrammarParser Build()
        {
            var nameToToken = new Dictionary<string, LLnToken>();
            var nonTerminalTokens = new List<NonTerminalPair>(roots.Count + nonTerminals.Count);

            foreach (var kv in terminals)
            {
                nameToToken.Add(
                    kv.Key, 
                    new TerminalToken(
                        kv.Value.Id, 
                        kv.Value.Explicit,
                        kv.Key,
                        kv.Value.Token, 
                        kv.Value.Pattern
                    )
                );
            }

            foreach (var kv in nonTerminals)
            {
                var token = new NonTerminalToken(kv.Key, kv.Value.Count);
                nameToToken.Add(kv.Key, token);
                nonTerminalTokens.Add(new NonTerminalPair
                {
                    Token = token,
                    Paths = kv.Value,
                });
            }

            foreach (var pair in nonTerminalTokens)
            {
                var token = pair.Token;
                foreach (var path in pair.Paths)
                {
                    var len = path.Path.Length;
                    var tokens = new LLnToken[len];
                    for (int i=0; i<len; i++)
                    {
                        if (!nameToToken.TryGetValue(path.Path[i], out var refToken))
                        {
                            throw GrammarDefineException.NonTerminalRefTokenNotFound(token.Name, path.Path[i]);
                        }
                        tokens[i] = refToken;
                    }

                    token.AddPath(path.Id, path.Explicit, tokens);
                }
            }

            var rootToken = new RootToken(roots.Count);
            foreach (var path in roots)
            {
                var len = path.Path.Length;
                var tokens = new LLnToken[len + 1];
                for (int i = 0; i < len; i++)
                {
                    if (!nameToToken.TryGetValue(path.Path[i], out var refToken))
                    {
                        throw GrammarDefineException.NonTerminalRefTokenNotFound(rootToken.Name, path.Path[i]);
                    }
                    tokens[i] = refToken;
                }

                tokens[len] = EOFToken.Instance;

                rootToken.AddPath(path.Id, false, tokens);
            }

            var parser = new LLnParser(rootToken);
            return parser;
        }

    }

}
