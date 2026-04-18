using NileLangCompiler;

class Program
{
    static void Main()
    {
        string sourceCode = @"
            reign myProgram {
                stone x = 5;
                stone y = 10;
                x + y;
            }
        ";

        Console.WriteLine("=== SCANNING ===");

        var scanner = new Scanner();
        var tokens = scanner.Scan(sourceCode);

        foreach (var token in tokens)
            Console.WriteLine(token);

        Console.WriteLine("\n=== PARSING (AST) ===");

        var parser = new Parser(tokens);
        ProgramAst ast = parser.ParseProgram();

        ASTPrinter.Print(ast);
    }
}
