using System;
using System.Collections.Generic;
using System.Text;
using _3DSecureKata.Model;

namespace _3DSecureKata
{
    public class GuvenliPos
    {
        private IBankaSecen _bankaSecen;
        private IGuvenliPosVeritabaniVekili _veritabaniVekili;
        public GuvenliPos(IBankaSecen bankaSecen,IGuvenliPosVeritabaniVekili veritabaniVekili)
        {
            _bankaSecen = bankaSecen;
            _veritabaniVekili = veritabaniVekili;
        }

        public BankaCekimIstegiSonucu CekimIstegiGonder(SiparisBilgi siparisBilgi)
        {
            SiparisKontrolSonuc siparisKontrolSonuc = siparisBilgileriGecerliMi(siparisBilgi);
            if (!siparisKontrolSonuc.Basarili)
                return new BankaCekimIstegiSonucu() {Basarili = false};

            IBanka banka = _bankaSecen.BankaSec(siparisBilgi);

            string guid = "";
            long islemKayitId = _veritabaniVekili.BankayaIstekOncesiIslemKaydet(siparisBilgi, guid);

            try
            {
                var cekimIstegiSonuc = banka?.CekimIstegiGonder(siparisBilgi);
                if (cekimIstegiSonuc != null)
                {
                    _veritabaniVekili.BankadanGelenCevapIleIslemKaydiniGuncelle(islemKayitId, cekimIstegiSonuc);
                    return cekimIstegiSonuc;
                }

                return null;
            }
            catch (Exception ex)
            {
                _veritabaniVekili.BankaHatasiniLogla(islemKayitId, ex);
                return new BankaCekimIstegiSonucu() {Basarili = false, Mesaj = "Banka çekimi sırasında hata oluştu - " + ex.Message};
            }
        }

        private SiparisKontrolSonuc siparisBilgileriGecerliMi(SiparisBilgi siparisBilgi)
        {
            if (siparisBilgi.Kart == null)
                return new SiparisKontrolSonuc() {Basarili = false, Mesaj = "Kart bilgileri bulunamadi."};

            if (siparisBilgi.Kart.KartNo.Length != 15 && siparisBilgi.Kart.KartNo.Length != 16)
                return new SiparisKontrolSonuc() {Basarili = false, Mesaj = "Kredi kartı numarası 15 veya 16 hane olmalı" };

            if (siparisBilgi.Kart.GuvenlikKodu.Length != 3)
                return new SiparisKontrolSonuc() {Basarili = false, Mesaj = "Kart güvenlik numarası 3 hane olmalı"};

            return new SiparisKontrolSonuc() {Basarili = true};
        }
    }
}
