// MediaQuerySet objects are immutable, for caching purposes.
// However, CSSOM (MediaList) offers an API to mutate the underlying media queries,
// so we fulfill that API by instead replacing the entire MediaQuerySet upon mutation. 
// Since MediaList does not own the MediaQuerySet it is mutating (replacing),
// MediaList instead holds a reference to the object that does (a MediaQuerySetOwner).
// This way the MediaQuerySet can be replaced at the source.

namespace UI.Renderer.Core.CSS;

public interface IMediaQuerySetOwner
{
    MediaQuerySet? MediaQueries { get; }
    void SetMediaQueries(MediaQuerySet? mediaQueries);
}
