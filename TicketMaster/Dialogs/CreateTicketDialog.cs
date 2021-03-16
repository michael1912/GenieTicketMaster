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
                BranchStepAsync,
                ProcessBranchStepAsync,
                DescriptionStepAsync,
                ProcessDescriptionStepAsync,
                UploadAttachmentStepAsync,
                ProcessAttachmentStepAsync,
                ConfirmStepAsync,
                FinalStepAsync,
            }));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> TitleStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var titleStepMsgText = "Please, provide a title.";
            var promptMessage = MessageFactory.Text(titleStepMsgText, titleStepMsgText, InputHints.ExpectingInput);
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = promptMessage }, cancellationToken);
        }

        private async Task<DialogTurnResult> ProcessTitleStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var ticketDetails = (TicketDetails)stepContext.Options;
            ticketDetails.Title = (string)stepContext.Result;
            return await stepContext.NextAsync(null, cancellationToken);
        }

        private async Task<DialogTurnResult> DescriptionStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var descriptionStepMsgText = "Please, provide a description.";
            var promptMessage = MessageFactory.Text(descriptionStepMsgText, descriptionStepMsgText, InputHints.ExpectingInput);
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = promptMessage }, cancellationToken);
        }

        private async Task<DialogTurnResult> ProcessDescriptionStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var ticketDetails = (TicketDetails)stepContext.Options;
            ticketDetails.Description = (string)stepContext.Result;

            return await stepContext.NextAsync(null, cancellationToken);
        }

        private async Task<DialogTurnResult> BranchStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var branchStepMsgText = "Please, provide a branch.";
            var promptMessage = MessageFactory.Text(branchStepMsgText, branchStepMsgText, InputHints.ExpectingInput);
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = promptMessage }, cancellationToken);
        }

        private async Task<DialogTurnResult> ProcessBranchStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var ticketDetails = (TicketDetails)stepContext.Options;
            ticketDetails.Branch = (string)stepContext.Result;

            return await stepContext.NextAsync(null, cancellationToken);
        }

        private static async Task<DialogTurnResult> UploadAttachmentStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var attachmentStepMsgText = $"Please upload an attachment";
            var promptMessage = MessageFactory.Text(attachmentStepMsgText, attachmentStepMsgText, InputHints.ExpectingInput);
            return await stepContext.PromptAsync(nameof(AttachmentPrompt), new PromptOptions { Prompt = promptMessage }, cancellationToken);
        }

        private async Task<DialogTurnResult> ProcessAttachmentStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            List<Attachment> attachments = (List<Attachment>)stepContext.Result;
            string attachmentStepMsgText = string.Empty;

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

                attachmentStepMsgText = $"Attachment '{file.Name}' has been received.";
                var message = MessageFactory.Text(attachmentStepMsgText, attachmentStepMsgText, InputHints.IgnoringInput);
                await stepContext.Context.SendActivityAsync(message, cancellationToken);
            }

            var ticketDetails = (TicketDetails)stepContext.Options;
            ticketDetails.Attachment = attachments;

            return await stepContext.NextAsync(null, cancellationToken);
        }

        private async Task<DialogTurnResult> ConfirmStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var ticketDetails = (TicketDetails)stepContext.Options;

            var messageText = $"Please confirm ticket details:{Environment.NewLine}" +
                $"Title: {ticketDetails.Title}{Environment.NewLine}" +
                $"Description: {ticketDetails.Description}{Environment.NewLine}" +
                $"Branch: {ticketDetails.Branch}";
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
