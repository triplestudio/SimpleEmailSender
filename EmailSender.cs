using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace SimpleEmailSender
{
    public class EmailSender
    {
        #region 运行时数据
        // 邮箱列表
        private List<string> _EmailList = new List<string>();
        // 完成数量
        private volatile int _OverCount = 0;
        // 邮件标题
        private string _Title;
        // 邮件内容
        private string _Content;
        // 完成回调（主要是为了写日志）
        private Action<string> _Callback;
        #endregion

        /// <summary>
        /// 是否全部完成
        /// </summary>
        /// <returns></returns>
        public bool IsOver()
        {
            return _OverCount == _EmailList.Count;
        }

        /// <summary>
        /// 发起任务（如果不符合发起条件，则返回 false）
        /// </summary>
        /// <param name="emails"></param>
        /// <param name="title"></param>
        /// <param name="content"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public bool Send(List<string> emails, string title, string content, Action<string> callback)
        {
            if (!IsOver())
            {
                return false;
            }

            _EmailList = emails;
            _OverCount = 0;
            _Title = title;
            _Content = content;
            _Callback = callback;

            Start();
            return true;
        }

        /// <summary>
        /// 启动任务
        /// 
        /// 以线程池方式运行，每个邮箱不论成败完成数加1，并回调通知。
        /// </summary>
        private void Start()
        {
            foreach (string email in _EmailList)
            {
                var _email = email;
                ThreadPool.QueueUserWorkItem(t =>
                {
                    var vr = MailHelper.SendMail(_Title, _email, "", _Content);
                    _OverCount++;
                    _Callback(String.Format("进度[{3}/{4}] {0} 发送 {1}，返回：{2}", _email, vr.IsValid ? "成功" : "失败", vr.Message, _OverCount, _EmailList.Count));
                });
            }
        }


    }
}
