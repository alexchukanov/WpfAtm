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
    public partial class DispenseTransactionPage : Page
    {
        KXCashDispenser dp;
        KXReceiptPrinter rp;
        KXCardReader cr;

        NavigationService nav;

        eCurrency cur = eCurrency.NONE;
        int amount = 0;
        int timeout = App.Timeout;
        int mixAlgo = 1;

        string accountNo = "123456";
        double balance = 10000.00;

        bool isReceipt = false;
        string receiptFormePath = @"C:\temp\ReceiptForm.txt";

        string receiptForm =
@"
KALIGNITE DEMO RECEIPT

Account No: {0}
Currency: {1}

Date: {2} {3}
Account Transfer: {4:0.00}

Balance: {5:0.00}
";
     
        public DispenseTransactionPage(eCurrency cur, int amount, bool isReceipt)
        {
            InitializeComponent();

            this.cur = cur;
            this.amount = amount;
            this.isReceipt = isReceipt;           

            dp = new KXCashDispenser();
            dp.CashDispensed += Dp_CashDispensed;
            dp.CashTaken += Dp_CashTaken;
            dp.NotDispensable += Dp_NotDispensable;
            dp.CashUnitError += Dp_CashUnitError;
            dp.DeviceError += Dp_DeviceError;

            rp = new KXReceiptPrinter();
            rp.DeviceError += Rp_DeviceError;
            rp.DefaultMediaName = "ReceiptPrinter";

            cr = new KXCardReader();          
            cr.CardTaken += Cr_CardTakenDispense;
            cr.CardEjected += Cr_CardEjectedDispense;
            cr.Timeout += Cr_TimeoutDispense;
            cr.CardCaptured += Cr_CardCapturedDispense;

            Loaded += DispenseTransactionPage_Loaded;
        }

        private void Cr_CardTakenDispense()
        {            
            if (isReceipt)
            {
                string date = DateTime.Now.ToShortDateString();
                string time = DateTime.Now.ToShortTimeString();

                string receipt = String.Format(receiptForm, accountNo, cur, date, time, amount, balance);

                File.WriteAllText(receiptFormePath, receipt);

                rp.PrintFile(receiptFormePath, -1, 0);
                rp.Eject(-1);
            }
            
            nav = NavigationService.GetNavigationService(this);
            nav.Navigate(new StartPage());
        }

        private void Cr_CardEjectedDispense()
        {
            lbTakeCard.Content = String.Format("Take card!");            
        }

        private void Cr_CardCapturedDispense()
        {
            nav = NavigationService.GetNavigationService(this);
            nav.Navigate(new StartPage("Support:"));
        }


        private void Dp_DeviceError()
        {
            nav = NavigationService.GetNavigationService(this);
            nav.Navigate(new AnotherAmountPage(cur));
        }

        private void Dp_CashUnitError(short CUNumber)
        {           
            nav = NavigationService.GetNavigationService(this);
            nav.Navigate(new AnotherAmountPage(cur));
        }

        private void Dp_NotDispensable()
        {
            nav = NavigationService.GetNavigationService(this);
            nav.Navigate(new AnotherAmountPage(cur));
        }

        private void Dp_CashTaken()
        {
            lbTakeCard.Content = "";
            cr.Eject(timeout);
        }

        private void Rp_DeviceError()
        {
           nav = NavigationService.GetNavigationService(this);
           nav.Navigate(new StartPage("Out of Service"));      
        }
        

        private void Dp_CashDispensed()
        {
            lbTakeCard.Content = String.Format("Take money!");
            dp.Present(timeout);
        }

    
        private void Cr_TimeoutDispense()
        {
            if (MediaStatusCR == eStMediaStatus.PRESENT || MediaStatusCR == eStMediaStatus.INJAWS)
            {
                cr.Capture();
            }
            else
            {
                nav = NavigationService.GetNavigationService(this);
                nav.Navigate(new StartPage());
            }
        }

              
        private void DispenseTransactionPage_Loaded(object sender, RoutedEventArgs e)
        {
            dp.MixAndDispense(amount, cur.ToString(), mixAlgo.ToString());
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
