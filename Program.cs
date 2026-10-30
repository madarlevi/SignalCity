
using Raylib_cs;
using SignalCity;
using System.Numerics;


Raylib.InitWindow(1280, 720, "SignalCity");
Raylib.SetTargetFPS(60);


List<Tower> towers = new();
List<Person> npcs = new();
List<Building> buildings = new();

EditorTool[] tools = Enum.GetValues<EditorTool>();
EditorTool activeTool = EditorTool.Select;

const int toolbarHeight = 50;
const int panelWidth = 260;

(int index, float distance) closestTowerIndex(Vector2 position)
{
    float distance = float.MaxValue;
    int index = -1;
    foreach (Tower tower in towers)
    {
        float currentDistance = Vector2.Distance(position, tower.Position);
        if (currentDistance < distance)
        {
            distance = currentDistance;
            index = towers.IndexOf(tower);
        }
    }
    return (index, distance);
}

(int index, float distance) bestTowerIndex(Vector2 position)
{
    float signal = float.MinValue;
    int index = -1;
    foreach (Tower tower in towers)
    {
        float currentSignal = CalculateSignal(tower, position);
        if (currentSignal > signal)
        {
            signal = currentSignal;
            index = towers.IndexOf(tower);
        }
    }
    return (index, signal);
}

float CalculateSignal(Tower tower, Vector2 position)
{
    float distance = Vector2.Distance(position, tower.Position);

    float x = distance / 100f;

    float result = tower.Power / (1f + x * x);

    return result;
}

float MaxSignal = CalculateSignal(new Tower(new Vector2(0, 0), 150f), new Vector2(0, 0));
int npcStartCount = 10;

for (int i = 0; i < npcStartCount; i++)
{
    npcs.Add(new Person(new Vector2(Raylib.GetRandomValue(0, Raylib.GetScreenWidth()), Raylib.GetRandomValue(0, Raylib.GetScreenHeight()))));
}

while (!Raylib.WindowShouldClose())
{
    Vector2 mouse = Raylib.GetMousePosition();

    if (Raylib.IsMouseButtonPressed(MouseButton.Left))
    {
        for (int i = 0; i < tools.Length; i++)
        {
            Rectangle button = new(10 + i * 105, 8, 100, 34);

            if (Raylib.CheckCollisionPointRec(mouse, button))
            {
                activeTool = tools[i];
                break;
            }
        }
    }

    Raylib.BeginDrawing();

    Raylib.ClearBackground(Color.Black);

    if (towers.Count != 0)
    {
        for (int x = 0; x < Raylib.GetScreenWidth(); x += 20)
        {
            for (int y = 0; y < Raylib.GetScreenHeight(); y += 20)
            {
                Vector2 position = new(x+10, y+10);
                float signal = CalculateSignal(towers[bestTowerIndex(position).index], position);
                float normalizedSignal = Math.Clamp(signal / MaxSignal, 0f, 1f);

                byte red = (byte)(255 * (1f - normalizedSignal));
                byte green = (byte)(255 * normalizedSignal);

                Color current = new Color(red, green, (byte)0, (byte)255);
                Raylib.DrawRectangle(x, y, 18, 18, current);
            }
        }
    }

    if (Raylib.IsMouseButtonPressed(MouseButton.Left) && mouse.Y >= toolbarHeight && mouse.X < Raylib.GetScreenWidth() - panelWidth)
    {
        switch (activeTool)
        {
            case EditorTool.Tower:
                towers.Add(new Tower(Raylib.GetMousePosition(), Raylib.GetRandomValue(50, 150)));
                break;

            case EditorTool.Person:
                npcs.Add(new Person(mouse));
                break;

            case EditorTool.Delete:
                (int index, float distance) result = closestTowerIndex(Raylib.GetMousePosition());
                if (result.distance < 20)
                {
                    towers.RemoveAt(result.index);
                }
                break;
        }
    }

    foreach (Tower tower in towers)
    {
        Raylib.DrawCircleV(tower.Position, 20, Color.Blue);
    }

    for (int i = 0; i < npcs.Count; i++)
    {
        if (towers.Count != 0)
        {
            Vector2 closestTower = towers[bestTowerIndex(npcs[i].Position).index].Position;
            Vector2 difference = closestTower - npcs[i].Position;

            float distance = difference.Length();
            if (distance > 40)
            {
                float movement = npcs[i].Speed * Raylib.GetFrameTime();

                if (distance > movement)
                {
                    Vector2 direction = Vector2.Normalize(difference);
                    npcs[i].Position += direction * movement;
                }
            }
        }
        Raylib.DrawCircleV(npcs[i].Position, 5, Color.Magenta);
    }

    Raylib.DrawRectangle(0, 0, 1280, toolbarHeight, Color.DarkGray);

    for (int i = 0; i < tools.Length; i++)
    {
        Rectangle button = new(10 + i * 105, 8, 100, 34);

        Color color = activeTool == tools[i]
            ? Color.Blue
            : Color.Gray;

        Raylib.DrawRectangleRec(button, color);

        Raylib.DrawText(
            tools[i].ToString(),
            (int)button.X + 5,
            (int)button.Y + 10,
            12,
            Color.White
        );
    }

    Raylib.EndDrawing();
}

Raylib.CloseWindow();
