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
    /// Interaction logic for TransactionPage.xaml
    /// </summary>
    public partial class CurrencyTransactionPage : Page
    {
        KXBape pp;       
        NavigationService nav;

        int timeout = App.Timeout;

        eCurrency cur = eCurrency.NONE;

        public CurrencyTransactionPage()
        {
            InitializeComponent();

            this.pp = new KXBape();           

            pp.KeyPressed += Pp_KeyPressedCurrency;
            pp.EntryComplete += Pp_EntryCompleteCurrency;
            pp.Timeout += Cr_TimeoutCurrency;
            pp.Cancelled += Pp_CancelledCurrency;

            Loaded += CurrencyTransactionPage_Loaded;
        }

        private void Pp_CancelledCurrency()
        {
            
        }

        private void Cr_TimeoutCurrency()
        {
            pp.CancelUserEntry();
            nav = NavigationService.GetNavigationService(this);
            nav.Navigate(new StartPage());
        }

        private void CurrencyTransactionPage_Loaded(object sender, RoutedEventArgs e)
        {
            UserEntry();
        }

        private void Pp_EntryCompleteCurrency(string pinCode)
        {
            nav = NavigationService.GetNavigationService(this);
            nav.Navigate(new StartPage());
        }

        private void Pp_KeyPressedCurrency(string key, int keyCode)
        {           
            pp.CancelUserEntry();

            switch (key)
                {               
                    case ("F1"): cur = eCurrency.EUR;
                        break;
                    case ("F2"): cur = eCurrency.RUB;
                        break;
                    case ("F3"): cur = eCurrency.USD;
                        break;
                    case ("F4"): cur = eCurrency.GBP;
                        break;
                    case ("F5"): cur = eCurrency.RON;
                        break;
                    case ("F7"): //CANCEL
                    case ("CANCEL"):
                        nav = NavigationService.GetNavigationService(this);
                        nav.Navigate(new StartPage());
                    return;
                    default:                        
                        return;
                }
                    
            nav = NavigationService.GetNavigationService(this);
            nav.Navigate(new AmountTransactionPage(cur));            
        }

        private void UserEntry()
        {
            bool isAutoEnd = false;
            short maxKeys = 1;
            string activeKeys = "F1, F2, F3, F4, F5, F7, CANCEL";
            string terminatorKeys = "CANCEL";

            int res = pp.UserEntry(maxKeys, isAutoEnd, activeKeys, terminatorKeys, timeout);
        }        
    }
}
