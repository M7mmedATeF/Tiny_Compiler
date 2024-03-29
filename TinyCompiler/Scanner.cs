﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace TinyCompiler
{
    public enum Token_Class
    {
        INT, STRING, FLOAT, READ, WRITE, REPEAT, UNTIL, IF, ELSEIF, ELSE, THEN, END, CONSTANT,MAIN_FN,
        RETURN, ENDL, IDENTIFIER, DO, WHILE_STATEMENT, COMMA, DOT, STRING_VAL,
        PLUS_OP, MINUS_OP, DEVIDE_OP, MULTIPLY_OP, ASSIGNMENT_OP, INCREMENT, DECREMENT, SIMICOLON,
        LESS_THAN, GREATER_THAN, EQUALS, NOT_EQUALS, AND, OR, LEFT_BRACKET, RIGHT_BRACKET, LEFT_CBRACKET,
        RIGHT_CBRACKET, LEFT_SBRACKET, RIGHT_SBRACKET
    }

    public class Token
    {
        public string lex;
        public Token_Class tok;
    }
    public class Scanner
    {
        public List<Token> tokens = new List<Token>();
        public List<string> ERRs = new List<string>();
        private Dictionary<string, Token_Class> ReservedKeys = new Dictionary<string, Token_Class>();
        private Dictionary<string, Token_Class> ReservedOP = new Dictionary<string, Token_Class>();

        // Constructor
        public Scanner()
        {
            // Key Words
            ReservedKeys.Add("main", Token_Class.MAIN_FN);
            ReservedKeys.Add("int", Token_Class.INT);
            ReservedKeys.Add("string", Token_Class.STRING);
            ReservedKeys.Add("float", Token_Class.FLOAT);
            ReservedKeys.Add("read", Token_Class.READ);
            ReservedKeys.Add("write", Token_Class.WRITE);
            ReservedKeys.Add("constant", Token_Class.CONSTANT);
            ReservedKeys.Add("stringValue", Token_Class.STRING_VAL);
            ReservedKeys.Add("repeat", Token_Class.REPEAT);
            ReservedKeys.Add("until", Token_Class.UNTIL);
            ReservedKeys.Add("if", Token_Class.IF);
            ReservedKeys.Add("elseif", Token_Class.ELSEIF);
            ReservedKeys.Add("else", Token_Class.ELSE);
            ReservedKeys.Add("then", Token_Class.THEN);
            ReservedKeys.Add("end", Token_Class.END);
            ReservedKeys.Add("return", Token_Class.RETURN);
            ReservedKeys.Add("endl", Token_Class.ENDL);
            ReservedKeys.Add("identefier", Token_Class.IDENTIFIER);
            ReservedKeys.Add("do", Token_Class.DO);
            ReservedKeys.Add("while", Token_Class.WHILE_STATEMENT);

            // Operators
            ReservedOP.Add("+", Token_Class.PLUS_OP);
            ReservedOP.Add("-", Token_Class.MINUS_OP);
            ReservedOP.Add("/", Token_Class.DEVIDE_OP);
            ReservedOP.Add("*", Token_Class.MULTIPLY_OP);
            ReservedOP.Add("++", Token_Class.INCREMENT);
            ReservedOP.Add("--", Token_Class.DECREMENT);
            ReservedOP.Add(";", Token_Class.SIMICOLON);
            ReservedOP.Add(",", Token_Class.COMMA);
            ReservedOP.Add(".", Token_Class.DOT);
            ReservedOP.Add(":=", Token_Class.ASSIGNMENT_OP);
            ReservedOP.Add("=", Token_Class.EQUALS);
            ReservedOP.Add("<", Token_Class.LESS_THAN);
            ReservedOP.Add(">", Token_Class.GREATER_THAN);
            ReservedOP.Add("<>", Token_Class.NOT_EQUALS);
            ReservedOP.Add("||", Token_Class.OR);
            ReservedOP.Add("&&", Token_Class.AND);
            ReservedOP.Add("(", Token_Class.LEFT_BRACKET);
            ReservedOP.Add(")", Token_Class.RIGHT_BRACKET);
            ReservedOP.Add("{", Token_Class.LEFT_CBRACKET);
            ReservedOP.Add("}", Token_Class.RIGHT_CBRACKET);
            ReservedOP.Add("[", Token_Class.LEFT_SBRACKET);
            ReservedOP.Add("]", Token_Class.RIGHT_SBRACKET);
        }

        // Start Scanning Data
        public void StartScanner(string code)
        {
            for (int i = 0; i < code.Length; i++)
            {
                string data = "";
                if (code[i] == ' ' || code[i] == '\n' || code[i] == '\t' || code[i] == '\r')
                    continue;

                // String ".*"
                else if (code[i] == '"')
                {
                    bool gotERR = false;
                    data += code[i]; // "
                    i++;
                    if (i < code.Length)
                    {
                        while (code[i] != '"') // .*
                        {
                            data += code[i];
                            i++;
                            if (i >= code.Length)
                            {
                                gotERR = true;
                                break;
                            }
                        }
                    }
                    else
                    {
                        gotERR = true;
                    }
                    if(!gotERR)
                        data += code[i]; // "

                    getToken(data);
                }

                // Identifier
                else if ((code[i] >= 'A' && code[i] <= 'z'))
                {
                    while ((code[i] >= 'A' && code[i] <= 'z') || (code[i] >= '0' && code[i] <= '9'))
                    {
                        data += code[i];
                        i++;
                        if (i >= code.Length)
                            break;
                    }

                    getToken(data);
                    i--;
                }

                // Number [0-9]
                else if (code[i] >= '0' && code[i] <= '9')
                {
                    while (code[i] >= '0' && code[i] <= '9')
                    {
                        data += code[i];
                        i++;
                        if (i >= code.Length)
                            break;
                    }

                    getToken(data);
                    i--;
                }

                // comment
                else if (code[i] == '/' && code[i+1] == '*')
                {
                    i += 2;
                    while (code[i] != '*' && code[i + 1] != '/')
                    {
                        i++;
                        if (i >= code.Length)
                            break;
                    }
                    i++;
                }

                // Operators
                else
                {
                    data += code[i];
                    if(i+1 != code.Length)
                    {
                        if (data == ":" && code[i + 1] == '=')
                        {
                            i++;
                            data += code[i];
                        }
                        else if (data == "|" && code[i + 1] == '|')
                        {
                            i++;
                            data += code[i];
                        }
                        else if (data == "&" && code[i + 1] == '&')
                        {
                            i++;
                            data += code[i];
                        }
                    }

                    getToken(data);
                }
            }
        }

        // Getting Token
        public void getToken(string LEX) {
            bool isUndefined = false;
            Token tkn = new Token();
            tkn.lex = LEX;

            // Is Reserved WORD ?
            if (ReservedKeys.ContainsKey(LEX))
                tkn.tok = ReservedKeys[LEX];
            // Is Reserved Operation ?
            else if (ReservedOP.ContainsKey(LEX))
                tkn.tok = ReservedOP[LEX];
            // Is String ?
            else if (isString(LEX))
                tkn.tok = ReservedKeys["stringValue"];
            // Is Idenetifier ?
            else if (isIdentifier(LEX))
                tkn.tok = ReservedKeys["identefier"];
            // Is Number ?
            else if (isNumber(LEX))
                tkn.tok = ReservedKeys["constant"];
            // TODO: Undefined
            else
            {
                isUndefined = true;
                ERRs.Add(tkn.lex + " is UNDEFINED");
            }

            // Add to List
            if(!isUndefined)
                tokens.Add(tkn);
        }

        // Helper Functions
        bool isIdentifier(String data)
        {
            // Handel " => IDENTIFIER ERR
            if(data[0] == '"')
            {
                return false;
            }
            Regex rg = new Regex("[A-Za-z]+[A-Za-z0-9]*[_[A-Za-z0-9]+]*");
            return rg.IsMatch(data);
        }
        bool isString(String data)
        {
            Regex rg = new Regex("\".*\"");
            return rg.IsMatch(data);
        }
        bool isNumber(String data)
        {
            Regex rg = new Regex("[0-9]+");
            return rg.IsMatch(data);
        }
    }
}
