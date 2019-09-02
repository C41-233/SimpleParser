using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SimpleParser;

namespace Test
{
    class Program
    {

        static void Main(string[] args)
        {
            var parse = new LexicalParser();
            var tokens = parse.Parse(@"G:\workspace\SimpleParser\test.java");

            foreach (var token in tokens)
            {
                Console.WriteLine($"{(TokenDefine)token.Type} {token.Value}");
            }

            Console.WriteLine();

            var grammar = new Grammar();

            grammar.DefineTerminal("tk_package", (int) TokenDefine.Token, token => token.Value == "package");
            grammar.DefineTerminal("tk_public", (int)TokenDefine.Token, token => token.Value == "public");
            grammar.DefineTerminal("tk_final", (int)TokenDefine.Token, token => token.Value == "final");
            grammar.DefineTerminal("tk_class", (int)TokenDefine.Token, token => token.Value == "class");

            grammar.DefineTerminal("tk_semicolon", (int)TokenDefine.Semicolon);
            grammar.DefineTerminal("tk_dot", (int)TokenDefine.Dot);
            grammar.DefineTerminal("tk_open_brace", (int)TokenDefine.OpenBrace);
            grammar.DefineTerminal("tk_close_brace", (int)TokenDefine.CloseBrace);

            grammar.DefineTerminal("identifier", (int)TokenDefine.Token, token => Regex.IsMatch(token.Value, "[a-zA-Z]+[a-zA-Z0-9]*"));

            grammar.DefineRoot("package_sentence", "class_definition");
            grammar.DefineRoot("class_definition");

            grammar.DefineNonTerminal("package_sentence", "tk_package", "package_sentence_body", "tk_semicolon");
            grammar.DefineNonTerminal("package_sentence_body", "identifier");
            grammar.DefineNonTerminal("package_sentence_body", "identifier", "tk_dot", "package_sentence_body");

            grammar.DefineNonTerminal("class_definition", "tk_public", "tk_final", "tk_class", "identifier", "tk_open_brace", "tk_close_brace");

            grammar.Parse(tokens);
        }
    }
}
