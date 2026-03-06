using UI.Renderer.Core.DOM;

namespace UI.Renderer.Core.CSS;

public class StyleSheetList
{
    TreeScope tree_scope_;
    
    public StyleSheetList(TreeScope tree_scope)
    {
        tree_scope_ = tree_scope;

        //CHECK(tree_scope);
    }
}
