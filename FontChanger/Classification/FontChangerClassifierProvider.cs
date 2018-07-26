using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Primitives;
using System.Net.Mime;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;

namespace FontChanger
{
    [Export(typeof(ITaggerProvider))]
    [ContentType("code")]
    [TagType(typeof(ClassificationTag))]
    public class FontChangerClassifierProvider : ITaggerProvider
    {
        [Import]
        internal IClassificationTypeRegistryService ClassificationTypeRegistry { get; set; }

        [Import] internal IBufferTagAggregatorFactoryService BufferTagAggregatorFactory { get; set; }

        public ITagger<T> CreateTagger<T>(ITextBuffer buffer) where T : ITag
        {
            var taggerAggregator = BufferTagAggregatorFactory.CreateTagAggregator<FontChangerTag>(buffer);

            return new FontChangerClassifier(buffer, taggerAggregator, ClassificationTypeRegistry) as ITagger<T>;
        }
    }

    internal class FontChangerClassifier : ITagger<ClassificationTag>
    {
        private readonly ITextBuffer _buffer;
        private readonly ITagAggregator<FontChangerTag> _taggerAggregator;
        private Dictionary<FontChangerTokenType, IClassificationType> _fontChangerTypes;

        public FontChangerClassifier(ITextBuffer buffer, ITagAggregator<FontChangerTag> taggerAggregator, IClassificationTypeRegistryService classificationTypeRegistry)
        {
            _buffer = buffer;
            _taggerAggregator = taggerAggregator;

            _fontChangerTypes = new Dictionary<FontChangerTokenType, IClassificationType>()
            {
                { FontChangerTokenType.Keyword, classificationTypeRegistry.GetClassificationType("FontChangerKeyword") },
                { FontChangerTokenType.Comment, classificationTypeRegistry.GetClassificationType("FontChangerKeyword") },
            };
        }

        public IEnumerable<ITagSpan<ClassificationTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            foreach (var tagSpan in _taggerAggregator.GetTags(spans))
            {
                var tagSpans = tagSpan.Span.GetSpans(spans[0].Snapshot);
                yield return
                    new TagSpan<ClassificationTag>(tagSpans[0],
                        new ClassificationTag(_fontChangerTypes[tagSpan.Tag.Token]));
            }
        }

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;
    }
}