using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Media;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.Utilities;

namespace FontChanger
{
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "FontChangerKeyword")]
    [Name("FontChangerKeyword")]
    [UserVisible(true)]
    [Order(Before = Priority.Default)]
    internal sealed class FontChangerKeyword : ClassificationFormatDefinition
    {
        public FontChangerKeyword()
        {
            DisplayName = "FontChangerClassifier"; // Human readable version of the name
            var typeface = new Typeface(new FontFamily("Operator Mono"), FontStyles.Italic, FontWeights.Normal, FontStretches.Normal);
            FontTypeface = typeface;
        }
    }
}
