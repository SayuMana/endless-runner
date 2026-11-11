# Endless Runner

Game 2D tiga jalur. Pemain berpindah jalur untuk menghindari rintangan yang datang makin cepat. Skor dihitung dari lama bertahan.

## Main

Buka `docs/index.html` lewat link GitHub Pages.

- Keyboard: panah kiri/kanan atau A/D
- Layar sentuh: geser kiri/kanan

## Kontrol teknis

- Unity 2022.3.62f3, WebGL, Built-in Render Pipeline
- Semua visual digambar runtime lewat `Texture2D`, tidak ada file aset
- Build Brotli dengan decompression fallback, jadi jalan di GitHub Pages yang tidak bisa set header
- Ukuran build 4.3 MB

## Struktur

- `Assets/Scripts/Core` state game dan data
- `Assets/Scripts/Player` kontrol jalur dan input
- `Assets/Scripts/World` spawner, rintangan, kurva kesulitan
- `Assets/Scripts/Score` skor
- `Assets/Scripts/View` render dan kamera
- `Assets/Scripts/UI` HUD
- `Assets/Data/GameBalance.asset` semua angka yang bisa di-tuning
- `docs/` hasil build untuk GitHub Pages
