
using Raylib_cs;
using System.Numerics;

Raylib.InitWindow(800, 600, "SignalCity");
Raylib.SetTargetFPS(60);

List<Vector2> towers = new();

(int index, float distance) cloasestTowerIndex(Vector2 position)
{
    float distance = float.MaxValue;
    int index = -1;
    foreach (Vector2 tower in towers)
    {
        float currentDistance = Vector2.Distance(position, tower);
        if (currentDistance < distance)
        {
            distance = currentDistance;
            index = towers.IndexOf(tower);
        }
    }
    return (index, distance);
}

float CalculateSignal(Vector2 tower, Vector2 position)
{
    float distance = Vector2.Distance(position, tower);

    float x = distance / 100f;

    float result = 100f / (1f + x * x);

    return result;
}

float MaxSignal = CalculateSignal(new Vector2(0, 0), new Vector2(0, 0));

while (!Raylib.WindowShouldClose())
{
    Raylib.BeginDrawing();

    Raylib.ClearBackground(Color.Gray);

    if (towers.Count != 0)
    {
        for (int x = 0; x < Raylib.GetScreenWidth(); x += 1)
        {
            for (int y = 0; y < Raylib.GetScreenHeight(); y += 1)
            {
                Vector2 position = new(x, y);
                float signal = CalculateSignal(towers[cloasestTowerIndex(position).index], position);
                float step = MaxSignal / 255f;
                Color current = new Color(MaxSignal / signal * step, 255 - MaxSignal / signal * step, 0, 255);
                Raylib.DrawPixel(x, y, current);
            }
        }
    }

    if (Raylib.IsMouseButtonPressed(MouseButton.Left))
    {
        towers.Add(Raylib.GetMousePosition());
    }

    if (Raylib.IsMouseButtonPressed(MouseButton.Right))
    {
        (int index, float distance) result = cloasestTowerIndex(Raylib.GetMousePosition());
        if (result.distance < 20)
        {
            towers.RemoveAt(result.index);
        }
    }

    foreach (Vector2 tower in towers)
    {
        Raylib.DrawCircleV(tower, 20, Color.Blue);
    }

    Raylib.DrawText("Tornyok száma: " + towers.Count, 10, 10, 20, Color.Black);

    Raylib.EndDrawing();
}

Raylib.CloseWindow();
