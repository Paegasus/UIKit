namespace UI.GFX.Geometry;

// RRectFBuilder makes setting RRectF parameters easier via a fluent API.
//
// Example: To build an RRectF at point(40, 50) with size(60, 70),
// with corner radii {(1,2),(3,4),(5,6),(7,8)}, use:
//
//  RRectF a = new RRectFBuilder()
//                  .SetOrigin(40, 50)
//                  .SetSize(60, 70)
//                  .SetUpperLeft(1, 2)
//                  .SetUpperRight(3, 4)
//                  .SetLowerRight(5, 6)
//                  .SetLowerLeft(7, 8)
//                  .Build();
public class RRectFBuilder
{
    private float _x, _y, _width, _height;
    private float _upperLeftX,  _upperLeftY;
    private float _upperRightX, _upperRightY;
    private float _lowerRightX, _lowerRightY;
    private float _lowerLeftX,  _lowerLeftY;

    public RRectFBuilder SetOrigin(float x, float y)
    {
        _x = x;
        _y = y;
        return this;
    }

    public RRectFBuilder SetOrigin(in PointF origin)
    {
        _x = origin.X;
        _y = origin.Y;
        return this;
    }

    public RRectFBuilder SetSize(float width, float height)
    {
        _width  = width;
        _height = height;
        return this;
    }

    public RRectFBuilder SetSize(in SizeF size)
    {
        _width  = size.Width;
        _height = size.Height;
        return this;
    }

    public RRectFBuilder SetRect(in RectF rect)
    {
        _x      = rect.X;
        _y      = rect.Y;
        _width  = rect.Width;
        _height = rect.Height;
        return this;
    }

    public RRectFBuilder SetRadius(float radius)
    {
        _upperLeftX  = _upperLeftY  = radius;
        _upperRightX = _upperRightY = radius;
        _lowerRightX = _lowerRightY = radius;
        _lowerLeftX  = _lowerLeftY  = radius;
        return this;
    }

    public RRectFBuilder SetRadius(float x_rad, float y_rad)
    {
        _upperLeftX  = _upperRightX  = _lowerRightX  = _lowerLeftX  = x_rad;
        _upperLeftY  = _upperRightY  = _lowerRightY  = _lowerLeftY  = y_rad;
        return this;
    }

    public RRectFBuilder SetUpperLeft(float x, float y)
    {
        _upperLeftX = x;
        _upperLeftY = y;
        return this;
    }

    public RRectFBuilder SetUpperRight(float x, float y)
    {
        _upperRightX = x;
        _upperRightY = y;
        return this;
    }

    public RRectFBuilder SetLowerRight(float x, float y)
    {
        _lowerRightX = x;
        _lowerRightY = y;
        return this;
    }

    public RRectFBuilder SetLowerLeft(float x, float y)
    {
        _lowerLeftX = x;
        _lowerLeftY = y;
        return this;
    }

    public RRectFBuilder SetCorners(in RoundedCornersF corners)
    {
        _upperLeftX  = _upperLeftY  = corners.UpperLeft;
        _upperRightX = _upperRightY = corners.UpperRight;
        _lowerRightX = _lowerRightY = corners.LowerRight;
        _lowerLeftX  = _lowerLeftY  = corners.LowerLeft;
        return this;
    }

    public RRectF Build() =>
        new RRectF(_x, _y, _width, _height,
                   _upperLeftX,  _upperLeftY,
                   _upperRightX, _upperRightY,
                   _lowerRightX, _lowerRightY,
                   _lowerLeftX,  _lowerLeftY);
}
