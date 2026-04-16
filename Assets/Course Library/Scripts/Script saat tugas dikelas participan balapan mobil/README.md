# 🎮 Unity Racing Game — Panduan Setup Scene
## Struktur Script

```
Scripts/
├── Core/
│   ├── GameManager.cs       ← Singleton utama, kelola state game
│   ├── CarController.cs     ← Kontrol mobil (Rigidbody-based)
│   ├── CountdownSystem.cs   ← Hitung mundur 3,2,1,GO!
│   ├── FinishLine.cs        ← Trigger garis finish + efek
│   ├── CameraController.cs  ← Follow + Intro zoom + Orbit cinematic
│   ├── AudioManager.cs      ← BGM, engine SFX, win/lose sound
│   └── UIManager.cs         ← Semua panel UI (Start, HUD, Win, Lose)
└── Obstacles/
    ├── Ramp.cs              ← Ramp lompatan dengan launch force
    ├── StaticObstacle.cs    ← Rintangan diam (topple/shake/shatter)
    ├── DynamicObstacle.cs   ← Rintangan bergerak (patrol/rotate/pendulum/orbit)
    └── Checkpoint.cs        ← Sistem checkpoint (opsional)
```

---

## 🛠️ Cara Setup di Unity

### 1. Buat GameObject `_GameManager`
- Tambahkan: `GameManager`, `UIManager`, `AudioManager`, `CountdownSystem`
- Hubungkan semua referensi via Inspector

### 2. Mobil (Player)
```
GameObject: Car
├── Tag: Player
├── Layer: Car
├── Components:
│   ├── Rigidbody (mass=800, drag=0.5, angularDrag=1)
│   ├── MeshCollider / BoxCollider
│   ├── CarController
│   └── AudioSource (untuk engine sound)
└── Children:
    └── GroundCheck (4 titik Empty di sudut bawah mobil)
```

### 3. Kamera
```
GameObject: Main Camera
├── Component: CameraController
└── Target: drag Car transform ke Inspector
```

### 4. Garis Finish
```
GameObject: FinishLine
├── BoxCollider (isTrigger = true, ukuran sepanjang jalur)
├── Component: FinishLine
└── Children:
    ├── FinishGate (mesh gerbang + glow material dengan emission)
    ├── Flag (Animator dengan trigger "Wave")
    ├── ConfettiParticle (Particle System)
    └── GlowBurst (Particle System)
```

### 5. Ramp
```
GameObject: Ramp
├── MeshCollider (isTrigger = false, untuk collider fisika)
├── BoxCollider (isTrigger = true, di area atas ramp)
├── Component: Ramp
│   ├── Launch Force: 800
│   ├── Launch Angle Deg: 25-35
│   └── Min Speed To Launch: 3
└── Children:
    ├── DustParticle (Particle System)
    └── JumpTrail (Trail Renderer)
```

### 6. Rintangan Statis
```
GameObject: Rock / Barrel / Wall
├── MeshCollider / BoxCollider
├── Rigidbody (opsional, untuk Topple)
└── Component: StaticObstacle
    ├── Reaction: Topple (batu/drum) / Shake (tembok) / Shatter
    └── DebrisParticle: particle debu/serpihan
```

### 7. Rintangan Dinamis
```
GameObject: MovingBarrel / SwingPendulum / RotatingBlade
├── Collider
├── Component: DynamicObstacle
│   ├── Pattern: Patrol / Rotate / Pendulum / Orbit
│   ├── Patrol: patrolOffset=(5,0,0), speed=3
│   ├── Rotate: axis=(0,1,0), speed=90
│   ├── Pendulum: angle=45, speed=2
│   └── Orbit: center=transform, radius=4, speed=60
└── HitParticle: Particle System
```

---

## 🎵 Audio Setup

### AudioManager Inspector:
| Field | Clip yang Dibutuhkan |
|-------|---------------------|
| bgmClip | Loop musik background (WAV/OGG) |
| engineIdleClip | Suara mesin idle |
| engineRevClip | Suara mesin rev tinggi |
| winClip | Fanfare menang |
| loseClip | Suara kalah/gagal |
| crashClip | Suara tabrakan |
| jumpClip | Suara melompat |
| landClip | Suara mendarat |

### CountdownSystem Inspector:
| Field | Clip |
|-------|------|
| beepClip | Suara beep tiap angka |
| goClip | Suara "GO!" |

---

## 🎨 Material Glow (Shader Graph / Built-in)

Untuk efek glow di garis finish:
1. Buat Material baru
2. Shader: `Standard` → centang `Emission`
3. Emission Color: Kuning/Oranye
4. Di FinishLine.cs, `glowRenderers` akan auto-update warna emission

---

## 🏁 Urutan State Game

```
IDLE (mobil diam, UI Start tampil)
    ↓ [tombol START]
COUNTDOWN (3...2...1...GO!)
    ↓ [GO! selesai]
PLAYING (kontrol aktif, timer jalan, BGM play)
    ↓ [mobil sentuh FinishLine]
FINISHED (timer berhenti, cinematic orbit, Win/Lose panel)
```

---

## 💡 Tips

- **Layer Ground**: Pastikan terrain/jalan ada di Layer "Ground" agar `IsGrounded()` dan `Ramp` berfungsi
- **Rigidbody Mobil**: Set `Interpolate = Interpolate` agar gerakan halus
- **Camera**: Tambahkan `Physics.IgnoreLayerCollision` antara layer mobil dan kamera jika kamera tembus objek
- **Confetti Particle**: Di Particle System, set Emission Rate = 50, Start Speed = 8, Gravity = 0.5 untuk efek bagus
