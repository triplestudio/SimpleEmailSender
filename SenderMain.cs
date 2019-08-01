using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace SimpleEmailSender
{
    public partial class SenderMain : Form
    {
        private EmailSender _Sender = new EmailSender();

        public SenderMain()
        {
            InitializeComponent();
        }

        // 发送按钮
        private void btnSend_Click(object sender, EventArgs e)
        {
            if (!_Sender.IsOver())
            {
                Log("之前的任务尚未完成，请等待完成！");
                return;
            }

            // 1. 提取邮件列表并格式化显示
            string mails = txtEmailList.Text.Trim();
            var list = ParseEmailList(mails);
            // 2. 格式化显示一下
            txtEmailList.Clear();
            foreach (var mail in list)
            {
                txtEmailList.AppendText(mail + "\r\n");
            }
            // 3. 发起任务
            var b = _Sender.Send(list, txtTitle.Text.Trim(), txtContent.Text, Log);
            Log(b ? "发起成功" : "发起失败");
        }

        /// <summary>
        /// 提取邮件列表
        /// </summary>
        /// <param name="mails"></param>
        /// <returns></returns>
        private List<string> ParseEmailList(string mails)
        {
            List<string> list = new List<string>();
            var mc = Regex.Matches(mails, @"\w+([\.-]?\w+)*@\w+([\.-]?\w+)*(\.\w{2,3})+", RegexOptions.IgnoreCase);

            foreach (Match c in mc)
            {
                list.Add(c.Value);
            }
            return list;
        }

        /// <summary>
        /// 日志输出支持线程中执行
        /// </summary>
        /// <param name="message"></param>
        private void Log(string message)
        {
            Invoke(new MethodInvoker(delegate
            {
                txtLog.AppendText(message + "\r\n");
            }));
        }
    }
}
