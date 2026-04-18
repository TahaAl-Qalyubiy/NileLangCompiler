using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace NileLangCompiler;

public class Scanner
{
    private static readonly string _pattern =
        @"(?<Comment>(//.*?$)|(/\*[\s\S]*?\*/))|" +
        @"(?<Keyword>\b(temple|reign|stone|water|papyrus|maat|judge|banish|flow|dynasty|carve|listen|tribute)\b)|" +
        @"(?<Float>\d+\.\d+)|" +
        @"(?<Integer>\d+)|" +
        @"(?<Identifier>[A-Za-z]\w*)|" +
        @"(?<Operator>(==|!=|>=|<=|\+\+|--|&&|\|\||[=><+\-!]))|" +
        @"(?<Symbol>[{};()])|" +
        @"(?<StringLiteral>""(?:[^""\\]|\\.)*"")|" +
        @"(?<Whitespace>\s+)|" +
        @"(?<Unknown>.)";

    private static readonly Regex _regex =
        new Regex(_pattern, RegexOptions.Compiled | RegexOptions.Multiline);

    public List<Token> Scan(string sourceCode)
    {
        var tokens = new List<Token>();
        int line = 1;

        var matches = _regex.Matches(sourceCode);

        foreach (Match match in matches)
        {
            if (match.Groups["Whitespace"].Success)
            {
                line += match.Value.Count(c => c == '\n');
                continue;
            }

            if (match.Groups["Comment"].Success)
                continue;

            TokenType type = TokenType.Unknown;
            string lexeme = match.Value;

            if (match.Groups["Keyword"].Success)
                Enum.TryParse(lexeme, true, out type);
            else if (match.Groups["Float"].Success)
                type = TokenType.Float;
            else if (match.Groups["Integer"].Success)
                type = TokenType.Integer;
            else if (match.Groups["Identifier"].Success)
                type = TokenType.Identifier;
            else if (match.Groups["StringLiteral"].Success)
                type = TokenType.StringLiteral;
            else if (match.Groups["Operator"].Success)
            {
                type = lexeme switch
                {
                    "==" => TokenType.Equals,
                    "!=" => TokenType.NotEquals,
                    ">=" => TokenType.GreaterOrEqual,
                    "<=" => TokenType.LessOrEqual,
                    "=" => TokenType.Assign,
                    ">" => TokenType.GreaterThan,
                    "<" => TokenType.LessThan,
                    "+" => TokenType.Plus,
                    "-" => TokenType.Minus,
                    "!" => TokenType.Not,
                    "&&" => TokenType.And,
                    "||" => TokenType.Or,
                    _ => TokenType.Unknown
                };
            }
            else if (match.Groups["Symbol"].Success)
            {
                type = lexeme switch
                {
                    "{" => TokenType.LeftBrace,
                    "}" => TokenType.RightBrace,
                    "(" => TokenType.LeftParen,
                    ")" => TokenType.RightParen,
                    ";" => TokenType.Semicolon,
                    _ => TokenType.Unknown
                };
            }
            else
            {
                Console.WriteLine($"[WARNING] Unknown: {lexeme}");
            }

            tokens.Add(new Token(type, lexeme, line));
        }

        tokens.Add(new Token(TokenType.EOF, "", line));
        return tokens;
    }
}