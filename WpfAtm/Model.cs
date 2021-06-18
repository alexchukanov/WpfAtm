using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace WpfAtm.Model
{
    class Model
    {
        
    }

    class CardReader
    {       

    }

    enum eTrackStatus
    {
        NOTREAD = 0,
        READ,
        BLANK,
        INVALID
    }

    enum eDeviceStatus
    {
        NODEVICE = 0,
        HEALTHY,
        FATAL
    }
    
    enum eEncryptorStatus
    {
        UNKNOWN = 0,
        HEALTHY,
        FATAL,
        NOTINITIALIZED,
        BUSY,
        INITIALIZED,
        TAMPERED
    }

    enum eStackerStatus
    {
        UNKNOWN = 0,
        OCCUPIED,
        EMPTY,
        NOTSUPPORTED
    }

    enum eStMediaStatus
    {
        NOTPRESENT,
 	    PRESENT,
 	    INJAWS,
 	    JAMMED
    }

    public enum eCurrency
    {
        NONE = 0,
        RUB = 643,
        GBP = 826,
        USD = 840,
        RON = 946,        
        EUR = 978       
    }


    public enum ePaperStatus
    {
        UNKNOWN,
        FULL,
        LOW,
        OUT
    }
}
