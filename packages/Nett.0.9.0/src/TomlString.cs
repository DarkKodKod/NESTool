﻿namespace Nett
{
    using System.Diagnostics;
    using Extensions;

    [DebuggerDisplay("{Value}")]
    public sealed class TomlString : TomlValue<string>
    {
        private readonly TypeOfString type;

        internal TomlString(ITomlRoot root, string value, TypeOfString type = TypeOfString.Default)
            : base(root, TrimFirstNewline(type, value))
        {
            this.type = type;
        }

        public enum TypeOfString
        {
            Default,
            Normal,
            Literal,
            Multiline,
            MultilineLiteral,
        }

        public override string ReadableTypeName => "string";

        public override TomlObjectType TomlType => TomlObjectType.String;

        public override void Visit(ITomlObjectVisitor visitor)
        {
            visitor.Visit(this);
        }

        internal override TomlObject CloneFor(ITomlRoot root) => this.CloneStringFor(root);

        internal TomlString CloneStringFor(ITomlRoot root) => CopyComments(new TomlString(root, this.Value), this);

        internal override TomlValue ValueWithRoot(ITomlRoot root) => this.StringWithRoot(root);

        internal override TomlObject WithRoot(ITomlRoot root) => this.StringWithRoot(root);

        internal TomlString StringWithRoot(ITomlRoot root)
        {
            root.CheckNotNull(nameof(root));

            return new TomlString(root, this.Value, this.type);
        }

        private static string TrimFirstNewline(TypeOfString type, string input)
        {
            return type == TypeOfString.Multiline || type == TypeOfString.MultilineLiteral
                ? input.TrimStart(new char[] { '\r' }).TrimStart(new char[] { '\n' })
                : input;
        }
    }
}
