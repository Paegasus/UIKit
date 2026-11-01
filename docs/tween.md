Ported `Tween` and its full upstream test suite.

**`UI/GFX/Animation/Tween.cs`** — replaces the 10-line stub with the complete `tween.h`/`tween.cc` port:

- All 33 `Tween::Type` values as nested `Tween.TweenType` (matching the existing `TransformOperation.TransformOperationType` precedent), including the `ACCEL_*_DECEL_*` curves.
- `CalculateValue` with every branch: `pow` curves, the `ThreePointCubicBezier` emphasized curve, and 23 `CubicBezier` curves held as `static readonly` instances.
- `ColorValueBetween` for both `SKColor` and `SKColorF`, with the premultiplied `BlendColorComponents(Float)` helpers and `base::ClampRound<uint8_t>` → `SafeConversions.ClampRound`.
- `DoubleValueBetween`, `FloatValueBetween`, `ClampedFloatValueBetween`, `IntValueBetween`, `LinearIntValueBetween`.
- `RectValueBetween`, `RectFValueBetween`, `TransformValueBetween`, `TransformOperationsValueBetween`, `SizeValueBetween`, `SizeFValueBetween`.

**Mapping choices**

| C++ | C# |
|---|---|
| `base::TimeTicks` | `TimeSpan` (monotonic timestamp) |
| `nextafter(delta, 0)` / `_nextafter` | `Math.BitDecrement`/`BitIncrement` (same one-ULP-toward-zero step, no platform fork) |
| `SkColor` / `SkColor4f` | `SKColor` / `SKColorF` |
| `DCHECK_GE(state, 0)` | `Debug.Assert` |
| `NOTREACHED()` fallthrough | `ArgumentOutOfRangeException` |

**`UI Tests/Tests/TweenTest.cs`** — 21 tests: every upstream `tween_unittest.cc` case (color tables, all three `IntValueBetween` directions, linear int rounding, `TimeTicks` clamping, corner-based `RectValueBetween`, `Size`/`SizeF` clamped extrapolation), plus new coverage for `RectFValueBetween`, `TransformValueBetween`, `TransformOperationsValueBetween`, `SKColorF` alpha/premultiply behavior, curve endpoints, and monotonicity across every `TweenType`.

dotnet test UI.slnx → 579 passed, 0 failed (558 existing + 21 new)


Two upstream quirks are preserved deliberately and documented in the tests: `SmoothInOut` returns `sin(state)` (so it reaches `sin(1)`, not `1`), and `Zero` always returns `0`.

Next up the stack would be `animation.h`/`animation.cc`, then `linear_animation` — everything else (`throb_animation`, `slide_animation`, `multi_animation`, `animation_container`) builds on `Animation`. Want me to take `Animation` + `LinearAnimation` next?
