void RectF::operator+=(const Vector2dF& offset) {
  origin_ += offset;
}

void RectF::operator-=(const Vector2dF& offset) {
  origin_ -= offset;
}

bool RectF::operator<(const RectF& other) const {
  if (origin_ != other.origin_)
    return origin_ < other.origin_;

  if (width() == other.width())
    return height() < other.height();
  return width() < other.width();
}





RectF UnionRectsEvenIfEmpty(const RectF& a, const RectF& b) {
  RectF result = a;
  result.UnionEvenIfEmpty(b);
  return result;
}

RectF SubtractRects(const RectF& a, const RectF& b) {
  RectF result = a;
  result.Subtract(b);
  return result;
}

// Construct a rectangle with top-left corner at |p1| and bottom-right corner
// at |p2|. If the exact result of top - bottom or left - right cannot be
// presented in float, then the height/width will be grown to the next
// float, so that it includes both |p1| and |p2|.
RectF BoundingRect(const PointF& p1, const PointF& p2) {
  float left = std::min(p1.x(), p2.x());
  float top = std::min(p1.y(), p2.y());
  float right = std::max(p1.x(), p2.x());
  float bottom = std::max(p1.y(), p2.y());
  float width = right - left;
  float height = bottom - top;

  // If the precision is lost during the calculation, always grow to the next
  // value to include both ends.
  if (left + width != right) {
    width = std::nextafter((width), std::numeric_limits<float>::infinity());
    if (std::isinf(width)) {
      width = std::numeric_limits<float>::max();
    }
  }
  if (top + height != bottom) {
    height = std::nextafter((height), std::numeric_limits<float>::infinity());
    if (std::isinf(height)) {
      height = std::numeric_limits<float>::max();
    }
  }

  return RectF(left, top, width, height);
}

RectF MaximumCoveredRect(const RectF& a, const RectF& b) {
  // Check a or b by itself.
  RectF maximum = a;
  float maximum_area = a.size().GetArea();
  if (b.size().GetArea() > maximum_area) {
    maximum = b;
    maximum_area = b.size().GetArea();
  }
  // Check the regions that include the intersection of a and b. This can be
  // done by taking the intersection and expanding it vertically and
  // horizontally. These expanded intersections will both still be covered by
  // a or b.
  RectF intersection = a;
  intersection.InclusiveIntersect(b);
  if (!intersection.size().IsZero()) {
    RectF vert_expanded_intersection = intersection;
    vert_expanded_intersection.set_y(std::min(a.y(), b.y()));
    vert_expanded_intersection.set_height(std::max(a.bottom(), b.bottom()) -
                                          vert_expanded_intersection.y());
    if (vert_expanded_intersection.size().GetArea() > maximum_area) {
      maximum = vert_expanded_intersection;
      maximum_area = vert_expanded_intersection.size().GetArea();
    }
    RectF horiz_expanded_intersection(intersection);
    horiz_expanded_intersection.set_x(std::min(a.x(), b.x()));
    horiz_expanded_intersection.set_width(std::max(a.right(), b.right()) -
                                          horiz_expanded_intersection.x());
    if (horiz_expanded_intersection.size().GetArea() > maximum_area) {
      maximum = horiz_expanded_intersection;
      maximum_area = horiz_expanded_intersection.size().GetArea();
    }
  }
  return maximum;
}

RectF MapRect(const RectF& r, const RectF& src_rect, const RectF& dest_rect) {
  if (src_rect.IsEmpty())
    return RectF();

  float width_scale = dest_rect.width() / src_rect.width();
  float height_scale = dest_rect.height() / src_rect.height();
  return RectF(dest_rect.x() + (r.x() - src_rect.x()) * width_scale,
               dest_rect.y() + (r.y() - src_rect.y()) * height_scale,
               r.width() * width_scale, r.height() * height_scale);
}
