using System;
using System.Collections.Generic;
using System.Text;
using _3DSecureKata.Model;

namespace _3DSecureKata
{
    public interface IGuvenliPosVeritabaniVekili
    {
        long BankayaIstekOncesiIslemKaydet(SiparisBilgi siparisBilgi, string guid);
        void BankadanGelenCevapIleIslemKaydiniGuncelle(long id, BankaCekimIstegiSonucu bankaSonuc);
        void BankaHatasiniLogla(long id, Exception hata);
    }
}
