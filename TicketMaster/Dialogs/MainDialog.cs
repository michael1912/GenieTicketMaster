using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TicketMaster.Dialogs
{
    public class MainDialog : ComponentDialog
    {
        private readonly TicketRecognizer _luisRecognizer;
        private readonly MailSender _mailSender;
        protected readonly ILogger Logger;

        // Dependency injection uses this constructor to instantiate MainDialog
        public MainDialog(TicketRecognizer luisRecognizer, CreateTicketDialog ticketDialog, MailSender mailSender, ILogger<MainDialog> logger)
            : base(nameof(MainDialog))
        {
            _luisRecognizer = luisRecognizer;
            _mailSender = mailSender;
            Logger = logger;

            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(ticketDialog);
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                IntroStepAsync,
                ActStepAsync,
                FinalStepAsync,
            }));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> IntroStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if (!_luisRecognizer.IsConfigured)
            {
                //await stepContext.Context.SendActivityAsync(
                //    MessageFactory.Text("NOTE: LUIS is not configured. To enable all capabilities, add 'LuisAppId', 'LuisAPIKey' and 'LuisAPIHostName' to the appsettings.json file.", inputHint: InputHints.IgnoringInput), cancellationToken);
            }

            await stepContext.Context.SendActivityAsync(
               MessageFactory.Text($"I'm a Genie Bot and I will guide you through the ticket creation process.", inputHint: InputHints.IgnoringInput), cancellationToken);

            return await stepContext.NextAsync(null, cancellationToken);
        }

        private async Task<DialogTurnResult> ActStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            // LUIS is not configured, we just run the CreatingTicketDialog path with an empty TicketDetailsInstance.
            return await stepContext.BeginDialogAsync(nameof(CreateTicketDialog), new TicketDetails(), cancellationToken);
        }

        private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            // If the child dialog ("CreateTicketDialog") was cancelled, the user failed to confirm
            // the Result here will be null.
            var ticketDetails = (TicketDetails)stepContext.Result;

            if (ticketDetails != null)
            {
                // Now we have all the ticket details call the email to ticket master.
                var messageText = $"Creating a ticket";
                var message = MessageFactory.Text(messageText, messageText, InputHints.IgnoringInput);
                await stepContext.Context.SendActivityAsync(message, cancellationToken);

                //send an email
                var mailArgs = GetMailArguments(ticketDetails);
                _mailSender.SendEmail(mailArgs);
            }

            return await stepContext.Parent.CancelAllDialogsAsync(cancellationToken);
        }

        private MailArguments GetMailArguments(TicketDetails ticketDetails)
        {
            var mailArguments = new MailArguments();
            mailArguments.Subject = ticketDetails.Title;
            mailArguments.Message += $"Target version:{Environment.NewLine}{ticketDetails.Branch}{Environment.NewLine}";
            mailArguments.Message += $"Link to execution:{Environment.NewLine}{ticketDetails.LinkToExecution}{Environment.NewLine}";
            mailArguments.Message += $"Description version:{Environment.NewLine}{ticketDetails.Description}";

            var mailAttachments = new List<System.Net.Mail.Attachment>() { };
            foreach (var filePath in ticketDetails.AttachmentPath)
            {
                mailAttachments.Add(new System.Net.Mail.Attachment(filePath));
            }

            return mailArguments;
        }
    }
}
