using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace GameComponents
{

    /// Represents a cell in a maze with four walls which will either be passable or impassable

    public class Cell
    {
        private bool leftWallBlocked = true, rightWallBlocked = true, topWallBlocked = true, bottomWallBlocked = true;
        private int cellRow, cellCol, heuristic, cost;
        private bool cellVisisted;


        /// Copy contructure.

        public Cell (Cell cell)
        {
            if(cell != null)
            {
                this.leftWallBlocked = cell.LeftWall;
                this.RightWall = cell.RightWall;
                this.TopWall = cell.TopWall;
                this.BottomWall = cell.BottomWall;
                this.Row = cell.Row;
                this.Col = cell.Col;
            }

        }


        /// Creates a cell with the specified row and column

        public Cell(int row, int col)
        {
            Row = row;
            Col = col;
        }


        /// The column of the cell.

        public int Col
        {
            get { return cellCol; }
            set { cellCol = value; }
        }


        /// Returns the row of the cell.

        public int Row
        {
            get { return cellRow; }
            set { cellRow = value; }
        }


        /// Whether the bottom of the cell is passable or not.

        public bool BottomWall
        {
            get { return bottomWallBlocked; }
            set { bottomWallBlocked = value; }
        }


        /// Whether the top of the cell is passable or not.

        public bool TopWall
        {
            get { return topWallBlocked; }
            set { topWallBlocked = value; }
        }


        /// Whether the right of the cell is passable or not.

        public bool RightWall
        {
            get { return rightWallBlocked; }
            set { rightWallBlocked = value; }
        }


        /// Whether the left of the cell is passable or not.

        public bool LeftWall
        {
            get { return leftWallBlocked; }
            set { leftWallBlocked = value; }
        }



        /// Indicates whether it has been visited

        public bool Visited
        {
            get { return cellVisisted; }
            set { cellVisisted = value; }
        }

        public int Heuristic { get => heuristic; set => heuristic = value; }
        public int Cost { get => cost; set => cost = value; }
    }
}
