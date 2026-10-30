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
    }
}
