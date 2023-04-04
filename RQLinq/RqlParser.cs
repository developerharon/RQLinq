using System.ComponentModel.DataAnnotations;

namespace RQLinq
{
    public class RqlParser
    {
        private readonly RqlToken[] _tokens;
        private int _position;
        private List<string> _diagnostics = new List<string>();

        public RqlParser(string text)
        {
            var tokens = new List<RqlToken>();

            var lexer = new RqlLexer(text);

            RqlToken token;

            do
            {
                token = lexer.NextToken();

                if (token.Kind != RqlTokenKind.BadToken)
                    tokens.Add(token);

            } while (token.Kind != RqlTokenKind.EndOfFileToken);

            _tokens = tokens.ToArray();
            _diagnostics.AddRange(lexer.Diagnostics);
        }

        public IEnumerable<string> Diagnostics => _diagnostics;

        private RqlToken Peek(int offset)
        {
            var index = _position + offset;

            if (index >= _tokens.Length)
                return _tokens[_tokens.Length - 1];

            return _tokens[index];
        }

        private RqlToken Current => Peek(0);

        private RqlToken NextToken()
        {
            var current = Current;
            _position++;
            return current;
        }

        private RqlToken Match(RqlTokenKind kind)
        {
            if (Current.Kind == kind)
                return NextToken();

            _diagnostics.Add($"ERROR: unexpected token <{Current.Kind}>, expected <{kind}>");
            return new RqlToken(kind, Current.Position, null!, null!);
        }

        public RqlSyntaxTree Parse()
        {
            var expression = ParseTerm();
            var endOfFileToken = Match(RqlTokenKind.EndOfFileToken);
            return new RqlSyntaxTree(_diagnostics, expression, endOfFileToken);
        }

        private RqlExpressionSyntax ParseTerm()
        {
            while (Current.Kind == RqlTokenKind.AND
                || Current.Kind == RqlTokenKind.OR)
            {
                var operatorToken = NextToken();
                var expressions = new List<RqlExpressionSyntax>();
                Match(RqlTokenKind.OpenParanthesisToken);

                var expression = ParseFactor();
                expressions.Add(expression);

                while (Current.Kind == RqlTokenKind.Comma)
                {
                    NextToken();
                    expression = ParseFactor();
                    expressions.Add(expression);
                }

                Match(RqlTokenKind.CloseParenthesisToken);
                return new CompoundExpressionSyntax(operatorToken, expressions);
            }

            return ParseFactor();
        }

        private RqlExpressionSyntax ParseFactor()
        {
            while (Current.Kind == RqlTokenKind.EQ
                || Current.Kind == RqlTokenKind.NE
                || Current.Kind == RqlTokenKind.LE
                || Current.Kind == RqlTokenKind.LT
                || Current.Kind == RqlTokenKind.GE
                || Current.Kind == RqlTokenKind.GT)
            {

                var operatorToken = NextToken();
                Match(RqlTokenKind.OpenParanthesisToken);
                var property = ParseFactor() as PropertyExpressionSyntax;
                Match(RqlTokenKind.Comma);
                var value = ParseFactor() as PropertyExpressionSyntax;
                Match(RqlTokenKind.CloseParenthesisToken);
                return new BinaryExpressionSyntax(operatorToken, property, value);
            }

            while (Current.Kind == RqlTokenKind.IN
                || Current.Kind == RqlTokenKind.OUT)
            {
                var operatorToken = NextToken();
                Match(RqlTokenKind.OpenParanthesisToken);

                var expressions = new List<RqlExpressionSyntax>();

                var inOperatorToken = new RqlToken(RqlTokenKind.EQ, int.MaxValue, "in", null!);

                if (operatorToken.Kind == RqlTokenKind.OUT)
                    inOperatorToken = new RqlToken(RqlTokenKind.NE, int.MaxValue, "out", null!);

                var property = ParseFactor() as PropertyExpressionSyntax;

                Match(RqlTokenKind.Comma);

                var parenthesized = false;

                if (Current.Kind == RqlTokenKind.OpenParanthesisToken)
                {
                    parenthesized = true;
                    NextToken();
                }

                var value = ParseFactor() as PropertyExpressionSyntax;
                expressions.Add(new BinaryExpressionSyntax(inOperatorToken, property, value));

                while (Current.Kind == RqlTokenKind.Comma)
                {
                    NextToken();
                    value = ParseFactor() as PropertyExpressionSyntax;
                    expressions.Add(new BinaryExpressionSyntax(inOperatorToken, property, value));
                }
                if (parenthesized)
                    Match(RqlTokenKind.CloseParenthesisToken);
                Match(RqlTokenKind.CloseParenthesisToken);
                return new CompoundExpressionSyntax(operatorToken, expressions);
            }
            return new PropertyExpressionSyntax(NextToken());
        }
    }
}