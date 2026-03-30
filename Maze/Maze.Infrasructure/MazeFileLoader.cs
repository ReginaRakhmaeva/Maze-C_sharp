using Core;
using System;
using System.IO;

namespace Infrasructure
{
    public class MazeFileLoader
    {
        private const int MaxSize = 50;

        public Maze Load(string path)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException("Maze file not found", path);

            using (var reader = new StreamReader(path))
            {
                string[] dimensions = reader.ReadLine()?.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (dimensions == null || dimensions.Length != 2)
                    throw new InvalidDataException("Invalid file format: expected dimensions");

                if (!int.TryParse(dimensions[0], out int rows) ||
                    !int.TryParse(dimensions[1], out int cols))
                    throw new InvalidDataException("Invalid dimensions format");

                if (rows <= 1 || cols <= 1 || rows > MaxSize || cols > MaxSize)
                    throw new InvalidDataException($"Maze dimensions must be between 2 and {MaxSize}");

                var maze = new Maze(rows, cols);

                ReadWallMatrix(reader, maze.RightWalls, rows, cols);

                ReadWallMatrix(reader, maze.BottomWalls, rows, cols);

                return maze;
            }
        }

        private void ReadWallMatrix(StreamReader reader, bool[,] walls, int rows, int cols)
        {
            string line;
            int row = 0;

            while (row < rows)
            {
                line = reader.ReadLine();
                if (line == null)
                    throw new InvalidDataException($"Unexpected end of file at row {row}");

                if (string.IsNullOrWhiteSpace(line))
                    continue;

                string[] cells = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (cells.Length != cols)
                    throw new InvalidDataException($"Expected {cols} values at row {row}, got {cells.Length}");

                for (int col = 0; col < cols; col++)
                {
                    if (cells[col] != "0" && cells[col] != "1")
                        throw new InvalidDataException($"Invalid value at [{row},{col}]: expected 0 or 1");

                    walls[row, col] = cells[col] == "1";
                }

                row++;
            }
        }
    }
}