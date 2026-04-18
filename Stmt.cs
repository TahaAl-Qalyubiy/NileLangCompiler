namespace NileLangCompiler;

public enum NileType
{
    Stone,
    Water,
    Papyrus,
}

public abstract class Stmt
{
}

public sealed class ExpressionStmt : Stmt
{
    public Expr Expression { get; }

    public ExpressionStmt(Expr expression)
    {
        Expression = expression;
    }
}

public sealed class VarDeclaration : Stmt
{
    public NileType Type { get; }
    public string Name { get; }
    public Expr Initializer { get; }

    public VarDeclaration(NileType type, string name, Expr initializer)
    {
        Type = type;
        Name = name;
        Initializer = initializer;
    }
}

public sealed class BlockStmt : Stmt
{
    public IReadOnlyList<Stmt> Statements { get; }

    public BlockStmt(IReadOnlyList<Stmt> statements)
    {
        Statements = statements;
    }
}

public sealed class IfStmt : Stmt
{
    public Expr Condition { get; }
    public Stmt ThenBranch { get; }
    public Stmt? ElseBranch { get; }

    public IfStmt(Expr condition, Stmt thenBranch, Stmt? elseBranch)
    {
        Condition = condition;
        ThenBranch = thenBranch;
        ElseBranch = elseBranch;
    }
}

public sealed class WhileStmt : Stmt
{
    public Expr Condition { get; }
    public Stmt Body { get; }

    public WhileStmt(Expr condition, Stmt body)
    {
        Condition = condition;
        Body = body;
    }
}

public sealed class ProgramAst
{
    public string Name { get; }
    public IReadOnlyList<Stmt> Statements { get; }

    public ProgramAst(string name, IReadOnlyList<Stmt> statements)
    {
        Name = name;
        Statements = statements;
    }
}
