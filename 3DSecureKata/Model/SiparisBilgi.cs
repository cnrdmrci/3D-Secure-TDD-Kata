using System;
using System.Collections.Generic;
using System.Text;

namespace _3DSecureKata.Model
{
    public class SiparisBilgi
    {
        public KrediKarti Kart { get; set; }
        public double Tutar { get; set; }
        public int TaksitAdedi { get; set; }

        public string SiparisAnahtar { get; set; }
        public string PassUrl { get; set; }
        public string FailUrl { get; set; }
	}
}
