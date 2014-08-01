using Adventure;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace TestProject1
{
    
    
    /// <summary>
    ///This is a test class for PotTest and is intended
    ///to contain all PotTest Unit Tests
    ///</summary>
    [TestClass()]
    public class PotTest
    {


        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///A test for Pot Constructor
        ///</summary>
        [TestMethod()]
        public void PotConstructorTest()
        {
            GameWorld game = null; // TODO: Initialize to an appropriate value
            Area area = null; // TODO: Initialize to an appropriate value
            Pot target = new Pot(game, area);
            Assert.Inconclusive("TODO: Implement code to verify target");
        }

        /// <summary>
        ///A test for LoadContent
        ///</summary>
        [TestMethod()]
        public void LoadContentTest()
        {
            GameWorld game = null; // TODO: Initialize to an appropriate value
            Area area = null; // TODO: Initialize to an appropriate value
            Pot target = new Pot(game, area); // TODO: Initialize to an appropriate value
            target.LoadContent();
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for OnEntityCollision
        ///</summary>
        [TestMethod()]
        public void OnEntityCollisionTest()
        {
            GameWorld game = null; // TODO: Initialize to an appropriate value
            Area area = null; // TODO: Initialize to an appropriate value
            Pot target = new Pot(game, area); // TODO: Initialize to an appropriate value
            Entity other = null; // TODO: Initialize to an appropriate value
            target.OnEntityCollision(other);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for SpawnPickup
        ///</summary>
        [TestMethod()]
        public void SpawnPickupTest()
        {
            GameWorld game = null; // TODO: Initialize to an appropriate value
            Area area = null; // TODO: Initialize to an appropriate value
            Pot target = new Pot(game, area); // TODO: Initialize to an appropriate value
            Pickup expected = null; // TODO: Initialize to an appropriate value
            Pickup actual;
            actual = target.SpawnPickup();
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for StartBreaking
        ///</summary>
        [TestMethod()]
        public void StartBreakingTest()
        {
            GameWorld game = null; // TODO: Initialize to an appropriate value
            Area area = null; // TODO: Initialize to an appropriate value
            Pot target = new Pot(game, area); // TODO: Initialize to an appropriate value
            target.StartBreaking();
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for land
        ///</summary>
        [TestMethod()]
        [DeploymentItem("Adventure.exe")]
        public void landTest()
        {
            PrivateObject param0 = null; // TODO: Initialize to an appropriate value
            Pot_Accessor target = new Pot_Accessor(param0); // TODO: Initialize to an appropriate value
            target.land();
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for DropChance
        ///</summary>
        [TestMethod()]
        public void DropChanceTest()
        {
            GameWorld game = null; // TODO: Initialize to an appropriate value
            Area area = null; // TODO: Initialize to an appropriate value
            Pot target = new Pot(game, area); // TODO: Initialize to an appropriate value
            float actual;
            actual = target.DropChance;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }
    }
}
