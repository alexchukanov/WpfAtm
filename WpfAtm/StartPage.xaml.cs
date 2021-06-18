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

using KXCardReaderLib;
using KXPinPadLib;
using KXCashDispenserLib;

using WpfAtm.Model;

using KXIndicatorsLib;

namespace WpfAtm
{
    /// <summary>
    /// Interaction logic for Page1.xaml
    /// </summary>
    public partial class StartPage : Page
    {        
        KXCardReader cr;
        KXCashDispenser dp;
        KXBape pp;
        KXIndicators ki;

        const string SupportPhone = "0 123 4455 7890";
        const string Out_of_Service = "Out of Service";
        string trackMap = "1,2,3,CHIP";
        int trackNum = 1;
        NavigationService nav;
        int timeout = App.Timeout;
        int timeoutInf = -1;

        public StartPage()
        {            
            InitializeComponent();

         
            Loaded += StartPage_Loaded;
        }

        public StartPage(string mes) : this()
        {
            lbCard.Content = String.Format("{0} {1}", mes, SupportPhone); 
        }


        private void StartPage_Loaded(object sender, RoutedEventArgs e)
        {            
            try
            {
                Application.Current.MainWindow.WindowState = WindowState.Maximized;
                Application.Current.MainWindow.WindowStyle = WindowStyle.None;

                this.pp = new KXBape();
                this.dp = new KXCashDispenser();
                this.cr = new KXCardReader();
                this.ki = new KXIndicators();



                cr.CardAccepted += Cr_CardAccepted;
                cr.CardTaken += Cr_CardTaken;
                cr.CardEjected += Cr_CardEjected;
                cr.ResetComplete += Cr_ResetComplete;
                cr.Timeout += Cr_TimeoutStartPage;
                cr.CardCaptured += Cr_CardCapturedStartPage;


                if (CheckDevicesHealth() && ClearCR())
                {
                    AcceptAndReadCardTrack(trackNum);                   
                }
                else
                {
                    lbMessage.Content = Out_of_Service;                    
                }
            }
            catch (Exception ex)
            {
               
            }
        }

        private bool CheckDevicesHealth()
        {
            return DeviceStatusCR == eDeviceStatus.HEALTHY
                   && DeviceStatusPP == eDeviceStatus.HEALTHY
                   && DeviceStatusDP == eDeviceStatus.HEALTHY
                   && StackerStatusDP == eStackerStatus.EMPTY;
        }

        private void Cr_CardCapturedStartPage()
        {
            lbCard.Content = "Support: ";
            lbPhone.Content = SupportPhone;
        }

        private void Cr_TimeoutStartPage()
        {           
            if (MediaStatusCR == eStMediaStatus.INJAWS )
            {
                cr.Capture();
            }
        }

        private void Cr_ResetComplete()
        {
            lbMessage.Content = "Take card!";
        }

        private void Cr_CardEjected()
        {
            lbMessage.Content = "Take card!";
                ki.SoundOn("EXCLAMATION", 30000);
        }

        private bool ClearCR()
        {
            bool res = true;

            if (MediaStatusCR == eStMediaStatus.PRESENT)
            {
                if (cr.CpCanEject)
                {                   
                    res = cr.Eject(timeout) == 0;
                }
                else
                {
                    res = false;
                }
            }
            else if (MediaStatusCR != eStMediaStatus.NOTPRESENT)
            {
                res = false;
            }

            return res;
        }


        private void Cr_CardTaken()
        {
            ki.SoundOff();
            lbCard.Content = "";
            AcceptAndReadCardTrack(trackNum);
        }

        private void Cr_CardAccepted()
        {
           
            eTrackStatus trackStatus = GetTrackStatus(trackNum);
                   
            if(trackStatus != eTrackStatus.READ && cr.CpCanEject)
            {
                lbCard.Content = "Card is not acceptable!";
                cr.Eject(timeout);
            }
            else
            {
               nav = NavigationService.GetNavigationService(this);
               nav.Navigate(new EnterPinCode(cr));
            }

        }

        private bool isTrackCanRead(int trackNum)
        {
            bool isCanRead = false;

            switch (trackNum)
            {
                case 1:
                    isCanRead = cr.CpCanReadTrack1;
                    break;
                case 2:
                    isCanRead = cr.CpCanReadTrack2;
                    break;
                case 3:
                    isCanRead = cr.CpCanReadTrack3;
                    break;
                default:
                    break;
            }

            return isCanRead;
        }

        private eTrackStatus GetTrackStatus(int trackNum)
        {
            eTrackStatus trackStatus = eTrackStatus.INVALID;

            switch (trackNum)
            {
                case 1:
                    trackStatus = Track1StatusCR;
                    break;
                case 2:
                    trackStatus = Track2StatusCR;
                    break;
                case 3:
                    trackStatus = Track3StatusCR;
                    break;
                default:                    
                    break;
            }

            return trackStatus;
        }



        private void AcceptAndReadCardTrack(int trackNum)
        {           
            if (isTrackCanRead(trackNum))
            {
                lbMessage.Content = "Insert card ...";
                cr.AcceptAndReadAvailableTracks(trackMap, timeoutInf);              
            }
            else
            {
                lbCard.Content = "Card is not accepted!";
                cr.Eject(timeoutInf);
            }            
        }

          
        private eTrackStatus Track1StatusCR
        {
            get
            {
                return (eTrackStatus)Enum.Parse(typeof(eTrackStatus), cr.Track1Status);
            }
        }

        private eTrackStatus Track2StatusCR
        {
            get
            {
                return (eTrackStatus)Enum.Parse(typeof(eTrackStatus), cr.Track2Status);
            }
        }

        private eTrackStatus Track3StatusCR
        {
            get
            {
                return (eTrackStatus)Enum.Parse(typeof(eTrackStatus), cr.Track3Status);
            }
        }


        //DEVICES STATUS
        private eDeviceStatus DeviceStatusCR
        {
            get
            {
                return (eDeviceStatus)Enum.Parse(typeof(eDeviceStatus), cr.StDeviceStatus);
            }
        }

        
        private eDeviceStatus DeviceStatusPP
        {
            get
            {
                return (eDeviceStatus)Enum.Parse(typeof(eDeviceStatus), pp.StDeviceStatus);
            }
        }
        

        private eDeviceStatus DeviceStatusDP
        {
            get
            {
                return (eDeviceStatus)Enum.Parse(typeof(eDeviceStatus), dp.StDeviceStatus);
            }
        }


        private eStackerStatus StackerStatusDP
        {
            get
            {
                return (eStackerStatus)Enum.Parse(typeof(eStackerStatus), dp.StStackerStatus);
            }
        }

        private eStMediaStatus MediaStatusCR
        {
            get
            {
                return (eStMediaStatus)Enum.Parse(typeof(eStMediaStatus), cr.StMediaStatus);
            }
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
           
        }
    }
}
