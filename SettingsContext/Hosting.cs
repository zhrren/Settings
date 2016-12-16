using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mark.SettingsContext
{
    public class Hosting
    {
        private string _env;

        public string Env
        {
            get { return _env; }
            set { _env = string.IsNullOrWhiteSpace(value) ? "development" : value; }
        }

        public bool IsDevelopment()
        {
            return string.Equals("development", Env, StringComparison.OrdinalIgnoreCase);
        }
    }
}
