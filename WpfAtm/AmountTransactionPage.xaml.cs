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
    public partial class AmountTransactionPage : Page
    {
        KXBape pp;
        NavigationService nav;

        eCurrency cur = eCurrency.NONE;
        int amount = 0;
        int timeout = App.Timeout;

        public AmountTransactionPage(eCurrency cur)
        {
            InitializeComponent();
       
            this.cur = cur;
         
            this.pp = new KXBape();

            pp.KeyPressed += Pp_KeyPressed6;
            pp.EntryComplete += Pp_EntryComplete6;
            pp.Cancelled += Pp_Cancelled6;

            pp.Timeout += Cr_TimeoutAmount;

            Loaded += AmountTransactionPage_Loaded;

            lbCurCode.Content = cur.ToString();
        }

        private void Pp_Cancelled6()
        {
           
        }

        private void AmountTransactionPage_Loaded(object sender, RoutedEventArgs e)
        {            
             UserEntry();
        }

        private void Cr_TimeoutAmount()
        {
            pp.CancelUserEntry();
            nav = NavigationService.GetNavigationService(this);
            nav.Navigate(new StartPage());
        }

      
        private void Pp_EntryComplete6(string pinCode)
        {           
            nav = NavigationService.GetNavigationService(this);
            nav.Navigate(new StartPage());
        }

        private void Pp_KeyPressed6(string key, int keyCode)
        {          
            pp.CancelUserEntry();

            switch (key)
            {
                    case ("F1"):
                        amount = 10;
                        break;
                    case ("F2"):
                        amount = 40;
                        break;
                    case ("F3"):
                        amount = 20;
                        break;
                    case ("F4"):
                        amount = 50;
                        break;
                    case ("F5"):
                        amount = 30;
                        break;
                    case ("F6"):
                        amount = 100;
                        break;
                    case ("F7"): //cancel
                        nav = NavigationService.GetNavigationService(this);
                        nav.Navigate(new StartPage());
                        return;
                    case ("F8"):  //another amount 
                        nav = NavigationService.GetNavigationService(this);
                        nav.Navigate(new OtherAmountPage(cur));
                        return;
                default:
                    break;
            }
                    
            nav = NavigationService.GetNavigationService(this);
            nav.Navigate(new ReceiptTransactionPage(cur, amount)); 
        }


        private void UserEntry()
        {
            bool isAutoEnd = false;
            short maxKeys = 1;
            string activeKeys = "F1, F2, F3, F4, F5, F6, F7, F8, CANCEL";
            string terminatorKeys = "CANCEL";

           int res = pp.UserEntry(maxKeys, isAutoEnd, activeKeys, terminatorKeys, timeout);
        }
    }
}
