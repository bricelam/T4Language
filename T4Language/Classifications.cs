using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace T4Language;

static class Classifications
{
#pragma warning disable 649
    [Export]
    [Name("T4 Argument")]
    public static readonly ClassificationTypeDefinition Argument;

    [Export]
    [Name("T4 Argument Value")]
    public static readonly ClassificationTypeDefinition ArgumentValue;

    [Export]
    [Name("T4 Delimiter")]
    public static readonly ClassificationTypeDefinition Delimiter;

    [Export]
    [Name("T4 Directive")]
    public static readonly ClassificationTypeDefinition Directive;

    [Export]
    [Name("T4 Operator")]
    public static readonly ClassificationTypeDefinition Operator;
#pragma warning restore 649

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "T4 Argument")]
    [Name("T4 Argument")]
    [UserVisible(true)]
    class ArgumentFormat : ClassificationFormatDefinition
    {
        public ArgumentFormat() { }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "T4 Argument Value")]
    [Name("T4 Argument Value")]
    [UserVisible(true)]
    class ArgumentValueFormat : ClassificationFormatDefinition
    {
        public ArgumentValueFormat() { }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "T4 Delimiter")]
    [Name("T4 Delimiter")]
    [UserVisible(true)]
    class DelimiterFormat : ClassificationFormatDefinition
    {
        public DelimiterFormat() { }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "T4 Directive")]
    [Name("T4 Directive")]
    [UserVisible(true)]
    class DirectiveFormat : ClassificationFormatDefinition
    {
        public DirectiveFormat() { }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "T4 Operator")]
    [Name("T4 Operator")]
    [UserVisible(true)]
    class OperatorFormat : ClassificationFormatDefinition
    {
        public OperatorFormat() { }
    }
}
