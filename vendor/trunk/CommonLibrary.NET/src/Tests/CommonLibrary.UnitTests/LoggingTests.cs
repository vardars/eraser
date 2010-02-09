using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;
using System.Resources;
using NUnit.Framework;

using ComLib;
using ComLib.Logging;


namespace CommonLibrary.Tests
{
    [TestFixture]
    public class LoggingTests
    {
        [Test]
        public void CanLogFile()
        {
            ILog log = new LogFile("TestApp", "%app%-%date%.log");
            log.Message("Testing1");
            log.Message("Testing2");
            log.Message("Testing3");
            log.Message("Testing4");
            log.Message("Testing5");
            log.Message("Testing6");
            log.Message("Testing7");            

        }
    }
}
