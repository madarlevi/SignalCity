
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
string? selectedType = null;
Building? selectedBuilding = null;
Person? selectedPerson = null;
Tower? selectedTower = null;

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

(int index, float distance) closestPersonIndex(Vector2 position)
{
    float distance = float.MaxValue;
    int index = -1;
    foreach (Person person in npcs)
    {
        float currentDistance = Vector2.Distance(position, person.Position);
        if (currentDistance < distance)
        {
            distance = currentDistance;
            index = npcs.IndexOf(person);
        }
    }
    return (index, distance);
}

(int index, float distance) closestBuildingIndex(Vector2 position)
{
    float distance = float.MaxValue;
    int index = -1;
    foreach (Building building in buildings)
    {
        float currentDistance = Vector2.Distance(position, building.Position);
        if (currentDistance < distance)
        {
            distance = currentDistance;
            index = buildings.IndexOf(building);
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

bool DrawButton(Rectangle rect, string text)
{
    Vector2 mouse = Raylib.GetMousePosition();

    bool hovered = Raylib.CheckCollisionPointRec(mouse, rect);

    Color color = hovered ? Color.LightGray : Color.Gray;

    Raylib.DrawRectangleRec(rect, color);

    Raylib.DrawText(
        text,
        (int)rect.X + 8,
        (int)rect.Y + 5,
        20,
        Color.Black
    );

    return hovered &&
        Raylib.IsMouseButtonPressed(MouseButton.Left);
}

float MaxSignal = CalculateSignal(new Tower(new Vector2(0, 0), 150f), new Vector2(0, 0));
int npcStartCount = 10;

for (int i = 0; i < npcStartCount; i++)
{
    npcs.Add(new Person(new Vector2(Raylib.GetRandomValue(0, Raylib.GetScreenWidth()), Raylib.GetRandomValue(50, Raylib.GetScreenHeight()))));
}

// Main game loop
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

    // Drawing

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

            case EditorTool.RectangleBuilding:
                buildings.Add(new Building(Raylib.GetMousePosition(), BuildingShape.Rectangle, new Vector2(Raylib.GetRandomValue(50, 150), Raylib.GetRandomValue(50, 150)), 10));
                break;

            case EditorTool.CircleBuilding:
                buildings.Add(new Building(Raylib.GetMousePosition(), BuildingShape.Circle, new Vector2(Raylib.GetRandomValue(50, 150), Raylib.GetRandomValue(50, 150)), 10));
                break;

            case EditorTool.Delete:
                (int index, float distance) result = closestTowerIndex(Raylib.GetMousePosition());
                if (result.distance < 20 && towers.Count != 0)
                {
                    towers.RemoveAt(result.index);
                }
                else
                {
                    result = closestPersonIndex(Raylib.GetMousePosition());
                    if (result.distance < 5 && npcs.Count != 0)
                    {
                        npcs.RemoveAt(result.index);
                    }
                    else
                    {
                        result = closestBuildingIndex(Raylib.GetMousePosition());
                        if (buildings.Count != 0 && result.distance < buildings[result.index].Size.X)
                        {
                            buildings.RemoveAt(result.index);
                        }
                    }
                }
                selectedTower = null;
                selectedPerson = null;
                selectedBuilding = null;
                selectedType = null;
                break;

            case EditorTool.Select:
                selectedTower = null;
                selectedPerson = null;
                selectedBuilding = null;
                selectedType = null;

                foreach (Tower tower in towers)
                {
                    if (Vector2.Distance(mouse, tower.Position) <= 20)
                    {
                        selectedTower = tower;
                        selectedType = "Tower";
                        break;
                    }
                }
                if (selectedType == null)
                {
                    foreach (Person person in npcs)
                    {
                        if (Vector2.Distance(mouse, person.Position) <= 5)
                        {
                            selectedPerson = person;
                            selectedType = "Person";
                            break;
                        }
                    }
                }
                if (selectedType == null)
                {
                    foreach (Building building in buildings)
                    {
                        if (Vector2.Distance(mouse, building.Position) <= building.Size.X)
                        {
                            selectedBuilding = building;
                            selectedType = "Building";
                            break;
                        }
                    }
                }
                break;
        }
    }

    foreach (Building building in buildings)
    {
        Raylib.DrawRectangleV(building.Position, building.Size, Color.Green);

        if (selectedBuilding == building && selectedType == "Building")
        {
            Raylib.DrawRectangleLines((int)building.Position.X, (int)building.Position.Y, (int)building.Size.X, (int)building.Size.Y, Color.Yellow);
        }
    }

    foreach (Tower tower in towers)
    {
        Raylib.DrawCircleV(tower.Position, 20, Color.Blue);

        if (selectedTower == tower && selectedType == "Tower")
        {
            Raylib.DrawCircleLines((int)tower.Position.X, (int)tower.Position.Y, 24, Color.Yellow);
        }
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
        if (selectedPerson == npcs[i] && selectedType == "Person")
        {
            Raylib.DrawCircleLines((int)npcs[i].Position.X, (int)npcs[i].Position.Y, 7, Color.Yellow);
        }
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

    int panelX = Raylib.GetScreenWidth() - panelWidth;

    Raylib.DrawRectangle(
        panelX, toolbarHeight,
        panelWidth,
        Raylib.GetScreenHeight() - toolbarHeight,
        Color.DarkGray
    );

    Raylib.DrawText(
        "PROPERTIES",
        panelX + 20, 70,
        24, Color.White
    );

    if (selectedTower != null && selectedType == "Tower")
    {
        Raylib.DrawText(
            "Tower",
            panelX + 20, 120,
            22, Color.White
        );

        Raylib.DrawText(
            $"Power: {selectedTower.Power}",
            panelX + 20, 170,
            20, Color.White
        );

        if (DrawButton(
            new Rectangle(panelX + 20, 210, 45, 35), "-"))
        {
            selectedTower.Power =
                Math.Max(0f, selectedTower.Power - 10f);
        }

        if (DrawButton(
            new Rectangle(panelX + 80, 210, 45, 35), "+"))
        {
            selectedTower.Power =
                Math.Min(150f, selectedTower.Power + 10f);
        }

        Raylib.DrawText(
            $"X: {selectedTower.Position.X:0}",
            panelX + 20, 285,
            20, Color.White
        );

        if (DrawButton(
            new Rectangle(panelX + 20, 325, 45, 35), "-"))
        {
            selectedTower.Position =
                new Vector2(Math.Max(0f, selectedTower.Position.X - 10f), selectedTower.Position.Y);
        }

        if (DrawButton(
            new Rectangle(panelX + 80, 325, 45, 35), "+"))
        {
            selectedTower.Position =
                new Vector2(Math.Min(Raylib.GetScreenWidth() - panelWidth, selectedTower.Position.X + 10f), selectedTower.Position.Y);
        }

        Raylib.DrawText(
            $"Y: {selectedTower.Position.Y:0}",
            panelX + 20, 400,
            20, Color.White
        );

        if (DrawButton(
            new Rectangle(panelX + 20, 440, 45, 35), "-"))
        {
            selectedTower.Position =
                new Vector2(selectedTower.Position.X, Math.Max(toolbarHeight, selectedTower.Position.Y - 10f));
        }

        if (DrawButton(
            new Rectangle(panelX + 80, 440, 45, 35), "+"))
        {
            selectedTower.Position =
                new Vector2(selectedTower.Position.X, Math.Min(Raylib.GetScreenHeight() - toolbarHeight, selectedTower.Position.Y + 10f));
        }
    }
    else if (selectedPerson != null && selectedType == "Person")
    {
        Raylib.DrawText(
            "Person",
            panelX + 20, 120,
            22, Color.White
        );

        Raylib.DrawText(
            $"X: {selectedPerson.Position.X:0}",
            panelX + 20, 285,
            20, Color.White
        );

        if (DrawButton(
            new Rectangle(panelX + 20, 325, 45, 35), "-"))
        {
            selectedPerson.Position =
                new Vector2(Math.Max(0f, selectedPerson.Position.X - 10f), selectedPerson.Position.Y);
        }

        if (DrawButton(
            new Rectangle(panelX + 80, 325, 45, 35), "+"))
        {
            selectedPerson.Position =
                new Vector2(Math.Min(Raylib.GetScreenWidth() - panelWidth, selectedPerson.Position.X + 10f), selectedPerson.Position.Y);
        }

        Raylib.DrawText(
            $"Y: {selectedPerson.Position.Y:0}",
            panelX + 20, 400,
            20, Color.White
        );

        if (DrawButton(
            new Rectangle(panelX + 20, 440, 45, 35), "-"))
        {
            selectedPerson.Position =
                new Vector2(selectedPerson.Position.X, Math.Max(toolbarHeight, selectedPerson.Position.Y - 10f));
        }

        if (DrawButton(
            new Rectangle(panelX + 80, 440, 45, 35), "+"))
        {
            selectedPerson.Position =
                new Vector2(selectedPerson.Position.X, Math.Min(Raylib.GetScreenHeight() - toolbarHeight, selectedPerson.Position.Y + 10f));
        }
    }
    else
    {
        Raylib.DrawText(
            "Nothing selected",
            panelX + 20, 120,
            18, Color.LightGray
        );
    }

    Raylib.EndDrawing();
}

Raylib.CloseWindow();
