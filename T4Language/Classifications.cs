﻿using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;
using System.ComponentModel.Composition;
using System.Windows.Media;

namespace T4Language
{
    static class Classifications
    {
#pragma warning disable 649
        [Export]
        [Name("T4 Attribute")]
        public static readonly ClassificationTypeDefinition Attribute;

        [Export]
        [Name("T4 Attribute Value")]
        public static readonly ClassificationTypeDefinition AttributeValue;

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
        [ClassificationType(ClassificationTypeNames = "T4 Attribute")]
        [Name("T4 Attribute")]
        [UserVisible(true)]
        class AttributeFormat : ClassificationFormatDefinition
        {
            public AttributeFormat()
            {
                ForegroundColor = Colors.Red;
            }
        }

        [Export(typeof(EditorFormatDefinition))]
        [ClassificationType(ClassificationTypeNames = "T4 Attribute Value")]
        [Name("T4 Attribute Value")]
        [UserVisible(true)]
        class AttributeValueFormat : ClassificationFormatDefinition
        {
            public AttributeValueFormat()
            {
                ForegroundColor = Colors.Blue;
            }
        }

        [Export(typeof(EditorFormatDefinition))]
        [ClassificationType(ClassificationTypeNames = "T4 Delimiter")]
        [Name("T4 Delimiter")]
        [UserVisible(true)]
        class DelimiterFormat : ClassificationFormatDefinition
        {
            public DelimiterFormat()
            {
                BackgroundColor = Colors.Yellow;
                ForegroundColor = Colors.Black;
            }
        }

        [Export(typeof(EditorFormatDefinition))]
        [ClassificationType(ClassificationTypeNames = "T4 Directive")]
        [Name("T4 Directive")]
        [UserVisible(true)]
        class DirectiveFormat : ClassificationFormatDefinition
        {
            public DirectiveFormat()
            {
                ForegroundColor = Colors.Maroon;
            }
        }

        [Export(typeof(EditorFormatDefinition))]
        [ClassificationType(ClassificationTypeNames = "T4 Operator")]
        [Name("T4 Operator")]
        [UserVisible(true)]
        class OperatorFormat : ClassificationFormatDefinition
        {
            public OperatorFormat()
            {
                ForegroundColor = Colors.Blue;
            }
        }
    }
}
