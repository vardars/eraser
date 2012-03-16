using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using ComLib;
using ComLib;

using NUnit.Framework;


namespace CommonLibrary.Tests
{

    [TestFixture]
    public class WikiMarkupTests
    {
        [Test]
        public void CanParseHeadings()
        {
            var scripts = @"$[widget-create name=""blogroll"" /]";
            //var auto = new Automation(interpret: true);
            //auto.LoadFile("auto-scripts.cas");            
        }
    }
}
