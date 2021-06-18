using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using KXPinPadLib;
using KXCardReaderLib;

using WpfAtm.Model;

namespace WpfAtm
{
    /// <summary>
    /// Interaction logic for Page2.xaml
    /// </summary>
    public partial class EnterPinCode : Page
    {
        KXBape pp;

        NavigationService nav;
        KXCardReader cr;

        int timeout = App.Timeout;

        int entryCounter = 0;

        int pinCode = 0;

        const string PASSWORD = "1234";

        public EnterPinCode(KXCardReader cr)
        {
            InitializeComponent();

            this.cr = cr;
            cr.CardCaptured += Cr_CardCapturedEnterPin;
            pp = new KXBape();
            pp.Timeout += Cr_TimeoutEnterPin;
           
            pp.KeyPressed += Pp_KeyPressedPinCode;
            pp.EntryComplete += Pp_EntryCompletePinCode;

            pp.Cancelled += Pp_CancelledPinCode;

            Loaded += EnterPinCode_Loaded;
        }

        private void Pp_CancelledPinCode()
        {           
            UserEntry();
        }

        private void Cr_TimeoutEnterPin()
        {           
            if (MediaStatusCR == eStMediaStatus.PRESENT)
            {
                pp.CancelUserEntry();
                nav = NavigationService.GetNavigationService(this);
                nav.Navigate(new StartPage());
            }
        }

      

        private void Cr_CardCapturedEnterPin()
        {
            pp.CancelUserEntry();
            nav = NavigationService.GetNavigationService(this);
            nav.Navigate(new StartPage("Support:"));
        }

        private void EnterPinCode_Loaded(object sender, RoutedEventArgs e)
        {          
            UserEntry();
        }

        private void Pp_EntryCompletePinCode(string pinCode)
        {           
                if (pwPinCode.Password == PASSWORD)
                {
                    pp.CancelUserEntry();
                    nav = NavigationService.GetNavigationService(this);
                    nav.Navigate(new CurrencyTransactionPage());
                }
                else if (pwPinCode.Password == "")
                {
                    pp.CancelUserEntry();
                    nav = NavigationService.GetNavigationService(this);
                    nav.Navigate(new StartPage());
                }
                else
                {
                    pwPinCode.Password = "";

                    if (entryCounter++ == 2 && MediaStatusCR == eStMediaStatus.PRESENT) // three attempts
                    {
                        cr.Capture();
                    }
                    else
                    {
                        lbError.Content = "Wrong PIN vode, try again!";
                        UserEntry();
                    }
                } 
        }

        private void Pp_KeyPressedPinCode(string key, int keyCode)
        {        
            lbError.Content = "";

            switch (key)
            {
                case ("F4"):
                case ("CLEAR"):
                    pwPinCode.Password = "";
                    pp.CancelUserEntry();
                    break;
                case ("F7"):
                case ("CANCEL"):
                    pwPinCode.Password = "";
                      break;
                case ("F8"):
                case ("ENTER"):                  
                    break;
                default:
                    pwPinCode.Password += key;
                    if (int.TryParse(pwPinCode.Password, out pinCode))
                    {
                        if (pinCode >= 9999)
                        {
                            pwPinCode.Password = "";
                            pp.CancelUserEntry();
                        }
                    }
                    else
                    {
                        nav = NavigationService.GetNavigationService(this);
                        nav.Navigate(new StartPage());
                    }
                    break;
            }
        }

        
        private void UserEntry()
        {
            bool isAutoEnd = false;
            short maxKeys = 5; // 0-9999 thousands

            string activeKeys = "NUMBERS, F4, F7, F8, ENTER, CLEAR, CANCEL";
            string terminatorKeys = "CANCEL, ENTER, F7, F8";
                     
            int res = pp.UserEntry(maxKeys, isAutoEnd, activeKeys, terminatorKeys, timeout);
        }





        private eStMediaStatus MediaStatusCR
        {
            get
            {
                return (eStMediaStatus)Enum.Parse(typeof(eStMediaStatus), cr.StMediaStatus);
            }
        }
    }
}
