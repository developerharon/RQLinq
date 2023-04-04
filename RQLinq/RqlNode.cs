namespace RQLinq
{
    public abstract class RqlNode
    {
        public abstract RqlTokenKind Kind { get; }
        public abstract IEnumerable<RqlNode> GetChilden();
    }
}