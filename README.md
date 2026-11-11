# Endless Runner

Game 2D ada 3 lane. Player bisa pindah jalur buat menghindari obstacle yang datang. Skor dihitung dari lamanya survive.

## Main

Buka `docs/index.html` lewat link GitHub Pages.

- Keyboard: Arrow kiri/kanan atau A/D
- Mobile: swipe kiri/kanan

## Detail

- Unity 2022.3.62f3, WebGL, Built-in Render Pipeline
- Semua visual digambar runtime lewat `Texture2D`
- Build Brotli dengan decompression fallback, jadi jalan di GitHub Pages yang tidak bisa set header
- Ukuran build 4.3 MB

## Struktur

- `Assets/Scripts/Core` state game dan data
- `Assets/Scripts/Player` kontrol jalur dan input
- `Assets/Scripts/World` spawner, rintangan, kurva kesulitan
- `Assets/Scripts/Score` skor
- `Assets/Scripts/View` render & kamera
- `Assets/Scripts/UI` HUD
- `Assets/Data/GameBalance.asset` semua angka yang bisa di-tuning
- `docs/` hasil build 
