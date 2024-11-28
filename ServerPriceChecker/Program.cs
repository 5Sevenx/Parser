using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Threading.Tasks;

public class Program
{
    public class Root
    {
        public List<Server> server { get; set; }
    }

    public class Server
    {
        public long id { get; set; }
        public string name { get; set; }
        public string cpu { get; set; }
        public decimal price { get; set; }
        public List<string> specials { get; set; } = new List<string>();
    }

    private static decimal previousPrice = -1;

    public static async Task Main()
    {
        string url = "https://www.hetzner.com/_resources/app/jsondata/live_data_sb.json?m=1732786048746";  // Site url

        // parse every 1m
        while (true)
        {
            await CheckServerPriceAsync(url);
            await Task.Delay(60000);  // 1m delay
        }
    }

    public static async Task CheckServerPriceAsync(string url)
    {
        using (var client = new HttpClient())
        {
            try
            {
                //=========================================================Json response from site============================================
                var response = await client.GetStringAsync(url);
                Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(response);

                //=========================================================Output server <=45 and specials contain "HWR"====================== 
                foreach (var server in myDeserializedClass.server)
                {
                    if (server.price <= 45 && server.specials.Contains("HWR"))
                    {
                        Console.WriteLine($"Server: {server.id} {server.cpu}, Price: {server.price} EUR");

                        // Send email when a matching server is found
                        await SendEmailAsync(server);
                    }
                }

                //=========================================================looking for price update===========================================
                if (previousPrice != -1 && previousPrice != myDeserializedClass.server[228].price)
                {
                    Console.WriteLine("Price updated!");
                }

                //=========================================================Save current result for next comparation===========================
                previousPrice = myDeserializedClass.server[228].price;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in processing server data: {ex.Message}");
            }
        }
    }

    public static async Task SendEmailAsync(Server server)
    {
        try
        {
            // SMTP settings 
            string smtpServer = "smtp.server.com"; 
            int smtpPort = 587; 
            string smtpUser = "user@server.com";
            string smtpPassword = "UserPasswd"; 

            // Email details
            string fromEmail = smtpUser;
            string toEmail = "reciver@gmail.com";
            string subject = "New Server Found!";
            string body = $"A new server has been found!\n\n" +
                          $"Server: {server.name}\n" +
                          $"ID: {server.id}\n" +
                          $"CPU: {server.cpu}\n" +
                          $"Price: {server.price} EUR\n" +
                          $"Specials: {string.Join(", ", server.specials)}";

            using (var client = new SmtpClient(smtpServer, smtpPort))
            {
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential(smtpUser, smtpPassword);
                client.EnableSsl = false;

                using (var message = new MailMessage())
                {
                    message.From = new MailAddress(fromEmail);
                    message.To.Add(new MailAddress(toEmail));
                    message.Subject = subject;
                    message.Body = body;
                    message.IsBodyHtml = false; 
                    await client.SendMailAsync(message);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending email: {ex.Message}");
        }
    }
}
