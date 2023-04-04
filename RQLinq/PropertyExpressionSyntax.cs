namespace RQLinq
{
    public class PropertyExpressionSyntax : RqlExpressionSyntax
    {
        public PropertyExpressionSyntax(RqlToken propertyToken)
        {
            PropertyToken = propertyToken;
        }

        public override RqlTokenKind Kind => RqlTokenKind.PropertyExpression;
        public RqlToken PropertyToken { get; }

        public override IEnumerable<RqlNode> GetChilden()
        {
            yield return PropertyToken;
        }
    }
}