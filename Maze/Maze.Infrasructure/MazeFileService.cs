using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core;

namespace Infrasructure
{
    public class MazeFileService
    {
        private readonly MazeFileLoader _loader;
        private readonly MazeFileSaver _saver;

        public MazeFileService()
        {
            _loader = new MazeFileLoader();
            _saver = new MazeFileSaver();
        }

        public Maze Load(string path) => _loader.Load(path);
        public void Save(string path, Maze maze) => _saver.Save(path, maze);
    }
}