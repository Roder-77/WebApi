namespace Services.Interface
{
    public interface ISendMailService
    {
        Task SendMail(string subject, string body, List<string>? carbonCopies = null, List<string>? blindCarbonCopies = null);
    }
}
