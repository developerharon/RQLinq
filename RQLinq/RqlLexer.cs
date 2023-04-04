namespace RQLinq
{
    public class RqlLexer
    {
        private readonly string _rql;
        private int _position;
        private List<string> _diagnostics = new List<string>();

        public RqlLexer(string rql)
        {
            _rql = rql;
        }

        public IEnumerable<string> Diagnostics => _diagnostics;

        private char Current
        {
            get
            {
                if (_position >= _rql.Length)
                    return '\0'; // zero terminator

                return _rql[_position];
            }
        }

        private void Next()
        {
            _position++;
        }

        public RqlToken NextToken()
        {
            if (_position >= _rql.Length)
                return new RqlToken(RqlTokenKind.EndOfFileToken, _position, "\0", null!);

            if (Current == '(')
                return new RqlToken(RqlTokenKind.OpenParanthesisToken, _position++, "(", null!);

            if (Current == ')')
                return new RqlToken(RqlTokenKind.CloseParenthesisToken, _position++, ")", null!);

            if (Current == ',')
                return new RqlToken(RqlTokenKind.Comma, _position++, ",", null!);


            var start = _position;

            while (Current != '(' && Current != ')' && Current != ',')
                Next();

            var length = _position - start;
            var text = _rql.Substring(start, length);

            if (string.Compare(text, "and", true) == 0)
                return new RqlToken(RqlTokenKind.AND, start, text, null!);

            if (string.Compare(text, "or", true) == 0)
                return new RqlToken(RqlTokenKind.OR, start, text, null!);

            if (string.Compare(text, "in", true) == 0)
                return new RqlToken(RqlTokenKind.IN, start, text, null!);

            if (string.Compare(text, "out", true) == 0)
                return new RqlToken(RqlTokenKind.OUT, start, text, null!);

            if (string.Compare(text, "eq", true) == 0)
                return new RqlToken(RqlTokenKind.EQ, start, text, null!);

            if (string.Compare(text, "ne", true) == 0)
                return new RqlToken(RqlTokenKind.NE, start, text, null!);

            if (string.Compare(text, "lt", true) == 0)
                return new RqlToken(RqlTokenKind.LT, start, text, null!);

            if (string.Compare(text, "le", true) == 0)
                return new RqlToken(RqlTokenKind.LE, start, text, null!);

            if (string.Compare(text, "gt", true) == 0)
                return new RqlToken(RqlTokenKind.GT, start, text, null!);

            if (string.Compare(text, "ge", true) == 0)
                return new RqlToken(RqlTokenKind.GE, start, text, null!);

            return new RqlToken(RqlTokenKind.OperandToken, start, text, text);
        }
    }
}