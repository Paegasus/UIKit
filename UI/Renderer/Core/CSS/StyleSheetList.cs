using UI.Renderer.Core.DOM;
using UI.Renderer.Framework.Text;

namespace UI.Renderer.Core.CSS;

public class StyleSheetList
{
    public StyleSheetList(TreeScope tree_scope)
    {
        tree_scope_ = tree_scope;

        //CHECK(tree_scope);
    }

    public uint length()
    {
        throw new NotImplementedException();
    }

    public StyleSheet item(uint index)
    {
        throw new NotImplementedException();
    }

    public HTMLStyleElement GetNamedItem(AtomicString str)
    {
        throw new NotImplementedException();
    }

    public Document GetDocument()
    {
        //return tree_scope_ ? tree_scope_.GetDocument() : null;
        throw new NotImplementedException();
    }

    public CSSStyleSheet AnonymousNamedGetter(AtomicString str)
    {
        throw new NotImplementedException();
    }

    public bool NamedPropertyQuery(AtomicString str, ExceptionState state)
    {
        throw new NotImplementedException();
    }

    //public void Trace(Visitor*) const override;


    private List<StyleSheet> StyleSheets()
    {
        throw new NotImplementedException();
    }

    private TreeScope tree_scope_;
    private List<CSSStyleSheet> style_sheet_vector_;

    /*
    const HeapVector<Member<StyleSheet>>& StyleSheets() const;

    private TreeScope tree_scope_;
    private List<CSSStyleSheet> style_sheet_vector_;
    
    
    */
}
