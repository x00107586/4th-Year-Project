using Newtonsoft.Json;

namespace BotForProject.Models
{
    public class ForexOutput
    {
        public Results Results { get; set; }
    }

    public class Results
    {
        [JsonProperty(PropertyName = "output1")]
        public Output[] Output { get; set; }
    }

    public class Output
    {
        [JsonProperty(PropertyName = "FOREX Rate")]
        public string ForexRate { get; set; }
    }
}
