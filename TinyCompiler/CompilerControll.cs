using System;
using System.Collections.Generic;
using System.Text;

namespace TinyCompiler
{
    public static class CompilerControll
    {
        public static Scanner scanner = new Scanner();
        public static Parser Jason_Parser = new Parser();
        public static Node treeroot;
        public static void start(string code)
        {
            scanner.StartScanner(code);
            Jason_Parser.StartParsing(scanner.tokens);
            treeroot = Jason_Parser.root;
        }
    }
}
