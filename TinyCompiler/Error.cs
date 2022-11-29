using System;
using System.Collections.Generic;
using System.Text;

namespace TinyCompiler
{
    public class Error
    {
        private bool addedErr = false;
        private Stack<char> brackets = new Stack<char>();
        public List<string> ERRs = new List<string>();
        public void getErr(string err) {
            // "asdasdas err
            if (err[0] == '"')
            {
                ERRs.Add(err + " : missing (\") in the End of String");
            }
        }


        // Bracket Errors Handeler
        // (
        public bool bracketBalance(char bracket)
        {
            
            if (bracket == '(' || bracket == '[' || bracket == '{')
            {
                brackets.Push(bracket);
            }
            else
            {
                if (brackets.Count != 0)
                {
                    char head = brackets.Pop();
                    if ((head == '(' && bracket != ')') || (head == '[' && bracket != ']') || (head == '{' && bracket != '}'))
                    {
                        ERRs.Add("Error in Brackets Balance");
                        addedErr = true;
                        return false;
                    }
                }
                else
                {
                    ERRs.Add("Error in Brackets Balance");
                    addedErr = true;
                    return false;
                }
            }
            return true;
        }
        public void endOfCode()
        {
            if (!addedErr)
            {
                if (brackets.Count != 0)
                {
                    ERRs.Add("Error in Brackets Balance");
                }
            }
        }
    }
}
