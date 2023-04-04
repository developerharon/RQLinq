namespace RQLinq
{
    public class RqlToken : RqlNode
    {
        public override RqlTokenKind Kind { get; }
        public int Position { get; }
        public string Text { get; }
        public object Value { get; }

        public RqlToken(RqlTokenKind kind, int position, string text, object value)
        {
            Kind = kind;
            Position = position;
            Text = text;
            Value = value;
        }

        public override IEnumerable<RqlNode> GetChilden()
        {
            return Enumerable.Empty<RqlNode>();
        }
    }
}