using System;
using System.Collections.Generic;
using System.Text;
using _3DSecureKata.Model;

namespace _3DSecureKata
{
    public interface IBanka
    {
        BankaCekimIstegiSonucu CekimIstegiGonder(SiparisBilgi siparisBilgi);
    }
}
