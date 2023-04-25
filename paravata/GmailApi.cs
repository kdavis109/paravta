using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Mail;
using System.Text;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Google.Apis.Gmail.v1.Data;
using MimeKit;


public class GmailApi
{
private const string APPLICATION_NAME = "apitest-384616";
private const string CREDENTIALS_FILE_PATH = @"C:\Users\kenda\Downloads\client_secret_433885651990-np5371hopcavq7rv7acum86ln1eg6c8n.apps.googleusercontent.com.json";
private GmailService service;

public GmailApi()
{
    service = new GmailService(new BaseClientService.Initializer
    {
        ApplicationName = APPLICATION_NAME,
        HttpClientInitializer = GetCredential().Result
    });

    // Set the HttpClient timeout to 5 minutes
    service.HttpClient.Timeout = TimeSpan.FromMinutes(5);
}

private async Task<UserCredential> GetCredential()
{
    UserCredential credential;

    using (var stream = new FileStream(CREDENTIALS_FILE_PATH, FileMode.Open, FileAccess.Read))
    {
        var credPath = Path.Combine(Directory.GetCurrentDirectory(), ".credentials", "gmail-dotnet-quickstart.json");

        credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
            GoogleClientSecrets.Load(stream).Secrets,
            new[] {
                GmailService.Scope.GmailSend,
                GmailService.Scope.GmailReadonly,
                GmailService.Scope.GmailCompose,
                GmailService.Scope.GmailModify
            },
            "user",
            CancellationToken.None,
            new FileDataStore(credPath, true)
        );
    }

    return credential;
}

public async Task SendEmailAsync(string recipient, string subject, string body, string? attachmentPath = null)
{
    var message = new MimeMessage();
    message.From.Add(new MailboxAddress("", service.Users.GetProfile("me").Execute().EmailAddress));
    message.To.Add(new MailboxAddress("", recipient));
    message.Subject = subject;
    var bodyBuilder = new BodyBuilder { TextBody = body };

    if (!string.IsNullOrEmpty(attachmentPath))
    {
        bodyBuilder.Attachments.Add(attachmentPath);
    }

    message.Body = bodyBuilder.ToMessageBody();
    var rawMessage = message.ToString();
    var base64Url = Convert.ToBase64String(Encoding.UTF8.GetBytes(rawMessage));
    var request = service.Users.Messages.Send(new Google.Apis.Gmail.v1.Data.Message
    {
        Raw = base64Url.Replace("/", "_").Replace("+", "-")
    }, "me");

    try
    {
        var response = await request.ExecuteAsync();
        if (response.LabelIds != null && response.LabelIds.Contains("SENT"))
        {
            Console.WriteLine("Email sent successfully.");
        }
        else
        {
            throw new InvalidOperationException($"Error sending email: The service gmail has thrown an exception. {response}");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error sending email: {ex.Message}");
    }
}
}

