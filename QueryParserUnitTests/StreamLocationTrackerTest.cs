using HigginsThomas.QueryParser.Core;
using HigginsThomas.QueryParser.Scanner;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HigginsThomas.QueryParser.UnitTests
{
    /// <summary>
    ///This is a test class for StreamLocationTrackerTest and is intended
    ///to contain all StreamLocationTrackerTest Unit Tests
    ///</summary>
    [TestClass()]
    public class StreamLocationTrackerTest
    {
        ///<summary>
        ///Confirm the behavior of the default StreamLocationTracker constructor
        ///</summary>
        [TestMethod()]
        [DeploymentItem("HigginsThomas.QueryParser.dll")]
        public void StreamLocationTrackerConstructorTest()
        {
            StreamLocation expected = new StreamLocation(0, 1, 1);

            StreamLocationTracker sut = new StreamLocationTracker();

            Assert.AreEqual(expected, sut.Location);
        }

        ///<summary>
        ///Confirm Advance advances by one from starting state.
        ///</summary>
        [TestMethod()]
        [DeploymentItem("HigginsThomas.QueryParser.dll")]
        public void AdvanceTest_FromStart()
        {
            StreamLocation expected = new StreamLocation(1, 1, 2);

            StreamLocationTracker sut = new StreamLocationTracker();
            sut.Advance();

            Assert.AreEqual(expected, sut.Location);
        }

        ///<summary>
        ///Confirm Advance advances by one from arbitrary state.
        ///</summary>
        [TestMethod()]
        [DeploymentItem("HigginsThomas.QueryParser.dll")]
        public void AdvanceTest_FromArbitraryState()
        {
            StreamLocation startState = new StreamLocation(7, 5, 3);
            StreamLocation expected = new StreamLocation(8, 5, 4);

            StreamLocationTracker sut = new StreamLocationTracker(startState);
            sut.Advance();

            Assert.AreEqual(expected, sut.Location);
        }

        /// <summary>
        ///Confirm NewLine advances by one line from starting state.
        ///</summary>
        [TestMethod()]
        [DeploymentItem("HigginsThomas.QueryParser.dll")]
        public void NewLineTest_FromStart()
        {
            StreamLocation expected = new StreamLocation(1, 2, 1);

            StreamLocationTracker sut = new StreamLocationTracker();
            sut.NewLine();

            Assert.AreEqual(expected, sut.Location);
        }

        ///<summary>
        ///Confirm NewLine advances by one line from arbitrary state.
        ///</summary>
        [TestMethod()]
        [DeploymentItem("HigginsThomas.QueryParser.dll")]
        public void NewLineTest_FromArbitraryState()
        {
            StreamLocation startState = new StreamLocation(7, 5, 3);
            StreamLocation expected = new StreamLocation(8, 6, 1);

            StreamLocationTracker sut = new StreamLocationTracker(startState);
            sut.NewLine();

            Assert.AreEqual(expected, sut.Location);
        }

        ///<summary>
        ///Verify Advance(false) advances one position/column
        ///</summary>
        [TestMethod()]
        [DeploymentItem("HigginsThomas.QueryParser.dll")]
        public void AdvanceTest_WithNewLineFalse()
        {
            StreamLocation startState = new StreamLocation(7, 5, 3);
            StreamLocation expected = new StreamLocation(8, 5, 4);

            StreamLocationTracker sut = new StreamLocationTracker(startState);
            sut.Advance(false);

            Assert.AreEqual(expected, sut.Location);
        }

        ///<summary>
        ///Verify Advance(false) advances one position/line
        ///</summary>
        [TestMethod()]
        [DeploymentItem("HigginsThomas.QueryParser.dll")]
        public void AdvanceTest_WithNewLineTrue()
        {
            StreamLocation startState = new StreamLocation(7, 5, 3);
            StreamLocation expected = new StreamLocation(8, 6, 1);

            StreamLocationTracker sut = new StreamLocationTracker(startState);
            sut.Advance(true);

            Assert.AreEqual(expected, sut.Location);
        }
    }
}
