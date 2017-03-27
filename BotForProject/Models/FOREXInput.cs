using System;
using Microsoft.Bot.Builder.FormFlow;

namespace BotForProject.Model
{
    [Serializable]
    public class ForexInput
    {
        [Prompt("What is the currency pairing? (€/$ or £/$)")]
        public string CurrencyPairing { get; set; }

        [Prompt("What year do you wish to view? (Number Format,I.e 2010)")]
        public string Year { get; set; }

        [Prompt("What month do you wish to view? (Number Format, I.e 03 for March)")]
        public string Month { get; set; }

        [Prompt("What date do you wish to view?")]
        public string Date { get; set; }

        [Prompt("What time do you wish to view?")]
        public string Time { get; set; }
    }
}
