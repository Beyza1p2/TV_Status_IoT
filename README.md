# TV_Status_IoT
#.NET nanoFramework ile Akıllı Ev Sistemleri:TV Durum Takibi
Bu proje, bir **ESP32** mikrodenetleyici kartı kullanarak evdeki akıllı televizyonun (LG Smart TV) açık/kapalı olma durumunu soket bağlantısı üzerinden gözetleyen ve bu veriyi gerçek zamanlı olarak buluta (**Firebase Realtime Database**) aktaran bir Nesnelerin İnterneti (IoT) projesidir. Proje aynı zamanda buluttaki bu veriyi çekip kullanıcıya gösteren bir C# masaüstü arayüz uygulaması (AkilliEvApp) içerir.
#NOT:Geliştirme sürecinde kopyala-yapıştır kodlar yerine; HTTP istek katmanlarını, bellek yönetimini (using blokları) ve donanım haberleşmesini mimari düzeyde öğrenerek geliştirdim.
#Proje Mimarisi ve Çalışma Mantığı:
Proje iki ana yazılım bileşeninin bulut üzerinde köprü kurmasıyla çalışır:
**Donanım Katmanı (ESP32 / .NET nanoFramework):**
   * Belirtilen yerel IP ve Port (8080) üzerinden TV'ye TCP Soket testi gönderir.
   * TV açık ise bağlantı başarılı olur ve Firebase'e `PUT` isteği ile "ACIK" verisini fırlatır.
   * TV kapalı ise bağlantı düşer, `catch` bloğu güvenle devreye girer ve Firebase'e "KAPALI" verisini yazar.
   * Hafıza sızıntılarını (Memory Leak) önlemek için tüm internet akışları iç içe `using` blokları ile anında imha edilir.
   * **Bulut Katmanı (Firebase Realtime Database):**
   * Cihaz ile arayüz uygulaması arasında ortak bir dijital pano görevi görür. Verileri JSON formatında anlık tutar.
**Arayüz Katmanı (C# Masaüstü Uygulaması):**
   * Buluttaki veriyi `GET` isteği ile asenkron (async/await) olarak çeker ve ekranı dondurmadan kullanıcıya yansıtır.
#Kullanılan Teknolojiler
* **Donanım Programlama:** C#, .NET nanoFramework
* **Bulut Çözümü:** Firebase Realtime Database
* **Masaüstü Arayüz:** C# MAUI(XAML)
* **Protokoller:** TCP/IP Sockets, HTTP (GET, PUT), JSON
* **İzleme & Loglama:** PuTTY (Serial Terminal)






