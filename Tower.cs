using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace SignalCity
{
    public class Tower
    {
        public Vector2 Position { get; set; }
        public float Power { get; set; }

        public Tower(Vector2 position, float power)
        {
            Position = position;
            Power = power;
        }
    }
}
