using GH.Context;
using GH.Utils;
using PostAgent.databases;
using PostAgent.Domain;
using PostAgent.Domain.App;
using PostAgent.Domain.Cfgs;
using PostAgent.forms;
using Serilog;
using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PostAgent.mail
{
    public static class MailAgent
    {
        public static async void Send()
        {
            BeginSending();
            Inform(InfoType.Info, "Ждите: отправка началась...");
            Inform(InfoType.Info, "Получение списка писем...");
            BaseIdLists letters = await Databases.GetModule().GetLetters();
            if (letters.TotalCount == 0)
            {
                Inform(InfoType.Info, "Писем нет!");
                FinishSending();
                return;
            }
            Inform(InfoType.Info, $"{letters.TotalCount} писем...");

            CfgPost cfgPost = RunContext.GetCfgApp().Get<CfgPost>();

            using (SmtpClient smtp = new SmtpClient(cfgPost.Host, cfgPost.Port))
            {
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.EnableSsl = true;
                smtp.Timeout = 10 * 1000;
                smtp.Credentials = new NetworkCredential(cfgPost.Username, cfgPost.Password);
                await SendAll(cfgPost, smtp, letters);
                Inform(InfoType.Info, $"Отправлено {letters.SentCount} из {letters.TotalCount}");
                await Databases.GetModule().Save();
                Inform(InfoType.Info, $"Сохранение...");
                FinishSending();
            }

        }

        //private static Task<BaseIdLists> GetLetters()
        //{
        //    return Task<BaseIdLists>.Run(() =>
        //    {
        //        return Databases.GetModule().GetBaseIdLists();
        //    });
        //}


        private static Task SendAll(CfgPost cfgPost, SmtpClient smtp, BaseIdLists letters)
        {
            return Task.Run(() =>
            {
                int errCount = 0; 
                try
                {
                    foreach (var letter in letters)
                    {
                        if (letter.Value.Count == 0)
                            continue;

                        cfgPost.Selected = letter.Key;

                        MailAddress from = new MailAddress(cfgPost.EmailAddress, cfgPost.DisplayName, Encoding.UTF8);
                        MailAddress replyTo = new MailAddress(cfgPost.ReplyEmailAddress ?? cfgPost.EmailAddress, cfgPost.DisplayName, Encoding.UTF8);

                        foreach (Letter mail in letter.Value)
                        {
                            Inform(InfoType.Info, $"Отправка: {mail.DisplayName}...");
                            MailAddress to = new MailAddress(mail.EmailAddress, mail.DisplayName, Encoding.UTF8);
                            using (MailMessage message = new MailMessage(from, to))
                            {
                                message.ReplyToList.Add(replyTo);
                                message.Subject = mail.Subject;
                                message.Body = mail.Body;
                                message.IsBodyHtml = true;
                                message.BodyEncoding = Encoding.UTF8;
                                message.SubjectEncoding = Encoding.UTF8;
                                if (mail.Attachment != null)
                                {
                                    if (File.Exists(mail.Attachment))
                                    {
                                        var attachment = new Attachment(mail.Attachment);
                                        attachment.NameEncoding = Encoding.UTF8;
                                        message.Attachments.Add(attachment);
                                        Inform(InfoType.Info, $"Вложение {attachment.Name}");
                                    }
                                    else
                                    {
                                        //Inform(InfoType.Error, $"Вложение {Path.GetFileName(mail.Attachment)} не найдено :(");
                                        Inform(InfoType.Error, $"Вложение {mail.Attachment} не найдено :(");
                                    }

                                }

                                try
                                {
                                    smtp.Send(message);
                                    mail.Sended = true;
                                }
                                catch (Exception e)
                                {
                                    errCount++;
                                    if (errCount > 5)                                    
                                        Application.Exit();
                                    
                                   Log.Error(e, "Error");                                
                                   Inform(InfoType.Error, e);
                                }

                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    errCount++;
                    if (errCount > 5)
                        Application.Exit();
                    Log.Error(e, "Error");
                    Inform(InfoType.Error, e);

                }
            });
        }
        private static void BeginSending()
        {
            if (Application.OpenForms[0] is IMaimForm maimForm)
                maimForm.BeginSending();
        }

        private static void FinishSending()
        {
            if (Application.OpenForms[0] is IMaimForm maimForm)
                maimForm.FinishSending();
        }
        private static void Inform(InfoType type, string message)
        {
            if (Application.OpenForms[0] is IMaimForm maimForm)
                maimForm.ShowMessage(type.GetDescription(), message);
        }

        private static void Inform(InfoType type, Exception e)
        {
            Inform(type, e.ToString());
        }

    }
}
