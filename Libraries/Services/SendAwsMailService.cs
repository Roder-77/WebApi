using Amazon;
using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Models;
using Services.Interface;
using System.Net;

namespace Services
{
    public class SendAwsMailService : ISendMailService
    {
        private readonly ILogger<SendAwsMailService> _logger;
        private readonly Mail _mail;
        private string _fromLocalPart;

        public SendAwsMailService(ILogger<SendAwsMailService> logger, IOptions<Appsettings> appsettings)
        {
            _logger = logger;
            _mail = appsettings.Value.Mail;
            _fromLocalPart = "no-reply";
        }

        /// <summary>
        /// 發信
        /// </summary>
        /// <param name="subject">主旨</param>
        /// <param name="body">內容</param>
        /// <param name="carbonCopies">副本列表</param>
        /// <param name="blindCarbonCopies">密件副本列表</param>
        public async Task SendMail(string subject, string body, List<string>? carbonCopies = null, List<string>? blindCarbonCopies = null)
        {
            try
            {
                if (!_mail.Aws.HasValue)
                    throw new ArgumentException(GetArgumentEmptyMessage("Aws settings"));

                if (string.IsNullOrWhiteSpace(subject))
                    throw new ArgumentException(GetArgumentEmptyMessage(nameof(subject)));

                if (string.IsNullOrWhiteSpace(body))
                    throw new ArgumentNullException(GetArgumentEmptyMessage(nameof(body)));

                var recipients = _mail.Recipients?.ToList() ?? new List<string>();

                if (!recipients.Any())
                    throw new ArgumentException(GetArgumentEmptyMessage(nameof(recipients)));

                using var client = new AmazonSimpleEmailServiceClient(_mail.Aws.AccessKeyId, _mail.Aws.SecretAccessKey, RegionEndpoint.APNortheast1);
                var sendRequest = new SendEmailRequest
                {
                    Source = $"{_fromLocalPart}@{_mail.Aws.Source.Domain}",
                    SourceArn = _mail.Aws.Source.Arn,
                    Destination = new Destination
                    {
                        ToAddresses = recipients,
                        CcAddresses = carbonCopies,
                        BccAddresses = blindCarbonCopies
                    },
                    Message = new Message
                    {
                        Subject = new Content(subject),
                        Body = new Body
                        {
                            Html = new Content
                            {
                                Charset = "UTF-8",
                                Data = body
                            }
                        }
                    }
                };

                var response = await client.SendEmailAsync(sendRequest);

                if (response.HttpStatusCode != HttpStatusCode.OK)
                    _logger.LogWarning($"{nameof(SendMail)} fail");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(SendMail)} error");
            }
        }

        private string GetArgumentEmptyMessage(string value) => $"{value} is empty";
    }
}
