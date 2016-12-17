using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mark.SettingsContext
{
    public class Hosting
    {
        private string _environmentName;

        public Hosting()
        {
            _environmentName = "Development";
        }

        public string EnvironmentName
        {
            get { return _environmentName; }
            set { _environmentName = string.IsNullOrWhiteSpace(value) ? "Development" : value; }
        }

        public bool IsDevelopment()
        {
            return string.Equals("Development", EnvironmentName, StringComparison.OrdinalIgnoreCase);
        }
    }
}
