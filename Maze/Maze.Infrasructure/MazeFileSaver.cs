using Core;
using System.IO;
using System.Text;

namespace Infrasructure
{
    public class MazeFileSaver
    {
        private const int MaxSize = 50;

        public void Save(string path, Maze maze)
        {
            if (maze == null)
                throw new System.ArgumentNullException(nameof(maze));

            if (maze.Rows <= 1 || maze.Cols <= 1 || maze.Rows > MaxSize || maze.Cols > MaxSize)
                throw new System.ArgumentException($"Maze dimensions must be between 2 and {MaxSize}");

            using (var writer = new StreamWriter(path, false, Encoding.UTF8))
            {
                writer.WriteLine($"{maze.Rows} {maze.Cols}");
                writer.WriteLine(); 

                WriteWallMatrix(writer, maze.RightWalls, maze.Rows, maze.Cols);
                writer.WriteLine(); 

                WriteWallMatrix(writer, maze.BottomWalls, maze.Rows, maze.Cols);
            }
        }

        private void WriteWallMatrix(StreamWriter writer, bool[,] walls, int rows, int cols)
        {
            for (int i = 0; i < rows; i++)
            {
                var line = new StringBuilder();
                for (int j = 0; j < cols; j++)
                {
                    line.Append(walls[i, j] ? "1" : "0");
                    if (j < cols - 1)
                        line.Append(' ');
                }
                writer.WriteLine(line.ToString());
            }
        }
    }
}