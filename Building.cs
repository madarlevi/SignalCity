using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace SignalCity
{
    public enum BuildingShape
    {
        Rectangle,
        Circle
    }

    public class Building
    {
        public Vector2 Position { get; set; }
        public BuildingShape Shape { get; set; }

        public Vector2 Size { get; set; }
        public float Absorption { get; set; }

        public Building(
            Vector2 position,
            BuildingShape shape,
            Vector2 size,
            float absorption)
        {
            Position = position;
            Shape = shape;
            Size = size;
            Absorption = absorption;
        }

        public bool Contains(Vector2 point)
        {
            switch (Shape)
            {
                case BuildingShape.Circle:
                    float radius = Size.X / 2f;

                    return Vector2.Distance(Position, point) <= radius;

                case BuildingShape.Rectangle:
                    return point.X >= Position.X - Size.X &&
                           point.X <= Position.X + Size.X &&
                           point.Y >= Position.Y - Size.Y &&
                           point.Y <= Position.Y + Size.Y;

                default:
                    return false;
            }
        }

        public bool Intersects(Building other)
        {
            if (Shape == BuildingShape.Circle &&
                other.Shape == BuildingShape.Circle)
            {
                float radiusA = Size.X / 2f;
                float radiusB = other.Size.X / 2f;

                return Vector2.Distance(Position, other.Position)
                       <= radiusA + radiusB;
            }

            if (Shape == BuildingShape.Rectangle &&
                other.Shape == BuildingShape.Rectangle)
            {
                return Math.Abs(Position.X - other.Position.X)
                           <= (Size.X + other.Size.X) / 2f &&
                       Math.Abs(Position.Y - other.Position.Y)
                           <= (Size.Y + other.Size.Y) / 2f;
            }

            // Circle - Rectangle
            Building circle = Shape == BuildingShape.Circle ? this : other;
            Building rectangle = Shape == BuildingShape.Rectangle ? this : other;

            float halfWidth = rectangle.Size.X / 2f;
            float halfHeight = rectangle.Size.Y / 2f;

            float closestX = Math.Clamp(
                circle.Position.X,
                rectangle.Position.X - halfWidth,
                rectangle.Position.X + halfWidth
            );

            float closestY = Math.Clamp(
                circle.Position.Y,
                rectangle.Position.Y - halfHeight,
                rectangle.Position.Y + halfHeight
            );

            float dx = circle.Position.X - closestX;
            float dy = circle.Position.Y - closestY;

            float radius = circle.Size.X / 2f;

            return dx * dx + dy * dy <= radius * radius;
        }

        public bool IntersectsCircle(Vector2 center, float radius)
        {
            if (Shape == BuildingShape.Circle)
            {
                return Vector2.Distance(Position, center)
                       <= Size.X / 2f + radius;
            }

            float halfWidth = Size.X / 2f;
            float halfHeight = Size.Y / 2f;

            float closestX = Math.Clamp(
                center.X,
                Position.X - halfWidth,
                Position.X + halfWidth
            );

            float closestY = Math.Clamp(
                center.Y,
                Position.Y - halfHeight,
                Position.Y + halfHeight
            );

            float dx = center.X - closestX;
            float dy = center.Y - closestY;

            return dx * dx + dy * dy <= radius * radius;
        }
    }
}
