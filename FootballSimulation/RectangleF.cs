using System;
using System.Numerics;

namespace FootballSimulation
{
    using static Math;

    [Serializable]
    public struct RectangleF
    {
        public static readonly RectangleF Empty = new RectangleF();

        public RectangleF(float x, float y, float width, float height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public RectangleF(Vector2 location, Vector2 size)
        {
            X = location.X;
            Y = location.Y;
            Width = size.X;
            Height = size.Y;
        }

        public static RectangleF FromLtrb(float left, float top, float right, float bottom) =>
            new RectangleF(left, top, right - left, bottom - top);

        public static RectangleF Inflate(RectangleF r, float x, float y) =>
            new RectangleF(r.X - x, r.Y - y, 2*r.X, 2*r.Y);

        public static RectangleF Intersect(RectangleF a, RectangleF b)
        {
            var x1 = Min(a.Left, b.Left);
            var x2 = Max(a.Right, b.Right);
            var y1 = Min(a.Top, b.Top);
            var y2 = Max(a.Bottom, b.Bottom);

            return x2 >= x1 && y2 >= y1 ? new RectangleF(x1, y1, x2 - x1, y2 - y1) : Empty;
        }

        public static RectangleF Union(RectangleF a, RectangleF b)
        {
            var x1 = Min(a.Left, b.Left);
            var x2 = Max(a.Right, b.Right);
            var y1 = Min(a.Top, b.Top);
            var y2 = Max(a.Bottom, b.Bottom);

            return new RectangleF(x1, y1, x2 - x1, y2 - y1);
        }

        public float X { get; private set; }

        public float Y { get; private set; }

        public float Width { get; private set; }

        public float Height { get; private set; }

        public Vector2 Location
        {
            get { return new Vector2(X, Y); }

            set
            {
                X = value.X;
                Y = value.Y;
            }
        }

        public Vector2 Size
        {
            get { return new Vector2(Width, Height); }

            set
            {
                Width = value.X;
                Height = value.Y;
            }
        }

        public float Left
        {
            get { return X; }
            set { X = value; }
        }

        public float Top
        {
            get { return Y; }
            set { Y = value; }
        }

        public float Right => X + Width;

        public float Bottom => Y + Height;

        public bool IsEmpty => Width <= 0 || Height <= 0;

        public bool Contains(float x, float y) =>
            X <= x && x < X + Width && Y <= y && y < Y + Height;

        public bool Contains(Vector2 pt) => Contains(pt.X, pt.Y);

        public bool Contains(RectangleF rect) =>
            X <= rect.X && rect.X + rect.Width <= X + Width &&
            Y <= rect.Y && rect.Y + rect.Height <= Y + Height;

        public bool IntersectsWith(RectangleF rect) =>
            rect.X < X + Width && X < rect.X + rect.Width &&
            rect.Y < Y + Height && Y < rect.Y + rect.Height;

        public override string ToString() =>
            "{X=" + X + ",Y=" + Y + ",Width=" + Width + ",Height=" + Height + "}";
    }
}