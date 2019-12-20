using static CouchPoker_Server.Misc.Security;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace CouchPoker_Server_UnitTests
{
    [TestClass]
    public class CipherTests
    {
        [TestMethod]
        public void EncryptionTest() {
            string testString = "2|C";
            string EncryptedText = Encrypt(testString);
            string DecryptedText = Decrypt(EncryptedText);
            Assert.AreEqual(testString, DecryptedText);
        }
    }
}
