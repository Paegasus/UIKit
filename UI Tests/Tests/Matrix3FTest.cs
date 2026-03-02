using Xunit;

using UI.GFX.Geometry;
using System.Diagnostics;

namespace UI.Tests;

public static class Matrix3FTest
{
    [Fact]
    private static void TestConstructors()
    {
        Matrix3F zeros = Matrix3F.Zeros();
        Matrix3F ones = Matrix3F.Ones();
        Matrix3F identity = Matrix3F.Identity();

        Matrix3F product_ones = Matrix3F.FromOuterProduct(new Vector3DF(1.0f, 1.0f, 1.0f), new Vector3DF(1.0f, 1.0f, 1.0f));
        Matrix3F product_zeros = Matrix3F.FromOuterProduct(new Vector3DF(1.0f, 1.0f, 1.0f), new Vector3DF(0.0f, 0.0f, 0.0f));
        Assert.Equal(ones, product_ones);
        Assert.Equal(zeros, product_zeros);

        for (int i = 0; i < 3; ++i)
        {
            for (int j = 0; j < 3; ++j)
                Assert.Equal(i == j ? 1.0f : 0.0f, identity.Get(i, j));
        }
    }

    private static void Test()
    {

    }
}
