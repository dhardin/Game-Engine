using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Engine.Map
{
    /// <summary>
    /// Controls the collision detection and response behavior of a tile.
    /// </summary>
    public enum TileCollision
    {
        Passable = 0,
        Impassable = 1,
    }

    /// <summary>
    /// Stores the appearance and collision behavior of a tile.
    /// </summary>
    public class Tile
    {
        public TileCollision Collision;

        /// <summary>
        /// Constructs a new tile.
        /// </summary>
        public Tile(TileCollision collision)
        {
            Collision = collision;
        }
    }
}
