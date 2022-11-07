namespace Services.Interface
{
    public interface IMailService
    {
        public delegate IMailService MailServiceResolver(MailServiceType type);

        public enum MailServiceType { Normal, Aws };

        Task SendMail(string subject, string body, List<string>? carbonCopies = null, List<string>? blindCarbonCopies = null);
    }
}
