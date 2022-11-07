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
    public class AwsMailService : IMailService
    {
        private readonly ILogger<AwsMailService> _logger;
        private readonly MailSettings _mailSettings;
        private string _fromLocalPart;

        public AwsMailService(ILogger<AwsMailService> logger, IOptions<MailSettings> mailSettings)
        {
            _logger = logger;
            _mailSettings = mailSettings.Value;
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
                if (!_mailSettings.Aws.HasValue)
                    throw new ArgumentException(GetArgumentEmptyMessage("Aws ses settings"));

                if (string.IsNullOrWhiteSpace(subject))
                    throw new ArgumentException(GetArgumentEmptyMessage(nameof(subject)));

                if (string.IsNullOrWhiteSpace(body))
                    throw new ArgumentNullException(GetArgumentEmptyMessage(nameof(body)));

                var recipients = _mailSettings.Recipients?.ToList() ?? new List<string>();

                if (!recipients.Any())
                    throw new ArgumentException(GetArgumentEmptyMessage(nameof(recipients)));

                using var client = new AmazonSimpleEmailServiceClient(_mailSettings.Aws.AccessKeyId, _mailSettings.Aws.SecretAccessKey, RegionEndpoint.APNortheast1);
                var sendRequest = new SendEmailRequest
                {
                    Source = $"{_fromLocalPart}@{_mailSettings.Aws.Source.Domain}",
                    SourceArn = _mailSettings.Aws.Source.Arn,
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
