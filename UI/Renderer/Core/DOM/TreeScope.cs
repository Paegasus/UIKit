namespace UI.Renderer.Core.DOM;

// The root node of a document tree (in which case this is a Document)
// or of a shadow tree (in which case this is a ShadowRoot).
// Various things, like element IDs, are scoped to the TreeScope in which they are rooted, if any.
//
// A class which inherits both Node and TreeScope must call clearRareData() in its destructor
// so that the Node destructor no longer does problematic NodeList cache manipulation in the destructor.
public class TreeScope
{
    enum HitTestPointType
    {
        kInternal = 1 << 1,
        kWebExposed = 1 << 2,
    }
}
