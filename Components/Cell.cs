using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace GameComponents
{
    /// <summary>
    /// Represents a cell in a maze
    /// with four walls which will 
    /// either be passable or impassable
    /// </summary>
    public class Cell
    {
        private bool leftWallBlocked = true, rightWallBlocked = true, topWallBlocked = true, bottomWallBlocked = true;
        private int cellRow, cellCol;
        private bool cellVisisted;


        /// <summary>
        /// Copy contructure.
        /// </summary>
        /// <param name="cell">The cell to create a copy of.</param>
        public Cell (Cell cell)
        {
            if(cell != null)
            {
                this.leftWallBlocked = cell.LeftWall;
                this.RightWall = cell.RightWall;
                this.TopWall = cell.TopWall;
                this.Row = cell.Row;
                this.Col = cell.Col;
                this.BottomWall = cell.BottomWall;
            }

        }

        /// <summary>
        /// Creates a cell with the 
        /// specified row and column
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        public Cell(int row, int col)
        {
            Row = row;
            Col = col;
        }

        /// <summary>
        /// The column of the cell.
        /// </summary>
        public int Col
        {
            get { return cellCol; }
            set { cellCol = value; }
        }

        /// <summary>
        /// Returns the row of the cell.
        /// </summary>
        public int Row
        {
            get { return cellRow; }
            set { cellRow = value; }
        }

        /// <summary>
        /// Whether the bottom of the cell
        /// is passable or not.
        /// </summary>
        public bool BottomWall
        {
            get { return bottomWallBlocked; }
            set { bottomWallBlocked = value; }
        }

        /// <summary>
        /// Whether the top of the cell
        /// is passable or not.
        /// </summary>
        public bool TopWall
        {
            get { return topWallBlocked; }
            set { topWallBlocked = value; }
        }

        /// <summary>
        /// Whether the right of the cell
        /// is passable or not.
        /// </summary>
        public bool RightWall
        {
            get { return rightWallBlocked; }
            set { rightWallBlocked = value; }
        }

        /// <summary>
        /// Whether the left of the cell
        /// is passable or not.
        /// </summary>
        public bool LeftWall
        {
            get { return leftWallBlocked; }
            set { leftWallBlocked = value; }
        }


        /// <summary>
        /// Indicates whether it has been visited
        /// </summary>
        public bool Visited
        {
            get { return cellVisisted; }
            set { cellVisisted = value; }
        }

    }
}
