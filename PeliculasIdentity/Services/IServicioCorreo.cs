namespace PeliculasIdentity.Services
{
    public interface IServicioCorreo
    {
        Response SendMail(string toName, string toEmail, string subject, string body);
    }
}