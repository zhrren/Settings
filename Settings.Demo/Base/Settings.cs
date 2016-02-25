using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Settings.Demo.Base
{
    public class Settings
    {

        public string ConnectionString { get; set; }
        public Message[] Messages { get; set; }
    }

    public class Message
    {
        public int Id { get; set; }
        public string Keyword { get; set; }
    }
}
