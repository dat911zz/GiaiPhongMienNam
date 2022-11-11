using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GameComponents;

namespace GameTests
{
    [TestClass]
    public class CellTests
    {

        /// <summary>
        /// Test construction of a single maze cell.
        /// </summary>
        [TestMethod]
        public void TestCellConstruction()
        {
            int row = 0;
            int col = 0;

            Cell testCell = new Cell(row, col);
            Assert.IsTrue(testCell.Col == col);
            Assert.IsTrue(testCell.Row == row);
            Cell secondCell = new Cell(testCell);

            Assert.AreEqual(testCell.Row, secondCell.Row);
            Assert.AreEqual(testCell.Col, secondCell.Col);
            Assert.AreEqual(testCell.LeftWall, secondCell.LeftWall);
            Assert.AreEqual(testCell.RightWall, secondCell.RightWall);
            Assert.AreEqual(testCell.TopWall, secondCell.TopWall);
            Assert.AreEqual(testCell.BottomWall, secondCell.BottomWall);
        }

        /// <summary>
        /// Test toggling walls.
        /// </summary>
         [TestMethod]
        public void TestWallToggling()
        {
            int row = 0;
            int col = 0;

            Cell testCell = new Cell(row, col);

            testCell.BottomWall = true;
            testCell.LeftWall = true;
            testCell.RightWall = true;
            testCell.TopWall = true;

            Assert.IsTrue(testCell.BottomWall);
            Assert.IsTrue(testCell.LeftWall);
            Assert.IsTrue(testCell.RightWall);
            Assert.IsTrue(testCell.TopWall);
        }

        /// <summary>
        /// Test toggling visisted / not visisted
        /// </summary>
        [TestMethod]
        public void TestVisitedToggling()
        {
            int row = 0;
            int col = 0;

            Cell testCell = new Cell(row, col);
            testCell.Visited = true;
            Assert.IsTrue(testCell.Visited == true);
        }
    }
}
