using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.IO;

namespace Engine.Map
{
    public class Level
    {
        #region Fields

        // Key locations       
        private Point _start = InvalidPosition;
        private Point _exit = InvalidPosition;
        private static readonly Point InvalidPosition = new Point(-1, -1);

        // list of characters
        private List<string> _lines = new List<string>();
        #endregion

        #region Properties
        /// <summary>
        /// Width of level measured in tiles.
        /// </summary>
        public int Width
        {
            get { return _lines[0].Length; }
        }

        /// <summary>
        /// Height of the level measured in tiles.
        /// </summary>
        public int Height
        {
            get { return _lines.Count; }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Gets character at position (x,y)
        /// </summary>
        public char TileAt(int x, int y)
        {
            // Each line represents a row
            return _lines[y][x];
        }

        public void SetTileAt(int x, int y, char tile)
        {
            char[] tiles = _lines[y].ToCharArray();
            tiles[x] = tile;

            _lines[y] = new string(tiles);

        }
        /// <summary>
        /// loads level
        /// </summary>
        public Level(/*string levelAsset*/)
        {
            //LoadTiles(TitleContainer.OpenStream(levelAsset));
        }
        #endregion

        #region Loading
        /// <summary>
        /// Reads in and loads tiles and sets behaviour
        /// </summary>
        private void LoadTiles(Stream fileStream)
        {
            // Load the level and ensure all of the lines are the same length.
            int width;

            using (StreamReader reader = new StreamReader(fileStream))
            {
                string line = reader.ReadLine();
                width = line.Length;
                while (line != null)
                {
                    _lines.Add(line);
                    if (line.Length != width)
                        throw new Exception(String.Format("The length of line {0} is different from all preceeding lines.", _lines.Count));
                    line = reader.ReadLine();
                }
            }
        }
        #endregion
    }
}
