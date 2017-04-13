﻿using HotTaoMonitoring.UserControls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CefSharp;
using CefSharp.WinForms;
using System.IO;
using HotCoreUtils.Helper;
using System.Collections.Specialized;

namespace HotTaoMonitoring
{
    public partial class EditForm : Form
    {
        NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        #region 移动窗口
        /*
         * 首先将窗体的边框样式修改为None，让窗体没有标题栏
         * 实现这个效果使用了三个事件：鼠标按下、鼠标弹起、鼠标移动
         * 鼠标按下时更改变量isMouseDown标记窗体可以随鼠标的移动而移动
         * 鼠标移动时根据鼠标的移动量更改窗体的location属性，实现窗体移动
         * 鼠标弹起时更改变量isMouseDown标记窗体不可以随鼠标的移动而移动
         */
        private bool isMouseDown = false;
        private Point FormLocation;     //form的location
        private Point mouseOffset;      //鼠标的按下位置
        public void WinForm_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isMouseDown = true;
                FormLocation = this.Location;
                mouseOffset = Control.MousePosition;
            }
        }

        public void WinForm_MouseUp(object sender, MouseEventArgs e)
        {
            isMouseDown = false;
        }
        public void WinForm_MouseMove(object sender, MouseEventArgs e)
        {
            int _x = 0;
            int _y = 0;
            if (isMouseDown)
            {
                Point pt = Control.MousePosition;
                _x = mouseOffset.X - pt.X;
                _y = mouseOffset.Y - pt.Y;

                this.Location = new Point(FormLocation.X - _x, FormLocation.Y - _y);
            }

        }
        #endregion

        private MainForm mainForm { get; set; }

        /// <summary>
        /// 监控窗口
        /// </summary>
        /// <value>The listen form.</value>
        private ListenForm listenForm { get; set; }

        /// <summary>
        /// 回复群标识
        /// </summary>
        /// <value>The name of to user.</value>
        public string toUserName { get; set; }

        /// <summary>
        /// 群名称
        /// </summary>
        /// <value>The name of to show.</value>
        public string toShowName { get; set; }


        /// <summary>
        /// 图片路径
        /// </summary>
        /// <value>The image path.</value>
        private string imagePath { get; set; }

        /// <summary>
        /// 回复目标昵称
        /// </summary>
        /// <value>The name of to nick.</value>
        public string toNickName { get; set; }

        /// <summary>
        /// 窗口是否关闭
        /// </summary>
        /// <value>true if this instance is close; otherwise, false.</value>
        public bool isHide { get; set; }

        public int rowIndex { get; set; }


        /// <summary>
        /// 当前群用户标识
        /// </summary>
        /// <value>The MSG send user.</value>
        public string MsgSendUser { get; set; }

        private ChromiumWebBrowser webKitBrowser1 { get; set; }


        private static string url = "chat.html";

        private const string _base64head = "data:image/png;base64,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";

        public EditForm(MainForm form, ListenForm listen)
        {
            InitializeComponent();
            mainForm = form;
            listenForm = listen;
            url = System.IO.Path.Combine(Application.StartupPath, url);
        }


        private void EditForm_Load(object sender, EventArgs e)
        {
            LoadBrowser();
            SetTitle(toNickName);
        }

        /// <summary>
        /// 设置标题
        /// </summary>
        /// <param name="title">The title.</param>
        public void SetTitle(string title)
        {
            lbTitle.Text = title;
        }

        public void LoadHtml()
        {
            _totalHtml = loadCacheData();
            webKitBrowser1.LoadHtml(_totalHtml, url);
        }


        private void LoadBrowser()
        {
            if (webKitBrowser1 == null)
            {

                webKitBrowser1 = new ChromiumWebBrowser(url);

                BrowserSettings settings = new BrowserSettings()
                {
                    LocalStorage = CefState.Enabled,
                    Javascript = CefState.Enabled,
                };
                webKitBrowser1.Location = new Point(1, 0);
                webKitBrowser1.Dock = DockStyle.Fill;
                webKitBrowser1.MouseMove += WinForm_MouseMove;
                hotWebKitBrowser.Controls.Add(webKitBrowser1);
                LoadHtml();
            }
        }

        private void picClose_Click(object sender, EventArgs e)
        {
            isHide = true;
            this.Hide();
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            bool isNotify = false;
            if (mainForm != null)
            {
                string sendText = txtContent.Text.Trim();
                string _RtfText = txtContent.Rtf;
                txtContent.Clear();
                string notify = string.Format("@{0}", toNickName);

                List<string> _ImageList = new List<string>();
                while (true)
                {
                    int _Index = _RtfText.IndexOf("pichgoal");
                    if (_Index == -1) break;
                    _RtfText = _RtfText.Remove(0, _Index + 8);

                    _Index = _RtfText.IndexOf("\r\n");

                    _RtfText = _RtfText.Remove(0, _Index);
                    string __RtfText = _RtfText.Replace("\r\n", "");

                    _Index = __RtfText.IndexOf("}");
                    string dd = __RtfText.Substring(0, _Index);
                    string filenamekey = EncryptHelper.MD5(__RtfText.Substring(0, _Index));
                    _ImageList.Add(filenamekey);
                }
                List<string> files = new List<string>();
                _ImageList.ForEach(str =>
                {
                    foreach (var item in SendFileList)
                    {
                        if (item.Key == str)
                            files.Add(item.Value);
                    }
                });

                if (files != null && files.Count() > 0)
                {
                    mainForm.wxlogin.AutoSendMessage(toUserName, notify);
                    isNotify = true;
                    foreach (var str in files)
                    {
                        string ext = str.Substring(str.LastIndexOf("."));
                        string filename = EncryptHelper.MD5(str) + ext;
                        using (Stream stream = new FileStream(str, FileMode.Open))
                        {
                            byte[] buffer = new byte[stream.Length];
                            //读取图片字节流
                            stream.Read(buffer, 0, Convert.ToInt32(stream.Length));
                            string base64 = "data:img/jpg;base64," + Convert.ToBase64String(buffer);
                            ShowSendImageMsg("我", base64, mainForm.MyIcon(), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                            mainForm.wxlogin.AutoSendImage(toUserName, filename, stream);
                        }
                    }
                    //设置消息列表回复状态
                    mainForm.listenForm.SetDataContentStatus(rowIndex, MsgSendUser, toUserName);
                }
                if (!string.IsNullOrEmpty(sendText))
                {
                    if (!isNotify)
                        mainForm.wxlogin.AutoSendMessage(toUserName, notify);//@指定用户
                    //给指定用户发消息
                    mainForm.wxlogin.AutoSendMessage(toUserName, sendText);
                    //发送回复消息到UI
                    ShowSendMsg("我", sendText.Replace("\n", "<br/>"), mainForm.MyIcon(), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    if (!isNotify)
                    {
                        //设置消息列表回复状态
                        mainForm.listenForm.SetDataContentStatus(rowIndex, MsgSendUser, toUserName);
                    }
                }
                writeCacheData();
                txtContent.Focus();
                if (SendFileList != null)
                    SendFileList.Clear();
            }
        }


        #region  消息框操作相关
        /// <summary>
        /// UI界面显示发送消息
        /// </summary>
        public void ShowSendMsg(string formShowName, string msg, string base64, string time)
        {
            string str = @"<script type=""text/javascript"">window.location.hash = ""#ok"";</script> 
            <p class=""timename"">" + time + @"</p> 
            <div class=""chat_content_group self"">               
            <p class=""chat_nick""><img src=" + (string.IsNullOrEmpty(base64) ? _base64head : base64) + @" width=""40px;"">" + @"</p>   
            <p class=""chat_content"">" + msg + @"</p>
            <p style=""clear: both""></p>
            </div><a id='ok'></a>";
            if (_totalHtml == "")
            {
                _totalHtml = _baseHtml;
            }
            LoadBrowser();
            _totalHtml = _totalHtml.Replace("<a id='ok'></a>", "") + str;
            webKitBrowser1.LoadHtml(_totalHtml, url);
        }
        /// <summary>
        /// UI界面显示发送图片消息
        /// </summary>
        /// <param name="formShowName">Name of the form show.</param>
        /// <param name="msg">The MSG.</param>
        public void ShowSendImageMsg(string formShowName, string base64Image, string base64, string time)
        {
            string str = @"<script type=""text/javascript"">window.location.hash = ""#ok"";</script> 
            <p class=""timename"">" + time + @"</p> 
            <div class=""chat_content_group self"">               
            <p class=""chat_nick""><img src=" + (string.IsNullOrEmpty(base64) ? _base64head : base64) + @" width=""40px;"">" + @"</p>   
            <p style=""float: right;margin-top: 0px;margin-right: 10px;""><img src=""" + base64Image + @""" width=""80px""></p>
            <p style=""clear: both""></p>
            </div><a id='ok'></a>";
            if (_totalHtml == "")
            {
                _totalHtml = _baseHtml;
            }
            LoadBrowser();
            _totalHtml = _totalHtml.Replace("<a id='ok'></a>", "") + str;
            webKitBrowser1.LoadHtml(_totalHtml, url);
        }


        /// <summary>
        /// UI界面显示接收消息
        /// </summary>
        public void ShowReceiveMsg(string toShowName, string msg, string time, string base64)
        {

            int idx = msg.IndexOf("<br/>");

            msg = msg.Substring(idx + 5);

            string str = @"<script type=""text/javascript"">window.location.hash = ""#ok"";</script> 
            <p class=""timename"">" + time + @"</p>             
            <div class=""chat_content_group buddy"">               
            <p class=""chat_nick""><img src=" + (string.IsNullOrEmpty(base64) ? _base64head : base64) + @" width=""40px;"">" + @"</p>   
            <p class=""chat_content"">" + msg + @"</p>
            <p style=""clear: both""></p>
            </div><a id='ok'></a>";
            if (_totalHtml == "")
            {
                _totalHtml = _baseHtml;
            }
            LoadBrowser();
            _totalHtml = _totalHtml.Replace("<a id='ok'></a>", "") + str;
            webKitBrowser1.LoadHtml(_totalHtml, url);
        }



        /// <summary>
        /// UI界面显示接收消息
        /// </summary>
        public void ShowReceiveMsg(string toShowName, string msg, string time, string base64, string msgImageBase64)
        {

            int idx = msg.IndexOf("<br/>");

            msg = msg.Substring(idx + 5);

            string str = @"<script type=""text/javascript"">window.location.hash = ""#ok"";</script> 
            <p class=""timename"">" + time + @"</p>             
            <div class=""chat_content_group buddy"">               
            <p class=""chat_nick""><img src=" + (string.IsNullOrEmpty(base64) ? _base64head : base64) + @" width=""40px;"">" + @"</p>   
            <p style=""float: left;margin-top: 0px;margin-left: 10px;""><img src=""" + msgImageBase64 + @""" width=""80px""></p>
            <p style=""clear: both""></p>
            </div><a id='ok'></a>";
            if (_totalHtml == "")
            {
                _totalHtml = _baseHtml;
            }
            LoadBrowser();
            _totalHtml = _totalHtml.Replace("<a id='ok'></a>", "") + str;
            webKitBrowser1.LoadHtml(_totalHtml, url);
        }




        /// <summary>
        /// 获取UI界面显示
        /// </summary>
        /// <param name="_toShowName">Name of the _to show.</param>
        /// <param name="_msg">The _MSG.</param>
        /// <returns>System.String.</returns>
        public string GetReceiveMsgHtml(string _toShowName, string _nickName, string _msg, string time, string base64)
        {
            string __totalHtml = loadCacheData(_toShowName, _nickName);
            int idx = _msg.IndexOf("<br/>");
            _msg = _msg.Substring(idx + 5);
            string str = @"<script type=""text/javascript"">window.location.hash = ""#ok"";</script> 
            <p class=""timename"">" + time + @"</p>     
            <div class=""chat_content_group buddy"">               
            <p class=""chat_nick""><img src=" + (string.IsNullOrEmpty(base64) ? _base64head : base64) + @" width=""40px;"">" + @"</p>   
            <p class=""chat_content"">" + _msg + @"</p>
            <p style=""clear: both""></p>
            </div><a id='ok'></a>";
            if (__totalHtml == "")
            {
                __totalHtml = _baseHtml;
            }
            __totalHtml = __totalHtml.Replace("<a id='ok'></a>", "") + str;
            return __totalHtml;
        }


        /// <summary>
        /// 获取UI界面显示
        /// </summary>
        /// <param name="_toShowName">Name of the _to show.</param>
        /// <param name="_msg">The _MSG.</param>
        /// <returns>System.String.</returns>
        public string GetReceiveMsgHtml(string _toShowName, string _nickName, string _msg, string time, string base64, string msgImageBase64)
        {
            string __totalHtml = loadCacheData(_toShowName, _nickName);
            int idx = _msg.IndexOf("<br/>");
            _msg = _msg.Substring(idx + 5);
            string str = @"<script type=""text/javascript"">window.location.hash = ""#ok"";</script> 
            <p class=""timename"">" + time + @"</p>     
            <div class=""chat_content_group buddy"">               
            <p class=""chat_nick""><img src=" + (string.IsNullOrEmpty(base64) ? _base64head : base64) + @" width=""40px;"">" + @"</p>   
            <p style=""float: left;margin-top: 0px;margin-left: 10px;""><img src=""" + msgImageBase64 + @""" width=""80px""></p>
            <p style=""clear: both""></p>
            </div><a id='ok'></a>";
            if (__totalHtml == "")
            {
                __totalHtml = _baseHtml;
            }
            __totalHtml = __totalHtml.Replace("<a id='ok'></a>", "") + str;
            return __totalHtml;
        }





        public string _totalHtml = "";
        public string _baseHtml = @"<html><head>
        <script type=""text/javascript"">window.location.hash = ""#ok"";</script>
        <style type=""text/css"">

        /*滚动条宽度*/  
        ::-webkit-scrollbar {  
        width: 8px;  
        }  
            
        /* 轨道样式 */  
        ::-webkit-scrollbar-track {  
        }  
   
        /* Handle样式 */  
        ::-webkit-scrollbar-thumb {  
        border-radius: 10px;  
        background: rgba(0,0,0,0.2);   
        }  
  
        /*当前窗口未激活的情况下*/  
        ::-webkit-scrollbar-thumb:window-inactive {  
        background: rgba(0,0,0,0.1);   
        }  
  
        /*hover到滚动条上*/  
        ::-webkit-scrollbar-thumb:vertical:hover{  
        background-color: rgba(0,0,0,0.3);  
        }  
        /*滚动条按下*/  
        ::-webkit-scrollbar-thumb:vertical:active{  
        background-color: rgba(0,0,0,0.7);  
        }  
        textarea{width: 500px;height: 300px;border: none;padding: 5px;}  

	    .chat_content_group.self {
        text-align: right;
        }
        .chat_content_group {
        padding: 0px;
        }
        .chat_content_group.self>.chat_content {
        text-align: left;
        }
        .chat_content_group.self>.chat_content {
    background: #7ccb6b;
    color: #fff;
    float: right;
    margin-right: 10px;
    margin-top:10px;
        /*background: -webkit-gradient(linear,left top,left bottom,from(white,#e1e1e1));
        background: -webkit-linear-gradient(white,#e1e1e1);
        background: -moz-linear-gradient(white,#e1e1e1);
        background: -ms-linear-gradient(white,#e1e1e1);
        background: -o-linear-gradient(white,#e1e1e1);
        background: linear-gradient(#fff,#e1e1e1);*/
        }
        .chat_content {
    display: inline-block;
    min-height: 16px;
    max-width: 50%;
    color: #292929;
    background: #c3f1fd;
    font-family: 微软雅黑;
    font-size: 14px;
    -webkit-border-radius: 5px;
    -moz-border-radius: 5px;
    border-radius: 5px;
    padding: 2px 15px;
    word-break: break-all;
    line-height: 1.4;
    float: left;
    margin-left: 9px;
    margin-top: 10px;
        }

        .chat_content_group.self>.chat_nick {
			float: right;
        text-align: right;
        }
        .chat_nick {
        font-size: 14px;
        margin: 0px;
        color:#8b8b8b;
			float: left;
        }

        .chat_content_group.self>.chat_content_avatar {
        float: right;
        margin: 0 0 0 10px;
        }

        .chat_content_group.buddy {
        text-align: left;
        }
        .chat_content_group {
        padding: 10px;
        }
        .chat_content_avatar {
        float: left;
        width: 40px;
        height: 40px;
        margin-right: 10px;
        }
        .imgtest{margin:10px 5px;
        overflow:hidden;}
        .list_ul figcaption p{
        font-size:11px;
        color:#aaa;
        }
        .imgtest figure div{
        display:inline-block;
        margin:5px auto;
        width:100px;
        height:100px;
        border-radius:100px;
        border:2px solid #fff;
        overflow:hidden;
        -webkit-box-shadow:0 0 3px #ccc;
        box-shadow:0 0 3px #ccc;
        }
        .imgtest img{width:100%;
        min-height:100%; text-align:center;}
			.timename{
                display: none;
				text-align: center;
                font-size: 12px;
                background: rgba(190, 190, 190, 0.5);
                width: 15%;
                margin: 0 auto;
                padding: 5px 2px;
                color: #fff;
                border-radius: 5px;
			}
	    </style>
        <script src=""https://cdn.bootcss.com/moment.js/2.18.1/moment.min.js""></script>
        <script src =""https://cdn.bootcss.com/moment.js/2.18.1/locale/zh-cn.js"" ></script >
        <script >
            window.onload = function () {
                var times = document.getElementsByClassName('timename');
                var now = new Date().getTime();
                for ( var i = 0; i < times.length; i ++ ) {
                    // 五分钟
                    var minute = 300000;
                    var t = times[i]
                    var time = new Date(t.innerHTML).getTime();
                    if (now - time >= minute)
                    {
                        t.style.display = 'block';
                    }
                    if (now - time >= 172800000)
                    {
                        t.innerHTML = moment(t.innerHTML).format('lll');
                    }
                    else
                    {
                        t.innerHTML = moment(t.innerHTML).calendar();
                    }
                }
            };
        </script>
        </head><body>";
        #endregion


        private bool IsUpload { get; set; }

        /// <summary>
        /// 保存到文件
        /// </summary>
        public void writeCacheData()
        {
            string filePath = System.IO.Path.Combine(Application.StartupPath, "data/cacheData");
            if (!Directory.Exists(filePath))
                Directory.CreateDirectory(filePath);
            filePath += "/" + EncryptHelper.MD5(toShowName + toNickName) + ".cache";
            if (!File.Exists(filePath))
                File.Create(filePath).Dispose();
            StreamWriter sw = new StreamWriter(@filePath, false);
            sw.Write(_totalHtml);
            sw.Close();//写入
        }


        /// <summary>
        /// 保存聊天记录
        /// </summary>
        /// <param name="_toShowName">Name of the _to show.</param>
        /// <param name="_toNickName">Name of the _to nick.</param>
        /// <param name="content">The content.</param>
        public void writeCacheData(string _toShowName, string _toNickName, string content)
        {
            string filePath = System.IO.Path.Combine(Application.StartupPath, "data/cacheData");
            if (!Directory.Exists(filePath))
                Directory.CreateDirectory(filePath);
            filePath += "/" + EncryptHelper.MD5(_toShowName + _toNickName) + ".cache";
            if (!File.Exists(filePath))
                File.Create(filePath).Dispose();
            StreamWriter sw = new StreamWriter(@filePath, false);
            sw.Write(content);
            sw.Close();//写入
        }


        /// <summary>
        /// 读取本地缓存数据
        /// </summary>
        /// <returns></returns>
        public string loadCacheData()
        {
            try
            {
                string filePath = System.IO.Path.Combine(Application.StartupPath, "data/cacheData/" + EncryptHelper.MD5(toShowName + toNickName) + ".cache");
                if (File.Exists(filePath))
                {
                    FileStream aFile = new FileStream(filePath, FileMode.Open);
                    StreamReader sr = new StreamReader(aFile);
                    string str = sr.ReadToEnd();
                    sr.Close();
                    sr.Dispose();
                    aFile.Close();
                    aFile.Dispose();
                    return str;
                }
                return string.Empty;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// 读取本地缓存数据
        /// </summary>
        /// <returns></returns>
        public string loadCacheData(string _toShowName, string _toNickName)
        {
            try
            {
                string filePath = System.IO.Path.Combine(Application.StartupPath, "data/cacheData/" + EncryptHelper.MD5(_toShowName + _toNickName) + ".cache");
                if (File.Exists(filePath))
                {
                    FileStream aFile = new FileStream(filePath, FileMode.Open);
                    StreamReader sr = new StreamReader(aFile);
                    string str = sr.ReadToEnd();
                    sr.Close();
                    sr.Dispose();
                    aFile.Close();
                    aFile.Dispose();
                    return str;
                }
                return string.Empty;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        private bool isKeyControl { get; set; }
        /// <summary>
        /// 发送图片文件列表
        /// </summary>
        /// <value>The send file list.</value>
        private static Dictionary<string, string> SendFileList { get; set; }


        private void EditForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.ControlKey || e.KeyCode == Keys.RControlKey || e.KeyCode == Keys.LControlKey)
            {
                e.Handled = true;
                isKeyControl = true;
            }
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                if (isKeyControl)
                    txtContent.AppendText("\r\n");
                else
                    btnSend_Click(null, null);
            }
            else if (e.KeyCode == Keys.V)
            {
                e.Handled = true;
                if (isKeyControl)
                {
                    if (SendFileList == null) SendFileList = new Dictionary<string, string>();
                    StringCollection colles = Clipboard.GetFileDropList();
                    if (colles != null && colles.Count > 0)
                    {
                        string path = colles[0];
                        if (!string.IsNullOrEmpty(path))
                        {
                            string ext = path.Substring(path.LastIndexOf("."));
                            if (!FilterFileType(ext)) return;
                            using (Stream stream = new FileStream(path, FileMode.Open))
                            {
                                //, 150, 120
                                Bitmap bmp = new Bitmap(Image.FromStream(stream), 150, 120);
                                Clipboard.SetDataObject(bmp, true);//将图片放在剪贴板中
                                if (txtContent.CanPaste(DataFormats.GetFormat(DataFormats.Bitmap)))
                                {
                                    txtContent.Paste();
                                    string _RtfText = txtContent.Rtf;
                                    while (true)
                                    {
                                        int _Index = _RtfText.IndexOf("pichgoal");
                                        if (_Index == -1) break;
                                        _RtfText = _RtfText.Remove(0, _Index + 8);

                                        _Index = _RtfText.IndexOf("\r\n");

                                        _RtfText = _RtfText.Remove(0, _Index);
                                        string __RtfText = _RtfText.Replace("\r\n", "");

                                        _Index = __RtfText.IndexOf("}");
                                        string filenamekey = EncryptHelper.MD5(__RtfText.Substring(0, _Index));
                                        if (!SendFileList.ContainsKey(filenamekey))
                                            SendFileList.Add(filenamekey, path);
                                    }
                                }
                            }
                        }
                    }
                    else
                        txtContent.Paste();
                }
            }
        }


        private bool FilterFileType(string ext)
        {
            const string exts = ".jpg|.png|.bmp|.ico";
            return exts.Contains(ext);
        }


        private void EditForm_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.ControlKey || e.KeyCode == Keys.RControlKey || e.KeyCode == Keys.LControlKey)
            {
                e.Handled = true;
                isKeyControl = false;
            }
        }

        private void txtContent_KeyDown(object sender, KeyEventArgs e)
        {
        }


        protected override CreateParams CreateParams
        {
            get
            {
                // const int WS_EX_APPWINDOW = 0x40000;
                const int WS_EX_TOOLWINDOW = 0x80;
                CreateParams cp = base.CreateParams;
                //cp.ExStyle &= (~WS_EX_APPWINDOW);    // 不显示在TaskBar
                cp.ExStyle |= WS_EX_TOOLWINDOW;      // 不显示在Alt-Tab
                return cp;
            }
        }

    }
}
