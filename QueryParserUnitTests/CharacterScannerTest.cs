using System;
using System.IO;
using HigginsThomas.QueryParser.Core;
using HigginsThomas.QueryParser.Scanner;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HigginsThomas.QueryParser.UnitTests
{
    /// <summary>
    ///This is a test class for CharacterStreamTest and is intended
    ///to contain all CharacterStreamTest Unit Tests
    ///</summary>
    [TestClass()]
    public class CharacterScannerTest
    {
        private struct ExpectedOutcome
        {
            public int ch;
            public StreamLocation loc;
        };

        ///<summary>
        ///Confirm the behavior of the CharacterStream(String) constructor
        ///</summary>
        [TestMethod()]
        [DeploymentItem("HigginsThomas.QueryParser.dll")]
        public void CharacterStreamConstructorTest_String()
        {
            string testInput = "Test";
            StreamLocation expectedLocation = new StreamLocation(0, 1, 1);

            CharacterScanner sut = new CharacterScanner(testInput);

            Assert.AreEqual(expectedLocation, sut.Location);
            Assert.AreEqual(false, sut.IsEOF(), "Should not be at EOF");
            Assert.AreEqual(true, sut.HasNext(), "Should have next");
            Assert.AreEqual('T', sut.Peek());
        }

        ///<summary>
        ///Confirm the behavior of the CharacterStream(StringReader) constructor
        ///</summary>
        [TestMethod()]
        [DeploymentItem("HigginsThomas.QueryParser.dll")]
        public void CharacterStreamConstructorTest_StringReader()
        {
            StringReader testInput = new StringReader("Test");
            StreamLocation expectedLocation = new StreamLocation(0, 1, 1);

            CharacterScanner sut = new CharacterScanner(testInput);

            Assert.AreEqual(false, sut.IsEOF(), "Should not be at EOF");
            Assert.AreEqual(true, sut.HasNext(), "Should have next");
            Assert.AreEqual(expectedLocation, sut.Location);
            Assert.AreEqual('T', sut.Peek());
        }

        ///<summary>
        ///Confirm the behavior of the Start method
        ///</summary>
        [TestMethod()]
        [DeploymentItem("HigginsThomas.QueryParser.dll")]
        public void CharacterStreamStartTest()
        {
            StreamLocation expectedLocation = new StreamLocation(0, 1, 1);
            CharacterScanner sut = new CharacterScanner("Test");

            sut.Start();

            Assert.IsTrue(IsStarted(sut), "Should not be able to set new location if started");
            Assert.AreEqual(expectedLocation, sut.Location);
        }

        ///<summary>
        ///Confirm the behavior of the Close method
        ///</summary>
        [TestMethod()]
        [DeploymentItem("HigginsThomas.QueryParser.dll")]
        public void CharacterStreamCloseTest()
        {
            StringReaderHarness harness = new StringReaderHarness("Test");
            CharacterScanner sut = new CharacterScanner(harness);

            sut.Close();

            Assert.AreEqual(true, harness.DisposeInvoked, "Dispose should have been invoked");
            Assert.AreEqual(true, sut.IsEOF(), "Should now indicate EOF");
        }

        ///<summary>
        ///Confirm essential stream behavior of the Consume and Location operations.
        ///</summary>
        [TestMethod()]
        [DeploymentItem("HigginsThomas.QueryParser.dll")]
        public void CharacterStreamConsumeAndLocationTest()
        {
            char EOF = new CharacterScanner("").EOF;
            // A three line input containing embedded "EOF" characters (which should be skipped over)
            string testInput = "A test\nof the " + EOF + EOF + " character stream.\nThree lines of stuff.";
            ExpectedOutcome[] expected = { new ExpectedOutcome() {ch= 'A',  loc= new StreamLocation( 0, 1,  1)},
                                           new ExpectedOutcome() {ch= ' ',  loc= new StreamLocation( 1, 1,  2)},
                                           new ExpectedOutcome() {ch= 't',  loc= new StreamLocation( 2, 1,  3)},
                                           new ExpectedOutcome() {ch= 'e',  loc= new StreamLocation( 3, 1,  4)},
                                           new ExpectedOutcome() {ch= 's',  loc= new StreamLocation( 4, 1,  5)},
                                           new ExpectedOutcome() {ch= 't',  loc= new StreamLocation( 5, 1,  6)},
                                           new ExpectedOutcome() {ch= '\n', loc= new StreamLocation( 6, 1,  7)},
                                           new ExpectedOutcome() {ch= 'o',  loc= new StreamLocation( 7, 2,  1)},
                                           new ExpectedOutcome() {ch= 'f',  loc= new StreamLocation( 8, 2,  2)},
                                           new ExpectedOutcome() {ch= ' ',  loc= new StreamLocation( 9, 2,  3)},
                                           new ExpectedOutcome() {ch= 't',  loc= new StreamLocation(10, 2,  4)},
                                           new ExpectedOutcome() {ch= 'h',  loc= new StreamLocation(11, 2,  5)},
                                           new ExpectedOutcome() {ch= 'e',  loc= new StreamLocation(12, 2,  6)},
                                           new ExpectedOutcome() {ch= ' ',  loc= new StreamLocation(13, 2,  7)},
                                           new ExpectedOutcome() {ch= ' ',  loc= new StreamLocation(16, 2, 10)},
                                           new ExpectedOutcome() {ch= 'c',  loc= new StreamLocation(17, 2, 11)},
                                           new ExpectedOutcome() {ch= 'h',  loc= new StreamLocation(18, 2, 12)},
                                           new ExpectedOutcome() {ch= 'a',  loc= new StreamLocation(19, 2, 13)},
                                           new ExpectedOutcome() {ch= 'r',  loc= new StreamLocation(20, 2, 14)},
                                           new ExpectedOutcome() {ch= 'a',  loc= new StreamLocation(21, 2, 15)},
                                           new ExpectedOutcome() {ch= 'c',  loc= new StreamLocation(22, 2, 16)},
                                           new ExpectedOutcome() {ch= 't',  loc= new StreamLocation(23, 2, 17)},
                                           new ExpectedOutcome() {ch= 'e',  loc= new StreamLocation(24, 2, 18)},
                                           new ExpectedOutcome() {ch= 'r',  loc= new StreamLocation(25, 2, 19)},
                                           new ExpectedOutcome() {ch= ' ',  loc= new StreamLocation(26, 2, 20)},
                                           new ExpectedOutcome() {ch= 's',  loc= new StreamLocation(27, 2, 21)},
                                           new ExpectedOutcome() {ch= 't',  loc= new StreamLocation(28, 2, 22)},
                                           new ExpectedOutcome() {ch= 'r',  loc= new StreamLocation(29, 2, 23)},
                                           new ExpectedOutcome() {ch= 'e',  loc= new StreamLocation(30, 2, 24)},
                                           new ExpectedOutcome() {ch= 'a',  loc= new StreamLocation(31, 2, 25)},
                                           new ExpectedOutcome() {ch= 'm',  loc= new StreamLocation(32, 2, 26)},
                                           new ExpectedOutcome() {ch= '.',  loc= new StreamLocation(33, 2, 27)},
                                           new ExpectedOutcome() {ch= '\n', loc= new StreamLocation(34, 2, 28)},
                                           new ExpectedOutcome() {ch= 'T',  loc= new StreamLocation(35, 3,  1)},
                                           new ExpectedOutcome() {ch= 'h',  loc= new StreamLocation(36, 3,  2)},
                                           new ExpectedOutcome() {ch= 'r',  loc= new StreamLocation(37, 3,  3)},
                                           new ExpectedOutcome() {ch= 'e',  loc= new StreamLocation(38, 3,  4)},
                                           new ExpectedOutcome() {ch= 'e',  loc= new StreamLocation(39, 3,  5)},
                                           new ExpectedOutcome() {ch= ' ',  loc= new StreamLocation(40, 3,  6)},
                                           new ExpectedOutcome() {ch= 'l',  loc= new StreamLocation(41, 3,  7)},
                                           new ExpectedOutcome() {ch= 'i',  loc= new StreamLocation(42, 3,  8)},
                                           new ExpectedOutcome() {ch= 'n',  loc= new StreamLocation(43, 3,  9)},
                                           new ExpectedOutcome() {ch= 'e',  loc= new StreamLocation(44, 3, 10)},
                                           new ExpectedOutcome() {ch= 's',  loc= new StreamLocation(45, 3, 11)},
                                           new ExpectedOutcome() {ch= ' ',  loc= new StreamLocation(46, 3, 12)},
                                           new ExpectedOutcome() {ch= 'o',  loc= new StreamLocation(47, 3, 13)},
                                           new ExpectedOutcome() {ch= 'f',  loc= new StreamLocation(48, 3, 14)},
                                           new ExpectedOutcome() {ch= ' ',  loc= new StreamLocation(49, 3, 15)},
                                           new ExpectedOutcome() {ch= 's',  loc= new StreamLocation(50, 3, 16)},
                                           new ExpectedOutcome() {ch= 't',  loc= new StreamLocation(51, 3, 17)},
                                           new ExpectedOutcome() {ch= 'u',  loc= new StreamLocation(52, 3, 18)},
                                           new ExpectedOutcome() {ch= 'f',  loc= new StreamLocation(53, 3, 19)},
                                           new ExpectedOutcome() {ch= 'f',  loc= new StreamLocation(54, 3, 20)},
                                           new ExpectedOutcome() {ch= '.',  loc= new StreamLocation(55, 3, 21)},
                                           new ExpectedOutcome() {ch= EOF, loc= new StreamLocation(56, 3, 22)},
                                           new ExpectedOutcome() {ch= EOF, loc= new StreamLocation(56, 3, 22)}
                                         };

            CharacterScanner sut = new CharacterScanner(testInput);
            sut.Start();

            int index = 0;
            foreach (ExpectedOutcome it in expected)
            {
                StreamLocation loc = sut.Location;
                char ch = sut.Consume();

                Assert.AreEqual(it.ch, ch, String.Format("Unexpected character @ {0}", index));
                Assert.AreEqual(it.loc, loc);

                ++index;
            }
        }

        ///<summary>
        ///Confirm sentinel returned if read past EOF
        ///</summary>
        [TestMethod, DeploymentItem("HigginsThomas.QueryParser.dll")]
        public void CharacterStreamConsumeTest_ReadPastEnd()
        {
            // A three line input containing embedded "EOF" characters (which should be skipped over)
            string testInput = "X";
            CharacterScanner sut = new CharacterScanner(testInput);
            sut.Start();
            sut.Consume();

            Assert.AreEqual(sut.EOF, sut.Consume());
        }

        ///<summary>
        ///Confirm Peek operation is stable (returns same result each time)
        ///</summary>
        [TestMethod, DeploymentItem("HigginsThomas.QueryParser.dll")]
        public void CharacterStreamPeekTest()
        {
            string testInput = "abcdefghijklmnopqrstuvwxyz";
            ExpectedOutcome expected = new ExpectedOutcome() { ch= 'e', loc= new StreamLocation(4, 1, 5) };

            CharacterScanner sut = new CharacterScanner(testInput);
            sut.Start();
            for (int i = 0; i < 4; ++i) sut.Consume();

            Assert.AreEqual(expected.ch, sut.Peek());
            Assert.AreEqual(expected.loc, sut.Location);

            Assert.AreEqual(expected.ch, sut.Peek(), "unexpected change!");
            Assert.AreEqual(expected.loc, sut.Location, "unexpected change!");
        }

        ///<summary>
        ///Confirm sentinel returned if read past EOF
        ///</summary>
        [TestMethod, DeploymentItem("HigginsThomas.QueryParser.dll")]
        public void CharacterStreamPeekTest_ReadPastEnd()
        {
            // A three line input containing embedded "EOF" characters (which should be skipped over)
            string testInput = "X";
            CharacterScanner sut = new CharacterScanner(testInput);
            sut.Start();
            sut.Consume();

            Assert.AreEqual(sut.EOF, sut.Peek());
        }

        ///<summary>
        ///Confirm Match operation when successful
        ///</summary>
        [TestMethod()]
        [DeploymentItem("HigginsThomas.QueryParser.dll")]
        public void CharacterStreamMatchTest_Success()
        {
            string testInput = "abcdefghijklmnopqrstuvwxyz";
            char expected = 'e';

            CharacterScanner sut = new CharacterScanner(testInput);
            sut.Start();
            for (int i = 0; i < 4; ++i) sut.Consume();

            sut.Match(expected);
        }

        ///<summary>
        ///Confirm Match operation when fails
        ///</summary>
        [TestMethod, ExpectedException(typeof(ParseException))]
        [DeploymentItem("HigginsThomas.QueryParser.dll")]
        public void CharacterStreamMatchTest_Failure()
        {
            string testInput = "abcdefghijklmnopqrstuvwxyz";
            char expectFail = 'f';

            CharacterScanner sut = new CharacterScanner(testInput);
            sut.Start();
            for (int i = 0; i < 4; ++i) sut.Consume();

            sut.Match(expectFail);
        }

        ///<summary>
        ///Confirm Reset operation is not supported
        ///</summary>
        [TestMethod]
        [DeploymentItem("HigginsThomas.QueryParser.dll")]
        public void CharacterStreamResetTest() 
        {
            string testInput = "abcdefghijklmnopqrstuvwxyz";
            bool expectResp = false;

            CharacterScanner sut = new CharacterScanner(testInput);
            sut.Start();
            for (int i = 0; i < 4; ++i) sut.Consume();
            bool resp = sut.Reset();

            Assert.AreEqual(expectResp, resp);
        }

        [TestMethod]
        [DeploymentItem("HigginsThomas.QueryParser.dll")]
        public void CharacterStreamIsEOFHasNextTest_NotEOF() 
        {
            string testInput = "a";
            bool expectEOF = false;
            StreamLocation expectedLocation = new StreamLocation(0, 1, 1);
            
            CharacterScanner sut = new CharacterScanner(testInput);
            sut.Start();

            Assert.AreEqual(expectEOF, sut.IsEOF(), "EOF unexpected");
            Assert.AreEqual(!expectEOF, sut.HasNext(), "HasNext expected");
            Assert.AreEqual(expectedLocation, sut.Location);
        }

        [TestMethod]
        [DeploymentItem("HigginsThomas.QueryParser.dll")]
        public void CharacterStreamIsEOFHasNextTest_AtEOF()
        {
            string testInput = "a";
            bool expectEOF = true;
            StreamLocation expectedLocation = new StreamLocation(1, 1, 2);

            CharacterScanner sut = new CharacterScanner(testInput);
            sut.Start();
            sut.Consume();

            Assert.AreEqual(expectEOF, sut.IsEOF(), "EOF expected");
            Assert.AreEqual(!expectEOF, sut.HasNext(), "HasNext unexpected");
            Assert.AreEqual(expectedLocation, sut.Location);
        }

        [TestMethod]
        [DeploymentItem("HigginsThomas.QueryParser.dll")]
        public void CharacterStreamIsEOFHasNextTest_EmptyStream()
        {
            string testInput = "";
            bool expectEOF = true;
            StreamLocation expectedLocation = new StreamLocation(0, 1, 1);

            CharacterScanner sut = new CharacterScanner(testInput);
            sut.Start();

            Assert.AreEqual(expectEOF, sut.IsEOF(), "EOF expected");
            Assert.AreEqual(!expectEOF, sut.HasNext(), "HasNext unexpected");
            Assert.AreEqual(expectedLocation, sut.Location);
        }

        #region Helpers
        #region Helper methods
        private static bool IsStarted(CharacterScanner sut)
        {
            try
            {
                sut.SetStartLocation(new StreamLocation(3, 2, 1));
                return false;
            }
            catch (ParserException)
            {
                return true;
            }
        }
        #endregion

        #region Helper classes
        private class StringReaderHarness : StringReader
        {
            public bool DisposeInvoked { get; private set; }

            public StringReaderHarness(string s) : base(s) { }

            protected override void Dispose(bool b)
            {
                base.Dispose(b);
                DisposeInvoked = true;
            }
        }
        #endregion
        #endregion
    }
}
