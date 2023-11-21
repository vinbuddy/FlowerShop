using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Configuration;
using System.Net.Mail;
using System.Net;

namespace FlowerShop.Ultils
{
    public class Ultils
    {
        public static string UploadFile(string folderPath, HttpPostedFileBase file)
        {
            var newFileName = Guid.NewGuid();
            var extension = Path.GetExtension(file.FileName);

            string fileName = Path.GetFileName(newFileName + extension);

            string path = Path.Combine(System.Web.HttpContext.Current.Server.MapPath(folderPath), fileName);
            file.SaveAs(path);

            return fileName;
        }

        private static string password = ConfigurationManager.AppSettings["PasswordEmail"];
        private static string email = ConfigurationManager.AppSettings["Email"];

        public static bool SendEmail(string name, string subject, string content, string toMail)
        {
            bool result = false;
            try
            {
                MailMessage message = new MailMessage();
                var smtp = new SmtpClient()
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    EnableSsl = true,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential()
                    {
                        UserName = email,
                        Password = password
                    }
                };

                MailAddress fromAddress = new MailAddress(email, name);
                message.From = fromAddress;
                message.To.Add(toMail);
                message.Subject = subject;
                message.IsBodyHtml = true;
                message.Body = content;
                smtp.Send(message);

                result = true;

            } catch(Exception)
            {
                result = false;

            }

            return result;
        }

    }
}