using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Settings.Tests
{
    public class Settings
    {
        public string RedisConnectionString { get; set; }
        public Media Media { get; set; }
        public Sms Sms { get; set; }
        public Message[] Messages { get; set; }
    }

    public class Media
    {
        public string Root { get; set; }
        public string Path { get; set; }
        public string[] Exts { get; set; }
    }
    public class Sms
    {
        public string Url { get; set; }
        public string SN { get; set; }
        public string Pwd { get; set; }
        public string Sign { get; set; }
        public string Templete { get; set; }
    }
    public class Message
    {
        public int Id { get; set; }
        public string Keyword { get; set; }
    }
}
