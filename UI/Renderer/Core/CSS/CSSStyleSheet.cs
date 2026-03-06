using UI.Renderer.Core.DOM;

namespace UI.Renderer.Core.CSS;

public sealed class CSSStyleSheet : StyleSheet, IMediaQuerySetOwner
{
    // StyleSheet implementation
    
    public override bool Disabled { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public override Node? OwnerNode => throw new NotImplementedException();

    public override string Href => throw new NotImplementedException();

    public override string Title => throw new NotImplementedException();

    public override string Type => throw new NotImplementedException();

    public override Uri BaseUrl => throw new NotImplementedException();

    public override bool IsLoading => throw new NotImplementedException();

    public override void ClearOwnerNode()
    {
        throw new NotImplementedException();
    }

    // IMediaQuerySetOwner implementation

    public MediaQuerySet? MediaQueries => throw new NotImplementedException();

    public void SetMediaQueries(MediaQuerySet? mediaQueries)
    {
        throw new NotImplementedException();
    }
}
