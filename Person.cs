using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace SignalCity
{
    public class Person
    {
        public Vector2 Position { get; set; }

        public float Laziness { get; set; }
        public float InternetNeed { get; set; }
        public float Speed { get; set; }
        public float Influenceability { get; set; }
        public float Sociability { get; set; }

        public Person(Vector2 position)
        {
            Position = position;

            Laziness = Random.Shared.NextSingle();
            InternetNeed = Random.Shared.NextSingle();

            Speed = 40f + Random.Shared.NextSingle() * 40f;

            Influenceability = Random.Shared.NextSingle();
            Sociability = Random.Shared.NextSingle();
        }
    }
}
