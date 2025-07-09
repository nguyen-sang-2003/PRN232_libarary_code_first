namespace PRNLibrary.Services;

using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using System.Threading.Tasks;

public class EmailServiceSettings
{
    public string SmtpServer { get; set; } = "127.0.0.1";
    public int SmtpPort { get; set; } = 1025;
    public string FromEmail { get; set; } = "noreply@example.com";
    public string FromName { get; set; } = "PRN Library";
    public string? SmtpUsername { get; set; } = null;
    public string? SmtpPassword { get; set; } = null;
}
public class EmailService
{
    public EmailServiceSettings Settings { get; }
    public EmailService(IOptions<EmailServiceSettings> options)
    {
        if (options == null)
        {
            throw new ArgumentNullException(nameof(options), "Email service settings cannot be null");
        }

        if (options.Value == null)
        {
            throw new ArgumentNullException(nameof(options.Value), "Email service settings cannot be null");
        }
        Settings = options.Value;
    }

    public async Task SendEmailAsync(
        string toEmail,
        string subject,
        string htmlContent
    )
    {
        var email = new MimeMessage();
        email.From.Add(MailboxAddress.Parse(Settings.FromEmail));
        email.To.Add(MailboxAddress.Parse(toEmail));
        email.Subject = subject;
        email.Body = new TextPart("html")
        {
            Text = htmlContent
        };
        using var smtp = new SmtpClient();
        try
        {
            if (Settings.SmtpUsername == null || Settings.SmtpPassword == null)
            {
                await smtp.ConnectAsync(Settings.SmtpServer, Settings.SmtpPort, SecureSocketOptions.Auto);
            }
            else
            {
                await smtp.ConnectAsync(Settings.SmtpServer, Settings.SmtpPort, SecureSocketOptions.StartTls);
                await smtp.AuthenticateAsync(Settings.SmtpUsername, Settings.SmtpPassword);
            }
            await smtp.SendAsync(email);
        }
        catch (Exception ex)
        {
            // Log the exception or handle it as needed
            Console.WriteLine($"Error sending email: {ex.Message}");
            Console.WriteLine(ex.StackTrace);
            throw new InvalidOperationException("Failed to send email.", ex);
        }
        finally
        {
            await smtp.DisconnectAsync(true);
        }
    }
}
