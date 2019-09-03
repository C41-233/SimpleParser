﻿using System;
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
            roots.Add((string[]) symbols.Clone());
        }

        public void Parse(IEnumerable<Token> tokens)
        {

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