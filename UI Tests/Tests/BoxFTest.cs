using Xunit;

using UI.GFX.Geometry;

namespace UI.Tests;

public static class BoxFTest
{
    [Fact]
    private static void TestConstructors()
    {
        Assert.Equal(new BoxF(0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f).ToString(), new BoxF().ToString());
        Assert.Equal(new BoxF(0.0f, 0.0f, 0.0f, -3.0f, -5.0f, -7.0f).ToString(), new BoxF().ToString());

        Assert.Equal(new BoxF(0.0f, 0.0f, 0.0f, 3.0f, 5.0f, 7.0f).ToString(), new BoxF(3.0f, 5.0f, 7.0f).ToString());
        Assert.Equal(new BoxF(0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f).ToString(), new BoxF(-3.0f, -5.0f, -7.0f).ToString());

        Assert.Equal(new BoxF(2.0f, 4.0f, 6.0f, 3.0f, 5.0f, 7.0f).ToString(), new BoxF(new Point3F(2.0f, 4.0f, 6.0f), 3.0f, 5.0f, 7.0f).ToString());
        Assert.Equal(new BoxF(2.0f, 4.0f, 6.0f, 0.0f, 0.0f, 0.0f).ToString(), new BoxF(new Point3F(2.0f, 4.0f, 6.0f), -3.0f, -5.0f, -7.0f).ToString());
    }

    [Fact]
    private static void TestIsEmpty()
    {
        Assert.True(new BoxF(0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f).IsEmpty);
        Assert.True(new BoxF(1.0f, 2.0f, 3.0f, 0.0f, 0.0f, 0.0f).IsEmpty);

        Assert.True(new BoxF(0.0f, 0.0f, 0.0f, 2.0f, 0.0f, 0.0f).IsEmpty);
        Assert.True(new BoxF(1.0f, 2.0f, 3.0f, 2.0f, 0.0f, 0.0f).IsEmpty);
        Assert.True(new BoxF(0.0f, 0.0f, 0.0f, 0.0f, 2.0f, 0.0f).IsEmpty);
        Assert.True(new BoxF(1.0f, 2.0f, 3.0f, 0.0f, 2.0f, 0.0f).IsEmpty);
        Assert.True(new BoxF(0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 2.0f).IsEmpty);
        Assert.True(new BoxF(1.0f, 2.0f, 3.0f, 0.0f, 0.0f, 2.0f).IsEmpty);

        Assert.False(new BoxF(0.0f, 0.0f, 0.0f, 0.0f, 2.0f, 2.0f).IsEmpty);
        Assert.False(new BoxF(1.0f, 2.0f, 3.0f, 0.0f, 2.0f, 2.0f).IsEmpty);
        Assert.False(new BoxF(0.0f, 0.0f, 0.0f, 2.0f, 0.0f, 2.0f).IsEmpty);
        Assert.False(new BoxF(1.0f, 2.0f, 3.0f, 2.0f, 0.0f, 2.0f).IsEmpty);
        Assert.False(new BoxF(0.0f, 0.0f, 0.0f, 2.0f, 2.0f, 0.0f).IsEmpty);
        Assert.False(new BoxF(1.0f, 2.0f, 3.0f, 2.0f, 2.0f, 0.0f).IsEmpty);

        Assert.False(new BoxF(0.0f, 0.0f, 0.0f, 2.0f, 2.0f, 2.0f).IsEmpty);
        Assert.False(new BoxF(1.0f, 2.0f, 3.0f, 2.0f, 2.0f, 2.0f).IsEmpty);
    }

    [Fact]
    private static void TestUnion()
    {
        BoxF empty_box = new();
        BoxF box1 = new(0.0f, 0.0f, 0.0f, 1.0f, 1.0f, 1.0f);
        BoxF box2 = new(0.0f, 0.0f, 0.0f, 4.0f, 6.0f, 8.0f);
        BoxF box3 = new(3.0f, 4.0f, 5.0f, 6.0f, 4.0f, 0.0f);

        Assert.Equal(empty_box.ToString(), BoxF.UnionBoxes(empty_box, empty_box).ToString());
        Assert.Equal(box1.ToString(), BoxF.UnionBoxes(empty_box, box1).ToString());
        Assert.Equal(box1.ToString(), BoxF.UnionBoxes(box1, empty_box).ToString());
        Assert.Equal(box2.ToString(), BoxF.UnionBoxes(empty_box, box2).ToString());
        Assert.Equal(box2.ToString(), BoxF.UnionBoxes(box2, empty_box).ToString());
        Assert.Equal(box3.ToString(), BoxF.UnionBoxes(empty_box, box3).ToString());
        Assert.Equal(box3.ToString(), BoxF.UnionBoxes(box3, empty_box).ToString());

        // box_1 is contained in box_2
        Assert.Equal(box2.ToString(), BoxF.UnionBoxes(box1, box2).ToString());
        Assert.Equal(box2.ToString(), BoxF.UnionBoxes(box2, box1).ToString());

        // box_1 and box_3 are disjoint
        Assert.Equal(new BoxF(0.0f, 0.0f, 0.0f, 9.0f, 8.0f, 5.0f).ToString(), BoxF.UnionBoxes(box1, box3).ToString());
        Assert.Equal(new BoxF(0.0f, 0.0f, 0.0f, 9.0f, 8.0f, 5.0f).ToString(), BoxF.UnionBoxes(box3, box1).ToString());

        // box_2 and box_3 intersect, but neither contains the other
        Assert.Equal(new BoxF(0.0f, 0.0f, 0.0f, 9.0f, 8.0f, 8.0f).ToString(), BoxF.UnionBoxes(box2, box3).ToString());
        Assert.Equal(new BoxF(0.0f, 0.0f, 0.0f, 9.0f, 8.0f, 8.0f).ToString(), BoxF.UnionBoxes(box3, box2).ToString());
    }

    [Fact]
    private static void TestExpandTo()
    {
        
    }
}
