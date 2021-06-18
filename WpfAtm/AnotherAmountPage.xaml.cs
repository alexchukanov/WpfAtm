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
using KXCashDispenserLib;
using KXReceiptPrinterLib;

using WpfAtm.Model;

using System.IO;

namespace WpfAtm
{
    /// <summary>
    /// Interaction logic for TransactionPage.xaml
    /// </summary>
    public partial class AnotherAmountPage : Page
    {
        KXBape pp;
        eCurrency cur = eCurrency.NONE;

        NavigationService nav;

        int timeout = App.Timeout;
        public AnotherAmountPage()
        {
            InitializeComponent();
        }

        public AnotherAmountPage(eCurrency cur):this()
        {
            
            this.cur = cur;

            this.pp = new KXBape();
            pp.KeyPressed += Pp_KeyPressedAnotherAmount;
            pp.EntryComplete += Pp_EntryCompleteAnotherAmount;
            pp.Timeout += Cr_TimeoutAnotherAmount;

            Loaded += AnotherAmountPage_Loaded;
        }

           
        private void Cr_TimeoutAnotherAmount()
        {
            pp.CancelUserEntry();
            nav = NavigationService.GetNavigationService(this);
            nav.Navigate(new StartPage());
        }

              
        private void AnotherAmountPage_Loaded(object sender, RoutedEventArgs e)
        {           
            UserEntry();
        }


        private void Pp_EntryCompleteAnotherAmount(string pinCode)
        {           
            nav = NavigationService.GetNavigationService(this);
            nav.Navigate(new StartPage());
        }

        private void Pp_KeyPressedAnotherAmount(string key, int keyCode)
        {           
            switch (key)
                {               
                    case ("F6"):  //other 
                    case ("F8"):  //amount 
                    pp.CancelUserEntry();
                    nav = NavigationService.GetNavigationService(this);
                        nav.Navigate(new CurrencyTransactionPage());
                        break;
                    default:
                        break;
                }
        }

       

        private void UserEntry()
        {           
            bool isAutoEnd = false;
            short maxKeys = 1;
            string activeKeys = "F6, F7, F8, CANCEL";
            string terminatorKeys = "CANCEL, F7";

            int res = pp.UserEntry(maxKeys, isAutoEnd, activeKeys, terminatorKeys, timeout);
        }
    }
}
