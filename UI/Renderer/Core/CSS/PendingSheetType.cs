namespace UI.Renderer.Core.CSS;

// TODO(xiaochengh): This enum is almost identical to RenderBlockingBehavior.
// Try to merge them.
public enum PendingSheetType
{
  // Not a pending sheet, hasn't started or already finished
  kNone,
  // Pending but does not block anything
  kNonBlocking,
  // Dynamically inserted render-blocking but not script-blocking sheet
  kDynamicRenderBlocking,
  // Parser-inserted sheet that by default blocks scripts. Also blocks rendering
  // if in head, or blocks parser if in body.
  kBlocking
};
