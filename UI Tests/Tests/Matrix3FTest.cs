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

    [Fact]
    private static void TestDataAccess()
    {
        Matrix3F matrix = Matrix3F.Ones();
        Matrix3F identity = Matrix3F.Identity();

        Assert.Equal(new Vector3DF(0.0f, 1.0f, 0.0f), identity.GetColumn(1));
        Assert.Equal(new Vector3DF(0.0f, 1.0f, 0.0f), identity.GetRow(1));
        matrix.Set(0.0f, 1.0f, 2.0f, 3.0f, 4.0f, 5.0f, 6.0f, 7.0f, 8.0f);
        Assert.Equal(new Vector3DF(2.0f, 5.0f, 8.0f), matrix.GetColumn(2));
        Assert.Equal(new Vector3DF(6.0f, 7.0f, 8.0f), matrix.GetRow(2));
        matrix.SetColumn(0, new Vector3DF(0.1f, 0.2f, 0.3f));
        matrix.SetColumn(0, new Vector3DF(0.1f, 0.2f, 0.3f));
        Assert.Equal(new Vector3DF(0.1f, 0.2f, 0.3f), matrix.GetColumn(0));
        Assert.Equal(new Vector3DF(0.1f, 1.0f, 2.0f), matrix.GetRow(0));

        Assert.Equal(0.1f, matrix.Get(0, 0));
        Assert.Equal(5.0f, matrix.Get(1, 2));
    }

    [Fact]
    private static void TestDeterminant()
    {
        Assert.Equal(1.0f, Matrix3F.Identity().Determinant());
        Assert.Equal(0.0f, Matrix3F.Zeros().Determinant());
        Assert.Equal(0.0f, Matrix3F.Ones().Determinant());

        // Now for something non-trivial...
        Matrix3F matrix = Matrix3F.Zeros();
        matrix.Set(0, 5, 6, 8, 7, 0, 1, 9, 0);
        Assert.Equal(390.0f, matrix.Determinant());
        matrix.Set(2, 0, 3 * matrix.Get(0, 0));
        matrix.Set(2, 1, 3 * matrix.Get(0, 1));
        matrix.Set(2, 2, 3 * matrix.Get(0, 2));
        Assert.Equal(0, matrix.Determinant());

        matrix.Set(0.57f, 0.205f, 0.942f,
                   0.314f, 0.845f, 0.826f,
                   0.131f, 0.025f, 0.962f);
        Assert.Equal(0.3149f, matrix.Determinant(), 0.0001f);
    }

    private static void Test1()
    {

    }

    private static void Test2()
    {

    }
}
