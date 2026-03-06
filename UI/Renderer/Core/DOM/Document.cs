namespace UI.Renderer.Core.DOM;

public class Document
{
    enum StyleAndLayoutTreeUpdate
    {
        // Style/layout-tree is not dirty.
        kNone,

        // Style/layout-tree is dirty, and it's possible to understand whether a
        // given element will be affected or not by analyzing its ancestor chain.
        kAnalyzed,

        // Style/layout-tree is dirty, but we cannot decide which specific elements
        // need to have its style or layout tree updated.
        kFull,
    }

    public Element ActiveElement()
    {
        throw new NotImplementedException();
    }

    public bool hasFocus()
    {
        throw new NotImplementedException();
    }
}
