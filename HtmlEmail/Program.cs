using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net.Mail;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Nustache.Core;

namespace HtmlEmail
{
    class Program
    {
        static void Main(string[] args)
        {
            Helpers.Register("FullName", FullName);
            var fromEmail = ConfigurationManager.AppSettings["fromEmail"];
            var toEmail = ConfigurationManager.AppSettings["toEmail"];
            var userName = ConfigurationManager.AppSettings["userName"];
            var password = ConfigurationManager.AppSettings["password"];
            var template = File.ReadAllText("EmailTemplate.html");
            var data = JsonConvert.DeserializeObject<Dictionary<string, string>>(
                File.ReadAllText("EmailData.json"));
            data.Add("date",DateTime.Now.ToShortDateString());
            var emailBody = ProcessTemplate(template, data);
            var mail = new MailMessage
            {
                From = new MailAddress(fromEmail),
                Subject = "Test Mail",
                Body = emailBody,
                IsBodyHtml = true
            };
            mail.To.Add(toEmail);
            var client = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new System.Net.NetworkCredential(userName, password),
                EnableSsl = true
            };

            client.Send(mail);
        }

        private static void FullName(RenderContext context, 
            IList<object> arguments, IDictionary<string, object> options, 
            RenderBlock fn, RenderBlock inverse)
        {
            if (arguments?.Count >= 2) 
                context.Write($"{arguments[0]} {arguments[1]}");
        }

        private static string ProcessTemplate(string template, 
            Dictionary<string, string> data)
        {
            //return Regex.Replace(template, "\\{\\{(.*?)\\}\\}", m =>
            //    m.Groups.Count > 1 && data.ContainsKey(m.Groups[1].Value) ? 
            //        data[m.Groups[1].Value] : m.Value);
            return Render.StringToString(template, data);
        }

    }
}
