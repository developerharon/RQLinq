namespace RQLinq
{
    public sealed class RqlSyntaxTree
    {
        public RqlSyntaxTree(IEnumerable<string> diagnostics, RqlExpressionSyntax root, RqlToken endOfFileToken)
        {
            Diagnostics = diagnostics.ToArray();
            Root = root;
            EndOfFileToken = endOfFileToken;
        }

        public IReadOnlyList<string> Diagnostics { get; }
        public RqlExpressionSyntax Root { get; }
        public RqlToken EndOfFileToken { get; }

        public static RqlSyntaxTree Parse(string text)
        {
            var parser = new RqlParser(text);
            return parser.Parse();
        }
    }
}