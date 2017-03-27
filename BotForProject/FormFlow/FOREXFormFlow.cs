using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using BotForProject.Model;
using BotForProject.Models;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Builder.FormFlow.Advanced;
using Newtonsoft.Json;
using Microsoft.Bot.Builder.Dialogs;

namespace BotForProject.FormFlow
{
    public class ForexFormFlow
    {
        public IForm<ForexInput> BuildForm()
        {
            var builder = new FormBuilder<ForexInput>()
                .Field(nameof(ForexInput.CurrencyPairing))
                .Field(nameof(ForexInput.Year))
                .Field(nameof(ForexInput.Month))
                .Field(nameof(ForexInput.Date))
                .Field(nameof(ForexInput.Time))
               
                .OnCompletion(processScheduling);

            return builder.Build();
        }



        #region Engagement Form Validation

        readonly OnCompletionAsyncDelegate<ForexInput> processScheduling = async (context, state) =>
        {
            await context.PostAsync("Loading...");

            using (var client = new HttpClient())
            {
                // Replace this with the API key for the web service
                const string apiKey = "WBNAVJWSznyqRM8heHnHnrQrZM4Qtm9Bhvft6NUAM7Q+AeCwXKJlGmVW0TLhetH+x50GVRNqE3ZQjlYvOidrXA==";

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
                client.BaseAddress = new Uri("https://ussouthcentral.services.azureml.net/workspaces/b2502cf86d114498b7149462fda3fef9/services/e7ecacd1dc3d42c6bc1989eb710c6227/execute?api-version=2.0&format=swagger");

                var scoreRequest = GetScoreRequest(state);
                var response = await client.PostAsJsonAsync("", scoreRequest);

                if (response.IsSuccessStatusCode)
                {
                    // Avoiding deadlock with ConfigureAwait(false)
                    var result = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                    // Deserializing the JSON returned by the Azure ML web service
                    var forexOutput = JsonConvert.DeserializeObject<ForexOutput>(result);

                    // Getting the output array
                    var output = forexOutput.Results.Output;

                    // Getting the prediction value
                    var forexPrice = float.Parse(output[0].ForexRate);

                    await context.PostAsync($"Predicted Forex Rate: {forexPrice.ToString("c2")}");
                }
                else
                {
                    await context.PostAsync($"The request failed with status code: {response.StatusCode}");

                    // Print the headers - they iclude the requert ID and the timestamp, which are useful for debugging the failure

                    await context.PostAsync(response.Headers.ToString());

                    var responseContent = await response.Content.ReadAsStringAsync();

                    await context.PostAsync(responseContent);
                }
            }
        };

        private static object GetScoreRequest(ForexInput state)
        {
            return new
            {
                Inputs = GetInputs(state),
                GlobalParameters = new Dictionary<string, string>()
            };
        }

        private static Dictionary<string, List<Dictionary<string, string>>> GetInputs(ForexInput state)
        {
            return new Dictionary<string, List<Dictionary<string, string>>>
            {
                {
                    "input1",
                    GetInputValues(state)
                },
            };
        }

        private static List<Dictionary<string, string>> GetInputValues(ForexInput state)
        {
            return new List<Dictionary<string, string>>
            {
                new Dictionary<string, string>
                {
                    {
                        "currency", state.CurrencyPairing
                    },
                    {
                        "year", state.Year
                    },
                    {
                        "month", state.Month
                    },
                    {
                        "date", state.Date
                    },
                    {
                        "time", state.Time
                    }
                }
            };
        }


        private static Task<ValidateResult> ValidateStartDate(ForexInput state, object value)
        {
            var result = new ValidateResult
            {
                IsValid = true,
                Value = value
            };

            var startDate = (DateTime)value;

            DateTime formattedDt;

            if (DateTime.TryParse(startDate.ToString(new CultureInfo("pt-BR")), out formattedDt))
            {
                startDate = formattedDt;
            }

            if (startDate < DateTime.Today)
            {
                result.Feedback = "Invalid start date";
                result.IsValid = false;
            }

            return Task.FromResult(result);
        }



        #endregion
    }
}
