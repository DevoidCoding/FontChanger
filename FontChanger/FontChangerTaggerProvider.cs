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
    [TagType(typeof(FontChangerTag))]
    public class FontChangerTaggerProvider : ITaggerProvider
    {
        private bool _reentrant;

        [Import]
        internal IClassifierAggregatorService ClassifierAggregatorService { get; set; }

        [Import]
        internal IBufferTagAggregatorFactoryService TagAggregatorFactory { get; set; }

        public ITagger<T> CreateTagger<T>(ITextBuffer buffer) where T : ITag
        {
            if (_reentrant)
                return null;

            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer));

            _reentrant = true;
            var classifierAggregator = ClassifierAggregatorService.GetClassifier(buffer);

            _reentrant = false;
            return new FontChangerTagger(buffer, classifierAggregator) as ITagger<T>;
        }
    }

    internal class FontChangerTagger : ITagger<FontChangerTag>, IDisposable
    {
        private ITextBuffer _buffer;
        private IClassifier _classifier;

        public FontChangerTagger(ITextBuffer buffer, IClassifier classifier)
        {
            _buffer = buffer;
            _classifier = classifier;

            _classifier.ClassificationChanged += ClassifierOnChanged;
        }

        private void ClassifierOnChanged(object sender, ClassificationChangedEventArgs e) => TagsChanged?.Invoke(this, new SnapshotSpanEventArgs(e.ChangeSpan));

        public IEnumerable<ITagSpan<FontChangerTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            if (_classifier == null || spans == null || spans.Count == 0)
                yield break;

            var snapshot = spans[0].Snapshot;
            
            foreach (var snapshotSpan in spans)
            {
                foreach (var classificationSpan in _classifier.GetClassificationSpans(snapshotSpan))
                {
                    var name = classificationSpan.ClassificationType.Classification.ToLowerInvariant();

                    if (name.Contains("keyword"))
                        yield return new TagSpan<FontChangerTag>(classificationSpan.Span, new FontChangerTag(FontChangerTokenType.Keyword));
                    else if (name.Contains("comment") || name.Contains("xml doc tag"))
                    {
                        yield return new TagSpan<FontChangerTag>(
                            classificationSpan.Span,
                            new FontChangerTag(FontChangerTokenType.Comment)
                        );
                    }
                }
            }
        }

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

        public void Dispose()
        {
            if (_classifier != null)
                _classifier.ClassificationChanged -= ClassifierOnChanged;
        }
    }
}