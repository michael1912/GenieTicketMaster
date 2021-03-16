using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace TicketMaster.Dialogs
{
    public class CreateTicketDialog : CancelAndHelpDialog
    {
        public CreateTicketDialog()
            : base(nameof(CreateTicketDialog))
        {
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new AttachmentPrompt(nameof(AttachmentPrompt)));
            AddDialog(new ConfirmPrompt(nameof(ConfirmPrompt)));
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                TitleStepAsync,
                ProcessTitleStepAsync,
                LabStepAsync,
                ProcessLabStepAsync,
                BranchStepAsync,
                ProcessBranchStepAsync,
                //ScenarioStepAsync,
                //ProcessScenarioStepAsync,
                ErrorStepAsync,
                ProcessErrorStepAsync,
                LinkToExecutionStepAsync,
                ProcessLinkToExecutionStepAsync,
                ConfirmAttachmentStepAsync,
                UploadAttachmentStepAsync,
                ProcessAttachmentStepAsync,
                ConfirmDescriptionStepAsync,
                DescriptionStepAsync,
                ProcessDescriptionStepAsync,
                ConfirmAllInputsStepAsync,
                FinalStepAsync,
            }));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> TitleStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {

            var titleStepMsgText = "Could you describe briefly what problem brings you here? I’ll set it to the title.";
            var promptMessage = MessageFactory.Text(titleStepMsgText, titleStepMsgText, InputHints.ExpectingInput);
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = promptMessage }, cancellationToken);
        }

        private async Task<DialogTurnResult> ProcessTitleStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var ticketDetails = (TicketDetails)stepContext.Options;
            ticketDetails.Title = (string)stepContext.Result;
            return await stepContext.NextAsync(null, cancellationToken);
        }
        private async Task<DialogTurnResult> LabStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var labStepMsgText = "Ok, got you. I need some more information to investigate this case. Let’s start with the environment. In what lab did it happen? Or, maybe it is a docker issue ? ";
            var promptMessage = MessageFactory.Text(labStepMsgText, labStepMsgText, InputHints.ExpectingInput);
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = promptMessage }, cancellationToken);
        }

        private async Task<DialogTurnResult> ProcessLabStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var ticketDetails = (TicketDetails)stepContext.Options;
            ticketDetails.Lab = (string)stepContext.Result;
            return await stepContext.NextAsync(null, cancellationToken);
        }

        private async Task<DialogTurnResult> LinkToExecutionStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var labStepMsgText = "So that's how it was! Let me now go into a little more detail. Could you please provide a link to the CI report or to build definition execution?";
            var promptMessage = MessageFactory.Text(labStepMsgText, labStepMsgText, InputHints.ExpectingInput);
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = promptMessage }, cancellationToken);
        }

        private async Task<DialogTurnResult> ProcessLinkToExecutionStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var ticketDetails = (TicketDetails)stepContext.Options;
            ticketDetails.LinkToExecution = (string)stepContext.Result;
            return await stepContext.NextAsync(null, cancellationToken);
        }

        private async Task<DialogTurnResult> ConfirmDescriptionStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var descriptionStepMsgText = "Any additional information you want to provide?";
            var promptMessage = MessageFactory.Text(descriptionStepMsgText, descriptionStepMsgText, InputHints.ExpectingInput);

            return await stepContext.PromptAsync(nameof(ConfirmPrompt), new PromptOptions { Prompt = promptMessage }, cancellationToken);
        }

        private async Task<DialogTurnResult> DescriptionStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if ((bool)stepContext.Result)
            {
                var descriptionStepMsgText = $"Please provide additiona details";
                var promptMessage = MessageFactory.Text(descriptionStepMsgText, descriptionStepMsgText, InputHints.ExpectingInput);
                return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = promptMessage }, cancellationToken);
            }

            return await stepContext.NextAsync(null, cancellationToken);
        }

        private async Task<DialogTurnResult> ProcessDescriptionStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var ticketDetails = (TicketDetails)stepContext.Options;
            ticketDetails.Description = (string)stepContext.Result;

            return await stepContext.NextAsync(null, cancellationToken);
        }

        private async Task<DialogTurnResult> BranchStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var branchStepMsgText = "Then I should identify with sources. On what branch did it occur?";
            var promptMessage = MessageFactory.Text(branchStepMsgText, branchStepMsgText, InputHints.ExpectingInput);
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = promptMessage }, cancellationToken);
        }

        private async Task<DialogTurnResult> ProcessBranchStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var ticketDetails = (TicketDetails)stepContext.Options;
            ticketDetails.Branch = (string)stepContext.Result;

            return await stepContext.NextAsync(null, cancellationToken);
        }
        private async Task<DialogTurnResult> ErrorStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var branchStepMsgText = "Perfect. Now, what really happened? What error message did you receive from your execution?";
            var promptMessage = MessageFactory.Text(branchStepMsgText, branchStepMsgText, InputHints.ExpectingInput);
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = promptMessage }, cancellationToken);
        }

        private async Task<DialogTurnResult> ProcessErrorStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var ticketDetails = (TicketDetails)stepContext.Options;
            ticketDetails.Error = (string)stepContext.Result;

            return await stepContext.NextAsync(null, cancellationToken);
        }

        //private async Task<DialogTurnResult> ScenarioStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        //{
        //    var branchStepMsgText = "Great, now i am all set with envirement. And what scenario or build definition did you run?";
        //    var promptMessage = MessageFactory.Text(branchStepMsgText, branchStepMsgText, InputHints.ExpectingInput);
        //    return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = promptMessage }, cancellationToken);
        //}

        //private async Task<DialogTurnResult> ProcessScenarioStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        //{
        //    var ticketDetails = (TicketDetails)stepContext.Options;
        //    ticketDetails.Scenario = (string)stepContext.Result;
        //    return await stepContext.NextAsync(null, cancellationToken);
        //}

        private async Task<DialogTurnResult> ConfirmAttachmentStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var ticketDetails = (TicketDetails)stepContext.Options;

            var messageText = $"Do you have attachments to upload?";
            var promptMessage = MessageFactory.Text(messageText, messageText, InputHints.ExpectingInput);

            return await stepContext.PromptAsync(nameof(ConfirmPrompt), new PromptOptions { Prompt = promptMessage }, cancellationToken);
        }

        private static async Task<DialogTurnResult> UploadAttachmentStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if ((bool)stepContext.Result)
            {
                var attachmentStepMsgText = $"Please attach relevant screenshots or logs. Fill free to attach a funny picture if don’t have any important documents)";
                var promptMessage = MessageFactory.Text(attachmentStepMsgText, attachmentStepMsgText, InputHints.ExpectingInput);
                return await stepContext.PromptAsync(nameof(AttachmentPrompt), new PromptOptions { Prompt = promptMessage }, cancellationToken);
            }

            return await stepContext.NextAsync(null, cancellationToken);
        }

        private async Task<DialogTurnResult> ProcessAttachmentStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var ticketDetails = (TicketDetails)stepContext.Options;
            List<Attachment> attachments = (List<Attachment>)stepContext.Result;
            if (attachments == null) 
            {
                return await stepContext.NextAsync(null, cancellationToken);
            }

            foreach (var file in attachments)
            {
                // Determine where the file is hosted.
                var remoteFileUrl = file.ContentUrl;

                // Save the attachment to the system temp directory.
                var localFileName = Path.Combine(Path.GetTempPath(), file.Name);

                // Download the actual attachment
                using (var webClient = new WebClient())
                {
                    webClient.DownloadFile(remoteFileUrl, localFileName);
                }

                ticketDetails.AttachmentPath.Add(localFileName);

                var attachmentStepMsgText = $"Attachment '{file.Name}' has been received.";
                var message = MessageFactory.Text(attachmentStepMsgText, attachmentStepMsgText, InputHints.IgnoringInput);
                await stepContext.Context.SendActivityAsync(message, cancellationToken);
            }

            return await stepContext.NextAsync(null, cancellationToken);
        }

        private async Task<DialogTurnResult> ConfirmAllInputsStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var ticketDetails = (TicketDetails)stepContext.Options;

            var messageText = $"So, let’s review.{Environment.NewLine}" +
                $"You faced up with \"{ticketDetails.Title}\" on {ticketDetails.Lab}, {ticketDetails.Branch}{Environment.NewLine}" +
                $"Error is \"{ticketDetails.Error}\"{Environment.NewLine}" +
                $"Now we are about to open a ticket.{Environment.NewLine}" +
                $"Click YES if all suits fine, take a breath, and click NO if something is wrong and you want to start from scratch";

            var promptMessage = MessageFactory.Text(messageText, messageText, InputHints.ExpectingInput);

            return await stepContext.PromptAsync(nameof(ConfirmPrompt), new PromptOptions { Prompt = promptMessage }, cancellationToken);
        }

        private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if ((bool)stepContext.Result)
            {
                var ticketDetails = (TicketDetails)stepContext.Options;

                return await stepContext.EndDialogAsync(ticketDetails, cancellationToken);
            }

            return await stepContext.EndDialogAsync(null, cancellationToken);
        }
    }
}
