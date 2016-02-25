using Settings.Demo.Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;

namespace Settings.Demo
{
    public class Global : System.Web.HttpApplication
    {

        private Mark.Settings.SettingsManager<Settings.Demo.Base.Settings> _settings;

        protected void Application_Start(object sender, EventArgs e)
        {
            _settings = new Mark.Settings.SettingsManager<Settings.Demo.Base.Settings>();
            _settings.Changed += _settings_Changed;
            _settings_Changed(null, null);
        }

        private void _settings_Changed(object sender, EventArgs e)
        {
            Apps.Settings = _settings.Renew();
            log4net.Config.XmlConfigurator.ConfigureAndWatch(new FileInfo(_settings.Combine("log4net.config")));
        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}