namespace UI.Renderer.Core.CSS;


public enum  RenderBlockingBehavior
{
  kUnset,                 // Render blocking value was not set.
  kBlocking,              // Render Blocking resource.
  kNonBlocking,           // Non-blocking resource.
  kNonBlockingDynamic,    // Dynamically injected non-blocking resource.
  kPotentiallyBlocking,   // Dynamically injected non-blocking resource.
  kInBodyParserBlocking,  // Blocks parser below element declaration.
};