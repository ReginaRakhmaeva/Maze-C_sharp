using System;

namespace Core
{
    public struct Point : IEquatable<Point>
    {
        public int Row { get; }
        public int Col { get; }

        public Point(int row, int col)
        {
            Row = row;
            Col = col;
        }

        public bool Equals(Point other)
        {
            return Row == other.Row && Col == other.Col;
        }

        public override bool Equals(object obj)
        {
            return obj is Point other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Row, Col);
        }

        public static bool operator ==(Point left, Point right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Point left, Point right)
        {
            return !left.Equals(right);
        }
    }
}
