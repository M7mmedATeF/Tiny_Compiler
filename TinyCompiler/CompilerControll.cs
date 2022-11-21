using System;
using System.Collections.Generic;
using System.Text;

namespace TinyCompiler
{
    public static class CompilerControll
    {
        public static Scanner scanner = new Scanner();
        public static List<Token> tokens;
        public static List<string> ERRs;

        public static void start(string code)
        {
            scanner.StartScanner(code);
        }
    }
}
