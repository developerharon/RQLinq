namespace RQLinq
{
    public sealed class BinaryExpressionSyntax : RqlExpressionSyntax
    {
        public BinaryExpressionSyntax(RqlToken operatorToken, PropertyExpressionSyntax left, PropertyExpressionSyntax right)
        {
            OperatorToken = operatorToken;
            Left = left;
            Right = right;
        }

        public RqlToken OperatorToken { get; }
        public PropertyExpressionSyntax Left { get; }
        public PropertyExpressionSyntax Right { get; }

        public override RqlTokenKind Kind => RqlTokenKind.BinaryExpression;

        public override IEnumerable<RqlNode> GetChilden()
        {
            yield return OperatorToken;
            yield return Left;
            yield return Right;
        }
    }
}