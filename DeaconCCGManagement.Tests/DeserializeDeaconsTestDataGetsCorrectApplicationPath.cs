using System;
using DeaconCCGManagement.Test_Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DeaconCCGManagement.Tests
{
    [TestClass]
    public class DeserializeDeaconsTestDataGetsCorrectApplicationPath
    {
        [TestMethod]
        public void TestMethod1()
        {
            var deserializer = new TestDataDeserializer();

            var allDeacons = deserializer.DeserializeMembersTestData();

            Assert.IsTrue(allDeacons.Count > 0);
        }
    }
}
