namespace NileLangCompiler;

/// <summary>
/// Prints the full AST as a hierarchical tree (console visualization only).
/// </summary>
public static class ASTPrinter
{
    public static void Print(ProgramAst program)
    {
        Console.WriteLine($"Program ({program.Name})");
        IReadOnlyList<Stmt> stmts = program.Statements;
        for (int i = 0; i < stmts.Count; i++)
            PrintStmt(stmts[i], "", i == stmts.Count - 1);
    }

    private static void PrintStmt(Stmt stmt, string prefix, bool isLast)
    {
        string branch = Branch(isLast);
        string extend = Extend(prefix, isLast);

        switch (stmt)
        {
            case VarDeclaration v:
                Console.WriteLine($"{prefix}{branch}VarDeclaration");
                Console.WriteLine($"{extend}├── Type: {FormatNileType(v.Type)}");
                Console.WriteLine($"{extend}├── Name: {v.Name}");
                Console.WriteLine($"{extend}└── Initializer:");
                PrintExpr(v.Initializer, extend + "    ", true);
                break;

            case ExpressionStmt e:
                Console.WriteLine($"{prefix}{branch}ExpressionStmt");
                PrintExpr(e.Expression, extend, true);
                break;

            case BlockStmt b:
                Console.WriteLine($"{prefix}{branch}BlockStmt");
                PrintStmtList(b.Statements, extend);
                break;

            case IfStmt iff:
                Console.WriteLine($"{prefix}{branch}IfStmt");
                Console.WriteLine($"{extend}├── Condition:");
                PrintExpr(iff.Condition, extend + "│   ", true);
                if (iff.ElseBranch is null)
                {
                    Console.WriteLine($"{extend}└── Then:");
                    PrintStmt(iff.ThenBranch, extend + "    ", true);
                }
                else
                {
                    Console.WriteLine($"{extend}├── Then:");
                    PrintStmt(iff.ThenBranch, extend + "│   ", true);
                    Console.WriteLine($"{extend}└── Else:");
                    PrintStmt(iff.ElseBranch, extend + "    ", true);
                }
                break;

            case WhileStmt w:
                Console.WriteLine($"{prefix}{branch}WhileStmt");
                Console.WriteLine($"{extend}├── Condition:");
                PrintExpr(w.Condition, extend + "│   ", true);
                Console.WriteLine($"{extend}└── Body:");
                PrintStmt(w.Body, extend + "    ", true);
                break;

            default:
                Console.WriteLine($"{prefix}{branch}{stmt.GetType().Name}");
                break;
        }
    }

    private static void PrintStmtList(IReadOnlyList<Stmt> statements, string prefix)
    {
        for (int i = 0; i < statements.Count; i++)
            PrintStmt(statements[i], prefix, i == statements.Count - 1);
    }

    private static void PrintExpr(Expr expr, string prefix, bool isLast)
    {
        string branch = Branch(isLast);
        string extend = Extend(prefix, isLast);

        switch (expr)
        {
            case BinaryExpr b:
                Console.WriteLine($"{prefix}{branch}Binary ({FormatBinaryOp(b.Operator)})");
                Console.WriteLine($"{extend}├── Left:");
                PrintExpr(b.Left, extend + "│   ", true);
                Console.WriteLine($"{extend}└── Right:");
                PrintExpr(b.Right, extend + "    ", true);
                break;

            case UnaryExpr u:
                Console.WriteLine($"{prefix}{branch}Unary ({FormatUnaryOp(u.Operator)})");
                Console.WriteLine($"{extend}└── Operand:");
                PrintExpr(u.Operand, extend + "    ", true);
                break;

            case LiteralExpr l:
                Console.WriteLine($"{prefix}{branch}Literal ({FormatLiteralKind(l.Kind)}) {l.Lexeme}");
                break;

            case VariableExpr v:
                Console.WriteLine($"{prefix}{branch}Variable {v.Name}");
                break;

            case AssignmentExpr a:
                Console.WriteLine($"{prefix}{branch}Assignment");
                Console.WriteLine($"{extend}├── Name: {a.Name}");
                Console.WriteLine($"{extend}└── Value:");
                PrintExpr(a.Value, extend + "    ", true);
                break;

            default:
                Console.WriteLine($"{prefix}{branch}{expr.GetType().Name}");
                break;
        }
    }

    private static string Branch(bool isLast) => isLast ? "└── " : "├── ";

    private static string Extend(string prefix, bool isLast) => prefix + (isLast ? "    " : "│   ");

    private static string FormatNileType(NileType type) => type switch
    {
        NileType.Stone => "stone",
        NileType.Water => "water",
        NileType.Papyrus => "papyrus",
        _ => type.ToString().ToLowerInvariant(),
    };

    private static string FormatLiteralKind(LiteralKind kind) => kind switch
    {
        LiteralKind.Integer => "integer",
        LiteralKind.Float => "float",
        _ => kind.ToString().ToLowerInvariant(),
    };

    private static string FormatBinaryOp(BinaryOperator op) => op switch
    {
        BinaryOperator.Equal => "==",
        BinaryOperator.NotEqual => "!=",
        BinaryOperator.Greater => ">",
        BinaryOperator.GreaterEqual => ">=",
        BinaryOperator.Less => "<",
        BinaryOperator.LessEqual => "<=",
        BinaryOperator.Add => "+",
        BinaryOperator.Subtract => "-",
        _ => op.ToString(),
    };

    private static string FormatUnaryOp(UnaryOperator op) => op switch
    {
        UnaryOperator.Negate => "-",
        UnaryOperator.Not => "!",
        _ => op.ToString(),
    };
}
