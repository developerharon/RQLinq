namespace RQLinq
{
    public sealed class CompoundExpressionSyntax : RqlExpressionSyntax
    {
        public CompoundExpressionSyntax(RqlToken operatorToken, List<RqlExpressionSyntax> expressions)
        {
            OperatorToken = operatorToken;
            Expressions = expressions;
        }

        public RqlToken OperatorToken { get; }
        public List<RqlExpressionSyntax> Expressions { get; }

        public override RqlTokenKind Kind => RqlTokenKind.CompoundExpression;

        public override IEnumerable<RqlNode> GetChilden()
        {
            yield return OperatorToken;
            foreach (var expression in Expressions)
            {
                yield return expression;
            }
        }
    }
}