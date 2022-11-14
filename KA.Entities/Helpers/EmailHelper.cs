using KA.Entities.Models.Email;
using Microsoft.Extensions.Configuration;
using System;
using System.Net.Mail;
using System.Text;

namespace KA.Entities.Helpers
{
    public class EmailHelper
    {
        private readonly string _host;
        private readonly string _from;
        private readonly string _alias;        

        public EmailHelper(IConfiguration configuration)
        {
            var smtpSection = configuration.GetSection("SMTP");
            if (smtpSection != null)
            {
                _host = smtpSection.GetSection("Host").Value;
                _from = smtpSection.GetSection("From").Value;
                _alias = smtpSection.GetSection("Alias").Value;
            }
        }

        public EmailHelper(string host, string from, string alias)
        {
            _host = host;
            _from = from;
            _alias = alias;
        }

        public bool SendMail(Email email)
        {
            try
            {
                using SmtpClient client = new SmtpClient(_host);
                MailMessage mailMessage = new MailMessage
                {
                    From = new MailAddress(_from, _alias)
                };                

                if (!string.IsNullOrWhiteSpace(email.ToEmail))
                {
                    mailMessage.To.Add(new MailAddress(email.ToEmail, string.IsNullOrWhiteSpace(email.ToName) ? email.ToEmail : email.ToName));
                }

                if (email.AddToEmail != null && email.AddToEmail.Length > 0)
                {
                    for (int i = 0; i < email.AddToEmail.Length; i++)
                    {
                        mailMessage.To.Add(new MailAddress(email.AddToEmail[i], string.IsNullOrWhiteSpace(email.AddToName[i]) ? email.AddToEmail[i] : email.AddToName[i]));
                    }
                }

                if (!string.IsNullOrWhiteSpace(email.CcEmail))
                {
                    mailMessage.CC.Add(new MailAddress(email.CcEmail, string.IsNullOrWhiteSpace(email.CcName) ? email.CcEmail : email.CcName));
                }

                if (email.AddCcEmail != null && email.AddCcEmail.Length > 0)
                {
                    for (int i = 0; i < email.AddCcEmail.Length; i++)
                    {
                        mailMessage.CC.Add(new MailAddress(email.AddCcEmail[i], string.IsNullOrWhiteSpace(email.AddCcName[i]) ? email.AddCcEmail[i] : email.AddCcName[i]));
                    }
                }

                var mailResult = false;

                if (mailMessage.To.Count > 0 || mailMessage.CC.Count > 0)
                {
                    mailMessage.BodyEncoding = Encoding.UTF8;
                    mailMessage.Subject = email.SubJect;
                    mailMessage.Body = email.Type switch
                    {
                        "Consign" => email.Body
                                        .Replace("_MAIL_SUBJECT_", email.SubJect)
                                        .Replace("_MAIL_BODY_", email.Body)
                                        .Replace("_SERVICE_DOMAIN_", email.ServiceDomain)
                                        .Replace("_MAIL_FOOTER_", email.Footer),
                        "FindPassword" => EmailForm.FormFindPassword
                                        .Replace("_MAIL_SUBJECT_", email.SubJect)
                                        .Replace("_MAIL_BODY_", email.Body)
                                        .Replace("_SERVICE_DOMAIN_", email.ServiceDomain)
                                        .Replace("_ETC1_", email.Etc1)
                                        .Replace("_MAIL_FOOTER_", email.Footer),
                        "JoinEmailAuthNum" => (email.IsKor ? EmailForm.FormEmailVerificationKr : EmailForm.FormEmailVerificationEn)
                                        .Replace("_MAIL_SUBJECT_", email.SubJect)
                                        .Replace("_MAIL_BODY_", email.Body)
                                        .Replace("_SERVICE_DOMAIN_", email.ServiceDomain)
                                        .Replace("_ETC1_", email.Etc1)
                                        .Replace("_ETC2_", email.Etc2)
                                        .Replace("_ETC3_", email.Etc3)
                                        .Replace("_ETC4_", email.Etc4)
                                        .Replace("_MAIL_FOOTER_", email.Footer),
                        _ => EmailForm.FormDefault
                                        .Replace("_MAIL_SUBJECT_", email.SubJect)
                                        .Replace("_MAIL_BODY_", email.Body)
                                        .Replace("_SERVICE_DOMAIN_", email.ServiceDomain)
                                        .Replace("_MAIL_FOOTER_", email.Footer)
                    };
                    mailMessage.IsBodyHtml = email.IsBodyHtml;
                    client.Send(mailMessage);
                    
                    mailResult = true;
                }

                return mailResult;
            }
            catch (Exception Ex) 
            {
                email.ErrorMessage = Ex.Message.ToString();
                email.ErrorStacktrace = Ex.StackTrace.ToString();
                return false; 
            }
        }
    }
}
