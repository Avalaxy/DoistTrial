using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Filters.Models;

namespace Filters
{
    public class ItemFilter
    {
        private const string And = "&";
        private const string Or = "|";
        private const string LeftBracket = "(";
        private const string RightBracket = ")"; 

        private readonly List<string> _precedence = new List<string> { And, Or }; 

        public Section FilterItems(IEnumerable<Item> items, string query)
        {
            var results = new Section();

            /*
             *  Use the Shunting-yard algorithm to rewrite the infix notation to Reverse Polish Notation so that it 
             *  can be processed in a stack: https://en.wikipedia.org/wiki/Shunting-yard_algorithm.
             */
            var operatorStack = new Stack<string>();
            var outputQueue = new Queue<string>();

            string[] tokens = TokenizeQuery(query);
            foreach (string token in tokens)
            {
                if (_precedence.Contains(token))
                {
                    while (operatorStack.Any()
                        && (!HasHigherPrecedenceThanOtherOperator(token, operatorStack.Peek()) 
                        || token.Equals(operatorStack.Peek())))
                    {
                        outputQueue.Enqueue(operatorStack.Pop());
                    }
                    operatorStack.Push(token);
                } 
                else if (token.Equals(LeftBracket)) operatorStack.Push(token);
                else if (token.Equals(RightBracket))
                {
                    while (operatorStack.Any())
                    {
                        var @operator = operatorStack.Pop();
                        if (@operator != LeftBracket) outputQueue.Enqueue(@operator);
                        else break;
                    }
                }
                else outputQueue.Enqueue(token);
            }
            foreach (string @operator in operatorStack)
            {
                outputQueue.Enqueue(@operator);
            }

            return results;
        }

        private string[] TokenizeQuery(string query)
        {
            string[] rawTokens = Regex.Split(query, @"(&|\||\(|\))");

            string[] cleanedTokens = (
                from rawToken in rawTokens
                where !string.IsNullOrWhiteSpace(rawToken)
                select rawToken.Trim()
                ).ToArray();

            return cleanedTokens;
        }

        private bool HasHigherPrecedenceThanOtherOperator(string @operator, string otherOperator)
        {
            foreach (string operatorPrecedence in _precedence)
            {
                if (operatorPrecedence == @operator) return true;
                if (operatorPrecedence == otherOperator) return false;
            }

            throw new Exception($"Operator {@operator} not supported.");
        }
    }
}
