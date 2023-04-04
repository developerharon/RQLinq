namespace RQLinq
{
    public class ValueExpressionSyntax : RqlExpressionSyntax
    {
        public ValueExpressionSyntax(RqlToken valueToken)
        {
            ValueToken = valueToken;
        }

        public override RqlTokenKind Kind => RqlTokenKind.ValueExpression;
        public RqlToken ValueToken { get; }

        public override IEnumerable<RqlNode> GetChilden()
        {
            yield return ValueToken;
        }
    }
}