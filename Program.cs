
using System.Text;

int level = 1;
int life = 3;
int totalCrumbs;
Random random = new();
ConsoleKeyInfo infoKey;
(int, int) coordinatePlayer;
(int, int) coordinateHurdle;
bool isCycleStart = false;
bool isCycleGame = true;
string filePath = String.Empty;
char[,] maze;
char player = '@';
char hurdle = 'X';
char defaultCell = '.';
string dir = string.Empty;

do
{
    Console.Clear();
    Console.CursorVisible = false;
    Console.WriteLine("Игра Лабиринт");
    Console.WriteLine("[1] Начать новую игру" +
                    "\n[2] Загрузить ранее сохраненную игру?" +
                    "\n[3] Выход");
    infoKey = Console.ReadKey(true);
    switch (infoKey.Key)
    {
        case ConsoleKey.D1:
            filePath = LevelPath(level);
            isCycleStart = IsFileExists(filePath);
            if (!isCycleStart)
            {
                Console.WriteLine($"\nОшибка загрузки уровня! Не найден файл ({filePath})");
                return;
            }
            else
                LoadGame(filePath);
            break;
        case ConsoleKey.D2:
            filePath = SaveGamePath();
            isCycleStart = IsFileExists(filePath);
            if (!isCycleStart)
            {
                Console.WriteLine($"\nНет сохраненной игры!");
                Console.WriteLine("Нажмите любую клавишу для продолжения...");
                Console.ReadKey();
            }
            break;
        case ConsoleKey.D3:
            return;
    }
}
while (!isCycleStart);

void LoadGame(string path)
{
    while (isCycleGame)
    {
        maze = LoadFileLevelToArray(path);

        do
        {
            coordinatePlayer = GetRandomPozition(maze, random);
            coordinateHurdle = GetRandomPozition(maze, random);
        }
        while (!IsEmptyCellOrCrumbsCell(maze, coordinatePlayer.Item1, coordinatePlayer.Item2)
                || !IsEmptyCellOrCrumbsCell(maze, coordinateHurdle.Item1, coordinateHurdle.Item2)
                || coordinatePlayer == coordinateHurdle);

        maze = СreatingStartMaze(maze, coordinatePlayer, coordinateHurdle);
        maze = AddCrumbsInMaze(maze, out totalCrumbs);

        while (totalCrumbs != 0)
        //while (true)
        {

            while (true)
            {
                Console.Clear();
                Console.WriteLine($"Уровень: {level} | Лабиринт {maze.GetLength(0)}x{maze.GetLength(1)} | Кол-во жизней: {life} | Крошек осталось: {totalCrumbs}\n");
                PrintMaze(maze);
                coordinateHurdle = GetCoordinateHurdleInMaze(coordinateHurdle.Item1, coordinateHurdle.Item2, dir);
                maze = NextPozitionInMaze(maze, coordinateHurdle, hurdle);
                Thread.Sleep(200);
                if (Console.KeyAvailable)
                    break;
            }

            infoKey = Console.ReadKey(true);
            coordinatePlayer = GetСoordinatePlayerInMaze(infoKey, coordinatePlayer.Item1, coordinatePlayer.Item2);
            maze = NextPozitionInMaze(maze, coordinatePlayer, player);

            if (infoKey.Key == ConsoleKey.Spacebar)
                SaveGame(maze);

            if (infoKey.Key == ConsoleKey.Escape)
                return;

            if (life < 0)
            {
                Console.WriteLine("Вы проиграли, закончились жизни");
                return;
            }
        }
        
        ShowMenuAfterGame();
    }
    

}

void SaveGame(char[,] array)
{
    string path = SaveGamePath();

    StringBuilder output = new StringBuilder();
    for (int row = 0; row < array.GetLength(0); row++)
    {
        for (int column = 0; column < array.GetLength(1); column++)
        {
            output.Append($"{array[row, column]}");

        }
        output.Append(Environment.NewLine);
    }
    try
    {
        File.WriteAllText(path, output.ToString());
    }
    catch (Exception ex)
    {
        Console.WriteLine($"<--Ошибка записи в файл: {ex.Message}");
    }
}

(int, int) GetCoordinateHurdleInMaze(int rowY, int columnX, string dirSt)
{
    // int y = rowY;
    // int x = columnX;
    if (IsEmptyCellOrCrumbsCell(maze, rowY, columnX))
        return (rowY, columnX);

    if (IsEmptyCellOrCrumbsCell(maze, rowY, columnX - 1) && IsEmptyCellOrCrumbsCell(maze, rowY, columnX + 1)
        && IsEmptyCellOrCrumbsCell(maze, rowY - 1, columnX) && IsEmptyCellOrCrumbsCell(maze, rowY + 1, columnX))
    {
        if (dirSt == "right")
        {
            dir = "up";
            return GetCoordinateHurdleInMaze(rowY - 1, columnX, dir);
        }

        if (dirSt == "down")
        {
            dir = "right";
            return GetCoordinateHurdleInMaze(rowY, columnX + 1, dir);
        }

        if (dirSt == "left")
        {
            dir = "down";
            return GetCoordinateHurdleInMaze(rowY + 1, columnX, dir);
        }

        if (dirSt == "up")
        {
            dir = "left";
            return GetCoordinateHurdleInMaze(rowY, columnX - 1, dir);
        }
    }


    if (IsEmptyCellOrCrumbsCell(maze, rowY, columnX - 1) && dirSt != "right")
    {
        dir = "left";
        return GetCoordinateHurdleInMaze(rowY, columnX - 1, dir);
    }

    if (IsEmptyCellOrCrumbsCell(maze, rowY - 1, columnX) && dirSt != "down")
    {
        dir = "up";
        return GetCoordinateHurdleInMaze(rowY - 1, columnX, dir);
    }

    if (IsEmptyCellOrCrumbsCell(maze, rowY, columnX + 1) && dirSt != "left")
    {
        dir = "right";
        return GetCoordinateHurdleInMaze(rowY, columnX + 1, dir);
    }

    if (IsEmptyCellOrCrumbsCell(maze, rowY + 1, columnX) && dirSt != "up")
    {
        dir = "down";
        return GetCoordinateHurdleInMaze(rowY + 1, columnX, dir);
    }

    else
    {
        dir = string.Empty;
        return GetCoordinateHurdleInMaze(rowY, columnX, dir);
    }
}

void ShowMenuAfterGame()
{
    Console.WriteLine("\n[1] Следующий уровень? [2] Выход");
    infoKey = Console.ReadKey(true);
    switch (infoKey.Key)
    {
        case ConsoleKey.D1:
            level++;
            filePath = LevelPath(level);
            if (IsFileExists(filePath))
                LoadGame(filePath);
            break;
        case ConsoleKey.D2:
            isCycleGame = false;
            break;
    }
}

string LevelPath(int value)
{
    return $".\\level\\level{value}.txt";
}

string SaveGamePath()
{
    return $".\\level\\save.txt";
}

static bool IsFileExists(string path)
{
    FileInfo file = new(path);
    return file.Exists;
}

char[,] NextPozitionInMaze(char[,] array, (int, int) coordinate, char symbol)
{
    for (int i = 0; i < array.GetLength(0); i++)
    {
        for (int j = 0; j < array.GetLength(1); j++)
        {
            if (symbol == player)
            {
                if (array[i, j] == player)
                    array[i, j] = ' ';
            }

            if (symbol == hurdle)
            {
                if (array[i, j] == hurdle)
                    array[i, j] = defaultCell;
            }
        }
    }
    if (symbol == hurdle)
        defaultCell = array[coordinate.Item1, coordinate.Item2];
    if (symbol == player)
        if (array[coordinate.Item1, coordinate.Item2] == '.')
            CrumbsCount();

    array[coordinate.Item1, coordinate.Item2] = symbol;
    return array;
}

(int, int) GetСoordinatePlayerInMaze(ConsoleKeyInfo infoKey, int rowY, int columnX)
{
    int x = columnX;
    int y = rowY;
    if (infoKey.Key == ConsoleKey.UpArrow) y--;
    if (infoKey.Key == ConsoleKey.DownArrow) y++;
    if (infoKey.Key == ConsoleKey.LeftArrow) x--;
    if (infoKey.Key == ConsoleKey.RightArrow) x++;

    if (IsEmptyCellOrCrumbsCell(maze, y, x))
    {
        return (y, x);
    }
    else if (IsHurdleCell(maze, y, x))
    {
        HurdleMinusLife();
        return (y, x);
    }
    else
        return (rowY, columnX);
}

void CrumbsCount()
{
    --totalCrumbs;
}

void HurdleMinusLife()
{
    life--;
}

char[,] СreatingStartMaze(char[,] array, (int, int) pozitionPlayer, (int, int) pozitionHurdle)
{
    array[pozitionPlayer.Item1, pozitionPlayer.Item2] = player;
    array[pozitionHurdle.Item1, pozitionHurdle.Item2] = hurdle;
    return array;
}

static bool IsEmptyCellOrCrumbsCell(char[,] array, int rowY, int columnX)
{
    return array[rowY, columnX] == ' ' || array[rowY, columnX] == '.';
}

bool IsHurdleCell(char[,] array, int rowY, int columnX)
{
    return array[rowY, columnX] == hurdle;
}

(int, int) GetRandomPozition(char[,] array, Random random)
{
    int y = random.Next(array.GetLength(0));
    int x = random.Next(array.GetLength(1));
    return (y, x);
}

static char[,] AddCrumbsInMaze(char[,] arr, out int count)
{
    count = 0;
    for (int i = 0; i < arr.GetLength(0); i++)
    {
        for (int j = 0; j < arr.GetLength(1); j++)
        {
            if (arr[i, j] == ' ')
            {
                arr[i, j] = '.';
                count++;
            }
        }
    }
    return arr;
}

static char[,] LoadFileLevelToArray(string fileLevel)
{
    string[] lines = File.ReadAllLines(fileLevel);
    char[,] array = new char[lines.Length, lines[0].Length];
    for (int i = 0; i < lines.Length; i++)
    {
        for (int j = 0; j < lines[0].Length; j++)
        {
            array[i, j] = lines[i][j];
        }
    }
    return array;
}

static void PrintMaze(char[,] arr)
{
    for (int i = 0; i < arr.GetLength(0); i++)
    {
        for (int j = 0; j < arr.GetLength(1); j++)
        {
            Console.Write(arr[i, j] + " ");
        }
        Console.WriteLine();
    }
}