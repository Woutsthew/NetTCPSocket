using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace NetTCPSocket
{
    public class Message
    {
        [JsonProperty] private Queue<string> commands { get; set; } = new Queue<string>();

        [JsonProperty] public string value { get; set; } = "";

        public Message() { }

        public Message(string commands, char separator = ' ') : this(commands, "", separator) { }

        public Message(string commands, string value, char separator = ' ')
        {
            if (commands.Length != 0)
                foreach (string command in commands.Split(separator))
                    this.commands.Enqueue(command);

            this.value = value;
        }

        public Message(List<string> commands, string value = "")
        {
            foreach (string command in commands)
                this.commands.Enqueue(command);

            this.value = value;
        }

        [JsonConstructor]
        public Message(Queue<string> commands, string value = "")
        {
            this.commands = commands;
            this.value = value;
        }

        public string Dequeue()
        {
            string command = "";

            if (commands.Count != 0)
                command = commands.Dequeue();

            return command;
        }

        public void Enqueue(string command) { commands.Enqueue(command); }

        public string GetAllCommand(string separator = " ") => String.Join(separator, commands.ToArray()) + (value.Length != 0 ? ":" + value : "");
    }
}
