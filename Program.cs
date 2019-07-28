using System;
using System.Threading;

namespace GameOfLife
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            Console.BackgroundColor = ConsoleColor.Black;
            // create a grid
            int gridSide = 50;
            int[,] grid = new int[gridSide, gridSide];
            int dead = 0;
            int alive = 1;

            //creates a bunch of alive cells randomly
            Random rnd = new Random();
            int numOfLives = 600;
            for (int i = 0; i < numOfLives; i++)
            {
                grid[rnd.Next(0, (gridSide - 1)), rnd.Next(0, (gridSide - 1))] = alive;
            }

            // remember: data is different than display.
            DisplayGrid(grid, gridSide, dead);

            while (true)
            {
                // pause so I can see the change
                Thread.Sleep(100);

                // make a new grid
                int[,] gridJunior = DoCensus(grid, gridSide, dead);

                DisplayGrid(gridJunior, gridSide, dead);

                // make the variable 'grid' represent the new generation
                grid = gridJunior;
            }
        }

        public static int NeighborCount(string[,] grid, int x, int y)
        {
            int counter = grid[x, y] == "X" ? -1 : 0;

            // check that the row/column is not the first/last
            // if I'm on the bottom, don't use the row below me
            int bottom = (x < (Math.Sqrt(grid.Length) - 1)) ? x + 1 : x;
            // if I'm on the top, don't use the row above me
            int top = (x > 0) ? x - 1 : x;
            // if I'm on the left, don't use the column to my left
            int left = (y > 0) ? y - 1 : y;
            // if I'm on the right, don't use the column to my right
            int right = (y < (Math.Sqrt(grid.Length) - 1)) ? y + 1 : y;

            for (int i = top; i <= bottom; ++i)
            {
                for (
                        int j = left;
                        j <= right;
                        ++j
                    )
                {
                    if (grid[i, j] == "X")
                    {
                        ++counter;
                    }
                    if (counter > 3) { break; }
                }
            }
            return counter;
        }

        static int CountNeighbors(bool[,] grid, int x, int y)
        {
            int neighbors = 0;
            if (grid[x, y])
            {
                neighbors--; //to make up for counting itself in the following loop
            }

            for (int dy = -1; dy < 2; dy++)
            {
                for (int dx = -1; dx < 2; dx++)
                {
                    int across = Wrap(x + dx, grid.GetLength(0));
                    int down = Wrap(y + dy, grid.GetLength(1));

                    if (grid[across, down])
                    {
                        neighbors++;
                    }
                }
            }

            return neighbors;
        }

        // if you go in any direction far enough, you end up where you were
        static int Wrap(int value, int limit)
        {
            int result = value;

            if (value < 0)
            {
                result = value + limit;
            }
            else if (value >= limit)
            {
                result = value - limit;
            }

            return result;
        }

        static bool IsLiveNeighbor(int[,] grid, int row, int column)
        {

            int across = Wrap(column, grid.GetLength(0));
            int down = Wrap(row, grid.GetLength(1));

            if (grid[down, across] == 1)
            {
                return true;
            }

            return false;
        }

        static int CountNeighbors(int row, int column, int[,] grid, int gridSize)
        {
            int liveNeighborCount = 0;

            // check 8 spaces around it.
            // if a neighbor is dead - nothing
            // if a neighbor is alive - add 1 to neighborCount

            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    if (IsLiveNeighbor(grid, row + i, column + j))
                    {
                        if (!(i == 0 && j == 0))
                        {
                            liveNeighborCount++;
                        }
                    }
                }
            }

            return liveNeighborCount;
        }

        static int[,] DoCensus(int[,] oldGrid, int gridSide, int dead)
        {
            int[,] newGrid = new int[gridSide, gridSide];
            int alive = 1;
            // for each spot in the old grid, 
            for (int i = 0; i < gridSide; i++)
            {
                for (int j = 0; j < gridSide; j++)
                {
                    int neighborCount = CountNeighbors(i, j, oldGrid, gridSide);
                    // find the neighbor count

                    // see if it should be alive or dead in the next generation
                    // if it's alive & has 2 neighbors - lives - live
                    // if it's alive & has 3 neighbors - lives - live
                    // if it's alive & any other number - dies - dead
                    if (oldGrid[i, j] == alive)
                    {
                        if (neighborCount == 2 || neighborCount == 3)
                        {
                            newGrid[i, j] = alive;
                        }
                        else
                        {
                            newGrid[i, j] = dead;
                        }
                    }
                    // if it's dead & has 3 neighbors - comes alive - live
                    // if it's dead & has any other neighbors - stays dead - dead
                    else
                    {
                        if (neighborCount == 3)
                        {
                            newGrid[i, j] = alive;
                        }
                        else
                        {
                            newGrid[i, j] = dead;
                        }
                    }
                }
            }
            return newGrid;
        }

        static void DisplayGrid(int[,] grid, int gridSide, int dead)
        {
            int firstSpot = 0;
            Console.Clear();
            Random rnd = new Random();

            for (int i = firstSpot; i < gridSide; i++)
            {
                for (int j = firstSpot; j < gridSide; j++)
                {
                    int cell = grid[i, j];
                    // Print differently depending on if the cell is alive or dead
                    if (cell == dead)
                    {
                        Console.ForegroundColor = ConsoleColor.Black;
                        Console.Write(".");
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.ForegroundColor = (ConsoleColor)rnd.Next(0, 16);
                        Console.Write("o");
                        Console.ResetColor();
                    }
                    Console.Write(" ");
                }
                Console.WriteLine();
            }
        }
    }
}