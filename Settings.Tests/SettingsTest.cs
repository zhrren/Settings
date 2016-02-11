﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Settings.Tests
{
    [TestClass]
    public class SettingsTest
    {
        private Settings Settings { get; set; }

        [TestInitialize]
        public void Initialize()
        {
            Mark.Settings.SettingsManager manager = new Mark.Settings.SettingsManager();
            manager.Changed += Manager_Changed;
            Settings= manager.Renew<Settings>();
        }

        private void Manager_Changed(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void TestMethod1()
        {
            var sign = Settings.Sms.Sign;
            Assert.IsNotNull(sign);
        }
    }
}