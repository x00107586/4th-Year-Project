using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BotForProject.Dialogs;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;

namespace BotForProject.Models
{
    [LuisModel(Constants.LuisID, Constants.LuisSubscriptionKey)]
    [Serializable]
    public class RootLUISDialog : LuisDialog<object>
    {
        [LuisIntent("Predict")]
        public async Task PredictPdProgreeAsync(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("Ok! Just a moment please...");

            context.Call(new ForexDialog(), ResumeAfterOptionDialog);

            // context.Wait(MessageReceived)
        }

        [LuisIntent("None")]
        public async Task NoneAsync(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("Im sorry, I do not understand...");

            context.Wait(MessageReceived);
        }

        private static async Task ResumeAfterOptionDialog(IDialogContext context, IAwaitable<object> result)
        {
            try
            {
                await result;
            }
            catch (Exception ex)
            {
                await context.PostAsync($"Error: {ex.Message}");
            }
            finally
            {
                context.Done<object>(null);
            }
        }
    }
}
