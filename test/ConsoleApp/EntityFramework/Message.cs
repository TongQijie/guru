using System;

using Guru.EntityFramework;

namespace ConsoleApp.EntityFramework
{
    public class Message
    {
        [SimpleValue("Id")]
        public long Id { get; set; }

        [SimpleValue("SendTo")]
        public string SendTo { get; set; }

        [SimpleValue("SendFrom")]
        public string SendFrom { get; set; }

        [SimpleValue("CreateTime")]
        public DateTime CreateTime { get; set; }

        [SimpleValue("Type")]
        public string Type { get; set; }

        [SimpleValue("Content")]
        public string Content { get; set; }

        [SimpleValue("PictureUrl")]
        public string PictureUrl { get; set; }

        [SimpleValue("MediaId")]
        public string MediaId { get; set; }
    }

    public class Test
    {
        [SimpleValue("Id")]
        public int Id { get; set; }

        [SimpleValue("Value")]
        public string Value { get; set; }
    }
}