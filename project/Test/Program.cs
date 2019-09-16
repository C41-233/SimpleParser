using System;
using System.Linq;
using System.Text.RegularExpressions;
using SimpleParser;
using SimpleParser.Grammars;
using SimpleParser.Grammars.Parser;

namespace Test
{
    class Program
    {

        static void Main(string[] args)
        {
            var parse = new MyParser();
            var tokens = parse.ParseFromFile(@"G:\workspace\SimpleParser\test.java");

            foreach (var token in tokens)
            {
                Console.WriteLine(token.Value);
            }

            var grammar = new GrammarBuilder();

            grammar.DefineTerminal("tk_package", false, (int)TokenDefine.Token, "package");
            grammar.DefineTerminal("tk_public", (int)TokenDefine.Token, "public");
            grammar.DefineTerminal("tk_final", (int)TokenDefine.Token, "final");
            grammar.DefineTerminal("tk_class", (int)TokenDefine.Token, "class");
            grammar.DefineTerminal("tk_void", (int)TokenDefine.Token, "void");
            grammar.DefineTerminal("tk_static", (int)TokenDefine.Token, "static");

            grammar.DefineTerminal("tk_semicolon", false, (int)TokenDefine.Semicolon);
            grammar.DefineTerminal("tk_dot", false, (int)TokenDefine.Dot);
            grammar.DefineTerminal("tk_open_brace", false, (int)TokenDefine.OpenBrace);
            grammar.DefineTerminal("tk_close_brace", false, (int)TokenDefine.CloseBrace);

            grammar.DefineTerminal("identifier", (int)TokenDefine.Token, "[a-zA-Z]+[a-zA-Z0-9]*");

            grammar.DefineRoot("class_definition");
            grammar.DefineRoot("package_sentences", "class_definition");

            grammar.DefineNonTerminal("package_sentences", true, "package_sentence_loop");
            grammar.DefineNonTerminal("package_sentence_loop", false, "package_sentence");
            grammar.DefineNonTerminal("package_sentence_loop", false, "package_sentence", "package_sentence_loop");

            grammar.DefineNonTerminal("package_sentence", "tk_package", "package_sentence_body_loop", "tk_semicolon");
            grammar.DefineNonTerminal("package_sentence_body_loop", false, "identifier");
            grammar.DefineNonTerminal("package_sentence_body_loop", false, "identifier", "tk_dot", "package_sentence_body_loop");

            grammar.DefineNonTerminal("class_modifiers", true, "tk_public", "tk_final");
            grammar.DefineNonTerminal("class_definition", "class_modifiers", "tk_class", "identifier", "tk_open_brace", "function_loop", "tk_close_brace");

            grammar.DefineNonTerminal("function_loop", false);
            grammar.DefineNonTerminal("function_loop", false, "function_definition");
            grammar.DefineNonTerminal("function_loop", false, "function_definition", "function_loop");

            grammar.DefineNonTerminal("function_definition", "tk_public", "tk_static", "tk_void", "identifier", "tk_open_brace", "tk_close_brace");

            grammar.Load("a->b");

             var parser = grammar.Build();

            parser.Parse(tokens, new ASTVisitor());
        }
    }

    class ASTVisitor : IASTVisitor
    {

        private int indent = 0;

        public void BeginNode(IASTNode node)
        {
            Pad();
            if (node.IsTerminal)
            {
                Console.WriteLine($"{node.Name} : {node.Value}");
            }
            else
            {
                Console.WriteLine($"<{node.Name}>");
            }

            indent++;
        }

        public void EndNode(IASTNode node)
        {
            indent--;
            if (!node.IsTerminal)
            {
                Pad();
                Console.WriteLine($"</{node.Name}>");
            }

        }

        private void Pad()
        {
            for (var i = 0; i < indent; i++)
            {
                Console.Write("    ");
            }
        }
    }
}
