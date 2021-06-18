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

using WpfAtm.Model;
using KXPinPadLib;


namespace WpfAtm
{
    /// <summary>
    /// Interaction logic for AmountTransactionPage.xaml
    /// </summary>
    public partial class OtherAmountPage : Page
    {
        KXBape pp;
        NavigationService nav;

        eCurrency cur = eCurrency.NONE;
        int amount = 0;
        int timeout = App.Timeout;

        public OtherAmountPage(eCurrency cur)
        {
            InitializeComponent();

            this.cur = cur;
            curCode.Content = cur.ToString();

            this.pp = new KXBape();

            pp.KeyPressed += Pp_KeyPressedOtherAmount;
            pp.EntryComplete += Pp_EntryCompleteOtherAmount;
            pp.Timeout += Cr_TimeoutOtherAmount;

            pp.Cancelled += Pp_CancelledOtherAmount;

            Loaded += OtherAmountTransactionPage_Loaded;
        }

        private void Pp_CancelledOtherAmount()
        {
            UserEntry();
        }

        private void Cr_TimeoutOtherAmount()
        {
            pp.CancelUserEntry();
            nav = NavigationService.GetNavigationService(this);
            nav.Navigate(new StartPage());
        }
     
        void OtherAmountTransactionPage_Loaded(object sender, RoutedEventArgs e)
        {
            UserEntry();
        }

        private void Pp_EntryCompleteOtherAmount(string pinCode)
        {         
            if (pinCode != "")
            {
                pp.CancelUserEntry();
                nav = NavigationService.GetNavigationService(this);
                nav.Navigate(new ReceiptTransactionPage(cur, amount));
            }
        }

        private void Pp_KeyPressedOtherAmount(string key, int keyCode)
        {          
            switch (key)
            {
               case ("F2"): 
               case ("CLEAR"):
                    tbOtherAmount.Text = "";
                    pp.CancelUserEntry();
                    break;               
                case ("F7"): 
                case ("CANCEL"):
                    pp.CancelUserEntry();
                    nav = NavigationService.GetNavigationService(this);
                    nav.Navigate(new StartPage());
                    return;
                case ("F8"):  
                case ("ENTER"):
                    break;
                default:
                    tbOtherAmount.Text += key;
                    if (int.TryParse(tbOtherAmount.Text, out amount))
                    {
                        if (amount >= 9999)
                        {
                            tbOtherAmount.Text = "";
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

            string activeKeys = "NUMBERS, F2, F7, F8, ENTER, CLEAR, CANCEL";
           
            string terminatorKeys = "CANCEL, ENTER, F7, F8";

            int res = pp.UserEntry(maxKeys, isAutoEnd, activeKeys, terminatorKeys, timeout);
        }
    }
}
