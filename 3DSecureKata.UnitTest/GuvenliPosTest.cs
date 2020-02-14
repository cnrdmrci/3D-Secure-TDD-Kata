using System;
using System.Collections.Generic;
using System.Text;
using _3DSecureKata.Model;
using Moq;
using NUnit.Framework;

namespace _3DSecureKata.UnitTest
{
    public class GuvenliPosTest
    {
        private Mock<IBankaSecen> _bankaSecen;
        private GuvenliPos _guvenliPos;
        private SiparisBilgi _siparisBilgi;
        private Mock<IBanka> _banka;
        private string _banka3DOnayUrl;
        private BankaCekimIstegiSonucu _bankaCekimIstegiSonucu;
        private Mock<IGuvenliPosVeritabaniVekili> _veritabaniVekili;

        [SetUp]
        public void Setup()
        {

            _bankaSecen = new Mock<IBankaSecen>();
            _veritabaniVekili = new Mock<IGuvenliPosVeritabaniVekili>();
            _banka = new Mock<IBanka>();

            _bankaSecen.Setup(x => x.BankaSec(It.IsAny<SiparisBilgi>())).Returns(_banka.Object);

            _guvenliPos = new GuvenliPos(_bankaSecen.Object,_veritabaniVekili.Object);
            _siparisBilgi = new SiparisBilgi
            {
                Kart = new KrediKarti
                {
                    KartNo = "0123456789123456",
                    AdSoyad = "Caner Demirci",
                    GuvenlikKodu = "000",
                    SonKullanmaYil = 2022,
                    SonKullanmaAy = 1
                },
                Tutar = 200,
                TaksitAdedi = 4
            };

            _banka3DOnayUrl = "https://ziraaat.com";
            _bankaCekimIstegiSonucu = new BankaCekimIstegiSonucu
            {
                Basarili = true,
                BankaSistemi3DOnayUrl = _banka3DOnayUrl
            };

            _banka.Setup(x => x.CekimIstegiGonder(_siparisBilgi)).Returns(_bankaCekimIstegiSonucu);
        }


        [Test]
        public void CekimIstegiGonderilirken_BankaninSecildiginiDogrular()
        {
            //when
            _guvenliPos.CekimIstegiGonder(_siparisBilgi);

            //then
            _bankaSecen.Verify(x=>x.BankaSec(_siparisBilgi));
        }

        [Test]
        public void CekimIstegiGonderilirken_BankayaIstekGonderildiginiDogrular()
        {
            //given

            //when
            _guvenliPos.CekimIstegiGonder(_siparisBilgi);

            //then
            _banka.Verify(x=>x.CekimIstegiGonder(_siparisBilgi));
        }

        [Test]
        public void SiparisBilgisindeKartNumarasiEksikse_HataDonerBankaSecilmez()
        {
            SiparisBilgi gecersizBilgi = new SiparisBilgi
            {
                Kart = new KrediKarti
                {
                    KartNo = "",
                    AdSoyad = "Caner Demirci",
                    GuvenlikKodu = "000",
                    SonKullanmaYil = 2022,
                    SonKullanmaAy = 1
                },
                Tutar = 200,
                TaksitAdedi = 4
            };

            BankaCekimIstegiSonucu bankaCekimIstegiSonucu = _guvenliPos.CekimIstegiGonder(gecersizBilgi);

            Assert.IsTrue(!bankaCekimIstegiSonucu.Basarili);
            _bankaSecen.Verify(x=>x.BankaSec(It.IsAny<SiparisBilgi>()),Times.Never);
            
        }

        [Test]
        public void BankadanDonen3DOnayUrlIstemciyeDoner()
        {

            BankaCekimIstegiSonucu sonuc = _guvenliPos.CekimIstegiGonder(_siparisBilgi);

            Assert.AreEqual(sonuc.BankaSistemi3DOnayUrl, _banka3DOnayUrl);
        }

        [Test]
        public void BankayaCekimIstegiGonderildiginde_AsenkronCevaptaKullanilmakUzereSiparisBilgisiKaydedilir()
        {
            _guvenliPos.CekimIstegiGonder(_siparisBilgi);

            _veritabaniVekili.Verify(x => x.BankayaIstekOncesiIslemKaydet(_siparisBilgi, It.IsAny<string>()));
            _veritabaniVekili.Verify(x=>x.BankadanGelenCevapIleIslemKaydiniGuncelle(It.IsAny<long>(),_bankaCekimIstegiSonucu));
        }

        [Test]
        public void BankaCekimIstegineExceptionAtarsa_BuLoglanir()
        {
            Exception hata = new Exception();
            _banka.Setup(x => x.CekimIstegiGonder(It.IsAny<SiparisBilgi>())).Throws(hata);

            _guvenliPos.CekimIstegiGonder(_siparisBilgi);

            _veritabaniVekili.Verify(x=>x.BankaHatasiniLogla(It.IsAny<long>(),hata));
        }
    }
}
