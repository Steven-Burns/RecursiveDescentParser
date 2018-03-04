using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfixExpressions
{
    class Program
    {
        public static void Main(string[] args)
        {
            InfixParser parser = new InfixParser();

            // correct expression cases
            parser.ParseExpression("1 + 12");
            parser.ParseExpression("1 + ( 2 + 3 )");
            parser.ParseExpression("( 2 + 3 ) + 4");

            // incorrect expression cases
            parser.ParseExpression("+");
            parser.ParseExpression("1 + ");
            parser.ParseExpression("( 1 )");
            parser.ParseExpression("( 1 + )");
            parser.ParseExpression("( ( 1 + 1 )");
            parser.ParseExpression("1 + 1 )");
            parser.ParseExpression("( 43 + )");
        }
    }



    class InfixParser
    {
        // This class implements a recursive decent parser for a very simple grammar.

        // A grammar is a set of rules for how to express something.  

        // An example of a rule for grammar in English is "A sentance must have a noun, then 
        // a verb, then a direct object" as in "the cat ate the chicken."

        // Human language grammars are very complicated, too complicated for computers to 
        // evaluate efficiently.  So we have computer languages that are simple and efficient 
        // to evaluate.  The job of  the programmer can be thought of translating ideas from 
        // human language to computer language -- adapting one grammar to another.

        // There's been lots of work in this area.  

        // It turns out that often, computer language (and human language), the rules are 
        // recursive, meaning that the elements of the grammar are defined in terms of compounds 
        // of themselves.  An example in English would be that a noun can be formed from a phrase, 
        // such as "going to the gym."  As in "Going to the gym makes me happy."

        // The observation of the recursive nature of most grammars led to a technique for parsing
        // called recursive decent.  The idea is that one can use the runtime function call stack 
        // to keep track of the structure of the rules the parser is evaluating.

        // To start, you need rules.

        // One notation for grammar rules is called Baukus-Naur Form (BNF), which I will give a 
        // simplified version for a simplified version of the grammar for your project.

        // For arithmetic expressions like 1 + ( 3 + 4 ), the rules looks like this:

        // expression :- operand operator operand
        // operand :- open expression close | number
        // open :- "("
        // close :- ")"
        // operator :- "+"
        // number :- string-of-digits

        // Which you can read as 
        // "An expression is given by an operand followed by an operator followed by another operand."
        // "An operand is an open thingy followed by an expression followed by a close thingy, OR, is a number."
        // "An open thingy is the "(" character."
        // "A close thingy is the ")" character."
        // "An operator is the "+" character."
        // "A number is a string of digits."

        // You can see the recursion in that expressions are defined in terms of operands and 
        // operands are defined in terms of expressions.

        // The idea with the technique is: 
        // 1. Break the input into an array of non-whitespace strings.  We call these tokens.
        // 2. Create a function called Accept that looks at a token and decides if it matches a pattern.  If it does,
        // move to the next token.  
        // 2. Represent the rules with functions that make use of the idea that each function Accepts the tokens 
        // that are being expected by the rule the function is representing. If the rule has an "or" to it (like the
        // operand rule above), use the return value of the Accept function to determine which pattern matches.

        // Note that the grammar rules above don't handle the case like "1 + 2 + 3 + 4 + 5".  Can you think how to 
        // change the rules to allow for that case?


        private string[] tokens;                // this will hold the list of tokens
        private int currentTokenIndex = -1;     // this is the index into the list of the token that will be examined next.

        private void MoveToNextToken()
        {
            if (currentTokenIndex >= tokens.Length)
            {
                throw NewSyntaxErrorException("End of input reached");
            }
            ++currentTokenIndex;
        }

    
        // If the current token matches s, 'accepts' the token by moving to the next token, and returns true.
        // If the current token does not match, returns false.

        private bool Accept(string s)
        {
            if (currentTokenIndex >= tokens.Length)
            {
                throw NewSyntaxErrorException("End of input reached");
            }
            if (tokens[currentTokenIndex] == s)
            {
                MoveToNextToken();
                return true;
            }
            return false;
        }


        // If the current token is a string of digits, 'accepts' the token by moving to the next token, and 
        // returns true.  Otherwise returns false.

        private bool AcceptNumber()
        {
            if (isNumber(tokens[currentTokenIndex]))
            {
                MoveToNextToken();
                return true;
            }
            return false;
        }


        // Implements the 'expression' rule.  Note how the implementation follows the grammar rule.

        private void Expression()
        {
            Operand();
            Operator();
            Operand();
        }


        private void Operand()
        {
            // This rule says that the pattern to be accepted is either an expression or a number.
            // We have to look at a token with the Accept function to see which we are given.

            if (Accept("("))
            {
                // Input has a '(', so we should expect an expression.
                Expression();
                if (!Accept(")"))
                {
                    throw NewSyntaxErrorException("Missing close )");
                }
            } 
            else if (!AcceptNumber())
            {
                // any other input that ( must be a number.
                throw NewSyntaxErrorException("Missing number operand");
            }
        }


        private void Operator()
        {
            if (!Accept("+"))
            {
                throw NewSyntaxErrorException("Missing operator");
            }
        }


        private Exception NewSyntaxErrorException(string error) 
        {
            if (currentTokenIndex < tokens.Length) 
            {
                return new Exception(
                    String.Format("Error '{0}' at token #{1}, '{2}'", error, currentTokenIndex, tokens[currentTokenIndex]));
            }
            return new Exception(String.Format("Error '{0}' at token index #{1}", error, currentTokenIndex));
        }


        // Returns true if the string is a series of digit characters
 
        private static bool isNumber(string s)
        {
            int result = 0;
            if (int.TryParse(s, out result))
            {
                return true;
            }
            return false;
        }


        // Parses and reports errors to the console, if any.

        public void ParseExpression(string s)
        {
            try 
            {
                tokens = s.Split(' ');
                currentTokenIndex = -1;
                MoveToNextToken();
                Expression();

                // If there is input left that the rules have not examined, then that's an error. That actually 
                // indicates a bug with our rules.

                if (currentTokenIndex < tokens.Length)
                {
                    throw NewSyntaxErrorException("Additional input found.");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(String.Format("Error parsing expression '{0}': '{1}'", s, e.Message));
            }
        }
    }
}
