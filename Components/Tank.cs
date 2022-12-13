using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameComponents
{
    public class Tank : Actor
    {
        private int numberOfShells = 200;
        private int numberOfHints = 1;
        private int shellsUsed = 0;
        private int numberOfSteps = 0;
        private Maze.Direction shotDirection;

        /// Creates an actor object which will have a certain cell within a grid.
        public Tank(Cell position)
        {
            shotDirection = Maze.Direction.None;
            this.Cell = position;
        }

        /// The number of shells the tank  remaining.
        public int NumberOfShells
        {
            get { return numberOfShells; }
            set { numberOfShells = value; }
        }

        /// The direction in which the cannon is aimed.
        public Maze.Direction ShotDirection
        {
            get { return shotDirection; }
            set { shotDirection = value; }
        }

        public int NumberOfHints 
        {
            get => numberOfHints;
            set => numberOfHints = value; 
        }
        public int ShellsUsed { get => shellsUsed; set => shellsUsed = value; }
        public int NumberOfSteps { get => numberOfSteps; set => numberOfSteps = value; }
    }
}
