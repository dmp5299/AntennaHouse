using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Mail;
using System.Windows.Forms;
using System.Net;

namespace AntennaHouseBusinessLayer.Library
{
    public class Mail
    {
        public void sendMail(string body)
        {
            MailAddress mailAddress = new MailAddress("pothierd5288@gmail.com");
            MailMessage mail = new MailMessage();
            mail.To.Add("david.pothier@rcmt.com");
            mail.From = mailAddress;
            mail.Subject = "AntennaHouse Error";

            mail.Body = body;
            mail.IsBodyHtml = true;
            mail.Priority = MailPriority.High;
            SmtpClient smtp = new SmtpClient();
            try
            {
                smtp.Host = "smtp.gmail.com"; //Or Your SMTP Server Address
                
                smtp.Credentials = new System.Net.NetworkCredential("pothierd5288@gmail.com", @"Dave&5210");
                smtp.EnableSsl = true;
                smtp.Send(mail);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}
