﻿using System.Text;

int level = 1;
int life = 1;
int totalCrumbs;
Random random = new();
ConsoleKeyInfo infoKey;
(int, int) coordinatePlayer;
(int, int) coordinateHurdle;
bool isExit = false;
char[,] maze = new char[0, 0];
string path = Directory.GetCurrentDirectory();

while (!isExit)
{
    string targetFile = $"{path}\\level\\level{level}.txt";
    FileInfo file = new(targetFile);
    if (!file.Exists)
    {
        Console.WriteLine("Отсутствует файл следующего уровня!");
        Environment.Exit(0);
    }

    Console.CursorVisible = false;

    maze = LoadFileLevelToArray(targetFile);

    do
    {
        coordinatePlayer = GetRandomPozition(maze, random);
        coordinateHurdle = GetRandomPozition(maze, random);
    }
    while (!IsEmptyCell(maze, coordinatePlayer.Item1, coordinatePlayer.Item2)
            || !IsEmptyCell(maze, coordinateHurdle.Item1, coordinateHurdle.Item2));

    maze = СreatingStartMaze(maze, coordinatePlayer, coordinateHurdle);
    maze = AddCrumbsInMaze(maze, out totalCrumbs);
    Console.WriteLine(totalCrumbs);
   while (totalCrumbs > 0)
    {
        Console.Clear();
        Console.WriteLine($"Уровень: {level}| Лабиринт {maze.GetLength(0)}x{maze.GetLength(1)} | Кол-во жизней: {life} | Крошек осталось: {totalCrumbs}\n");
        // totalCrumbs = crumbs;
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
     
        int cmd = ReadIntInput("[1] Следующий уровень? [2] Выход");
        switch (cmd)
        {
            case 1:
                level++;
                break;
            case 2:
                isExit = true;
                break;
        }
}

int ReadIntInput(string message)
{
    Console.WriteLine(message);
    return int.Parse(Console.ReadLine());
}

static char[,] NextPozitionInMaze(char[,] array, (int, int) coordinate)
{
    // minusCrumbs = totalCount;
    for (int i = 0; i < array.GetLength(0); i++)
    {
        for (int j = 0; j < array.GetLength(1); j++)
        {
            if (array[i, j] == '@')
                array[i, j] = ' ';
        }
    }
    // if (array[coordinate.Item1, coordinate.Item2] == '.')
    // {
    //     minusCrumbs--;
    // }
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
    else if(IsCrumbsCell(maze, Y, X))
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
    Console.ForegroundColor = ConsoleColor.Red;
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
    char[,] c = new char[lines.Length, lines[0].Length];
    for (int i = 0; i < lines.Length; i++)
    {
        for (int j = 0; j < lines[0].Length; j++)
        {
            c[i, j] = lines[i][j];
        }
    }
    return c;
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