using System;
using System.Diagnostics;
using System.Threading;
using System.Net.NetworkInformation;
using nanoFramework.Networking;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace AkilliEv
{
    public class Program
    {
        public const string ssıd = "Wifi_SSID";
        public const string pass = "Wifi_Passwort";
        public const string TvIp = "192.168.1.104";
        private static CancellationTokenSource cts;
        public const int portNum = 8080;
        private const string firebaseUrl = "https://smarthomeproject-e0bb8-default-rtdb.firebaseio.com/TvDurumu.json";
        public static void Main()
        {

            cts = new CancellationTokenSource();//Makro düzey projenin tamamını uzaktan durdurmak için kullanırız.

            Debug.WriteLine("---SİSTEM BAŞLATILDI---");
            WifiNetworkHelper.ConnectDhcp(ssıd, pass,token:cts.Token);

            while (!cts.Token.IsCancellationRequested)//dışarıdan iptal tetiklenmediği sürece dönsün.
            {
                //nesneyi boş olarak önceden bildiriyoruz.
                Socket socketTesti = null;
                try
                {
                    //Socket oluşturma
                    socketTesti = new Socket(AddressFamily.InterNetwork, SocketType.Stream,
                           ProtocolType.Tcp);
                    socketTesti.SendTimeout = 2000;//mikro düzey.
                    socketTesti.ReceiveTimeout = 2000;

                    IPAddress IpNesnesi = IPAddress.Parse(TvIp);
                    //string IP yı alıp PC nin analayabileceği dildeki nesneye çeviriyoruz.
                    //şimdi de IP ve portu birleştirip tam adres kartı yapıyoruz.
                    IPEndPoint paketAddress = new IPEndPoint(IpNesnesi, portNum);
                    //Sockete paket kartı verip bağlantıyı tetikliyoruz.
                    socketTesti.Connect(paketAddress);
                    Debug.WriteLine("LG Smart TV AÇIK!,Tasarruf Modu PASİF🪫");

                    VeriGonder("ACIK");

                    Debug.WriteLine($"Buluttan doğrulanan:{VeriCek()}");
                    
       
                }
                catch(Exception)
                {
                    
                    Debug.WriteLine("GÜVENLİ] LG Smart TV şu an KAPALI veya Stand-By modunda.Akım gözetleniyor.🔌");

                    VeriGonder("KAPALI");

                    Debug.WriteLine($"Buluttan doğrulanan:{VeriCek()}");

                }
                finally
                {
                    if (socketTesti != null)
                    {
                        socketTesti.Close();//ESP nin RAM ini rahatlatıyoruz.
                    }
                    Thread.Sleep(5000);
                }
            }
            
        }
        //Kodun okunabilirliğini ve kullanılabilirliğini artırmak için Main in dışına yazdım.
        //Request
        private static void VeriGonder(string durum)
        {
            //Firebase'e göndermek üzere bir HTTP isteği hazırlıyoruz.
            HttpWebRequest istek = (HttpWebRequest)WebRequest.Create(firebaseUrl);
            istek.Method = "PUT";//Sunucudaki mevcut veriyi güncellemek için kullandım.
            byte[] veriPaketi = System.Text.Encoding.UTF8.GetBytes("\"" + durum + "\"");
            //Firebase e gidecek verinin formatını bildiriyoruz.
            istek.ContentType = "application/json";
            //Firebase ne kadarlık bir veri boyutu geleceğini bilmesi için
            istek.ContentLength = veriPaketi.Length;
            //Borunun ağzını verimizi yani isteğimizi göndermek için açıyoruz.
            using (Stream s = istek.GetRequestStream())
            {
                s.Write(veriPaketi, 0, veriPaketi.Length);
            }
            //Response-Firebase in veriyi alıp almadığını öğreniyoruz.
            try
            {
                //Gönderdiğimiiz istekten yanıt alıyoruz.
                using (HttpWebResponse cevap = (HttpWebResponse)istek.GetResponse())
                {
                    if (cevap.StatusCode == HttpStatusCode.OK)
                    {
                        Debug.WriteLine("Veri başarıyla buluta gönderildi ✅");
                    }

                }


            }
            catch (Exception ex)
            {
                Debug.WriteLine($"HATA oluştu ❌ {ex.Message}");
            }
        }
            private static string VeriCek() {

            HttpWebRequest istek=(HttpWebRequest)WebRequest.Create(firebaseUrl);
            istek.Method = "GET";//Firebase e GET isteği atıyoruz.
            try
            {
                using(HttpWebResponse cevap = (HttpWebResponse)istek.GetResponse())//Kargo kamyonu
                {
                    using (Stream v = cevap.GetResponseStream())//Boru
                    {
                        using (StreamReader okuyucu = new StreamReader(v))//Tercüman
                        {
                            //burda yaptığımız açtığımız borudan gelen bytleri yakalamaktır.
                            string okunanVeri = okuyucu.ReadToEnd();

                            return okunanVeri;
                          }
                        }
                    }

                        }
            catch(Exception ex)
            {
                Debug.WriteLine($"HATA oluştu ❌ {ex.Message}");
                return " ";//hata olursa eli boş dönmesin.
            }
                    }
                }
            }

           
    


