#  İlaç Kutusu OCR & DataMatrix Okuyucu

İlaç kutularının üzerindeki yazıları (OCR) ve DataMatrix barkodlarını otomatik olarak okuyan bir görüntü işleme uygulamasıdır. Kutu üzerindeki Lot No, Üretim Tarihi, Son Kullanma Tarihi gibi bilgileri ve DataMatrix içindeki GS1 standartlı verileri (GTIN, SN, LOT, MAN, EXP) çıkarır.

## 🎯 Amaç

Bu projenin temel amacı **ilaç kutusu** ve üzerindeki **DataMatrix** barkodunu okumaktır.

- Kutu üzerinde DataMatrix varsa → DataMatrix barkodunu decode eder ve GS1 verilerini parse eder (GTIN, Seri No, Lot, Üretici, SKT).
- DataMatrix yoksa → OCR ile kutu etiketi üzerindeki bilgileri (Parti No, Üretim Tarihi, SKT) okur.
- Her iki durumda da sonuçlar yapılandırılmış entity nesneleri olarak döndürülür.

## 🏗️ Teknoloji Altyapısı

| Teknoloji | Kullanım Amacı |
|---|---|
| **.NET 8.0 (C#)** | Uygulama çatısı |
| **OpenCvSharp4** | Görüntü işleme ve DataMatrix bölgesi tespiti |
| **Tesseract OCR** | Optik karakter tanıma |
| **ZXing.Net** | DataMatrix barkod okuma |
| **SimpleSoft.Gs1Parser** | GS1 standartlı barkod verisi parse etme |
| **BenchmarkDotNet** | Performans ölçüm ve karşılaştırma |

## 📁 Proje Yapısı

```
OCR/
├── Program.cs                          # Ana giriş noktası
├── Entities/
│   ├── BoxEntity.cs                    # Kutu etiketi bilgileri (BatchNo, MfgDate, ExpDate, Price)
│   ├── DatamatrixEntity.cs             # DataMatrix bilgileri (GTIN, SN, LOT, MAN, ExpDate)
│   └── OcvResultEntity.cs              # İşlem sonuç sınıfı
├── Features/
│   ├── DatamarixFeatures/
│   │   └── DatamatrixReader.cs         # DataMatrix barkod okuma
│   ├── OCRFeatures/
│   │   ├── CharacterRecognition.cs     # Tesseract OCR entegrasyonu
│   │   └── ImageProcessing.cs          # Görsel ön-işleme
│   └── OCVFeatures/
│       └── Ocv.cs                      # Ana orkestrasyon sınıfı
├── Helpers/
│   ├── DatamatrixHelpers/
│   │   ├── DatamatrixFinder.cs         # DataMatrix bölgesi tespiti
│   │   └── Gs1Parser.cs               # GS1 veri ayrıştırma
│   ├── ImageProcessingHelpers/
│   │   └── GetDataMatrix.cs            # DataMatrix görsel yardımcıları
│   ├── OCRHelpers/
│   │   └── TesseractPathFinder.cs      # Tesseract veri dosyası yolu
│   └── OutputHelpers/
│       ├── OutputParser.cs             # Çıktı formatlama
│       └── RegexHelper.cs             # Regex desenleri (GTIN, SN, LOT, vb.)
└── ExamplePhotos/                      # Örnek ilaç kutusu görselleri
```

## 🚀 Kurulum ve Çalıştırma

### Gereksinimler

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- Tesseract eğitilmiş veri dosyası (`eng.traineddata`)

### Adımlar

```bash
# Depoyu klonlayın
git clone <repo-url>
cd OCR

# Bağımlılıkları yükleyin ve derleyin
dotnet restore
dotnet build

# Çalıştırın
dotnet run --project OCR
```

> **Not:** `Program.cs` içindeki `filePath` değişkenini okumak istediğiniz ilaç kutusu görselinin yolu ile güncelleyin.

## 📋 İş Akışı

```
Görsel Girdi (İlaç Kutusu Fotoğrafı)
        │
        ▼
   OpenCV ile Görsel Okuma
        │
        ├──► DataMatrix Bölgesi Tespiti
        │           │
        │           ▼
        │    ZXing ile DataMatrix Decode
        │           │
        │           ▼
        │    GS1 Parser ile Veri Ayrıştırma
        │
        ├──► Görsel Ön-İşleme (Threshold, Crop, vb.)
        │           │
        │           ▼
        │    Tesseract OCR ile Metin Okuma
        │           │
        │           ▼
        │    Regex ile Veri Çıkarma
        │
        ▼
  Sonuç Birleştirme (OcvResult)
```

## 🌿 Branch Bilgisi

> [!IMPORTANT]
> **`11` branch'i Windows OS** için yapılandırılmıştır. Windows üzerinde çalışıyorsanız bu branch'i kullanın.
> Ana branch (dev,main/master) macOS için yapılandırılmıştır.

## 📦 NuGet Paketleri

- `OpenCvSharp4` (4.13.0)
- `OpenCvSharp4.Extensions` (4.13.0)
- `OpenCvSharp4.runtime.osx_arm64` (4.8.1) — *macOS ARM64 için*
- `Tesseract` (4.1.1)
- `ZXing.Net.Bindings.Windows.Compatibility` (0.16.14)
- `SimpleSoft.Gs1Parser` (1.0.0)
- `BenchmarkDotNet` (0.15.8)

## 📄 Lisans

Bu proje özel kullanım içindir.
