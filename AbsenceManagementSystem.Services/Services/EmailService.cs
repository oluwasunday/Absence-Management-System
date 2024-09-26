using AbsenceManagementSystem.Core.DTO;
using AbsenceManagementSystem.Core.Handlers;
using AbsenceManagementSystem.Core.IServices;
using AbsenceManagementSystem.Core.UnitOfWork;
using Microsoft.AspNetCore.Http;
using System.Net.Mail;
using System.Net.Mime;
using sib_api_v3_sdk.Api;
using sib_api_v3_sdk.Client;
using sib_api_v3_sdk.Model;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System.Linq;

namespace AbsenceManagementSystem.Services.Services
{
    public class EmailService : IEmailService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuthenticationService _authenticationService;
        private readonly EmailSettings _emailSettings;
        private IConfiguration _configuration;

        public EmailService(IUnitOfWork unitOfWork, IConfiguration configuration, IAuthenticationService authenticationService, IOptions<EmailSettings> options)
        {
            _unitOfWork = unitOfWork;
            _configuration = configuration;
            _authenticationService = authenticationService;
            _emailSettings = options.Value;
        }

        public async Task<Response<string>> SendEmailAsync(EmailRequestDto mailRequest)
        {
            try
            {
                //Configuration.Default.ApiKey.Add("api-key", _emailSettings.ApiKey);
                Configuration.Default.ApiKey.Add("api-key", "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");

                var apiInstance = new TransactionalEmailsApi();
                string SenderName = _emailSettings.DisplayName;
                string SenderEmail = _emailSettings.Mail;
                SendSmtpEmailSender Email = new SendSmtpEmailSender(SenderName, SenderEmail);
                string ToEmail = mailRequest.ToEmail;
                string ToName = mailRequest.ToEmail;
                SendSmtpEmailTo smtpEmailTo = new SendSmtpEmailTo(ToEmail, ToName);
                List<SendSmtpEmailTo> To = new List<SendSmtpEmailTo>();
                To.Add(smtpEmailTo);

                //TODO: comment out later
                string CcName = mailRequest.CcName;
                string CcEmail = mailRequest.CcEmail;
                SendSmtpEmailCc CcData = new SendSmtpEmailCc(CcEmail, CcName);
                List<SendSmtpEmailCc> Cc = new List<SendSmtpEmailCc>();
                Cc.Add(CcData);


                string HtmlContent = mailRequest.Body;// $"<html><body><h1>This is my first transactional email for Absence Management System</h1></body></html>";
                string TextContent = mailRequest.Body;
                string Subject = mailRequest.Subject;
                //string ReplyToName = "John Doe";
                //string ReplyToEmail = "replyto@domain.com";
                //SendSmtpEmailReplyTo ReplyTo = new SendSmtpEmailReplyTo(null, null);
                //SendSmtpEmailReplyTo ReplyTo = new SendSmtpEmailReplyTo(ReplyToEmail, ReplyToName);
                string AttachmentUrl = null;
                string stringInBase64 = "aGVsbG8gdGhpcyBpcyB0ZXN0";
                byte[] Content = System.Convert.FromBase64String(stringInBase64);
                string AttachmentName = "test.txt";
                SendSmtpEmailAttachment AttachmentContent = new SendSmtpEmailAttachment(AttachmentUrl, Content, AttachmentName);
                List<SendSmtpEmailAttachment> Attachment = new List<SendSmtpEmailAttachment>();
                Attachment.Add(AttachmentContent);

                JObject Headers = new JObject();
                Headers.Add("Some-Custom-Name", "unique-id-1234");
                long? TemplateId = null;

                JObject Params = new JObject();
                //Params.Add("parameter", "My param value");
                //Params.Add("subject", "New Subject");

                // add tags
                List<string> Tags = new List<string>();
                Tags.Add(mailRequest.ToEmail.Split('@')[0]);

                SendSmtpEmailTo1 smtpEmailTo1 = new SendSmtpEmailTo1(ToEmail, ToName);
                List<SendSmtpEmailTo1> To1 = new List<SendSmtpEmailTo1>();
                To1.Add(smtpEmailTo1);
                Dictionary<string, object> _parmas = new Dictionary<string, object>();
                _parmas.Add("params", Params);
                //SendSmtpEmailReplyTo1 ReplyTo1 = new SendSmtpEmailReplyTo1();
                SendSmtpEmailMessageVersions messageVersion = new SendSmtpEmailMessageVersions(To1, _parmas, null, Cc, null, subject: Subject);
                List<SendSmtpEmailMessageVersions> messageVersiopns = new List<SendSmtpEmailMessageVersions>();
                messageVersiopns.Add(messageVersion);
                try
                {
                    var sendSmtpEmail = new SendSmtpEmail(Email, To, null, Cc, HtmlContent, TextContent, Subject, null, Attachment, Headers, TemplateId, null, messageVersiopns, Tags);
                    CreateSmtpEmail result = apiInstance.SendTransacEmail(sendSmtpEmail);
                    Debug.WriteLine(result.ToJson());
                    Console.WriteLine(result.ToJson());
                    //Console.ReadLine();

                    return new Response<string>
                    {
                        StatusCode = StatusCodes.Status204NoContent,
                        Succeeded = true,
                        Data = string.Join(',', result.MessageIds),
                        Message = "Mail sent successfully",
                        Errors = null
                    };
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                    Console.WriteLine(e.Message);
                    //Console.ReadLine();

                    return new Response<string>
                    {
                        StatusCode = StatusCodes.Status204NoContent,
                        Succeeded = false,
                        Data = "failed to send email",
                        Message = "Mail not sent",
                        Errors = $"{e.Message} - {e.StackTrace}"
                    };
                }
            }
            catch (Exception e)
            {
                return new Response<string>
                {
                    StatusCode = StatusCodes.Status204NoContent,
                    Succeeded = false,
                    Data = "failed to send email",
                    Message = "Mail not sent",
                    Errors = $"{e.Message} - {e.StackTrace}"
                };
            }
        }
    }
}
