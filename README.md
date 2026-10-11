# ColorBlockJam

Renkli blokları grid üzerinde sürükleyip aynı renkteki kapılardan çıkarmaya dayalı bir bulmaca oyunu.

Bu dosyada aldığım mimari kararları ve nedenlerini tutuyorum.

## Demo

| Cihazda oynanış | Level Editor |
|---|---|
| [![Cihazda oynanış](https://drive.google.com/thumbnail?id=1iJUyQTDcsOPSUdnywnFixQgo5Ly_S264&sz=w400)](https://drive.google.com/file/d/1iJUyQTDcsOPSUdnywnFixQgo5Ly_S264/preview) | [![Level Editor](https://drive.google.com/thumbnail?id=1nN4AkMikXsfMwj1uXkPPC2X9ewVQFwcS&sz=w400)](https://drive.google.com/file/d/1nN4AkMikXsfMwj1uXkPPC2X9ewVQFwcS/preview) |
| [▶ İzle](https://drive.google.com/file/d/1iJUyQTDcsOPSUdnywnFixQgo5Ly_S264/preview) | [▶ İzle](https://drive.google.com/file/d/1nN4AkMikXsfMwj1uXkPPC2X9ewVQFwcS/preview) |

**Android APK:** [İndir](https://drive.google.com/uc?export=download&id=1BJ7TiLFEO6VlRv8XJSMMEu-ZXdmhsovL)

---

## Yapay zekâ ile geliştirme

Projenin kodu, prefab'ları, asset'leri ve testleri Claude Code ile geliştirildi. Unity editörüne Unity MCP ile bağlandı. Yapay zekâyı kod yazan bir araç olarak değil, yönettiğim bir geliştirici gibi kullandım: neyin, hangi mimariyle ve hangi kalitede yapılacağına ben karar verdim, her adımı ben onayladım ve gözden geçirdim.

**Ortam**

- Claude Code dosyaları yazıyor; Unity MCP üzerinden editörü kendisi kullanıyor:
  - Derlemeyi tetikleyip konsolu okuyor.
  - EditMode testlerini çalıştırıyor.
  - Prefab ve asset'leri `AssetDatabase` ile oluşturuyor.
  - Play moduna girip oyunu kare kare ilerletiyor.
- Görsel kontrol ekran görüntüsüyle değil, kameranın bir RenderTexture'a çizdirilmesiyle yapılıyor. Böylece yapay zekâ sadece oyunu görüyor, ekranımdaki başka bir şeyi görmüyor.
- Çalışma kurallarını bir kez anlatıp kalıcı hafızaya yazdırdım; her oturumda bunlara uyuyor:
  - Asset taşıma/silme sadece `AssetDatabase` ile.
  - Play modundayken derleme yok.
  - Test öncesi oyuncu kaydı yedeklenip sonra geri yükleniyor.
  - Commit'i hiçbir zaman yapay zekâ atmıyor.

**Planlama: kararlar benden**

- İşi fazlara böldüm ve her fazı ayrı branch ve PR olarak ilerlettim:
  - Altyapı, sahne ve level yükleme
  - Tahta ve bloklar
  - Kazanma/kaybetme
  - Oyun içi arayüz ve ana menü
  - Level Editor
  - Görsel iyileştirme
- Büyük bir işe başlamadan önce plan istiyorum. Gereksinimleri ve tasarım tercihlerimi madde madde ben yazıyorum. Örneğin Level Editor için:
  - Paletler nerede duracak, seçili kutu nasıl belli olacak.
  - Load ve Save nerede duracak.
  - Kapı nasıl yerleşecek.
  - Mevcut sistemlere dokunulmayacak.

  Yapay zekâ belirsiz noktaları soruyor, ben karar veriyorum.
- Beğenmediğim planı reddedip yön değiştiriyorum. Blokların kapıya giden yolunun kontrolünde önerilen çözümü fazla karmaşık buldum. Grid doluluğunu ayrıca tutup tutmadığımızı sorgulayıp tartıştıktan sonra daha basit, fiziğe dayalı yaklaşımda karar kıldım.
- Mimari çerçeveyi ben çizdim:
  - Zenject yerine `ServiceLocator`.
  - Event'ler sadece bağımsız bildirimler için; akış yöneticide doğrudan çağrılarla.
  - Popup'lar asla event ile açılmıyor.
  - Feature bazlı klasörler; her assembly'ye GUID ile referans.
  - Runtime'da `GetComponent`/`Find`/`Camera.main` yok.
  - Ayarlanabilir değerler config ScriptableObject'lerinde.
  - Soyut sınıflar `Base` önekli.

**Review: her fazdan sonra**

- Her fazın sonunda kodu okuyup sorguluyorum ve düzelttiriyorum:
  - "Bu sınıf nerede kullanılıyor?"
  - "Bu isim ne yaptığını anlatıyor mu?"
  - "Bu iş bu sınıfın sorumluluğu mu?"
- Review'da yakaladığım ve düzelttirdiğim örnekler:
  - Fazla event'e bölünmüş akışı tek yöneticiye topladım.
  - Kalan blok sayısını tutan spawner'ı sadece oluşturan hale getirdim.
  - Oyuncu verisi, coin ve can tek sınıftaydı; `PlayerDataStore`, `WalletController` ve `LivesController` olarak ayırttım.
  - Oyunu etkileyen `DoorView`/`ExitView` ikilisi tek bir `Door` oldu; `View`'ların sadece görsel iş yapması kuralını koydum.
- SOLID açısından analiz istiyorum, çıkan maddeleri tek tek değerlendirip hangilerinin düzeltileceğini seçiyorum.
- Kalite kurallarım kalıcı: aşırı mühendislik yok, yorum satırı ve ölü kod yok, kod tekrarı yok, sadece anlamlı testler.

**Doğrulama: "çalışıyor" demek yetmiyor**

- Mantık saf fonksiyonlara ayrılıp test ediliyor; EditMode testleri her değişiklikten sonra çalışıyor.
- Arayüz verdiğim referans görsellere göre kuruldu. Render'lar referansla üst üste bindirilerek ölçüldü; farklı ekran oranlarında ve çentikli ekranlarda denendi.
- Görsel ayarlarda yapay zekâ referanstan renk ve perspektif ölçüyor, varyantları yan yana çiziyor; seçimi ben yapıyorum. Gerekirse kamera ve ışığı play modunda kendim ayarlıyorum, yapay zekâ o değerleri projeye kaydediyor.
- Level'ların çözülebilirliği tahmine bırakılmadı. Yapay zekâya oyunun kurallarını birebir uygulayan bir çözücü yazdırdım. Her level önce çözücüden geçti, sonra Unity'de fizikle tek tek oynatılıp bitirildi.
- Son kontrol bende: oyunu kendim oynuyorum, diff'leri okuyorum ve commit'leri kendim atıyorum.

---

## Kapsam

- Önce sadece Main ve Gameplay sahneleriyle başladım.
- Level'ları başta test amaçlı, elle yazılmış basit level'larla yapıyorum. Level editörünü `LevelData` yapısı oturduktan sonra ayrı bir branch'te yazacağım. Böylece editörü değişmeye devam eden bir veri formatına göre yazmak zorunda kalmıyorum.
- Merkezi bir config sistemi kurmadım. Her sistem ihtiyaç duyduğunda kendi ScriptableObject config'ini oluşturuyor.

## Klasör ve assembly yapısı

```
Scripts/
  Core/           Game.Core            oyundan bağımsız altyapı (ServiceLocator, EventBus, BaseService)
  Events/         Game.Events          bütün event'ler
  <X>System/      Game.<X>System       oyun sistemleri: LevelSystem, PlayerSystem, SceneSystem …
  UI/<Ekran>/     Game.UI.<Ekran>      arayüz ekranları: LoadingScreen …
  UI/Common/      Game.UI.Common       birden fazla ekranda kullanılan arayüz parçaları (Settings, coin/can göstergeleri)
                  her birinin içi: Interfaces/ · Data/ · Controllers/ · Views/
```

- Oyun sistemleri `<X>System` olarak adlandırılıyor, arayüz ekranları `UI` altında toplanıyor. Böylece klasör listesine bakan biri neyin oyun sistemi, neyin ekran olduğunu ilk bakışta ayırt edebiliyor. Assembly adları ve namespace'ler de klasör adlarıyla aynı.

- Kodu katmanlara göre (Contracts, Data, Gameplay, UI gibi) de bölebilirdim. Ama o zaman tek bir sistem dört ayrı klasöre dağılıyor ve koda bakan biri bir sistemin neyden oluştuğunu tek yerde göremiyordu. Bu yüzden klasörler sisteme (feature) göre; bir sisteme ait her şey kendi klasöründe.
- Her feature'ın içi aynı şekilde bölünüyor: `Interfaces` (dışarıya açılan sözleşme), `Data` (veri ve ScriptableObject'ler), `Controllers` (servisler ve mantık), `Views` (sahnede görünen her şey).
- `Data` sınıfları sadece veri tutuyor, kural içermiyor; `View`'lar sadece görsel iş yapıyor. Oyunu etkileyen MonoBehaviour'lar (blok, kapı, kamera yerleşimi) View sayılmıyor; `View` eki olmadan kendi adlarıyla (`Block`, `Door`, `CameraFitter`) Controllers'ta duruyor. Bu yüzden:
  - Çıkışın yönü ve kapladığı kenarlar `ExitPlacement`'ta değil, `ExitGeometry`'de hesaplanıyor.
  - "Son level'dan sonra başa dön" kuralı `LevelCatalog`'da değil `LevelLoader`'da; katalog sadece listeyi tutuyor.
  - "Son Play On kademesinde kal" kuralı `PlayOnConfig`'te değil `GameplayManager`'da.
  - Settings popup'ı ayarları kendisi okuyup yazmıyor; değerleri ve değişiklik callback'ini `Setup` ile alıyor, yazmayı popup'ı açan taraf yapıyor. Ayrı bir Gameplay katmanı yok; bir feature'ın oyunda görünen tarafı kendi `Views` klasöründe.
- Interface'leri ayrı bir sözleşme assembly'sinde tutarak "her şeye interface ile erişim" kuralını derleyiciye zorlatabilirdim. Aynı şeyi daha az yapıyla C#'ın kendi aracıyla yapıyorum: interface'ler, event'ler ve veri tipleri `public`, implementasyonlar `internal`. Başka bir feature interface'i görebiliyor ama somut sınıfı göremiyor. Implementasyonlar sahneye component olarak eklendiği için kimsenin onlara doğrudan erişmesi de gerekmiyor.
- Testlerin ve editör araçlarının bazı iç sınıflara (örneğin `WallLayout`, `ExitFit`, `Block`) erişmesi gerekiyor. Bunun için o sınıfları `public` yapabilirdim, ama o zaman diğer feature'lar da onları görürdü. Bunun yerine her assembly'nin `AssemblyInfo.cs` dosyasında `InternalsVisibleTo` ile sadece test assembly'sine ve kendi editör assembly'sine izin veriyorum; oyun kodu için sınıflar kapalı kalıyor.
- Event'leri her feature kendi içinde de tanımlayabilirdi. Ama oyunda herhangi bir sistem herhangi bir event'e abone olabilir; event'ler kendi feature'ında olsaydı abone olan sistem, event'i atan feature'a bağımlı olmak zorunda kalırdı. Bu yüzden bütün event'ler ayrı bir `Game.Events` assembly'sinde toplanıyor. Bütün feature'lar ona bağlı olduğu için o hiçbir feature'a bağlanamıyor; bu yüzden event'ler feature'a özel tipler taşıyamıyor.
- Rol klasörleri (`Interfaces`, `Controllers` …) namespace'e eklenmiyor; namespace sistem düzeyinde (`Game.LevelSystem`).
- Kullanılmayan klasör ve assembly referanslarını tutmuyorum; ihtiyaç doğduğunda ekleniyor.
- Assembly referansları isimle de verilebilirdi. GUID ile veriyorum; bir assembly yeniden adlandırıldığında referanslar kopmuyor.
- Sahnelerde duran her obje bir prefab. Sahneye özel bağlantılar (UI kamerası gibi) prefab'da değil, sahnedeki instance'ta duruyor.
- Prefab klasörleri script klasörleriyle aynı yapıda (`Prefabs/LevelSystem` ↔ `Scripts/LevelSystem`, `Prefabs/MainMenuSystem` ↔ `Scripts/MainMenuSystem`); bir prefab'ın script'i neredeyse prefab'ı da orada. Script karşılığı olmayan sahne kurulum prefab'ları (kameralar, ışık) kendi klasörlerinde duruyor.
- Her iki sahnede aynı kamera prefab'ını kullanabilirdim. Ama Gameplay'de farklı açıda bir kamera kullanılacağı için `MainMenuCamera` ve `GameplayCamera` ayrı prefab'lar.
- `*ServiceLocator` prefab'larında sadece servisler duruyor. `GameplayManager` veya loading ekranı gibi servis olmayan objeler sahnede kendi prefab'larıyla duruyor.

## Bağımlılık yönetimi

- DI için Zenject kullanabilirdim. Ama hem öğrenme eşiği hem de cold start maliyeti yüksek olduğu için daha basit bir `ServiceLocator` yazdım.
- `ServiceLocator`'ı saf statik bir sınıf olarak da yazabilirdim. Ama proje boyunca yaşayan servislerle birlikte Bootstrap sahnesinde dursun diye `MonoSingleton` olarak `ProjectServiceLocator` prefab'ının köküne koydum. `MonoSingleton` `DontDestroyOnLoad` olduğu için bu nesneler sahne geçişlerinde yaşamaya devam ediyor.
- Servisler iki yerde duruyor: bütün projede kullanılanlar Bootstrap'taki `ProjectServiceLocator`'da (DontDestroyOnLoad), bir sahneye ait olanlar o sahnenin `*ServiceLocator` prefab'ında (örneğin `GameplayServiceLocator`). Sahneye ait servisler sahneyle birlikte doğup ölüyor; sahne kapanınca kayıtları da kendiliğinden siliniyor.
- Her sahne için ayrı bir ServiceLocator da kurabilirdim. Ama sahneler artık tek tek yüklendiği için iki sahne aynı anda aynı interface'i kaydetmiyor; servislerin ömrü zaten `Awake`'te kayıt, `OnDestroy`'da kayıt silme kuralından geliyor. Bu yüzden tek bir `ServiceLocator` yeterli. `GameplayServiceLocator` gibi sahne prefab'larının kendi locator'ları yok; içlerindeki servisler bu tek `ServiceLocator`'a kaydoluyor.
- Servisleri `*ServiceLocator` objesine component olarak da ekleyebilirdim. Hangi servislerin olduğu Hierarchy'de net görünsün diye her servis, servisle aynı isimde bir child objede duruyor.
- Bütün servis ve controller'lara interface üzerinden erişiyorum, somut sınıflara doğrudan bağımlılık yok. Bir controller'ın birden fazla interface'i olabiliyor; `Awake`'te kendini her biri için ayrı ayrı kaydediyor.
- Servislerin kayıt sırası önemli olmasın istedim. Execution order ile sıralamak yerine şu kuralı koydum: `Awake`'te sadece kayıt (`Bind`) yapılır, başka servislere erişim (`Get`) `Start`'tan itibaren yapılır. Unity bütün `Awake`'leri `Start`'lardan önce çağırdığı için sıra bağımlılığı oluşmuyor.
- `BaseService`'i `BaseService<T>` şeklinde generic bir sınıf olarak da yazabilirdim. Böylece her servis kendini kendi tipiyle kaydedebilir, servislere de bu tip üzerinden erişilirdi. Ama servislere interface üzerinden erişmeye ve kayıtları interface ile yapmaya karar verdiğim için bu generic parametrenin bir görevi kalmadı.

## Event sistemi

- Event'leri, birbirinden bağımsız çalışan sistemlerin haberleşmesi için kullanıyorum. Bir event'e abone olan her sistem onu kendi içinde işler.
- Oyunun genel akışı (süre, kazanma/kaybetme, hangi popup'ın açılacağı) tek bir yöneticide, doğrudan çağrılarla ilerliyor; popup'lar event ile açılmıyor. Event'ler akışa bağımsız tepki veren veya dışarıdan erişilmesi gerekmeyen şeyler için: sahne yükleniyor / ana menü yüklendi / level hazır (loading ekranı, kamera), blok çıktı (yönetici sayar), ekrana dokunuldu (yönetici süreyi başlatır), input kilidi (`DragController`), coin değişti (coin göstergeleri). Bunları atan taraf kimin dinlediğini bilmiyor.
- Event'ler class olarak da tanımlanabilirdi. Ama sık atılan bir şey için her seferinde allocation ve GC yükü oluşmasını istemediğim için struct (`readonly struct : IEvent`) kullandım. Event'leri gönderirken `new` ile oluşturmak yeterli.
- Event'ler için ortak bir taban sınıf da kullanılabilirdi. Ama struct'lar sınıftan türeyemediği ve taban sınıfın tek işi "bu bir event" demek olacağı için `IEvent` marker interface'i yeterli oldu.
- Event'leri hiçbir listede tutmuyorum ve sıraya koymuyorum. Event'ler tutulup sırası ayarlanacak şeyler değil. Sıra ve bekleme gerektiren akışlar event'lerle değil, `await` edilen adımlarla yönetiliyor.
- Event'leri sıraya koyup sonra gönderen bir kuyruk eklenebilirdi. Ama tek tük bir kullanım için ayrı bir sistem tutmak istemedim.
- Event'leri beklenebilir (awaitable) yapan bir yapı da kurulabilirdi. Ama event, abone olan her sistemin kendi başına işlediği bir bildirim; beklenmesi gereken yerlerde ilgili işi UniTask ile `await` ediyorum.
- `Fire` sırasında bir handler abonelikten çıkabilsin diye abone listesi sondan başa dolaşılıyor.
- Bir handler hata verirse hata loglanıyor ve kalan handler'lar çalışmaya devam ediyor; tek bir sistemdeki sorun aynı event'i dinleyen diğer sistemleri durdurmuyor.

## Oyun akışı

- Oyun için bir state machine (Ready, Playing, Paused, Won, Failed gibi state'ler) kurulabilirdi. Ama şu an ele alınması gereken bir durum olmadığı için kurmadım; akışı `GameplayManager` yönetiyor, ihtiyaç doğduğunda eklenecek.
- Sıralı akışlar için genel bir sequence sistemi kurulabilirdi. İhtiyaç olana kadar yazmıyorum; şimdilik sıralı bir akış gerektiğinde onu tek bir koordinatör metot içinde `await`'lerle yazıyorum. Benzer akışlar çoğalırsa ayrı bir PR'da genel bir sisteme çevrilecek.
- Sahneleri additive yükleyip Main'i açık tutabilirdim. Ama iki sahne aynı anda açıkken kameralar, EventSystem'ler ve sahneye ait servisler çakışmaya başlar. Bu yüzden sahneler tek tek yükleniyor (Main ↔ Gameplay); her sahne kendi kamerası ve servisleriyle kendi kendine yetiyor.
- Play butonu bir event atıp akışı o event üzerinden başlatabilirdi. Ama "Gameplay'i aç" bir komut; dinleyen olmazsa sessizce hiçbir şey olmazdı. Bu yüzden komutu doğrudan `ISceneLoader` üzerinden çağırıyorum. Yüklemenin başladığını `SceneLoader`, level'ın hazır olduğunu Gameplay sahnesi event olarak atıyor; loading ekranı kimin başlattığından bağımsız çalışıyor.
- Loading ekranı Bootstrap'ta kendi prefab'ıyla duruyor ve sahne değişirken kaybolmaması için kendini DontDestroyOnLoad yapıyor. Kameraya bağlı kalmaması için `Screen Space - Overlay` modunda.
- Loading ekranı her sahne yüklemesinde (`SceneLoadStartedEvent`) açılıyor; ana menü yüklenince (`MainMenuLoadedEvent`) ya da level hazır olunca (`LevelReadyEvent`) kapanıyor. Ana menüye dönüş de Gameplay'e geçiş gibi `ISceneLoader` üzerinden.

## Level akışı

- Level'ı Bootstrap'taki bir servis de başlatabilirdi, ama o zaman sahnenin yüklenmesini ve sahnedeki servislerin hazır olmasını dışarıdan beklemesi gerekirdi. Level akışı Gameplay sahnesine ait olduğu için onu sahnedeki `GameplayManager` başlatıyor: level datasını yükle → level'ı oluştur → süreyi başlat → `LevelReadyEvent`. Her adımı `await` ettiğim için loading ekranı ancak her şey bittiğinde kapanıyor.
- Aynı sahnedeki `Start`'ların hangi sırayla çalışacağı garanti değil. Bir frame bekleyip Start'ların bitmesini varsaymak yerine, `GameplayManager` kullandığı servislerin hazır olmasını (`IInitializable.IsInitialized`) açıkça bekliyor. Bir servisin hazırlanması ileride birden fazla frame sürerse de akış bozulmuyor.
- Servislerin dışarıya açtığı tek yaşam döngüsü bilgisi sadece okunabilen `IInitializable.IsInitialized`. `Initialize`/`DeInitialize` `BaseService`'te `protected`; onları servisi kullanan kimse görmüyor. Hiçbir yerde tip olarak kullanılmayan `IService` arayüzü kaldırıldı.
- `LevelCreator.Create` senkron; level'ı kurmak bekleme gerektirmediği için `UniTask` döndürmüyor.
- İş parçalara bölündü: `GameplayManager` akışı yönetir, `LevelLoader` datayı getirir, `LevelCreator` level'ı kurar. Loader oyuncu verisini bilmez; hangi level'ın yükleneceğini `GameplayManager` söyler.
- `LevelLoader`'i proje servisi olarak da tutabilirdim. Ama yüklenen datanın ömrü level'ın ömrü olduğu için Gameplay'de duruyor; sahne kapanınca Addressables handle'ı da serbest kalıyor.
- Kazanma ve kaybetme kararı `GameplayManager`'da. Kalan blok sayısını spawner tutmuyor; spawner'ın işi sadece oluşturmak. Yönetici sayıyı level datasından alıyor ve her `BlockExitedEvent`'te düşürüyor; süre bitince de level durumuna bakıyor. Kalan blok yoksa level kazanılıyor, varsa Out of Time popup'ı açılıyor.
- Bir popup açıkken son blok çıkarsa level popup kapanıp oyun devam ettiğinde kazanılıyor. Level Complete popup'ı kısa bir gecikmeyle açılıyor (`GameplayConfig`).
- Süre sayacı (`LevelTimer`) `Update` yerine UniTask ile saniyede bir ilerliyor; her karede çalışan bir kod yok. Duraklatma hem Settings'ten hem Out of Time'dan geldiği için sayaç kendi küçük sınıfında (başlat, duraklat, devam, süre ekle).
- Süre oyuncu hamle yapmadan başlamıyor: ekranda UI dışında herhangi bir yere ilk dokunuşta başlıyor. Dokunuşu input sistemi `ScreenTouchedEvent` ile duyuruyor; yönetici ilk dokunuşta süreyi başlatıp event'i dinlemeyi bırakıyor. Buton dokunuşları ve üstteki Level/Time barı ile coin barına dokunmak süreyi başlatmıyor.
- Süre başlamadan level'dan çıkmak can kaybettirmiyor: Settings → Home doğrudan ana menüye gidiyor. Retry ve booster butonları da süre başlayana kadar soluk ve pasif; tıklanamayan butonlarda basma animasyonu oynamıyor. Referansta da bu butonlar süre panelinde tutuluyor.
- Level süresinin yarısı geçince süre rakamları her saniye büyüyüp küçülüyor; son 10 saniyede süre kutusunun arkası kırmızı yanıp sönüyor. Büyüme sabit bir saniyede başlasaydı uzun ve kısa level'larda aynı hissettirmezdi; oranı (`0.5`) ve 10 saniyelik eşik `HudConfig`'te. Referanstaki süre panelinde de ayrı bir uyarı arka planı var. Ölçek, süreler ve soluk butonların saydamlığı da `HudConfig`'te.
- Bir popup açılınca sayaç duruyor ve sürükleme kilitleniyor. Kilit `InputLockChangedEvent` ile veriliyor; `DragController` level sistemini tanımıyor.
- Popup'lar akışı bilmiyor: yönetici `IPopupService` ile popup'ı açıyor, `Setup` ile içeriğini ve hangi butonun ne yapacağını veriyor; popup sadece gösteriyor ve tıklananı geri bildiriyor. Popup'ları kapatan da yönetici; örneğin coin yetmiyorsa Continue'ya basınca hiçbir şey olmuyor, popup açık kalıyor. Böylece aynı can kaybı popup'ı Restart'ta (X iptal eder), Settings → Home'da (X Settings'e geri döner) ve süre bitip vazgeçildiğinde (X de can kaybettirip ana menüye döner) farklı davranabiliyor.
- Level Complete'te Continue ödülü, X2 iki katını verip ana menüye dönüyor. Reklam butonları şimdilik reklamsız doğrudan ödül veriyor; ödül değerleri (40 coin, X2) `GameplayConfig`'te.
- Süre bitince devam etmek (Play On) kademeli: 900 coin/+20 sn, 1900 coin/+20 sn, 2900 coin/+20 sn; 3. kademeden sonra fiyat son kademede kalıyor. Referansta da devam sayısı tutuluyor ve fiyat ile eklenen süre bu sayıya göre değişiyor, ama değerler sunucudan geldiği için bilinmiyor; bu yüzden kademeler `PlayOnConfig`'te listeleniyor ve tasarımcı değiştirebiliyor. Reklamla devam etmek de bir kademe ilerletiyor.
- Coin yetmiyorsa Out of Time'daki fiyat kırmızı görünüyor.
- Can kaybettiren seçimlerde (Retry, Home, süre bitince vazgeçmek) bir can düşüyor. Can kalmadıysa Retry ana menüye dönüyor.

## Level yükleme

- Mevcut level'ın datası Addressables üzerinden, oyuncu verisindeki level'a göre yükleniyor.
- Level'lar `Level_{n}` gibi bir adres kuralıyla da bulunabilirdi. Sırayı Inspector'dan değiştirebilmek için sıralı bir `LevelCatalog` (Addressable referans listesi) tercih ettim.
- Katalog da Addressables üzerinden yüklenebilirdi. Ama sadece referans tutan küçük bir asset olduğu için `LevelLoader` onu doğrudan tutuyor; level'ların kendisi Addressable.
- Oyuncu verisi PlayerPrefs'te de tutulabilirdi. JSON olarak kaydediyorum.
- Oyuncu verisinin arayüzü `IPlayerDataStore` idi. Coin ve can ayrı arayüzlere (`ICoinWallet`, `ILives`) çıkınca içinde sadece level ilerlemesi kaldı; adı da yaptığı işi söylesin diye `IPlayerLevelProgress` oldu.
- Coin ve can kurallarını da başta kayıt sınıfı (`PlayerDataStore`) uyguluyordu; kayıt, coin ve can tek sınıftaydı. Her biri kendi sorumluluğuna ayrıldı: `PlayerDataStore` sadece `player.json`'u okuyup yazıyor ve level ilerlemesini tutuyor, `WalletController` (`ICoinWallet`) coin'i, `LivesController` (`ILives`) canı yönetiyor. Veri hâlâ tek dosyada; controller'lar store'a prefab'da referansla bağlı.
- Kayıt dosyası okunurken önce config'teki varsayılanlarla (başlangıç coin'i, tam can) bir veri oluşturuluyor, dosya üzerine yazılıyor. Böylece dosyada olmayan yeni alanlar varsayılan değerini alıyor, eski kayıtlar bozulmuyor.
- Can, son kayıttan bu yana geçen gerçek zamana göre doluyor; oyun kapalıyken geçen süre de sayılıyor. Dolum ve can kaybetme kuralları `LivesController`'da zamanı parametre alan saf fonksiyonlar, testleri var. En fazla can, dolum süresi ve başlangıç coin'i `PlayerConfig`'te.
- Coin göstergesi birden fazla yerde (HUD, Out of Time) duruyor ve cüzdan onları tanımamalı; coin değişince `CoinsChangedEvent` atılıyor, göstergeler açılınca değeri okuyup sonra event'i dinliyor. 1000 ve üstü "1,32k" gibi kısaltılıyor. Can göstergesi zaten dolum süresini saniyede bir güncellediği için değeri kendisi okuyor.
- Dosyaya JSON okuma/yazma oyundan bağımsız bir yardımcı olduğu için `Core/Utilities/JsonFileUtility`'de.
- Ayarlar (titreşim, ses, müzik) oyuncu ilerlemesinden ayrı `settings.json`'da, `IPlayerSettings` üzerinden; ayar adıyla okunup yazılıyor (`IsOn`, `Set`), böylece popup'ı açan taraf bağlantıyı tek satırda kuruyor. Şimdilik sadece kaydediliyor; ses ve titreşim eklendiğinde buradan okunacak.
- Oyuncunun level sayısı hep artar. Son level'dan sonra level datası başa döner ama gösterilen level numarası sıfırlanmaz.

## Grid ve bloklar

- Level verisinde blok konumlarını dünya koordinatı olarak da tutabilirdim. Grid hücresi (`Vector2Int`) olarak tutuyorum; level'ı tasarlayan kişi "3. sütun, 2. satır" diye düşünüyor ve level editörü de bu sayede basit kalacak.
- Blok şekillerini kod içinde bir enum ve tabloyla da tanımlayabilirdim. Her şekil bir ScriptableObject (`BlockShapeData`, hücre listesi); yeni bir şekil kod değişikliği olmadan Inspector'dan eklenebiliyor. Renkler de aynı şekilde (`BlockColorData`).
- Blok hareketini grid doluluğuna bakan saf bir C# mantığıyla da çözebilirdim. Physics2D'yi tercih ettim: çarpışmayı ve engelin kenarı boyunca kaymayı fizik motoru çözüyor. Blok durağanken Kinematic, sürüklenirken Dynamic; hızı parmağa doğru ve `MoveSpeed` ile sınırlı. Diğer bloklar Kinematic kaldığı için sürüklenen blok onları itemiyor. Bırakınca en yakın hücreye kısa bir tween ile kayarak oturuyor; süre ve easing `BlockConfig`'te.
- Seçilen bloğun beyaz outline'ı tek parça görünmeli. Blok çeyrek parçalardan oluştuğu için her parçaya ayrı outline çizmek parçaların birleşim yerlerinde çizgi bırakırdı. Önce bloğun bütün parçaları stencil'e maske yazıyor, sonra şişirilmiş kopya sadece maskenin dışına çiziliyor; böylece outline bloğun tamamının çevresinde tek parça oluyor. Renk ve kalınlık `BlockConfig`'te.
- Ayarlanabilir değerler (hız, snap süresi, dokunma yarıçapı, kamera açısı vb.) prefab'larda değil, sistemin config ScriptableObject'inde tutuluyor; tasarımcı hepsini tek yerden değiştirebiliyor.
- Hücre collider'ları köşeleri yuvarlatılmış; blok bir engele takılınca kenar boyunca kayabiliyor.
- Fizik materyali (`BlockPhysics`) ilk başta tamamen sürtünmesizdi; bloklar çok kaygandı, birbirine değmeleri hissedilmiyordu. Sürtünmeyi 0.2'ye çıkardım; böylece temas hissediliyor. Sürükleme hızını (`BlockConfig.MoveSpeed`) da 20'den 12'ye indirmeyi denedim, ama hareket yavaş kaldı ve 20'ye geri döndü. İkisi de editörden ayarlanabiliyor.
- Blok, duvar, kapı ve engel collider'larının hepsi hücre çizgisinden aynı küçük mesafe kadar içeride. Bloklar duvarlardan daha fazla içeride olsaydı, bir bloğa bastıran blok kendi hücresinden taşar, yanından geçtiği engelin köşesine takılırdı. Tamamen hücre boyunda olsalardı da 1 hücrelik dar geçitlerde sıkışırlardı.
- Physics2D sadece XY düzleminde çalıştığı için tahta XY düzleminde kuruluyor ve kamera +Z yönüne bakıyor.
- Grid'in arayüzü iki parçaya ayrılmış: `IGridBuilder` sadece grid'i kuruyor ve onu sadece `LevelCreator` kullanıyor; `IGrid` sadece sorgu (alan, hücre↔dünya dönüşümü, bir kenarın dünya pozisyonu). Böylece blok, kamera veya spawner'lar grid'i yeniden kuramıyor. Hücre boyutu ve duvar kalınlığı dışarı açılmıyor; kapının duvar hizasındaki pozisyonunu grid'in kendisi veriyor (`EdgeToWorld`). İkisini de aynı `GridController` uyguluyor ve iki arayüzle bind ediyor.
- Dokunma için Lean Touch kullanıyorum; dokunma ve mouse farkını kendisi soyutluyor.
- Sürüklemeyi doğrudan blok sınıfı üzerinden yapabilirdim. Oyuna ileride blok dışında sürüklenebilecek objeler de eklenebileceği için sürükleme `IDraggable` arayüzü (`BeginDrag`, `DragTo`, `EndDrag`) üzerinden yapılıyor; parmağı takip eden `DragController` blokları değil, sadece bu arayüzü tanıyor. Arayüzün adı ilk başta `ISelectable`'dı; seçmekten fazlasını yaptığı ve "Move" de bloğun nasıl hareket ettiğini (fizik, snap) anlatacağı için etkileşimi anlatan "Drag" adını seçtim.
- Hücre collider'ları hücreden biraz küçük ve köşeleri yuvarlatılmış olduğu için aynı bloğun hücreleri arasında küçük boşluklar kalıyor. Dokunma tek bir noktaya değil, dokunulan noktanın çevresindeki küçük bir daireye bakıyor ve dokunulan noktaya en yakın sürüklenebilir objeyi alıyor; yarıçap `DragConfig`'te.
- Kamerayı sabit bir pozisyona da koyabilirdim. Level'ların boyutu farklı olduğu için kamera level hazır olunca sabit açıyla tahtaya göre konumlanıp tahtayı ekrana sığdırıyor; açı, FOV ve kenar boşluğu `CameraConfig`'te. Tahta ekranın tamamına değil, UI'daki `BoardArea` alanına sığdırılıyor; HUD ve booster barıyla hiçbir ekran oranında çakışmıyor. Kamera tahtayı bu alanın ortasına gelecek şekilde kaydırıyor.
- Grid içindeki engelleri tahtanın bir parçası olarak (grid'in collider'ları gibi) da tutabilirdim. İleride bir booster ile tek tek ortadan kaldırılabilecekleri için her engel hücresi kendi collider'ı olan ayrı bir obje.
- Tahta kare olmak zorunda değil; level verisindeki boş hücreler (`Holes`) tahtadan çıkarılıyor ve tahtanın tamamı, boşlukların çevresi de dahil, duvarla çevriliyor.
- Çıkışlar level verisinde hücre, yön, uzunluk ve renk olarak tutuluyor; duvarla aynı hizada duruyorlar. Bir blok, çıkışa sığmıyorsa o çıkıştan çıkamayacak. Kapı her hücre için ayrı parça olarak diziliyor, çıkış yönünü gösteren ok ise çıkış başına bir tane ve çıkışın ortasında.
- Kapı modelinin kendi materyali emission açık geliyordu; renk emission'a da verildiği için kapılar bloklardan çok daha parlak görünüyordu. Kapı artık blokla aynı materyali (`ModularBlock_Base`) kullanıyor ve rengi aynı şekilde, sadece ana renk olarak alıyor; aynı renkteki kapı ve blok aynı tonda görünüyor.
- Bir blok, sürüklenirken kendi rengindeki kapıya değince ve kapıya dik genişliği kapının aralığına sığıyorsa kapıdan kayarak çıkıyor. Kontrol kapıda, sadece temas varken çalışıyor; `Enter` yerine `Stay` kullandım ki blok kapıya hizasız değip sonra kayarak hizalanınca da çıkabilsin. Temas eden blok her fizik adımında aranmıyor; temas başlarken (`OnCollisionEnter2D`) bir kez bulunup temas bitene kadar tutuluyor.
- Kapı ilk başta görsel bir `DoorView` ve çıkış hesabını yapan ayrı bir `ExitView` olarak bölünmüştü; kapı çarpışmayı alıp hesabı `ExitView`'a yaptırıyordu. Oyunu bu kadar etkileyen bir sınıfa "View" demek yanıltıcıydı ve ara sınıfın bir görevi yoktu. Artık tek bir `Door` (Controllers) var: rengini alıyor, teması tutuyor, sığmayı kontrol edip bloğu çıkarıyor. Oyuna etki eden MonoBehaviour'lar `View` ekiyle değil, kendi adıyla Controllers'ta duruyor. Sığma kuralı (`ExitFit`) hücre aralıklarıyla çalışan saf bir fonksiyon, test edilebiliyor.
- Renk ve genişlik yetmiyor; bloğun kapıya giden yolu da boş olmalı. L gibi girintili şekillerde arka hücreler çıkarken girintiden geçiyor; orada başka bir blok veya engel varsa blok çıkmıyor, o engel çekilince aynı temasta çıkıyor. Hangi hücrelerin boş olması gerektiği saf bir fonksiyonla (`BlockShapeMath.ExitPath`) hesaplanıyor, doluluk fizikten soruluyor. Tahtanın ayrı bir doluluk haritası tutulmuyor; harita, her snap ve çıkışta fizikle eşit tutulması gereken ikinci bir kaynak olurdu.
- Kapı bloğu tanımak için BlockSystem'deki `IBlock` arayüzünü kullanıyor (renk, kapladığı hücreler, çıkış). Çıkış sistemi blok sistemini tanıyor, tersi değil; blok hangi kapıdan çıktığını bilmiyor, sadece verilen yöne doğru çıkıp `BlockExitedEvent` atıyor.
- Blok kapıya hizasız da değebiliyor (yarım hücreye kadar tolerans var). Doğrudan kapıya doğru hareket etseydi yolu çapraz olur, bir kısmı kapıdan değil duvardan geçiyormuş gibi görünürdü. Bu yüzden çıkış iki aşamalı: önce blok hücresine snap'leniyor, sonra kapıdan düz geçiyor.
- Blok kapıdan geçerken kapı tarafından öğütülüyormuş gibi görünüyor: blok sabit hızla kapıya doğru ilerliyor, kapı çizgisini geçen her parça o anda gizleniyor ve yerinde bloğun renginde bir parçalanma efekti çıkıyor; parçalar kapıya en yakından başlayarak sırayla kayboluyor. Sıra zamanlamayla değil gerçek pozisyonla belirlendiği için her şekil ve her yönde doğru çalışıyor.
- Parçalanma efekti her parça için yeni bir obje oluşturmuyor; Gameplay sahnesinde level başına tek bir particle system var (`BlockSpawner`'a sahnede referans olarak veriliyor, servis prefab'ında sadece servisler duruyor) ve parçalar ona `Emit` ile, kendi pozisyon ve renklerini vererek parçacık bırakıyor. Hız, easing ve parça başına parçacık sayısı `BlockConfig`'te.
- Duvar, grid kurulurken çıkışların kapladığı kenarlar boş bırakılarak diziliyor; böylece kapı ile duvar aynı yeri hiçbir zaman paylaşmıyor. Çıkışın hangi kenarları kapladığını çıkış verisi kendisi hesaplıyor, grid çıkış sistemini tanımıyor, sadece boş bırakacağı kenarları alıyor.
- Collider'lar kodla oluşturulmuyor; her duvar parçası, kapı ve engel köşesi collider'ını kendi prefab'ında taşıyor. Kapının yerinde duvar olmadığı için kapı da collider taşıyor ve çıkışı şimdilik kapatıyor; çıkış mantığı gelince sığan blok için açılacak.
- Duvar köşelerinde düz köşede tek parça L modeli kullanılıyor. L'nin kollarından biri bir kapıya denk geliyorsa veya köşe tahtanın içe dönük bir köşesiyse tek kareli köşe parçası kullanılıyor.
- Hücre boyutu 1. Modeller farklı ölçek ve yönlerde geldiği için ölçek ve açı düzeltmeleri kodda değil, her prefab'ın içindeki görsel objede yapılıyor.
- Blok görünümünü her şekil için ayrı bir modelle de kurabilirdim. Hücreleri dört çeyrek parçadan oluşturuyorum ve her parçanın düz, kenar, dış köşe veya iç köşe olacağı komşu hücrelere bakılarak seçiliyor. Böylece yeni bir şekil için model gerekmiyor.
- Bu parçaları her blok oluşturulurken runtime'da dizebilirdim. Bunun yerine blok prefab'ı şekil oluşturulurken bir kere üretiliyor (`Block_<İsim>`, temel `Block` prefab'ının variant'ı; hücre collider'ları ve parçalar nested prefab). Runtime'da blok bu prefab'dan oluşuyor ve dışarıdan sadece renk ve konum veriliyor; döndürülmüş yerleşimlerde prefab döndürülüyor. Kod sadeleşiyor.
- Yeni şekiller `ColorBlock/Shape Creator` penceresinden oluşturuluyor: isim giriliyor, 5×5 gridde hücrelere tıklanıyor, altında bloğun nasıl görüneceği canlı önizleniyor; Create hem şekil asset'ini (`BlockShape_<İsim>`) hem prefab'ı üretip birbirine bağlıyor. Aynı isimde şekil veya prefab varsa uyarıp oluşturmuyor. Şekli inspector'dan düzenleyip prefab'ını güncellemek yerine bu yolu seçtim: bir şekil asset'i kopyalanınca prefab referansı da kopyalanıyor ve güncelleme başka bir şeklin prefab'ını ezebiliyordu. Araç sadece yeni şekil oluşturuyor; bir şekli değiştirmek gerekirse yenisi oluşturuluyor. Şekil asset'i seçilince inspector'ın Preview panelinde bağlı prefab yukarıdan önizleniyor.
- Prefab'ın pivot'u gridin sol alt hücresi ve gridde "P" ile işaretli. Create, şekli önce sol alta kaydırıyor (en küçük x ve y 0 olacak şekilde); şekil gridin neresine çizilirse çizilsin pivot şeklin köşesinde oluyor.
- Shape Creator'ın kullandığı şablonlar (blok ve hücre prefab'ları, dört parça prefab'ı, outline materyalleri) ve parça yerleşim hesabı (`BlockPartLayout`) sadece editörde gerekiyor. Runtime `BlockConfig`'te dursalardı spawner üzerinden build'e de girerlerdi; editör assembly'sindeki `BlockAuthoringConfig`'te duruyorlar.
- Prefab'ı olmayan bir şekil spawn edilmeye çalışılırsa hata fırlatılıyor; sessizce atlansaydı level'daki blok sayısı tutmaz, level hiç kazanılamazdı.
- Bloğun ihtiyaç duyduğu referanslar (Rigidbody, renderer listesi, outline'lı ve outline'sız materyal dizileri) runtime'da aranıp oluşturulmuyor; prefab üretilirken prefab'a yazılıyor. Seçimde bütün parçalar aynı iki diziden birini alıyor. Outline materyalleri değişirse prefab'ların tekrar üretilmesi gerekiyor; renk ve kalınlık ise `BlockConfig`'ten runtime'da geliyor.
- Genel kural: runtime'da `GetComponent`, `Camera.main` veya `Find` ile referans aranmıyor; gereken referanslar prefab'da ya da sahnede serialized olarak veriliyor (kapının renderer'ları `Door`'da, kameralar `CameraFitter` ve `DragController`'da). Tek istisna fizik: fizik sadece bir collider döndürdüğü için obje, çarpışmanın Rigidbody'si üzerinden bulunuyor (`TryGetComponent`). Bu sadece parmak ilk dokunduğunda (sürüklenecek obje) ve blok kapıya ilk değdiğinde (çıkacak blok) çalışıyor; her karede değil.

## Arayüz

- Ekranlar ve popup'lar verilen referans görsellere göre: oyun içi HUD (level, süre, coin, Restart, Pause), görsel booster barı, Settings, Out of Time, can kaybı ve Level Complete. Konum ve boyutlar referans ekran görüntülerinden ölçüldü; render alınıp referansla üst üste bindirilerek karşılaştırıldı.
- Canvas referans çözünürlüğü 1080×2400 ve `Expand` modunda: uzun ekranlarda (ince fold telefonlar dahil) genişliğe, kısa ekranlarda yüksekliğe göre ölçekleniyor; UI hiçbir oranda ekrandan taşmıyor. HUD üst köşelere, booster barı alta, popup'lar ortaya sabit.
- HUD, tahta alanı ve popup'lar cihazın safe area'sına (`SafeAreaView`) yerleşiyor; çentikli ekranlarda üst bar ve Out of Time'daki can/coin barları çentiğin altına inmiyor. Karartma katmanı safe area dışında kalıyor ki ekranın tamamını kaplasın.
- Kitte olmayan birkaç görsel mevcut görsellerden üretildi: kırık kalp (`Heart`'tan), sarı Watch Ad butonu (yeşil butonun boyanmışı), Level Complete ışınları ve oyun arka planı degradesi.
- Booster barı şimdilik sadece görsel; butonlar basılınca tepki veriyor, işlevleri yok.
- `GameplayManager` her popup'a ayrı referans tutuyordu. Popup'ları yöneten ayrı bir `PopupSystem` kurdum: yönetici sadece `IPopupService` tanıyor (`Show<T>()`, `Close()`, `CloseAll()`), popup'lara referans tutmuyor.
- Popup'lar yığın (stack) şeklinde açılıyor: yeni popup açılınca alttaki kapanıyor, arkada görünmüyor; `Close()` en üsttekini kapatıp alttakini tekrar açıyor. Örneğin Settings → Home'da Settings kapanıp can kaybı popup'ı açılıyor, X ile Settings geri geliyor. Alttaki popup'ın içeriği korunuyor, yeniden kurulmuyor.
- Her popup kendi karartmasını taşısaydı popup'lar arasında geçerken karartma da kapanıp açılırdı. Tek bir karartma katmanı var; yığında popup olduğu sürece açık kalıyor ve servis onu hep en üstteki popup'ın arkasına taşıyor.
- Popup'lar sahnede hazır durmuyor; servisin prefab listesinden ilk açıldıklarında oluşturulup saklanıyor. Servis diğer servisler gibi `GameplayServiceLocator`'da; popup'ların açılacağı kök ve karartma sahnedeki canvas'tan referans olarak veriliyor.
- Popup'lar ortak bir `BasePopupView` tabanından türüyor (abstract taban sınıflar `BaseService` gibi `Base` önekiyle adlandırılıyor); açılış ve kapanış DOTween ile. Süreler, ölçek, ease ve karartma oranı `PopupConfig`'te; toggle ve buton basma efekti `UIConfig`'te.
- Settings popup'ı ana menüde de kullanılacağı için `UI/Common`'da ve kendi prefab'ında; Gameplay'deki popup servisinin listesinde. Legal Terms, Restore Purchases, Support ve Language şimdilik bir şey yapmıyor.
- Oyun içi HUD ve popup'lar `GameplaySystem` assembly'sinde, akışı yöneten `GameplayManager` ile birlikte; hepsi `internal`. HUD popup değil, yöneticiye prefab'da serialized referansla bağlı.
- Arayüz görselleri sprite olarak import ediliyor; panel, buton ve tutucu görsellerinde 9-slice kenarları var, bu yüzden aynı görsel farklı boyutlarda bozulmadan kullanılabiliyor. Büyük ikonlar 256 piksele indirildi. Kit ve ikonlar tek bir Sprite Atlas'ta toplanıyor; kaynak dokular sıkıştırmasız, sıkıştırmayı atlas yapıyor.
- Font olarak Lilita One kullanılıyor (OFL lisanslı, ücretsiz); TextMesh Pro'nun varsayılan fontu. Yazılardaki kenar çizgisi ve gölge font materyal preset'lerinden geliyor.


### Ana menü

- Ana menü Gameplay ile aynı desende: akışı `MainMenuManager` yönetiyor (Level butonu, Settings, sekme seçimi), view'lar sadece gösteriyor ve tıklamayı bildiriyor. Main sahnesinin kendi servis prefab'ı (`MainServiceLocator`) ve popup servisi var.
- Üst bar: profil kutusu, can (sayı ve dolum süresi), coin ve Settings. Can ve coin göstergeleri Gameplay'dekilerle aynı bileşenler (`LivesView`, `CoinsView`).
- Settings, Gameplay'deki popup'ın aynısı; Home verilmediğinde Home butonu gizleniyor.
- Ortada sadece oyuncunun mevcut level'ını gösteren "Level N" butonu var; level yolu, event butonları ve ilerleme barı kapsam dışı.
- Alt barda 5 sekme var. Kilitli sekme tıklanamıyor, sadece ikon; Home dışındaki diğerleri şimdilik boş bir ekran açıyor. Sekme adları sekmenin genişliğine sığmazsa otomatik küçülüyor. Seçilen sekmenin ikonu büyüyüp yukarı kalkıyor, adı görünüyor ve vurgu arka planı o sekmeye kayıyor; ölçek, yükseklik ve süre `MainMenuConfig`'te.
- Can eksikken üst bardaki can barına basınca "Refill Your Lives" popup'ı açılıyor; can 0 iken Level butonu da oyunu açmak yerine aynı popup'ı açıyor. Popup kalan canı ve sonraki canın süresini gösteriyor. "GET +1" (şimdilik reklamsız) bir can ekliyor, "Refill" coin karşılığında canı dolduruyor; coin yetmiyorsa fiyat kırmızı. Fiyat ve reklam ödülü `MainMenuConfig`'te. Can ekleme ve doldurma kuralları `LivesController`'da (`Add`, `Fill`), en fazla can sınırını aşmıyor.
- Kitte olmayan pembe başlık plakası mavi başlıktan boyanarak, reklam butonundaki klaket ikonu çizilerek üretildi.
- Arka plan görseli bütün sekmelerde sabit ve ekranı en-boy oranını bozmadan dolduruyor.

## Editör araçları

### Shape Creator (`ColorBlock/Shape Creator`)

- **Şekil:** İsim gir, 5×5 gridde hücrelere sol tıkla ekle/çıkar; sağ tık hücreyi siler. "P" hücresi prefab'ın pivot'u. Altta blok canlı önizleniyor. **Create Shape**, `BlockShape_<İsim>` asset'ini ve `Block_<İsim>` prefab'ını oluşturur.
- **Renk:** İsim gir, rengi seç; **Create Color**, `BlockColor_<İsim>` asset'ini oluşturur.
- Oluşturulan şekil ve renkler Level Editor paletine kendiliğinden eklenir, silinenler paletten çıkar.

### Level Editor (`ColorBlock/Level Editor`)

- **Açma:** Level Number'a numarayı yaz, **Load**.
- **Ayarlar:** Grid Size (en fazla 12×12) ve Time (saniye). Grid küçülünce dışarıda kalanlar silinir.
- **Paletler:** Shapes, Colors ve Tiles (Door, Wall, Empty, Cell). Seçili kutucuk beyaz çerçeveli.
  - **Shape:** İmleçteki önizleme sığıyorsa tıklayınca blok yerleşir; kırmızıysa çakışıyor. **R** şekli döndürür (pencere odaktayken), açı Shapes başlığında yazar. Blok son seçilen rengi alır.
  - **Color:** Bloğa ya da kapıya tıklayınca rengini değiştirir.
  - **Door:** Tahtanın dış kenarına veya bir boşluğun kenarına tıklayınca 1 birimlik kapı ekler. Aynı kenarda yan yana duran aynı renkli kapılar tek kapı olur.
  - **Wall** engel koyar/kaldırır, **Empty** hücreyi boşluk yapar, **Cell** boşluğu geri açar.
- **Sağ tık** altındakini siler. **Ctrl+Z** geri alır.
- **Uyarılar:** Kapısı olmayan blok rengi, bloğu olmayan kapı rengi ve kendi kapısının önünde başlayan blok. Kaydetmeyi engellemez.
- **Kaydetme:** Save Level Number'a numarayı yaz, **Save**. Var olan level'ın üzerine yazar; son level'ın bir fazlası yeni level oluşturup kataloğa ve Addressables "Levels" grubuna ekler. Açılan level'dan farklı bir level'ın üzerine yazmadan önce onay sorar.

## Performans

- Hedef FPS cihaz kalitesine göre seçiliyor: bellek ve işlemci çekirdek sayısı eşiği geçen cihazlarda 60, diğerlerinde 30. Seçimi Bootstrap sahnesindeki `FrameRateController` (Core) açılışta bir kere yapıyor; eşikler ve iki FPS değeri `FrameRateConfig`'te. Seçim kuralı saf bir fonksiyon, testi var.

Level_001'de Game view istatistikleriyle ölçüldü:

| Adım | Batch | SetPass | Gölge atan | Üçgen |
|---|---|---|---|---|
| Başlangıç | 262 | 47 | 119 | 58k |
| Zemin ve duvarlar static batching + zemin gölgesi kapalı (birlikte ölçüldü) | 135 | 39 | 90 | 49k |
| Gölgeler tamamen kapalı | 68 | 16 | 0 | 25k |

- Oyun kodunda tek update döngüsü sürüklenen bloğun `FixedUpdate`'i; geri kalan her şey olayla çalışıyor (LeanTouch, EventBus, DOTween). Blok component'i sadece sürüklenirken açık, böylece duran bloklar her fizik adımında çağrılmıyor.
- Zemin ve duvarlar hiç hareket etmediği için grid kurulunca `StaticBatchingUtility.Combine` ile birleştiriliyor. Engeller ve kapılar bu birleştirmeye girmiyor; ileride kaldırılacak veya açılacaklar.
- Oyunda hiç gölge yok: URP'de gölgeler ve ışığın gölgesi kapalı. Böylece her objenin gölge için ikinci kez çizildiği pass tamamen kalkıyor.
- Rigidbody interpolation sadece sürükleme sırasında açık: fizik 50 Hz'de çalışırken sürüklenen blok ekranda akıcı görünsün diye. Bırakınca kapanıyor, çünkü snap tween'i pozisyonu kendisi yazıyor.
- Dynamic batching kapalı. Blok ve kapılar renk için MaterialPropertyBlock kullandığından zaten batch'lenemiyor; uygun küçük mesh'ler (zemin, düz duvar) static batching ile birleşiyor. Açmak her karede CPU'da vertex dönüştürme maliyeti getirirdi.

---

Proje, yapay zekâ ile birlikte yaklaşık 12–15 saatlik çalışmayla tamamlandı.
