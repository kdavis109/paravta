using System;
using System.Threading.Tasks;
using System.Threading;

class Program
{
static async Task Main(string[] args)
{
var chatGPTApi = new ChatGPTApi();
var gmailApi = new GmailApi();

bool preTrained = false;
string preTrainedResponse = "";

Console.Clear();

string welcomeMessage = "Welcome to your email assistant Jeff!";
foreach (char c in welcomeMessage)
{
    Console.Write("\x1b[1;31m");
    Console.Write(c);
    Thread.Sleep(50); // animation speed
}
Console.WriteLine("\x1b[0m"); // Reset text properties
Console.WriteLine();

bool exitProgram = false; // initialize exit flag

while (!exitProgram) // check for exit flag
{
    string finalMessage = "";
    Console.WriteLine("Type out your thoughts and they will be turned into an email! (type 'done' when you want to end the conversation, or 'exit' to quit Email assistant):");

    while (true)
    {
        Console.Write("You: ");
        var input = Console.ReadLine();

        if (input.ToLower() == "done")
        {
            break;
        }
        else if (input.ToLower() == "exit") // set exit flag if user inputs "exit"
        {
            exitProgram = true;
            break;
        }

        if (!preTrained) // Send pre-training data to ChatGPT API
        {
            preTrainedResponse = chatGPTApi.GenerateText($"Please write a well-formatted email out of these thoughts {input}");
            preTrained = true;
            Console.WriteLine($"Jeff (pre-trained): {preTrainedResponse}");
            finalMessage = preTrainedResponse;
            continue;
        }

        // Generate text from ChatGPT API
        var response = chatGPTApi.GenerateText(input);
        finalMessage = response;
        // Display generated text
        Console.WriteLine($"Jeff: {response}");
    }


   if (!exitProgram) // only prompt for email sending if exit flag is not set
{
    // Confirm the final message
    Console.WriteLine("Final Draft:");
    Console.WriteLine(finalMessage);
    Console.Write("Confirm (yes/no): ");
    var confirm = Console.ReadLine().ToLower() == "yes";

    if (confirm)
    {
        Console.Write("Enter the email subject: ");
        string subject = Console.ReadLine();

        while (true)
        {
            Console.Write("Enter the recipient's email address: ");
            string recipientEmail1 = Console.ReadLine();

            Console.Write("Confirm the recipient's email address: ");
            string recipientEmail2 = Console.ReadLine();

    if (recipientEmail1 == recipientEmail2)
    {
        try
        {
            await gmailApi.SendEmailAsync(recipientEmail1, subject, finalMessage);
            Console.ForegroundColor = ConsoleColor.Green; // Set the color to green
            Console.WriteLine("Email sent successfully.");
            Console.ResetColor(); // Reset the text color
            break;
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red; // Set the color to red
            Console.WriteLine($"Error sending email: {ex.Message}");
            Console.ResetColor(); // Reset the text color
        }
    }
    else
    {
        Console.WriteLine("Emails do not match. Please try again.");
    }
   }
   }
  }
 }
} 
}
