using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameComponents
{
    /// <summary>
    /// Represents the actor 
    /// which will be responsible
    /// for moving throughout the maze
    /// </summary>
    public abstract class Actor
    {
       private Cell cell; /// the cell the actor is occupying

        /// <summary>
        /// The last direction the actor moved.
        /// </summary>
        public Maze.Direction LastDirectionMoved
        {
            get;
            set;
        }

        /// <summary>
        /// Returns the game cell
        /// in which the actor exists
        /// </summary>
        public Cell Cell
        {
            get { return cell; }
            set { cell = value; }
        }

    }
}
