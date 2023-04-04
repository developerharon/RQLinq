namespace RQLinq
{
    public enum RqlTokenKind
    {
        EndOfFileToken,
        OpenParanthesisToken,
        CloseParenthesisToken,
        Comma,
        AND,
        EQ,
        IN,
        BadToken,
        NE,
        BinaryExpression,
        CompoundExpression,
        OperandToken,
        PropertyExpression,
        ValueExpression,
        LE,
        LT,
        GE,
        GT,
        OR,
        OUT
    }
}