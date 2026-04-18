using System.Globalization;

namespace NileLangCompiler;

public class Parser
{
    private readonly List<Token> tokens;
    private int current;

    public Parser(List<Token> tokens)
    {
        this.tokens = tokens;
    }

    private Token Peek() => tokens[current];

    private Token Advance()
    {
        if (!IsAtEnd()) current++;
        return Previous();
    }

    private Token Previous() => tokens[current - 1];

    private bool IsAtEnd() => Peek().Type == TokenType.EOF;

    private bool Check(TokenType type)
    {
        if (IsAtEnd()) return false;
        return Peek().Type == type;
    }

    private bool Match(params TokenType[] types)
    {
        foreach (var type in types)
        {
            if (Check(type))
            {
                Advance();
                return true;
            }
        }
        return false;
    }

    private Token Consume(TokenType type, string message)
    {
        if (Check(type)) return Advance();
        throw new Exception(message);
    }

    public ProgramAst ParseProgram()
    {
        Consume(TokenType.Reign, "Expected 'reign'");
        Token name = Consume(TokenType.Identifier, "Expected program name");
        Consume(TokenType.LeftBrace, "Expected '{'");

        List<Stmt> body = ParseStatementList();

        Consume(TokenType.RightBrace, "Expected '}'");

        return new ProgramAst(name.Lexeme, body);
    }

    private List<Stmt> ParseStatementList()
    {
        var statements = new List<Stmt>();
        while (!Check(TokenType.RightBrace) && !IsAtEnd())
        {
            statements.Add(ParseStatement());
        }
        return statements;
    }

    private Stmt ParseStatement()
    {
        if (Check(TokenType.Stone) || Check(TokenType.Water) || Check(TokenType.Papyrus))
        {
            NileType type = ParseType();
            return ParseVarDeclaration(type);
        }

        if (Match(TokenType.Judge))
            return ParseIf();

        if (Match(TokenType.Flow))
            return ParseWhile();

        if (Check(TokenType.LeftBrace))
            return ParseBlock();

        return ParseExpressionStatement();
    }

    private NileType ParseType()
    {
        if (Match(TokenType.Stone)) return NileType.Stone;
        if (Match(TokenType.Water)) return NileType.Water;
        if (Match(TokenType.Papyrus)) return NileType.Papyrus;
        throw new Exception("Expected type (stone, water, or papyrus)");
    }

    private Stmt ParseVarDeclaration(NileType type)
    {
        Token name = Consume(TokenType.Identifier, "Expected variable name");
        Consume(TokenType.Assign, "Expected '='");
        Expr initializer = ParseExpression();
        Consume(TokenType.Semicolon, "Expected ';'");
        return new VarDeclaration(type, name.Lexeme, initializer);
    }

    private Stmt ParseIf()
    {
        Consume(TokenType.LeftParen, "Expected '('");
        Expr condition = ParseExpression();
        Consume(TokenType.RightParen, "Expected ')'");

        Stmt thenBranch = ParseBlock();

        Stmt? elseBranch = null;
        if (Match(TokenType.Banish))
            elseBranch = ParseBlock();

        return new IfStmt(condition, thenBranch, elseBranch);
    }

    private Stmt ParseWhile()
    {
        Consume(TokenType.LeftParen, "Expected '('");
        Expr condition = ParseExpression();
        Consume(TokenType.RightParen, "Expected ')'");

        Stmt body = ParseBlock();
        return new WhileStmt(condition, body);
    }

    private Stmt ParseBlock()
    {
        Consume(TokenType.LeftBrace, "Expected '{'");
        List<Stmt> inner = ParseStatementList();
        Consume(TokenType.RightBrace, "Expected '}'");
        return new BlockStmt(inner);
    }

    private Stmt ParseExpressionStatement()
    {
        Expr expr = ParseExpression();
        Consume(TokenType.Semicolon, "Expected ';'");
        return new ExpressionStmt(expr);
    }

    public Expr ParseExpression() => ParseAssignment();

    private Expr ParseAssignment()
    {
        int start = current;
        Expr expr = ParseEquality();

        if (Check(TokenType.Assign))
        {
            if (current - start != 1 || tokens[start].Type != TokenType.Identifier)
                throw new Exception("Invalid assignment target");

            string name = tokens[start].Lexeme;
            Advance();
            Expr value = ParseAssignment();
            return new AssignmentExpr(name, value);
        }

        return expr;
    }

    private Expr ParseEquality()
    {
        Expr expr = ParseComparison();

        while (Match(TokenType.Equals, TokenType.NotEquals))
        {
            BinaryOperator op = Previous().Type switch
            {
                TokenType.Equals => BinaryOperator.Equal,
                TokenType.NotEquals => BinaryOperator.NotEqual,
                _ => throw new Exception("Internal parser error"),
            };
            Expr right = ParseComparison();
            expr = new BinaryExpr(expr, op, right);
        }

        return expr;
    }

    private Expr ParseComparison()
    {
        Expr expr = ParseTerm();

        while (Match(TokenType.GreaterThan, TokenType.GreaterOrEqual,
                     TokenType.LessThan, TokenType.LessOrEqual))
        {
            BinaryOperator op = Previous().Type switch
            {
                TokenType.GreaterThan => BinaryOperator.Greater,
                TokenType.GreaterOrEqual => BinaryOperator.GreaterEqual,
                TokenType.LessThan => BinaryOperator.Less,
                TokenType.LessOrEqual => BinaryOperator.LessEqual,
                _ => throw new Exception("Internal parser error"),
            };
            Expr right = ParseTerm();
            expr = new BinaryExpr(expr, op, right);
        }

        return expr;
    }

    private Expr ParseTerm()
    {
        Expr expr = ParseFactor();

        while (Match(TokenType.Plus, TokenType.Minus))
        {
            BinaryOperator op = Previous().Type switch
            {
                TokenType.Plus => BinaryOperator.Add,
                TokenType.Minus => BinaryOperator.Subtract,
                _ => throw new Exception("Internal parser error"),
            };
            Expr right = ParseFactor();
            expr = new BinaryExpr(expr, op, right);
        }

        return expr;
    }

    private Expr ParseFactor() => ParseUnary();

    private Expr ParseUnary()
    {
        if (Match(TokenType.Not))
            return new UnaryExpr(UnaryOperator.Not, ParseUnary());

        if (Match(TokenType.Minus))
            return new UnaryExpr(UnaryOperator.Negate, ParseUnary());

        return ParsePrimary();
    }

    private Expr ParsePrimary()
    {
        if (Match(TokenType.Integer))
        {
            string lexeme = Previous().Lexeme;
            _ = int.Parse(lexeme, CultureInfo.InvariantCulture);
            return new LiteralExpr(LiteralKind.Integer, lexeme);
        }

        if (Match(TokenType.Float))
        {
            string lexeme = Previous().Lexeme;
            _ = double.Parse(lexeme, CultureInfo.InvariantCulture);
            return new LiteralExpr(LiteralKind.Float, lexeme);
        }

        if (Match(TokenType.Identifier))
            return new VariableExpr(Previous().Lexeme);

        if (Match(TokenType.LeftParen))
        {
            Expr inner = ParseExpression();
            Consume(TokenType.RightParen, "Expected ')'");
            return inner;
        }

        throw new Exception("Unexpected token in expression");
    }
}
