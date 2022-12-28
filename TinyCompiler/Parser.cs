using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace TinyCompiler
{

    public class Node
    {
        public List<Node> Children = new List<Node>();

        public string Name;
        public Node(string N)
        {
            this.Name = N;
        }
    }

    public class Parser
    {
        int tokensIndex;
        List<Token> TokenStream;
        public Node root;

        /**
         * Starting Code and get Entry Point (Main Function)
         */
        public Node StartParsing(List<Token> TokenStream)
        {
            this.tokensIndex = 0;
            this.TokenStream = TokenStream;
            root = new Node("Root");
            root.Children.Add(Program());
            return root;
        }

        private Node Program()
        {
            Node program = new Node("Program");
            program.Children.Add(Func_States());
            if (tokensIndex < TokenStream.Count())
                program.Children.Add(Main_Func());
            else
                CompilerControll.scanner.errHandel.ERRs.Add("The Program Must have The Entry Point (MAIN_FN Function)!!\r\n");
                    
            return program;
        }

        /**
         * --------------------- Function's Structure ---------------------
         */
        private Node Func_States()
        {
            Node functionStatements = new Node("Function Statements");
            if (tokensIndex < TokenStream.Count() && isStartWithDatatype() && Token_Class.MAIN_FN != TokenStream[tokensIndex+1].tok)
            {
                functionStatements.Children.Add(Func_State());
                functionStatements.Children.Add(Func_States());
                return functionStatements;
            }

            return null;
        }

        private Node Func_State()
        {
            Node functionStatement = new Node("Function Statement");
            functionStatement.Children.Add(Func_Declr());
            functionStatement.Children.Add(Func_Body());

            return functionStatement;
        }

        private Node Main_Func()
        {
            Node MAIN_FNFunction = new Node("MAIN_FN Function");
            MAIN_FNFunction.Children.Add(Datatype());
            MAIN_FNFunction.Children.Add(match(Token_Class.MAIN_FN));
            MAIN_FNFunction.Children.Add(match(Token_Class.LEFT_BRACKET));
            MAIN_FNFunction.Children.Add(match(Token_Class.RIGHT_BRACKET));
            MAIN_FNFunction.Children.Add(Func_Body());

            return MAIN_FNFunction;
        }

        private Node Func_Declr()
        {
            Node functionDeclaration = new Node("Function Declaration");
            functionDeclaration.Children.Add(Datatype());
            functionDeclaration.Children.Add(FN_Name());
            functionDeclaration.Children.Add(match(Token_Class.LEFT_BRACKET));
            if (isStartWithDatatype()) // Param (n
            {
                functionDeclaration.Children.Add(Param());
                functionDeclaration.Children.Add(Other_Param());
            }
            functionDeclaration.Children.Add(match(Token_Class.RIGHT_BRACKET));

            return functionDeclaration;
        }

        private Node Param()
        {
            Node parameter = new Node("Parameter");
            parameter.Children.Add(Datatype());
            parameter.Children.Add(match(Token_Class.IDENTIFIER));

            return parameter;
        }

        private Node Other_Param()
        { // int x,sring s,
            Node multiParameter = new Node("Multi Parameter");
            if (tokensIndex < TokenStream.Count() && Token_Class.COMMA == TokenStream[tokensIndex].tok)
            {
                multiParameter.Children.Add(match(Token_Class.COMMA));
                multiParameter.Children.Add(Param());
                multiParameter.Children.Add(Other_Param());
                return multiParameter;
            }

            return null;
        }

        private Node Func_Body()
        {
            Node functionBody = new Node("Function Body");
            functionBody.Children.Add(match(Token_Class.LEFT_CBRACKET));
            functionBody.Children.Add(Statements());
            functionBody.Children.Add(Return_State());
            functionBody.Children.Add(match(Token_Class.RIGHT_CBRACKET));

            return functionBody;
        }

        /**
         * --------------------- Statement's Structure ---------------------
         */
        private Node Statements()
        {
            Node statements = new Node("Statements");
            if (tokensIndex < TokenStream.Count() && (Token_Class.IDENTIFIER == TokenStream[tokensIndex].tok
                || isStartWithDatatype() || Token_Class.WRITE == TokenStream[tokensIndex].tok || Token_Class.READ == TokenStream[tokensIndex].tok
                || Token_Class.IF == TokenStream[tokensIndex].tok || Token_Class.REPEAT == TokenStream[tokensIndex].tok))
            {
                statements.Children.Add(Statement());
                statements.Children.Add(Statements());
                return statements;
            }

            return null;
        }

        private Node Statement()
        {
            Node statement = new Node("Statement");
            if (tokensIndex < TokenStream.Count())
            {
                if (Token_Class.IDENTIFIER == TokenStream[tokensIndex].tok && Token_Class.ASSIGNMENT_OP == TokenStream[tokensIndex + 1].tok)
                {
                    statement.Children.Add(Assign_State());
                    statement.Children.Add(match(Token_Class.SIMICOLON));
                }
                else if (Token_Class.RETURN == TokenStream[tokensIndex].tok)
                    statement.Children.Add(Return_State());
                else if (isStartWithDatatype())
                    statement.Children.Add(Declr_State());
                else if (Token_Class.WRITE == TokenStream[tokensIndex].tok)
                    statement.Children.Add(Write_State());
                else if (Token_Class.READ == TokenStream[tokensIndex].tok)
                    statement.Children.Add(Read_State());
                else if (Token_Class.IDENTIFIER == TokenStream[tokensIndex].tok && Token_Class.LEFT_BRACKET == TokenStream[tokensIndex + 1].tok)
                {
                    statement.Children.Add(FN_Call());
                    statement.Children.Add(match(Token_Class.SIMICOLON));
                } 
                else if (Token_Class.IF == TokenStream[tokensIndex].tok)
                    statement.Children.Add(IF_State());
                else if (Token_Class.REPEAT == TokenStream[tokensIndex].tok)
                    statement.Children.Add(Repeat_State());
                else tokensIndex++;
            }
            return statement;
        }
        
        private Node Datatype()
        {
            Node datatype = new Node("Datatype");
            if (tokensIndex < TokenStream.Count())
            {
                if (Token_Class.INT == TokenStream[tokensIndex].tok)
                    datatype.Children.Add(match(Token_Class.INT));
                else if (Token_Class.FLOAT == TokenStream[tokensIndex].tok)
                    datatype.Children.Add(match(Token_Class.FLOAT));
                else if (Token_Class.STRING == TokenStream[tokensIndex].tok)
                    datatype.Children.Add(match(Token_Class.STRING));
            }
            return datatype;
        }

        private Node FN_Name()
        {
            Node functionName = new Node("Function Name");  
            functionName.Children.Add(match(Token_Class.IDENTIFIER));

            return functionName;
        }

        private Node Assign_State()
        {
            Node assignmentStatement = new Node("Assignment Statement");
            assignmentStatement.Children.Add(match(Token_Class.IDENTIFIER));
            assignmentStatement.Children.Add(match(Token_Class.ASSIGNMENT_OP));
            assignmentStatement.Children.Add(Exp());

            return assignmentStatement;
        }

        private Node Declr_State()
        {
            Node declarationStatement = new Node("Declaration Statement");
            declarationStatement.Children.Add(Datatype());
            if (tokensIndex < TokenStream.Count())
            {
                if (Token_Class.IDENTIFIER == TokenStream[tokensIndex].tok && Token_Class.ASSIGNMENT_OP == TokenStream[tokensIndex + 1].tok)
                    declarationStatement.Children.Add(Assign_State());
                else if (Token_Class.IDENTIFIER == TokenStream[tokensIndex].tok)
                    declarationStatement.Children.Add(match(Token_Class.IDENTIFIER));
            }
            declarationStatement.Children.Add(Declr_States());
            declarationStatement.Children.Add(match(Token_Class.SIMICOLON));

            return declarationStatement;
        }

        private Node Declr_States()
        {
            Node multipleDeclarations = new Node("Multiple Declarations");
            if (tokensIndex < TokenStream.Count() && Token_Class.COMMA == TokenStream[tokensIndex].tok)
            {
                multipleDeclarations.Children.Add(match(Token_Class.COMMA));
                if (Token_Class.IDENTIFIER == TokenStream[tokensIndex].tok && Token_Class.ASSIGNMENT_OP == TokenStream[tokensIndex + 1].tok)
                    multipleDeclarations.Children.Add(Assign_State());
                else if (Token_Class.IDENTIFIER == TokenStream[tokensIndex].tok)
                    multipleDeclarations.Children.Add(match(Token_Class.IDENTIFIER));

                multipleDeclarations.Children.Add(Declr_States());
                return multipleDeclarations;
            }

            return null;
        }

        private Node Read_State()
        {
            Node READStatement = new Node("READ Statement");
            READStatement.Children.Add(match(Token_Class.READ));
            READStatement.Children.Add(match(Token_Class.IDENTIFIER));
            READStatement.Children.Add(match(Token_Class.SIMICOLON));

            return READStatement;
        }

        private Node Write_State()
        {
            Node WRITEStatement = new Node("WRITE Statement");
            WRITEStatement.Children.Add(match(Token_Class.WRITE));
            if (tokensIndex < TokenStream.Count() && Token_Class.ENDL == TokenStream[tokensIndex].tok)
                WRITEStatement.Children.Add(match(Token_Class.ENDL));
            else
                WRITEStatement.Children.Add(Exp());

            WRITEStatement.Children.Add(match(Token_Class.SIMICOLON));

            return WRITEStatement;
        }
        
        private Node FN_Call()
        {
            Node functionCall = new Node("Function Call");
            functionCall.Children.Add(match(Token_Class.IDENTIFIER));
            functionCall.Children.Add(match(Token_Class.LEFT_BRACKET));
            if (tokensIndex < TokenStream.Count())
            {
                if (Token_Class.IDENTIFIER == TokenStream[tokensIndex].tok)
                    functionCall.Children.Add(match(Token_Class.IDENTIFIER));
                else if (Token_Class.CONSTANT == TokenStream[tokensIndex].tok)
                    functionCall.Children.Add(match(Token_Class.CONSTANT));
            }
            functionCall.Children.Add(Other_IDs());
            functionCall.Children.Add(match(Token_Class.RIGHT_BRACKET));

            return functionCall;
        }

        private Node Other_IDs()
        {
            Node multiIdentifiers = new Node("Multi Identifiers");
            if (tokensIndex < TokenStream.Count() && Token_Class.COMMA == TokenStream[tokensIndex].tok)
            {
                multiIdentifiers.Children.Add(match(Token_Class.COMMA));
                if (tokensIndex < TokenStream.Count())
                {
                    if (Token_Class.IDENTIFIER == TokenStream[tokensIndex].tok)
                        multiIdentifiers.Children.Add(match(Token_Class.IDENTIFIER));
                    else
                        multiIdentifiers.Children.Add(match(Token_Class.CONSTANT));
                }
                multiIdentifiers.Children.Add(Other_IDs());
                return multiIdentifiers;
            }

            return null;
        }

        private Node IF_State()
        {
            Node ifStatement = new Node("If Statement");
            ifStatement.Children.Add(match(Token_Class.IF));
            ifStatement.Children.Add(Cond_State());
            ifStatement.Children.Add(match(Token_Class.THEN));
            ifStatement.Children.Add(Other_IF_States());
            if (tokensIndex < TokenStream.Count())
            {
                if (Token_Class.ELSEIF == TokenStream[tokensIndex].tok)
                    ifStatement.Children.Add(ElseIF_State());
                else if (Token_Class.ELSE == TokenStream[tokensIndex].tok)
                    ifStatement.Children.Add(Else_State());
                else
                    ifStatement.Children.Add(match(Token_Class.END));
            }

            return ifStatement;
        }
        
        private Node ElseIF_State()
        {
            Node elseIfStatement = new Node("ElseIf Statement");
            elseIfStatement.Children.Add(match(Token_Class.ELSEIF));
            elseIfStatement.Children.Add(Cond_State());
            elseIfStatement.Children.Add(match(Token_Class.THEN));
            elseIfStatement.Children.Add(Other_IF_States());
            if (tokensIndex < TokenStream.Count())
            {
                if (Token_Class.ELSEIF == TokenStream[tokensIndex].tok)
                    elseIfStatement.Children.Add(ElseIF_State());
                else if (Token_Class.ELSE == TokenStream[tokensIndex].tok)
                    elseIfStatement.Children.Add(Else_State());
                else
                    elseIfStatement.Children.Add(match(Token_Class.END));
            }

            return elseIfStatement;
        }

        private Node Else_State()
        {
            Node elseStatement = new Node("Else Statement");
            elseStatement.Children.Add(match(Token_Class.ELSE));
            elseStatement.Children.Add(Other_IF_States());
            elseStatement.Children.Add(match(Token_Class.END));

            return elseStatement;
        }

        private Node Other_IF_States()
        {
            Node multipleStatements = new Node("Multiple Statements");
            multipleStatements.Children.Add(Statement());
            multipleStatements.Children.Add(Other_IF_State());

            return multipleStatements;
        }

        private Node Other_IF_State()
        {
            Node multipleStatement = new Node("Multiple Statement");
            if (tokensIndex < TokenStream.Count() && (Token_Class.IDENTIFIER == TokenStream[tokensIndex].tok
                || isStartWithDatatype() || Token_Class.WRITE == TokenStream[tokensIndex].tok || Token_Class.READ == TokenStream[tokensIndex].tok
                || Token_Class.IF == TokenStream[tokensIndex].tok || Token_Class.REPEAT == TokenStream[tokensIndex].tok || Token_Class.RETURN == TokenStream[tokensIndex].tok))
            {
                multipleStatement.Children.Add(Statement());
                multipleStatement.Children.Add(Other_IF_State());
                return multipleStatement;
            }

            return null;
        }

        private Node Repeat_State()
        {
            Node REPEATStatement = new Node("REPEAT Statement");
            REPEATStatement.Children.Add(match(Token_Class.REPEAT));
            REPEATStatement.Children.Add(Other_IF_States());
            REPEATStatement.Children.Add(match(Token_Class.UNTIL));
            REPEATStatement.Children.Add(Cond_State());

            return REPEATStatement;
        }

        private Node Return_State()
        {
            Node returnStatement = new Node("Return Statement");
            returnStatement.Children.Add(match(Token_Class.RETURN));
            returnStatement.Children.Add(Exp());
            returnStatement.Children.Add(match(Token_Class.SIMICOLON));

            return returnStatement;
        }

        /**
         * --------------------- Expression's Structure ---------------------
         * --------------------- Equation & Condition ---------------------
         */
        private Node Exp() 
        {
            Node expression = new Node("Expression");
            if (tokensIndex < TokenStream.Count())
            {
                if (Token_Class.STRING_VAL == TokenStream[tokensIndex].tok)
                    expression.Children.Add(match(Token_Class.STRING_VAL));
                else if ((isStartWithTerm() && isStartWithOperation(tokensIndex + 1))
                        || (Token_Class.LEFT_BRACKET == TokenStream[tokensIndex].tok))
                    expression.Children.Add(Equation());
                else if (isStartWithTerm())
                    expression.Children.Add(Term());
            }
            return expression;
        }
        
        private Node Term()
        {
            Node term = new Node("Term");
            if (tokensIndex < TokenStream.Count())
            {
                if (Token_Class.CONSTANT == TokenStream[tokensIndex].tok)
                    term.Children.Add(match(Token_Class.CONSTANT));
                else if (Token_Class.IDENTIFIER == TokenStream[tokensIndex].tok && Token_Class.LEFT_BRACKET == TokenStream[tokensIndex + 1].tok)
                    term.Children.Add(FN_Call());
                else if (Token_Class.IDENTIFIER == TokenStream[tokensIndex].tok)
                    term.Children.Add(match(Token_Class.IDENTIFIER));
            }
            return term;
        }
        
        private Node Equation()
        {
            Node equation = new Node("Equation");
            if (isStartWithTerm())
            {
                equation.Children.Add(Term());
                equation.Children.Add(Multi_Terms());
            }
            else 
            {
                equation.Children.Add(BRACKET_Op());
                if (tokensIndex < TokenStream.Count() && Token_Class.LEFT_BRACKET == TokenStream[tokensIndex + 1].tok)
                {
                    equation.Children.Add(OPs());
                    equation.Children.Add(BRACKET_Op());
                }
                else
                {
                    equation.Children.Add(OPs());
                    equation.Children.Add(Exp());
                }
            }

            return equation;
        }

        private Node Multi_Terms()
        {
            Node multipleTerms = new Node("Multiple Terms");
            multipleTerms.Children.Add(OPs());
            multipleTerms.Children.Add(Exp());
            multipleTerms.Children.Add(Multi_Term());

            return multipleTerms;
        }

        private Node Multi_Term()
        {
            Node multipleTerm = new Node("Multiple Term");
            if (isStartWithOperation(tokensIndex))
            {
                multipleTerm.Children.Add(OPs());
                multipleTerm.Children.Add(Exp());
                multipleTerm.Children.Add(Multi_Term());
                return multipleTerm;
            }

            return null;
        }

        private Node OPs()
        {
            Node operation = new Node("Operation");
            if (tokensIndex < TokenStream.Count())
            {
                if (Token_Class.PLUS_OP == TokenStream[tokensIndex].tok)
                    operation.Children.Add(match(Token_Class.PLUS_OP));
                else if (Token_Class.MINUS_OP == TokenStream[tokensIndex].tok)
                    operation.Children.Add(match(Token_Class.MINUS_OP));
                else if (Token_Class.MULTIPLY_OP == TokenStream[tokensIndex].tok)
                    operation.Children.Add(match(Token_Class.MULTIPLY_OP));
                else if (Token_Class.DEVIDE_OP == TokenStream[tokensIndex].tok)
                    operation.Children.Add(match(Token_Class.DEVIDE_OP));
            }
            return operation;
        }

        private Node BRACKET_Op()
        {
            Node br_Op = new Node("Br_Op");
            br_Op.Children.Add(match(Token_Class.LEFT_BRACKET));
            br_Op.Children.Add(Term());
            br_Op.Children.Add(Multi_Terms());
            br_Op.Children.Add(match(Token_Class.RIGHT_BRACKET));

            return br_Op;
        }
        
        private Node Cond_State()
        {
            Node conditionStatement = new Node("Condition Statements");
            conditionStatement.Children.Add(Condition());
            conditionStatement.Children.Add(Cond_States());

            return conditionStatement;
        }

        private Node Cond_States()
        {
            Node multipleCondition = new Node("Multiple Condition");
            if (tokensIndex < TokenStream.Count() && (Token_Class.AND == TokenStream[tokensIndex].tok || Token_Class.OR == TokenStream[tokensIndex].tok))
            {
                multipleCondition.Children.Add(Bool_Ops());
                multipleCondition.Children.Add(Condition());
                multipleCondition.Children.Add(Cond_States());
                return multipleCondition;
            }

            return null;
        }

        private Node Condition()
        {
            Node condition = new Node("Condition");
            condition.Children.Add(match(Token_Class.IDENTIFIER));
            condition.Children.Add(Cond_Op());
            condition.Children.Add(Exp());
            
            return condition;
        }

        private Node Bool_Ops()
        {
            Node booleanOperator = new Node("Boolean Operator");
            if (tokensIndex < TokenStream.Count())
            {
                if (Token_Class.AND == TokenStream[tokensIndex].tok)
                    booleanOperator.Children.Add(match(Token_Class.AND));
                else if (Token_Class.OR == TokenStream[tokensIndex].tok)
                    booleanOperator.Children.Add(match(Token_Class.OR));
            }
            return booleanOperator;
        }

        private Node Cond_Op()
        {
            Node conditionOperator = new Node("Condition Operator");
            if (tokensIndex < TokenStream.Count())
            {
                if (Token_Class.LESS_THAN == TokenStream[tokensIndex].tok)
                    conditionOperator.Children.Add(match(Token_Class.LESS_THAN));
                else if (Token_Class.GREATER_THAN == TokenStream[tokensIndex].tok)
                    conditionOperator.Children.Add(match(Token_Class.GREATER_THAN));
                else if (Token_Class.EQUALS == TokenStream[tokensIndex].tok)
                    conditionOperator.Children.Add(match(Token_Class.EQUALS));
                else if (Token_Class.NOT_EQUALS == TokenStream[tokensIndex].tok)
                    conditionOperator.Children.Add(match(Token_Class.NOT_EQUALS));
            }

            return conditionOperator;
        }

        /**
         * --------------------- Helper Functions ---------------------
         */
        private bool isStartWithDatatype()
        {
            if (tokensIndex < TokenStream.Count() && (Token_Class.INT == TokenStream[tokensIndex].tok || Token_Class.FLOAT == TokenStream[tokensIndex].tok
                || Token_Class.STRING == TokenStream[tokensIndex].tok))
                return true;
            
            return false;
        }

        private bool isStartWithTerm()
        {
            if (tokensIndex < TokenStream.Count() && (Token_Class.CONSTANT == TokenStream[tokensIndex].tok 
                || Token_Class.IDENTIFIER == TokenStream[tokensIndex].tok))
                 return true;
            
            return false;
        }

        private bool isStartWithOperation(int tokens_Index)
        {
            if (tokensIndex < TokenStream.Count() && (Token_Class.PLUS_OP == TokenStream[tokens_Index].tok || Token_Class.MINUS_OP == TokenStream[tokens_Index].tok
                || Token_Class.MULTIPLY_OP == TokenStream[tokens_Index].tok || Token_Class.DEVIDE_OP == TokenStream[tokens_Index].tok))
                return true;
           
            return false;
        }

        /**
         * --------------------- Template's Functions ---------------------
         */

        public Node match(Token_Class ExpectedToken)
        {
            if (tokensIndex < TokenStream.Count)
            {
                if (ExpectedToken == TokenStream[tokensIndex].tok)
                {
                    tokensIndex++;
                    Node newNode = new Node(ExpectedToken.ToString());

                    return newNode;
                }

                else
                {
                    CompilerControll.scanner.errHandel.ERRs.Add("Parsing Error: Expected "
                        + ExpectedToken.ToString() + " and " +
                        TokenStream[tokensIndex].tok.ToString() +
                        "  found\r\n");
                    tokensIndex++;
                    return null;
                }
            }
            else
            {
                CompilerControll.scanner.errHandel.ERRs.Add("Parsing Error: Expected "
                        + ExpectedToken.ToString() + "\r\n");
                tokensIndex++;
                return null;
            }
        }

        public static TreeNode PrintParseTree(Node root)
        {
            TreeNode tree = new TreeNode("Parse Tree");
            TreeNode treeRoot = PrintTree(root);
            if (treeRoot != null)
                tree.Nodes.Add(treeRoot);
            return tree;
        }
        
        private static TreeNode PrintTree(Node root)
        {
            if (root == null || root.Name == null)
                return null;
            TreeNode tree = new TreeNode(root.Name);
            if (root.Children.Count == 0)
                return tree;
            foreach (Node child in root.Children)
            {
                if (child == null)
                    continue;
                tree.Nodes.Add(PrintTree(child));
            }
            return tree;
        }

    }
}
