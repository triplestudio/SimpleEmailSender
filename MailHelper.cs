using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Text;

namespace SimpleEmailSender
{
    public class MailHelper
    {
        public static String EMAIL_USERNAME = ConfigurationManager.AppSettings["send_user_email"];
        public static String EMAIL_DISPNAME = ConfigurationManager.AppSettings["send_user_disp"];
        public static String EMAIL_PASSWORD = ConfigurationManager.AppSettings["send_user_pass"];
        public static String EMAIL_SMTP = ConfigurationManager.AppSettings["email_stmp"];
        public static ValidateResult SendMail(string email, string name, string content)
        {
            return SendMail("系统消息", email, name, content);
        }

        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="title">邮件标题</param>
        /// <param name="email">收件人地址</param>
        /// <param name="name">收件人名称</param>
        /// <param name="content">邮件内容</param>
        public static ValidateResult SendMail(string title, string email, string name, string content)
        {
            MailAddress from = new MailAddress(EMAIL_USERNAME, EMAIL_DISPNAME); //邮件的发件人"jinshuikeji@163.com", "system"
            MailMessage mail = new MailMessage();
            //设置邮件的标题
            mail.Subject = title;

            //设置邮件的发件人
            //Pass:如果不想显示自己的邮箱地址，这里可以填符合mail格式的任意名称，真正发mail的用户不在这里设定，这个仅仅只做显示用
            mail.From = from;

            //设置邮件的收件人
            mail.To.Add(new MailAddress(email, name));

            //设置邮件的内容
            mail.Body = content;

            //设置邮件的格式
            mail.BodyEncoding = System.Text.Encoding.UTF8;
            mail.IsBodyHtml = true;
            //设置邮件的发送级别
            mail.Priority = MailPriority.Normal;

            mail.DeliveryNotificationOptions = DeliveryNotificationOptions.OnSuccess;

            SmtpClient client = new SmtpClient();
            //设置用于 SMTP 事务的主机的名称，填IP地址也可以了
            client.Host = EMAIL_SMTP; 
            //设置用于 SMTP 事务的端口，默认的是 25
            client.Port = 25;
            client.UseDefaultCredentials = false;
            //这里才是真正的邮箱登陆名和密码
            client.Credentials = new System.Net.NetworkCredential(EMAIL_USERNAME, EMAIL_PASSWORD);
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            //都定义完了，正式发送了，很是简单吧！


            ValidateResult vr = new ValidateResult(true, "发送成功！");
            try
            {
                client.Send(mail);
                return vr;
            }
            catch (Exception e)
            {
                vr.IsValid = false;
                vr.Message = e.Message;
                return vr;
            }
        }
    }

    public class ValidateResult
    {
        public bool IsValid { get; set; }
        public string Message { get; set; }

        public ValidateResult() {  
        }

        public ValidateResult(bool v, string m)
        {
            IsValid = v;
            Message = m;
        }
    }
}
