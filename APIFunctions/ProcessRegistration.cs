using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SendGrid.Helpers.Mail;

namespace APIFunctions
{
    public class ProcessRegistration
    {
        [FunctionName("ProcessRegistration")]
        public void Run([QueueTrigger("apiregistrations", Connection = "storageConnection")]string myQueueItem, ILogger log,[SendGrid(ApiKey = "SendKey")] out SendGridMessage message)
        {
            log.LogInformation($"C# Queue trigger function processed: {myQueueItem}");
            var newReg = JsonConvert.DeserializeObject<Registration>(myQueueItem);
            log.LogInformation($"User Name: {newReg.UserName}");
            log.LogInformation($"Email: {newReg.email}");
            log.LogInformation($"Conf Code: {newReg.confCode}");
            message = new SendGridMessage();

            try
            {
                message.AddTo(newReg.email);
                message.SetFrom(new EmailAddress("navraj.aus@gmail.com"));
                if (newReg.type == "register")
                {
                    var mailData = new WelcomeeMailData();
                    mailData.user = newReg.UserName;
                    message.SetTemplateId("d-aa7fd154de1d46d8ae330c1d88a78880");
                    mailData.confCode = newReg.confCode;
                    message.SetTemplateData(mailData);
                }
                else
                {
                    message.AddContent("text/html", "Welcome to our cool new website");
                    message.SetSubject("Welcome to our site");
                }

            }
            catch (Exception ex)
            {
                log.LogInformation($"Error Message: {ex.Message}");
                log.LogInformation($"Error Trace: {ex.StackTrace}");
            }


        }
    }

    public class WelcomeeMailData
    {
        public string user { get; set; }
        public string confCode { get; set; } 
    }

    public class Registration
    {
        public string UserName { get; set; }
        public string type { get; set; }
        public string email { get; set; }
        public string confCode { get; set; }
    }
}
