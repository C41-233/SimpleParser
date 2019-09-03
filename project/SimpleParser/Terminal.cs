using System;

namespace SimpleParser
{
    internal class Terminal
    {
        public int Token;
        public Predicate<Token> Predicate;

        public bool Match(Token token)
        {
            if (token.Type != Token)
            {
                return false;
            }

            if (Predicate != null && !Predicate(token))
            {
                return false;
            }

            return true;
        }

    }
}