using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MahApps.Metro.Controls;
using SimpleBrowser;
using CsQuery;
using System.Text.RegularExpressions;
using System.Windows.Threading;
using System.Threading;
using mshtml;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Net;
using BotSuite;
using BotSuite.Imaging;
using BotSuite.Input;
using System.Runtime.InteropServices;
using WPFNotification;
namespace wpf_CilginTavuk
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public struct Tavuk
        {
            public string isim { get; set; }
            public int saatlikYumurta { get; set; }
            public int fiyat { get; set; }
            public int adet { get; set; }
        }



        //Global Değişkenler
        Browser browser = new Browser();
        Tavuk[] _tavuk = new Tavuk[6];

        string _Username = "";
        string _Password = "";
        string SolvedCaptcha = "xxxx";

        DispatcherTimer ChromeDispatcher = new DispatcherTimer();
        DispatcherTimer Stopwatch = new DispatcherTimer();


        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", EntryPoint = "FindWindow", SetLastError = true)]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);


        public MainWindow()
        {
            InitializeComponent();
            comboBoxinterval.SelectedIndex = 2;
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);

            ChromeDispatcher.Interval = new TimeSpan(0, 0, 5);
            ChromeDispatcher.Tick += new EventHandler(ChromeDispatcher_Tick);

            Stopwatch.Interval = new TimeSpan(0, 0, 1);
            Stopwatch.Tick += new EventHandler(Stopwatch_Tick);
            Stopwatch.Start();

            browser.Timeout = 5000;
            log("Program Başladı");

        }

        private void log(string message)
        {
            tbxLOG.AppendText("[ " + DateTime.Now.ToString("HH:mm:ss") + " ]  " + message + "\n");
            tbxLOG.ScrollToEnd();

        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            _Username = tbxUsername.Text;
            _Password = tbxPassword.Text;

            Login();
            KümesBilgileriGüncelle();
            BilgileriGüncelle();
        }

        private void Login()
        {
            try
            {
                browser.UserAgent = "Mozilla/5.0 (Windows; U; Windows NT 6.1; en-US) AppleWebKit/534.10 (KHTML, like Gecko) Chrome/8.0.552.224 Safari/534.10";
                browser.Navigate("https://cilgintavuklar.com/giris.php");
                if (LastRequestFailed(browser)) return;

                browser.Find(ElementType.TextField, "name", "kullanici_adi").Value = _Username;
                browser.Find(ElementType.TextField, "name", "sifre").Value = _Password;
                browser.Find(ElementType.Button, "name", "submit").Click();
                if (LastRequestFailed(browser)) return;

                if (browser.ContainsText("Akçe Bakiyeniz"))
                {
                    log("Giriş Yapıldı");
                }
                else
                {
                    log("Yeniden Deneyin...");
                }

            }
            catch (Exception ex)
            {

                log(ex.Message);
            }
        }

        int login_try_time = 0;
        private bool LastRequestFailed(Browser browser_)
        {
            if (browser_.LastWebException != null)
            {
                var error = browser_.LastWebException.Message;
                log(error);
                browser.Close();
                browser = new SimpleBrowser.Browser();
                if (login_try_time < 4)
                {
                    login_try_time++;
                    Login();
                }
                if (login_try_time >= 4)
                {
                    login_try_time = 0;
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        public void KümesBilgileriGüncelle()
        {
            if (browser.Url != new Uri("http://cilgintavuklar.com/panel.php"))
            {
                browser.Navigate("http://cilgintavuklar.com/panel.php");
                if (LastRequestFailed(browser)) return;
                if (!browser.ContainsText("Kümes Bilgileriniz"))
                {
                    return;
                }

            }
            var dom = CQ.Create(browser.CurrentHtml);

            _tavuk[0].isim = "Pamuk";
            _tavuk[0].saatlikYumurta = 90;
            _tavuk[0].fiyat = 600;
            _tavuk[0].adet = Convert.ToInt32(dom.Select("body > div > div > section.content > center > div.col-lg-5.col-md-12.col-sm-12.col-xs-12 > div > div.box-body > div:nth-child(1) > div > div > h3:nth-child(5)").Text().Replace(" Adet", null));


            _tavuk[1].isim = "Kızarık";
            _tavuk[1].saatlikYumurta = 300;
            _tavuk[1].fiyat = 1600;
            _tavuk[1].adet = Convert.ToInt32(dom.Select("body > div > div > section.content > center > div.col-lg-5.col-md-12.col-sm-12.col-xs-12 > div > div.box-body > div:nth-child(2) > div > div > h3:nth-child(5)").Text().Replace(" Adet", null));

            _tavuk[2].isim = "Benekli";
            _tavuk[2].saatlikYumurta = 1100;
            _tavuk[2].fiyat = 5500;
            _tavuk[2].adet = Convert.ToInt32(dom.Select("body > div > div > section.content > center > div.col-lg-5.col-md-12.col-sm-12.col-xs-12 > div > div.box-body > div:nth-child(3) > div > div > h3:nth-child(5)").Text().Replace(" Adet", null));

            _tavuk[3].isim = "Dombom";
            _tavuk[3].saatlikYumurta = 2600;
            _tavuk[3].fiyat = 14000;
            _tavuk[3].adet = Convert.ToInt32(dom.Select("body > div > div > section.content > center > div.col-lg-5.col-md-12.col-sm-12.col-xs-12 > div > div.box-body > div:nth-child(4) > div > div > h3:nth-child(5)").Text().Replace(" Adet", null));

            _tavuk[4].isim = "Çilli";
            _tavuk[4].saatlikYumurta = 4500;
            _tavuk[4].fiyat = 25000;
            _tavuk[4].adet = Convert.ToInt32(dom.Select("body > div > div > section.content > center > div.col-lg-5.col-md-12.col-sm-12.col-xs-12 > div > div.box-body > div:nth-child(5) > div > div > h3:nth-child(5)").Text().Replace(" Adet", null));

            _tavuk[5].isim = "Pofuduk";
            _tavuk[5].saatlikYumurta = 12000;
            _tavuk[5].fiyat = 100;
            _tavuk[5].adet = Convert.ToInt32(dom.Select("body > div > div > section.content > center > div.col-lg-5.col-md-12.col-sm-12.col-xs-12 > div > div.box-body > div:nth-child(6) > div > div > h3:nth-child(5)").Text().Replace(" Adet", null));


            lblTavuk1_isim.Text = _tavuk[0].isim;
            lblTavuk1_SaatlikYumurta.Text = _tavuk[0].saatlikYumurta.ToString();
            lblTavuk1_Fiyat.Text = _tavuk[0].fiyat.ToString();
            lblTavuk1_Adet.Text = _tavuk[0].adet.ToString();

            lblTavuk2_isim.Text = _tavuk[1].isim;
            lblTavuk2_SaatlikYumurta.Text = _tavuk[1].saatlikYumurta.ToString();
            lblTavuk2_Fiyat.Text = _tavuk[1].fiyat.ToString();
            lblTavuk2_Adet.Text = _tavuk[1].adet.ToString();

            lblTavuk3_isim.Text = _tavuk[2].isim;
            lblTavuk3_SaatlikYumurta.Text = _tavuk[2].saatlikYumurta.ToString();
            lblTavuk3_Fiyat.Text = _tavuk[2].fiyat.ToString();
            lblTavuk3_Adet.Text = _tavuk[2].adet.ToString();

            lblTavuk4_isim.Text = _tavuk[3].isim;
            lblTavuk4_SaatlikYumurta.Text = _tavuk[3].saatlikYumurta.ToString();
            lblTavuk4_Fiyat.Text = _tavuk[3].fiyat.ToString();
            lblTavuk4_Adet.Text = _tavuk[3].adet.ToString();

            lblTavuk5_isim.Text = _tavuk[4].isim;
            lblTavuk5_SaatlikYumurta.Text = _tavuk[4].saatlikYumurta.ToString();
            lblTavuk5_Fiyat.Text = _tavuk[4].fiyat.ToString();
            lblTavuk5_Adet.Text = _tavuk[4].adet.ToString();

            lblTavuk6_isim.Text = _tavuk[5].isim;
            lblTavuk6_SaatlikYumurta.Text = _tavuk[5].saatlikYumurta.ToString();
            lblTavuk6_Fiyat.Text = _tavuk[5].fiyat.ToString();
            lblTavuk6_Adet.Text = _tavuk[5].adet.ToString();

            log("Kümes Bilgileri Güncellendi");
        }

        public void BilgileriGüncelle()
        {


            browser.Navigate("http://www.cilgintavuklar.com/pazar.php");
            if (LastRequestFailed(browser)) return;
            if (!browser.ContainsText("Akçe Bakiyeniz"))
            {
                return;
            }
            

            var dom = CQ.Create(browser.CurrentHtml);

            lblBakiye.Text = dom.Select("body > div > div > section.content > center > div:nth-child(8) > div > div.inner > font").Text();
            lblYumurta.Text = dom.Select("body > div > div > section.content > center > div:nth-child(9) > div > div > font").Text();
            lblYemMasraf.Text = dom.Select("body > div > div > section.content > center > div:nth-child(10) > div > div > font").Text();
            lblTLBakiye.Text = dom.Select("body > div > div > section.content > center > div:nth-child(11) > div > div.inner > font").Text();


            //Pop Up when full
            if (Convert.ToInt32(lblYumurta.Text)>=1950)
            {
                this.Topmost = true;
                
            }
            else
            {
                this.Topmost = false;
            }
        }

        public void YumurtalarıSat()
        {
            if (browser.Url != new Uri("http://www.cilgintavuklar.com/pazar.php"))
            {
                browser.Navigate("http://www.cilgintavuklar.com/pazar.php");
                if (LastRequestFailed(browser)) return;
                if (!browser.ContainsText("Yumurta Tezgahı"))
                {
                    return;
                }
            }
            browser.Find(ElementType.Button, "value", "Yumurtaları Sat").Click();
            if (LastRequestFailed(browser)) return;
            log("Yumurtalar Satıldı");


        }

        public async void Bonus()
        {

            if (browser.Url != new Uri("http://cilgintavuklar.com/bonus.php"))
            {
                browser.Navigate("http://cilgintavuklar.com/bonus.php");
                if (LastRequestFailed(browser)) return;
                if (!browser.ContainsText("Doğrulama İşlemi"))
                {
                    return;
                }
            }
            var dom = CQ.Create(browser.CurrentHtml);
            await Task.Delay(5000);
            dom = CQ.Create(browser.CurrentHtml);

            var xx = dom.Select("#resim > img");

        }

        private void btnYumurtaSat_Click(object sender, RoutedEventArgs e)
        {
            
            BilgileriGüncelle();
        }

        private async void btnKazan_Click(object sender, RoutedEventArgs e)
        {

        }

        DispatcherTimer dispatcherTimer = new DispatcherTimer();


        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            //YumurtalarıSat();
            BilgileriGüncelle();
        }

        private void toggleAutoMod_IsCheckedChanged(object sender, EventArgs e)
        {
            if (toggleAutoMod.IsChecked == true)
            {

                dispatcherTimer.Start();
                log("Auto Mod Açıldı");
            }
            else
            {
                dispatcherTimer.Stop();
                log("Auto Mod Kapandı");

            }
        }

        private void comboBoxinterval_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (comboBoxinterval.SelectedIndex)
            {
                case 0: dispatcherTimer.Interval = new TimeSpan(0, 1, 0); break;
                case 1: dispatcherTimer.Interval = new TimeSpan(0, 2, 0); break;
                case 2: dispatcherTimer.Interval = new TimeSpan(0, 5, 0); break;
                case 3: dispatcherTimer.Interval = new TimeSpan(0, 10, 0); break;
                case 4: dispatcherTimer.Interval = new TimeSpan(0, 30, 0); break;
            }
        }


        int CaptchaCounter = 1;
        private void SaveCaptchaImage()
        {

            var doc = (IHTMLDocument2)webbrowser1.Document as IHTMLDocument2;
            var imgRange = (IHTMLControlRange)((HTMLBody)doc.body).createControlRange();
            var check = false;
            var i = 1;
            foreach (IHTMLImgElement img in doc.images)
            {
                if (img.width == 100 && img.height == 30)
                {
                    imgRange.add((IHTMLControlElement)img);

                    imgRange.execCommand("Copy", false, null);

                    using (Bitmap bmp = (Bitmap)System.Windows.Forms.Clipboard.GetDataObject().GetData(System.Windows.Forms.DataFormats.Bitmap))
                    {
                        try
                        {
                            //File.Delete(System.AppDomain.CurrentDomain.BaseDirectory + "/Captcha.png");
                            try { bmp.Save(System.AppDomain.CurrentDomain.BaseDirectory + "/Captcha" + CaptchaCounter + ".png"); } catch (Exception x) { var a = x.Message; }
                            try { bmp.Save(System.AppDomain.CurrentDomain.BaseDirectory + "/CaptchaForSolve.png"); } catch (Exception x) { var a = x.Message; }

                            i++;

                            log("Captcha İndirildi");
                            UpdateCaptchaImage();
                            check = true;
                            bmp.Dispose();
                        }
                        catch (Exception x)
                        {
                            var a = x.Message; //Öylesine
                        }


                    }
                }


            }
            if (check == false)
            {
                log("Captcha Bulunamadı");
            }

        }


        private async void button_Click(object sender, RoutedEventArgs e)
        {
          
            //BilgileriGüncelle();
        }


        private void UpdateCaptchaImage()
        {
            imgKod.Source = new BitmapImage(new Uri(System.AppDomain.CurrentDomain.BaseDirectory + "/Captcha" + CaptchaCounter + ".png"));

        }

        private async Task WBLogin()
        {
            if (webbrowser1.Source != new Uri("http://cilgintavuklar.com/giris.php"))
            {
                webbrowser1.Source = new Uri("http://cilgintavuklar.com/giris.php");
                await Task.Delay(2000);
            }




            var document = webbrowser1.Document as mshtml.HTMLDocument;
            var id = document.getElementsByName("kullanici_adi");
            var pw = document.getElementsByName("sifre");
            var loginbtn = document.getElementsByName("submit");


            //Kullanıcı Adı
            foreach (mshtml.IHTMLElement element in id)
            {
                try
                {
                    if (element.getAttribute("name").Equals("kullanici_adi"))
                    {
                        element.setAttribute("value", "eroglu94");

                    }
                }
                catch (Exception ex)
                {

                    var x = ex.Message;
                }
            }

            //Şifre
            foreach (mshtml.IHTMLElement element in pw)
            {
                try
                {
                    if (element.getAttribute("name").Equals("sifre"))
                    {
                        element.setAttribute("value", "8874788474a");

                    }
                }
                catch (Exception ex)
                {

                    var x = ex.Message;
                }
            }

            //Login Button
            foreach (mshtml.IHTMLElement element in loginbtn)
            {
                try
                {
                    if (element.getAttribute("name").Equals("submit"))
                    {
                        element.click();

                    }
                }
                catch (Exception ex)
                {

                    var x = ex.Message;
                }
            }

            while (FindWindow("#32770", "Message from webpage").ToString() == "0")
            {
                await Task.Delay(100);
            }

            try
            {
                var hwnd = FindWindow("#32770", "Message from webpage");
                hwnd = FindWindowEx(hwnd, IntPtr.Zero, "Button", "OK");
                uint message = 0xf5;
                SendMessage(hwnd, message, IntPtr.Zero, IntPtr.Zero);
            }
            catch (Exception ex)
            {

                log(ex.Message);
            }
        }

        private async Task WBBonusPage()
        {
            await Task.Delay(200);
            if (webbrowser1.Source == new Uri("http://cilgintavuklar.com/giris.php"))
            {
                await WBLogin();
                log("Bonus Sayfasına Yönlendiriliyor");
                webbrowser1.Navigate("http://cilgintavuklar.com/bonus.php");
                await Task.Delay(2000);
            }
            else if (webbrowser1.Source != new Uri("http://cilgintavuklar.com/bonus.php"))
            {
                log("Bonus Sayfasına Yönlendiriliyor");
                webbrowser1.Navigate("http://cilgintavuklar.com/bonus.php");

                await Task.Delay(2000);
            }
            else if (webbrowser1.Source == new Uri("http://cilgintavuklar.com/bonus.php"))
            {
                webbrowser1.Navigate("http://cilgintavuklar.com/bonus.php");
                log("Sayfa Yenileniyor");
                await Task.Delay(2000);
            }


            var check = false;


            while (check == false)
            {
                var document = webbrowser1.Document as mshtml.HTMLDocument;
                var element = document.getElementById("reklam");

                try
                {
                    if (element.style.display == "none")
                    {
                        check = true;

                    }
                }
                catch (Exception ex)
                {

                    var x = ex.Message;
                }

                await Task.Delay(20);
            }

            log("Bulundu");
            SaveCaptchaImage();

        }

        private void WBBonusSubmit()
        {
            var document = webbrowser1.Document as mshtml.HTMLDocument;
            var id = document.getElementsByName("kod");
            var buton = document.getElementsByTagName(nameof(button));

            //Kullanıcı Adı
            foreach (mshtml.IHTMLElement element in id)
            {
                try
                {
                    if (element.getAttribute("value").Equals("Örneğin : 5"))
                    {
                        element.setAttribute("value", SolvedCaptcha);

                    }
                }
                catch (Exception ex)
                {

                    var x = ex.Message;
                }
            }

            foreach (mshtml.IHTMLElement element in buton)
            {
                try
                {
                    if (element.getAttribute("innerText").Equals("Bonus Yumurta Kazan"))
                    {
                        element.click();

                    }
                }
                catch (Exception ex)
                {

                    var x = ex.Message;
                }
            }


        }

        private async Task SendCaptchaToCrack(string Path)
        {
            var bmpScreenshot = ScreenShot.Create();
            var Screenshot = new ImageData(bmpScreenshot);
            var ss_Browse = new ImageData(Properties.Resources.ss_browse);

            var rectangle = BotSuite.Imaging.Template.Image(Screenshot, ss_Browse, 0);
            if (!rectangle.IsEmpty)
            {
                BotSuite.Input.Mouse.Move(rectangle.Location, false, 0);
                BotSuite.Input.Mouse.LeftClick();
                await Task.Delay(300);
                SendKeys.SendWait(Path);
                SendKeys.SendWait("{ENTER}");

                var ss_Submit = new ImageData(Properties.Resources.ss_submit);
                rectangle = BotSuite.Imaging.Template.Image(Screenshot, ss_Submit, 0);
                if (!rectangle.IsEmpty)
                {
                    BotSuite.Input.Mouse.Move(rectangle.Location, false, 0);
                    BotSuite.Input.Mouse.LeftClick();
                    await Task.Delay(100);
                    GetSolvedCaptcha();
                    
                    log("Captcha Çözüldü (" + SolvedCaptcha + ")");
                }

            }

        }

        private void GetSolvedCaptcha()
        {
            var document = wb2.Document as mshtml.HTMLDocument;
            var CaptchaSpan = document.getElementById("captcha_result");
            SolvedCaptcha = CaptchaSpan.getAttribute("outerText").ToString();

        }

        // WebBrowser Script Error Disable START
        private const string DisableScriptError = @"function noError() {return true; } window.onerror = noError;";

        private void webbrowser1_Navigated(object sender, NavigationEventArgs e)
        {
            InjectDisableScript();
        }

        private void InjectDisableScript()
        {

            try
            {
                var doc = webbrowser1.Document as HTMLDocumentClass;
                var doc2 = webbrowser1.Document as HTMLDocument;


                //Questo crea lo script per la soprressione degli errori


                var scriptErrorSuppressed = (IHTMLScriptElement)doc2.createElement("SCRIPT");

                scriptErrorSuppressed.type = "text/javascript";
                scriptErrorSuppressed.text = DisableScriptError;

                var nodes = doc.getElementsByTagName("head");

                foreach (IHTMLElement elem in nodes)
                {
                    //Appendo lo script all'head cosi Ã¨ attivo

                    var head = (HTMLHeadElementClass)elem;
                    head.appendChild((IHTMLDOMNode)scriptErrorSuppressed);
                }
            }
            catch (Exception)
            {


            }
        }
        // WebBrowser Script Error Disable FINISH

        private void btnWBGiris_Click(object sender, RoutedEventArgs e)
        {
            WBLogin();
        }

        private void wb2_Navigated(object sender, NavigationEventArgs e)
        {
            InjectDisableScript();
        }

        private void ChromeCaptchaCutter()
        {
            var rec = new System.Drawing.Rectangle
            {
                X = 532,
                Y = 501,
                Width = 100,
                Height = 30
            };

            var bmpScreenshot = ScreenShot.Create(rec);
            var Screenshot = new ImageData(bmpScreenshot).CreateBitmap();


            Screenshot.Save(System.AppDomain.CurrentDomain.BaseDirectory + "/Captcha" + CaptchaCounter + ".png");
        }

        private async Task ChromeBonusSumbit()
        {
            BotSuite.Input.Mouse.Move(new System.Drawing.Point(562, 549), false, 0);
            BotSuite.Input.Mouse.LeftClick();
            await Task.Delay(200);
            SendKeys.SendWait(SolvedCaptcha);
            await Task.Delay(70);
            BotSuite.Input.Mouse.Move(new System.Drawing.Point(582, 593), false, 0);
            await Task.Delay(70);
            BotSuite.Input.Mouse.LeftClick();
            //await Task.Delay(500);


            while (true)
            {
                var bmpScreenshot = ScreenShot.Create();
                var Screenshot = new ImageData(bmpScreenshot);
                var ss_Browse = new ImageData(Properties.Resources.ss_bonusageridon);

                var rectangle = BotSuite.Imaging.Template.Image(Screenshot, ss_Browse, 0);
                if (!rectangle.IsEmpty)
                {
                    BotSuite.Input.Mouse.Move(rectangle.Location, false, 0);
                    await Task.Delay(70);
                    BotSuite.Input.Mouse.LeftClick();
                    await Task.Delay(70);
                    BotSuite.Input.Mouse.LeftClick();
                    break;
                }

                await Task.Delay(100);
            }



        }

        int randomSecond = 5;
        private async void ChromeDispatcher_Tick(object sender, EventArgs e)
        {
            try
            {
                var bmpScreenshot = ScreenShot.Create();
                var Screenshot = new ImageData(bmpScreenshot);
                var ss_bonussumbit = new ImageData(Properties.Resources.ss_bonussumbit);

                var rectangle = BotSuite.Imaging.Template.Image(Screenshot, ss_bonussumbit, 0);
                if (!rectangle.IsEmpty)
                {

                }
                else
                {
                    return;
                }




                ChromeDispatcher.Stop();



                ChromeCaptchaCutter();
                await Task.Delay(50);
                UpdateCaptchaImage();
                await SendCaptchaToCrack(System.AppDomain.CurrentDomain.BaseDirectory + "Captcha" + CaptchaCounter + ".png");

                var errorChance = BotSuite.Utility.RandomInt(1, 50);
                if (errorChance == 1)
                {
                    var randomnumber = BotSuite.Utility.RandomInt(1, 16);
                    var randomnumber2 = BotSuite.Utility.RandomInt(1, 6);
                    const string chars = "0123456789abcdef";
                    SolvedCaptcha = SolvedCaptcha.Replace(SolvedCaptcha[randomnumber2], chars[randomnumber]);
                }

                await ChromeBonusSumbit();
                randomSecond = BotSuite.Utility.RandomInt(8, 9);

                _swTrack = randomSecond;
                //ChromeDispatcher.Interval = new TimeSpan(0, 0, randomSecond);
                ChromeDispatcher.Interval = new TimeSpan(0, 0, 0, 0, 100);
                BilgileriGüncelle();
                CaptchaCounter++;
                this.Topmost = true;
                this.Topmost = false;
                wb2.GoBack();
                ChromeDispatcher.Start();
            }
            catch (Exception)
            {
                ChromeDispatcher.Stop();
                ChromeDispatcher.Start();
                CaptchaCounter++;
            }
        }

        int _swTrack = 5;
        private void Stopwatch_Tick(object sender, EventArgs e)
        {
            if (ChromeDispatcher.IsEnabled == true)
            {
                _swTrack = _swTrack - 1;
                lblBonusDispatcher.Content = _swTrack;
            }

        }

        private void toggleBonus_IsCheckedChanged(object sender, EventArgs e)
        {
            if (toggleBonus.IsChecked == true)
            {
                ChromeDispatcher.Interval = new TimeSpan(0, 0, randomSecond);
                ChromeDispatcher.Start();
                log("Bonus Modu Açık");
            }
            else
            {
                ChromeDispatcher.Stop();
                log("Bonus Modu Kapandı");
            }
        }

        private static void Notification()
        {
            WPFNotification.Services.INotificationDialogService NDS = new WPFNotification.Services.NotificationDialogService();
            var Notification = new WPFNotification.Model.Notification
            {
                Title = "Deneme Title",
                Message = "Bu bir deneme MEsajıdır"
            };

            NDS.ShowNotificationWindow(Notification);
        }
    }
}
