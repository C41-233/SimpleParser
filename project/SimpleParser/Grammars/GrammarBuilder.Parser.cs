using System;
using System.Collections.Generic;
using SimpleParser.Grammars.Meta;

namespace SimpleParser.Grammars
{
    public sealed partial class GrammarBuilder
    {

        private struct ParseNode
        {
            public MetaTokenType Type;
            public string Value;
        }

        public int ParseNonTerminal(string name, bool isExplicit, string expression)
        {
            var parser = new MetaParser();
            var stack = new Stack<ParseNode>();
            foreach (var token in parser.Parse(expression))
            {
                ParseInternal(stack, token);
            }

            var remain = new List<ParseNode>();
            while (stack.Count > 0)
            {
                remain.Add(stack.Pop());
            }
            ParseGroup(remain, name, true);
            return 0;
        }

        private void ParseInternal(Stack<ParseNode> stack, MetaToken token)
        {
            switch (token.Type)
            {
                case MetaTokenType.LP:
                case MetaTokenType.Symbol:
                    stack.Push(new ParseNode
                    {
                        Type = token.Type,
                        Value = token.Value,
                    });
                    break;
                case MetaTokenType.RP:
                    ParseGroup(stack);
                    break;
                case MetaTokenType.ZeroOrMore:
                    ParseZeroOrMore(stack);
                    break;
            }
        }

        private void ParseGroup(Stack<ParseNode> stack)
        {
            var list = new List<ParseNode>();
            while (true)
            {
                var top = stack.Pop();
                if (top.Type == MetaTokenType.LP)
                {
                    break;
                }
                list.Add(top);
            }

            var name = $"${id++}";
            stack.Push(new ParseNode
            {
                Type = MetaTokenType.Symbol,
                Value = name,
            });
            ParseGroup(list, name, false);
        }

        private void ParseGroup(List<ParseNode> list, string name, bool isExplicit)
        {
            var symbols = new List<string>();
            for (var i = list.Count - 1; i >= 0; i--)
            {
                var node = list[i];
                if (node.Type == MetaTokenType.Symbol)
                {
                    symbols.Add(node.Value);
                }
                else if (node.Type == MetaTokenType.Or)
                {
                    DefineNonTerminal(name, isExplicit, symbols.ToArray());
                    symbols.Clear();
                }
            }

            if (symbols.Count > 0)
            {
                DefineNonTerminal(name, isExplicit, symbols.ToArray());
                symbols.Clear();
            }
        }

        private void ParseZeroOrMore(Stack<ParseNode> stack)
        {
            var name = $"${id++}";
            var element = stack.Pop();
            stack.Push(new ParseNode
            {
                Type = MetaTokenType.Symbol,
                Value = name,
            });
            DefineNonTerminal(name, false);
            DefineNonTerminal(name, false, element.Value, name);
        }

    }
}
