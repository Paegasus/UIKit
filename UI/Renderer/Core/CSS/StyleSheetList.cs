using UI.Renderer.Core.DOM;
using UI.Renderer.Core.HTML;
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

    public CSSStyleSheet? AnonymousNamedGetter(AtomicString name)
    {
        /*
        if (GetDocument() != null)
            UseCounter.Count(GetDocument()!, WebFeature.StyleSheetListAnonymousNamedGetter);
        */

        HTMLStyleElement? item = GetNamedItem(name);

        if (item == null)
            return null;

        CSSStyleSheet? sheet = item.Sheet;

        /*
        if (sheet != null)
            UseCounter.Count(GetDocument()!, WebFeature.StyleSheetListNonNullAnonymousNamedGetter);
        */

        return sheet;
        
    }
    
    //public bool NamedPropertyQuery(AtomicString str, ExceptionState state)
    public bool NamedPropertyQuery(AtomicString name)
    {
        return AnonymousNamedGetter(name) != null;
    }

    //public void Trace(Visitor*) const override;


    private List<StyleSheet> StyleSheets()
    {
        throw new NotImplementedException();
    }

    private TreeScope tree_scope_;
    private List<CSSStyleSheet> style_sheet_vector_;

    //const HeapVector<Member<StyleSheet>>& StyleSheets() const;
}
