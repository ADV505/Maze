
int level = 1;
int life = 3;
int totalCrumbs;
Random random = new();
ConsoleKeyInfo infoKey;
(int, int) coordinatePlayer;
(int, int) coordinateHurdle;
bool isCycleStart = false;
bool isCycleGame = false;
string filePath = String.Empty;
char[,] maze;


do
{
    Console.Clear();

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
            break;
        case ConsoleKey.D2:
            filePath = SaveGamePath();
            isCycleStart = IsFileExists(filePath);
            if (!isCycleStart)
            {
                Console.WriteLine($"\nНет сохраненной игры!");
                Console.WriteLine("Нажмите любую клавишу для продолжения...");
                Console.Read();
            }
            break;
        case ConsoleKey.D3:
            return;
    }
}
while (!isCycleStart);

while (!isCycleGame)
{
    filePath = LevelPath(level);
    if (!IsFileExists(filePath))
    {
        Console.WriteLine($"\nОшибка загрузки уровня! Не найден файл ({filePath})");
        return;
    }

    maze = LoadFileLevelToArray(filePath);

    do
    {
        coordinatePlayer = GetRandomPozition(maze, random);
        coordinateHurdle = GetRandomPozition(maze, random);
    }
    while (!IsEmptyCell(maze, coordinatePlayer.Item1, coordinatePlayer.Item2)
            || !IsEmptyCell(maze, coordinateHurdle.Item1, coordinateHurdle.Item2));

    maze = СreatingStartMaze(maze, coordinatePlayer, coordinateHurdle);
    maze = AddCrumbsInMaze(maze, out totalCrumbs);

    while (totalCrumbs > 0)
    {
        Console.Clear();
        Console.WriteLine($"Уровень: {level} | Лабиринт {maze.GetLength(0)}x{maze.GetLength(1)} | Кол-во жизней: {life} | Крошек осталось: {totalCrumbs}\n");
        PrintMaze(maze);
        Console.ForegroundColor = ConsoleColor.White;
        infoKey = Console.ReadKey(true);
        coordinatePlayer = GetСoordinatePlayerInMaze(infoKey, coordinatePlayer.Item1, coordinatePlayer.Item2);
        maze = NextPozitionInMaze(maze, coordinatePlayer);
        if (infoKey.Key == ConsoleKey.Escape)
            return;
        if (life < 0)
        {
            Console.WriteLine("Вы проиграли, закончились жизни");
            return;
        }
    }
    Console.WriteLine("[1] Следующий уровень? [2] Выход");
    infoKey = Console.ReadKey();
    switch (infoKey.Key)
    {
        case ConsoleKey.D1:
            level++;
            break;
        case ConsoleKey.D2:
            isCycleGame = true;
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

static char[,] NextPozitionInMaze(char[,] array, (int, int) coordinate)
{
    for (int i = 0; i < array.GetLength(0); i++)
    {
        for (int j = 0; j < array.GetLength(1); j++)
        {
            if (array[i, j] == '@')
                array[i, j] = ' ';
        }
    }
    array[coordinate.Item1, coordinate.Item2] = '@';
    return array;
}

(int, int) GetСoordinatePlayerInMaze(ConsoleKeyInfo infoKey, int rowY, int columnX)
{
    int X = columnX;
    int Y = rowY;
    if (infoKey.Key == ConsoleKey.UpArrow) Y--;
    if (infoKey.Key == ConsoleKey.DownArrow) Y++;
    if (infoKey.Key == ConsoleKey.LeftArrow) X--;
    if (infoKey.Key == ConsoleKey.RightArrow) X++;

    if (IsEmptyCell(maze, Y, X))
    {
        return (Y, X);
    }
    else if (IsCrumbsCell(maze, Y, X))
    {
        CrumbsCountMinus();
        return (Y, X);
    }
    else if (IsHurdleCell(maze, Y, X))
    {
        HurdleMinusLife();
        return (Y, X);
    }
    else
        return (rowY, columnX);
}

bool IsCrumbsCell(char[,] array, int rowY, int columnX)
{
    return array[rowY, columnX] == '.';
}

void CrumbsCountMinus()
{
    totalCrumbs--;
}

void HurdleMinusLife()
{
    life--;
    //Console.ForegroundColor = ConsoleColor.Red;
}

static char[,] СreatingStartMaze(char[,] array, (int, int) pozitionPlayer, (int, int) pozitionHundle)
{
    array[pozitionPlayer.Item1, pozitionPlayer.Item2] = '@';
    array[pozitionHundle.Item1, pozitionHundle.Item2] = 'X';
    return array;
}

static bool IsEmptyCell(char[,] array, int rowY, int columnX)
{
    return array[rowY, columnX] == ' ';
}

static bool IsHurdleCell(char[,] array, int rowY, int columnX)
{
    return array[rowY, columnX] == 'X';
}

(int, int) GetRandomPozition(char[,] array, Random random)
{
    int Y = random.Next(array.GetLength(0));
    int X = random.Next(array.GetLength(1));
    return (Y, X);
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