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

        /*
		Transform rotation_90_unrounded;
		rotation_90_unrounded.Rotate(90.0 + 1e-10);
		info = MaskFilterInfo(
		  RRectF(RectF(0, 0, 20.f, 25.f), 1.f, 2.f, 3.f, 4.f, 5.f, 6.f, 7.f, 8.f),
		  CreateGradient(50));
		EXPECT_TRUE(rotation_90_unrounded.Preserves2dAxisAlignment());
		info.ApplyTransform(rotation_90_unrounded);
		EXPECT_EQ(expected, info);

		auto scale = Transform::MakeScale(2.f, 3.f);
		info = MaskFilterInfo(
		  RRectF(RectF(0, 0, 20.f, 25.f), 1.f, 2.f, 3.f, 4.f, 5.f, 6.f, 7.f, 8.f),
		  CreateGradient(50));
		expected = MaskFilterInfo(RRectF(RectF(0, 0, 40.f, 75.f), 2.f, 6.f, 6.f, 12.f,
									   10.f, 18.f, 14.f, 24.f),
								CreateGradient(61));
		info.ApplyTransform(scale);
		EXPECT_EQ(expected, info);

		Transform rotation;
		rotation.Rotate(45);
		info.ApplyTransform(rotation);
		EXPECT_TRUE(info.IsEmpty());
        */
    }

    //[Fact]
    private static void TestApplyAxisTransform2D()
    {
        
    }
}
