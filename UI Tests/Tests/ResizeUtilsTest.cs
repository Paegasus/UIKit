using Xunit;

using UI.GFX.Geometry;

using static UI.GFX.Geometry.ResizeUtils;

namespace UI.Tests;

public static class ResizeUtilsTest
{
    // Aspect ratio is defined by width / height.
    const float kAspectRatioSquare = 1.0f;
    const float kAspectRatioHorizontal = 2.0f;
    const float kAspectRatioVertical = 0.5f;

    static readonly Size kMinSizeHorizontal = new(20, 10);
    static readonly Size kMaxSizeHorizontal = new(50, 25);
    static readonly Size kMinSizeVertical = new(10, 20);
    static readonly Size kMaxSizeVertical = new(25, 50);

    public static string HitTestToString(ResizeEdge resize_edge) => resize_edge switch
    {
        ResizeEdge.kTop => "top",
        ResizeEdge.kTopRight => "top-right",
        ResizeEdge.kRight => "right",
        ResizeEdge.kBottomRight => "bottom-right",
        ResizeEdge.kBottom => "bottom",
        ResizeEdge.kBottomLeft => "bottom-left",
        ResizeEdge.kLeft => "left",
        ResizeEdge.kTopLeft => "top-left",
        _ => throw new ArgumentOutOfRangeException(nameof(resize_edge))
    };

    record SizingParams(
        ResizeEdge ResizeEdge,
        float AspectRatio,
        Size MinSize,
        Size? MaxSize,
        Rect InputRect,
        Rect ExpectedOutputRect)
    {
        public override string ToString() =>
            $"{HitTestToString(ResizeEdge)} ratio={AspectRatio} " +
            $"[{MinSize}..{(MaxSize.HasValue ? MaxSize.Value.ToString() : "null")}] " +
            $"{InputRect} -> {ExpectedOutputRect}";
    }

    public static IEnumerable<object[]> SquareCases() =>
        kSizeRectToSquareAspectRatioTestCases.Select(p => new object[] { p });

    public static IEnumerable<object[]> HorizontalCases() =>
        kSizeRectToHorizontalAspectRatioTestCases.Select(p => new object[] { p });

    public static IEnumerable<object[]> VerticalCases() =>
        kSizeRectToVerticalAspectRatioTestCases.Select(p => new object[] { p });

    static readonly SizingParams[] kSizeRectToSquareAspectRatioTestCases =
    [
        // Dragging the top resizer up.
        new(ResizeEdge.kTop,         kAspectRatioSquare, kMinSizeHorizontal, kMaxSizeHorizontal, new(100, 98,  22, 24), new(100, 98,  24, 24)),
        
        // Dragging the bottom resizer down.
        new(ResizeEdge.kBottom,      kAspectRatioSquare, kMinSizeHorizontal, kMaxSizeHorizontal, new(100, 100, 22, 24), new(100, 100, 24, 24)),
        
        // Dragging the left resizer right.
        new(ResizeEdge.kLeft,        kAspectRatioSquare, kMinSizeHorizontal, kMaxSizeHorizontal, new(102, 100, 22, 24), new(102, 102, 22, 22)),
        
        // Dragging the right resizer left.
        new(ResizeEdge.kRight,       kAspectRatioSquare, kMinSizeHorizontal, kMaxSizeHorizontal, new(100, 100, 22, 24), new(100, 100, 22, 22)),
        
        // Dragging the top-left resizer right.
        new(ResizeEdge.kTopLeft,     kAspectRatioSquare, kMinSizeHorizontal, kMaxSizeHorizontal, new(102, 100, 22, 24), new(102, 102, 22, 22)),
        
        // Dragging the top-right resizer down.
        new(ResizeEdge.kTopRight,    kAspectRatioSquare, kMinSizeHorizontal, kMaxSizeHorizontal, new(100, 102, 24, 22), new(100, 102, 22, 22)),
        
        // Dragging the bottom-left resizer right.
        new(ResizeEdge.kBottomLeft,  kAspectRatioSquare, kMinSizeHorizontal, kMaxSizeHorizontal, new(100, 102, 22, 24), new(100, 102, 22, 22)),
        
        // Dragging the bottom-right resizer up.
        new(ResizeEdge.kBottomRight, kAspectRatioSquare, kMinSizeHorizontal, kMaxSizeHorizontal, new(100, 100, 24, 22), new(100, 100, 22, 22)),
        
        // Dragging the bottom-right resizer left.
        // Rect already as small as `kMinSizeHorizontal` allows.
        new(ResizeEdge.kBottomRight, kAspectRatioSquare, kMinSizeHorizontal, kMaxSizeHorizontal,
        new(100, 100, kMinSizeHorizontal.Width, kMinSizeHorizontal.Width),
        new(100, 100, kMinSizeHorizontal.Width, kMinSizeHorizontal.Width)),
        
        // Dragging the top-left resizer left.
        // Rect already as large as `kMaxSizeHorizontal` allows.
        new(ResizeEdge.kTopLeft,     kAspectRatioSquare, kMinSizeHorizontal, kMaxSizeHorizontal,
        new(100, 100, kMaxSizeHorizontal.Height, kMaxSizeHorizontal.Height),
        new(100, 100, kMaxSizeHorizontal.Height, kMaxSizeHorizontal.Height)),
        
        // Dragging the top-left resizer left.
        // No max size specified.
        new(ResizeEdge.kTopLeft,     kAspectRatioSquare, kMinSizeHorizontal, null, new(102, 100, 22, 24), new(102, 102, 22, 22)),
    ];

    static readonly SizingParams[] kSizeRectToHorizontalAspectRatioTestCases =
    [
        // Dragging the top resizer down.
        new(ResizeEdge.kTop,  kAspectRatioHorizontal, kMinSizeHorizontal, kMaxSizeHorizontal, new(100, 102, 48, 22), new(100, 102, 44, 22)),
        
        // Dragging the left resizer left.
        new(ResizeEdge.kLeft, kAspectRatioHorizontal, kMinSizeHorizontal, kMaxSizeHorizontal, new(96,  100, 48, 22), new(96,  98,  48, 24)),
        
        // Rect already as small as `kMinSizeHorizontal` allows.
        new(ResizeEdge.kTop,  kAspectRatioHorizontal, kMinSizeHorizontal, kMaxSizeHorizontal,
        new(100, 100, kMinSizeHorizontal.Width, kMinSizeHorizontal.Height),
        new(100, 100, kMinSizeHorizontal.Width, kMinSizeHorizontal.Height)),
        
        // Rect already as large as `kMaxSizeHorizontal` allows.
        new(ResizeEdge.kTop,  kAspectRatioHorizontal, kMinSizeHorizontal, kMaxSizeHorizontal,
        new(100, 100, kMaxSizeHorizontal.Width, kMaxSizeHorizontal.Height),
        new(100, 100, kMaxSizeHorizontal.Width, kMaxSizeHorizontal.Height)),
        
        // Dragging the left resizer left.
        // No max size specified.
        new(ResizeEdge.kLeft, kAspectRatioHorizontal, kMinSizeHorizontal, null, new(96, 100, 48, 22), new(96, 98, 48, 24)),
    ];

    static readonly SizingParams[] kSizeRectToVerticalAspectRatioTestCases =
    [
        // Dragging the bottom resizer up.
        new(ResizeEdge.kBottom, kAspectRatioVertical, kMinSizeVertical, kMaxSizeVertical, new(100, 100, 24, 44), new(100, 100, 22, 44)),
        
        // Dragging the right resizer right.
        new(ResizeEdge.kRight,  kAspectRatioVertical, kMinSizeVertical, kMaxSizeVertical, new(100, 100, 24, 44), new(100, 100, 24, 48)),
        
        // Rect already as small as `kMinSizeVertical` allows.
        new(ResizeEdge.kTop,    kAspectRatioVertical, kMinSizeVertical, kMaxSizeVertical,
        new(100, 100, kMinSizeVertical.Width, kMinSizeVertical.Height),
        new(100, 100, kMinSizeVertical.Width, kMinSizeVertical.Height)),
        
        // Rect already as large as `kMaxSizeVertical` allows.
        new(ResizeEdge.kTop,    kAspectRatioVertical, kMinSizeVertical, kMaxSizeVertical,
        new(100, 100, kMaxSizeVertical.Width, kMaxSizeVertical.Height),
        new(100, 100, kMaxSizeVertical.Width, kMaxSizeVertical.Height)),
        
        // Dragging the right resizer right.
        // No max size specified.
        new(ResizeEdge.kRight,  kAspectRatioVertical, kMinSizeVertical, null, new(100, 100, 24, 44), new(100, 100, 24, 48)),
    ];

    [Theory]
    [MemberData(nameof(SquareCases))]
    [MemberData(nameof(HorizontalCases))]
    [MemberData(nameof(VerticalCases))]
    private static void TestSizeRectToAspectRatio(SizingParams p)
    {
        Rect rect = p.InputRect;
        SizeRectToAspectRatio(p.ResizeEdge, p.AspectRatio, p.MinSize, p.MaxSize, ref rect);
        Assert.Equal(p.ExpectedOutputRect, rect);
    }

    [Theory]
    [MemberData(nameof(SquareCases))]
    [MemberData(nameof(HorizontalCases))]
    [MemberData(nameof(VerticalCases))]
    private static void TestSizeRectToAspectRatioWithExcludedMargin(SizingParams p)
    {
        Rect rect = p.InputRect;
        Size excluded_margin = new(2, 4);
        SizeRectToAspectRatioWithExcludedMargin(
            p.ResizeEdge, p.AspectRatio, p.MinSize, p.MaxSize, excluded_margin, ref rect);
        // With excluded margin, size should have the same aspect ratio once we remove
        // the margin.
        Size adjusted_size = rect.Size - excluded_margin;
        double actual_ratio = (double)adjusted_size.Width / adjusted_size.Height;
        // Note that all of the aspect ratios are exactly representable,
        // so `EQ` is really expected.
        Assert.Equal((double)p.AspectRatio, actual_ratio);
        // Also verify min / max.
        Assert.True(rect.Size.Width >= p.MinSize.Width);
        Assert.True(rect.Size.Height >= p.MinSize.Height);
        if (p.MaxSize.HasValue)
        {
            Assert.True(rect.Size.Width <= p.MaxSize.Value.Width);
            Assert.True(rect.Size.Height <= p.MaxSize.Value.Height);
        }
    }
}
