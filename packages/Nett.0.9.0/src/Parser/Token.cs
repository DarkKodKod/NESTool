﻿namespace Nett.Parser
{
    using System.Diagnostics;
    using System.Text;

    internal enum TokenType
    {
        Unknown,
        NewLine,
        Eof,

        BareKey,
        Comment,
        Dot,
        Key,
        Assign,
        Comma,

        LBrac,
        RBrac,
        LCurly,
        RCurly,

        Integer,
        HexInteger,
        OctalInteger,
        BinaryInteger,
        Float,
        Bool,
        String,
        LiteralString,
        MultilineString,
        MultilineLiteralString,
        Date,
        LocalTime,
        DateTime,
        Duration,
    }

    [DebuggerDisplay("{value}:{type}")]
    internal struct Token
    {
#pragma warning disable SA1307 // Accessible fields must begin with upper-case letter
        public int col;
        public int line;
        public TokenType type;
        public string value;
        public string errorHint;
#pragma warning restore SA1307 // Accessible fields must begin with upper-case letter

        public Token(TokenType type, string value)
        {
            this.type = type;
            this.value = value;
            this.line = 0;
            this.col = 0;
            this.errorHint = null;
        }

        public bool IsEmpty => this.value == null || this.value.Trim().Length <= 0;

        public bool IsEof => this.type == TokenType.Eof;

        public bool IsNewLine => this.type == TokenType.NewLine;

        public static Token Unknown(string value, string hint, int line, int col)
        {
            return new Token(TokenType.Unknown, value)
            {
                errorHint = hint,
                line = line,
                col = col,
            };
        }

        public static Token CreateUnknownTokenFromFragment(CharBuffer cs, StringBuilder fragment)
        {
            while (!cs.TokenDone()) { fragment.Append(cs.Consume()); }

            return new Token(TokenType.Unknown, fragment.ToString());
        }

        public static Token NewLine(int line, int col) => new Token(TokenType.NewLine, "<NewLine>") { line = line, col = col };

        public static Token EndOfFile(int line, int col) => new Token(TokenType.Eof, "<EndOfFile>") { line = line, col = col };

        public override string ToString()
            => $"{this.value}:{this.type}";
    }
}
