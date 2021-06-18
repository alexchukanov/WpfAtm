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
using KXReceiptPrinterLib;

using WpfAtm.Model;

namespace WpfAtm
{
    /// <summary>
    /// Interaction logic for TransactionPage.xaml
    /// </summary>
    public partial class ReceiptTransactionPage : Page
    {
        KXBape pp;
        NavigationService nav;
        KXReceiptPrinter rp;

        eCurrency cur = eCurrency.NONE;
        ePaperStatus paperStatus = ePaperStatus.UNKNOWN;
             
        int amount = 0;
        int timeout = App.Timeout;

        public ReceiptTransactionPage(eCurrency cur, int amount)
        {
            InitializeComponent();

            this.cur = cur;
            this.amount = amount;

            this.pp = new KXBape();

            pp.KeyPressed += Pp_KeyPressedReceipt;
            pp.EntryComplete += Pp_EntryCompleteReceipt;
            pp.Timeout += Cr_TimeoutReceipt;

            rp = new KXReceiptPrinter();
            paperStatus = PaperStatusRP;

            Loaded += ReceiptTransactionPage_Loaded;
        }

        private void Cr_TimeoutReceipt()
        {
            pp.CancelUserEntry();
            nav = NavigationService.GetNavigationService(this);
            nav.Navigate(new StartPage());
        }

        private void ReceiptTransactionPage_Loaded(object sender, RoutedEventArgs e)
        {
            UserEntry();
        }

        private void Pp_EntryCompleteReceipt(string pinCode)
        {
            nav = NavigationService.GetNavigationService(this);
            nav.Navigate(new StartPage());            
        }

        private void Pp_KeyPressedReceipt(string key, int keyCode)
        {
            bool isReceipt = false;
            pp.CancelUserEntry();

            switch (key)
            {               
                 case ("F4"): //Yes 
                    if (paperStatus == ePaperStatus.FULL || paperStatus == ePaperStatus.LOW)
                    {
                        isReceipt = true;
                        nav = NavigationService.GetNavigationService(this);
                        nav.Navigate(new DispenseTransactionPage(cur, amount, isReceipt));
                    }
                    else
                    {
                        lbErrorReceipt.Content = "Sorry, no paper!";
                        UserEntry();
                    }
                    break;
                case ("F8"): //No                     
                        nav = NavigationService.GetNavigationService(this);
                        nav.Navigate(new DispenseTransactionPage(cur, amount, isReceipt));                   
                    break;
                default:                   
                    break;
            }          
        }


        private void UserEntry()
        {
            bool isAutoEnd = false;
            short maxKeys = 1;
            string activeKeys = "F4, F8, F7, CANCEL";
            string terminatorKeys = "CANCEL, F7";

            int res = pp.UserEntry(maxKeys, isAutoEnd, activeKeys, terminatorKeys, timeout);
        }

        private ePaperStatus PaperStatusRP
        {
            get
            {
                return (ePaperStatus)Enum.Parse(typeof(ePaperStatus), rp.StPaperStatus);
            }
        }
    }
}
