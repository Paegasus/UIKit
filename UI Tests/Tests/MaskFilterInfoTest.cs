using Xunit;

using UI.GFX.Geometry;
using System.Diagnostics;

namespace UI.Tests;

public static class MaskFilterInfoTest
{

    private static LinearGradient CreateGradient(short angle)
    {
        LinearGradient gradient = new(angle);
        gradient.AddStep(0.5f, 50);
        return gradient;
    }

    [Fact]
    private static void TestApplyTransform()
    {
        MaskFilterInfo info = new(new RRectF(1.0f, 2.0f, 20.0f, 25.0f, 5.0f));
  		MaskFilterInfo expected = info;
		info.ApplyTransform(new Transform());
		Assert.Equal(expected, info);
        
		var translation = Transform.MakeTranslation(-3.5f, 7.75f);
		expected = new MaskFilterInfo(new RRectF(-2.5f, 9.75f, 20.0f, 25.0f, 5.0f));
		info.ApplyTransform(translation);
		Assert.Equal(expected, info);

		info = new MaskFilterInfo(new RRectF(1.0f, 2.0f, 20.0f, 25.0f, 5.0f), CreateGradient(50));
		expected =
		  new MaskFilterInfo(new RRectF(-2.5f, 9.75f, 20.0f, 25.0f, 5.0f), CreateGradient(50));
		info.ApplyTransform(translation);
		Assert.Equal(expected, info);

		var rotation_90_clock = Transform.Make90degRotation();
		info = new MaskFilterInfo(
		  new RRectF(new RectF(0, 0, 20.0f, 25.0f), 1.0f, 2.0f, 3.0f, 4.0f, 5.0f, 6.0f, 7.0f, 8.0f),
		  CreateGradient(50));
		expected = new MaskFilterInfo(new RRectF(new RectF(-25.0f, 0, 25.0f, 20.0f), 8.0f, 7.0f, 2.0f,
									   1.0f, 4.0f, 3.0f, 6.0f, 5.0f), CreateGradient(-40));
		info.ApplyTransform(rotation_90_clock);
		Assert.Equal(expected, info);

		Transform rotation_90_unrounded = new();
		rotation_90_unrounded.Rotate(90.0 + 1e-10);
		info = new MaskFilterInfo(
		  new RRectF(new RectF(0, 0, 20.0f, 25.0f), 1.0f, 2.0f, 3.0f, 4.0f, 5.0f, 6.0f, 7.0f, 8.0f),
		  CreateGradient(50));
		Assert.True(rotation_90_unrounded.Preserves2dAxisAlignment());
		info.ApplyTransform(rotation_90_unrounded);
		Assert.Equal(expected, info);

		var scale = Transform.MakeScale(2.0f, 3.0f);
		info = new MaskFilterInfo(
		  new RRectF(new RectF(0, 0, 20.0f, 25.0f), 1.0f, 2.0f, 3.0f, 4.0f, 5.0f, 6.0f, 7.0f, 8.0f),
		  CreateGradient(50));
		expected = new MaskFilterInfo(new RRectF(new RectF(0, 0, 40.0f, 75.0f), 2.0f, 6.0f, 6.0f, 12.0f,
									   10.0f, 18.0f, 14.0f, 24.0f),
								CreateGradient(61));
		info.ApplyTransform(scale);
		Assert.Equal(expected, info);
        
		Transform rotation = new();
		rotation.Rotate(45);
		info.ApplyTransform(rotation);
		Assert.True(info.IsEmpty());
    }

    [Fact]
    private static void TestApplyAxisTransform2D()
    {
        var info = new MaskFilterInfo(new RRectF(new RectF(0, 0, 20.0f, 25.0f), 1.0f, 2.0f, 3.0f, 4.0f, 5.0f, 6.0f, 7.0f, 8.0f), CreateGradient(50));
        MaskFilterInfo expected = info;
        info.ApplyTransform(new AxisTransform2D());
        Assert.Equal(expected, info);

        MaskFilterInfo scaled = info;
        expected = new MaskFilterInfo(new RRectF(new RectF(0, 0, 40.0f, 75.0f), 2.0f, 6.0f, 6.0f, 12.0f, 10.0f, 18.0f, 14.0f, 24.0f), CreateGradient(61));
        scaled.ApplyTransform(AxisTransform2D.FromScaleAndTranslation(new Vector2DF(2.0f, 3.0f), new Vector2DF()));
        Assert.Equal(expected, scaled);

        /*
        MaskFilterInfo scaled_translated = scaled;
        expected = new MaskFilterInfo(new RRectF(new RectF(-3.5f, 7.75f, 40.0f, 75.0f), 2.0f, 6.0f, 6.0f, 12.0f, 10.0f, 18.0f, 14.0f, 24.0f), CreateGradient(61));
        scaled_translated.ApplyTransform(AxisTransform2D.FromScaleAndTranslation(new Vector2DF(1.0f, 1.0f), new Vector2DF(-3.5f, 7.75f)));
        Assert.Equal(expected, scaled_translated);

        MaskFilterInfo scaled_translated_2 = info;
        scaled_translated_2.ApplyTransform(AxisTransform2D.FromScaleAndTranslation(new Vector2DF(2.0f, 3.0f), new Vector2DF(-3.5f, 7.75f)));
        Assert.Equal(expected, scaled_translated_2);

        const float kInf = float.PositiveInfinity;
        const float kNan = float.NaN;

        bool failure_is_empty(in AxisTransform2D transform)
        {
            MaskFilterInfo transformed = info;
            transformed.ApplyTransform(transform);
            return transformed.IsEmpty();
        }

        Assert.True(failure_is_empty(AxisTransform2D.FromScaleAndTranslation(new Vector2DF(kInf, 1), new Vector2DF(1, 1))));
        Assert.True(failure_is_empty(AxisTransform2D.FromScaleAndTranslation(new Vector2DF(kNan, 1), new Vector2DF(1, 1))));
        Assert.True(failure_is_empty(AxisTransform2D.FromScaleAndTranslation(new Vector2DF(1, 1), new Vector2DF(1, kInf))));
        Assert.True(failure_is_empty(AxisTransform2D.FromScaleAndTranslation(new Vector2DF(1, 1), new Vector2DF(1, kNan))));
        */
    }
}
