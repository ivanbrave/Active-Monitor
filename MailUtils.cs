using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using Nustache.Core;

namespace CSMMon.ConsoleTool
{
    class MailUtils
    {
        const string altsmtphost = "smtphost.redmond.corp.microsoft.com";

        public static void SendEmail(string from, string to, string cc, string bcc, string subject, string template, string templateDirectory = null, string[] attachments = null, MailPriority priority = MailPriority.Normal)
        {
            if (templateDirectory == null)
            {
                templateDirectory = Environment.CurrentDirectory;
            }

            MailMessage mailMessage = new MailMessage { From = new MailAddress(from) };

            if (to != null)
            {
                mailMessage.To.Add(to);
            }

            if (cc != null)
            {
                mailMessage.CC.Add(cc);
            }

            if (bcc != null)
            {
                mailMessage.Bcc.Add(bcc);
            }

            mailMessage.Subject = subject;
            mailMessage.IsBodyHtml = true;
            mailMessage.Priority = priority;

            if (attachments != null)
            {
                foreach (string attachment in attachments)
                {
                    mailMessage.Attachments.Add(new Attachment(attachment));
                }
            }

            string text = Render.StringToString(template, null, null);
            int num = 10000;
            List<LinkedResource> list = new List<LinkedResource>();
            int num2 = text.IndexOf("<img", StringComparison.InvariantCultureIgnoreCase);
            if (num2 != -1)
            {
                do
                {
                    int num3 = text.IndexOf("src", num2, StringComparison.InvariantCultureIgnoreCase);
                    num3 = text.IndexOf("\"", num3) + 1;
                    int num4 = text.IndexOf("\"", num3 + 1);
                    string text2 = text.Substring(num3, num4 - num3).Replace("/", @"\");
                    if (text2.StartsWith(@"file:\\\"))
                    {
                        text2 = text2.Substring(8);
                    }
                    else
                    {
                        text2 = Path.Combine(templateDirectory, text2);
                    }
                    list.Add(new LinkedResource(text2)
                    {
                        ContentId = "resource" + num
                    });
                    text = string.Concat(new object[]
					{
						text.Substring(0, num3 - 1),
						"cid:resource",
						num,
						text.Substring(num4 + 1)
					});
                    num2 += 17 - (num4 - num3);
                    num2 = text.IndexOf("<img", num2 + 1, StringComparison.InvariantCultureIgnoreCase);
                    num++;
                }
                while (num2 != -1);
            }

            AlternateView alternateView = AlternateView.CreateAlternateViewFromString(text, null, "text/html");
            foreach (LinkedResource current in list)
            {
                alternateView.LinkedResources.Add(current);
            }

            mailMessage.AlternateViews.Add(alternateView);
            SmtpClient smtpClient = new SmtpClient(altsmtphost)
            {
                UseDefaultCredentials = true
            };

            smtpClient.Send(mailMessage);
            smtpClient.Dispose();
            mailMessage.Dispose();
        }
    }
}
