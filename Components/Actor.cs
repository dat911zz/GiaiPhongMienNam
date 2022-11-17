using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameComponents
{

    /// Represents the actor  which will be responsible for moving throughout the maze

    public abstract class Actor
    {
       private Cell cell; /// the cell the actor is occupying


        /// The last direction the actor moved.

        public Maze.Direction LastDirectionMoved
        {
            get;
            set;
        }


        /// Returns the game cell in which the actor exists

        public Cell Cell
        {
            get { return cell; }
            set { cell = value; }
        }

    }
}
