using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;

namespace FontChanger
{
    public class FontChangerTag : ITag
    {
        public FontChangerTokenType Token { get; set; }
        public FontChangerTag(FontChangerTokenType token)
        {
            Token = token;
        }
    }
}