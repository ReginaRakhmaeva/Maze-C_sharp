using System;
using System.Collections.Generic;

namespace Core
{
    public class Maze
    {
        public int Rows { get; }
        public int Cols { get; }

        public bool[,] RightWalls;

        public bool[,] BottomWalls;

        public Maze(int rows, int cols)
        {
            Rows = rows;
            Cols = cols;
            RightWalls = new bool[Rows, Cols];
            BottomWalls = new bool[Rows, Cols];

            for (int r = 0; r < Rows; r++)
            {
                for (int c = 0; c < Cols; c++)
                {
                    RightWalls[r, c] = true;
                    BottomWalls[r, c] = true;
                }
            }
        }
    }
}