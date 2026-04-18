namespace NileLangCompiler;

public enum BinaryOperator
{
    Equal,
    NotEqual,
    Greater,
    GreaterEqual,
    Less,
    LessEqual,
    Add,
    Subtract,
}

public enum UnaryOperator
{
    Negate,
    Not,
}

public enum LiteralKind
{
    Integer,
    Float,
}

public abstract class Expr
{
}

public sealed class BinaryExpr : Expr
{
    public Expr Left { get; }
    public BinaryOperator Operator { get; }
    public Expr Right { get; }

    public BinaryExpr(Expr left, BinaryOperator op, Expr right)
    {
        Left = left;
        Operator = op;
        Right = right;
    }
}

public sealed class UnaryExpr : Expr
{
    public UnaryOperator Operator { get; }
    public Expr Operand { get; }

    public UnaryExpr(UnaryOperator op, Expr operand)
    {
        Operator = op;
        Operand = operand;
    }
}

public sealed class LiteralExpr : Expr
{
    public LiteralKind Kind { get; }
    public string Lexeme { get; }

    public LiteralExpr(LiteralKind kind, string lexeme)
    {
        Kind = kind;
        Lexeme = lexeme;
    }
}

public sealed class VariableExpr : Expr
{
    public string Name { get; }

    public VariableExpr(string name)
    {
        Name = name;
    }
}

public sealed class AssignmentExpr : Expr
{
    public string Name { get; }
    public Expr Value { get; }

    public AssignmentExpr(string name, Expr value)
    {
        Name = name;
        Value = value;
    }
}
