# Stegosaurus

[![NuGet](https://img.shields.io/nuget/vpre/Stegosaurus?label=NuGet&logo=nuget)](https://www.nuget.org/packages/Stegosaurus/)

**Stegosaurus** is a lightweight command-line tool written in C# for securely hiding encrypted messages inside PNG images using LSB (Least Significant Bit) steganography.

It combines strong AES encryption (via PBKDF2-derived keys) with image steganography, making it suitable for sending secret messages disguised inside innocuous-looking image files.


## 🔐 Features

- **AES-256 encryption** with a password-derived key (PBKDF2 + SHA256)
- **LSB steganography** using R, G, B channels (alpha is untouched)
- **PRNG-controlled encoding** Pseudo-random number generator (PRNG) for added entropy based on password hash
- **Graceful error handling** for corrupt, small, or invalid image files
- **PNG-only** for now — JPG support would require DCT-based encoding


## 📦 Installation

Stegosaurus is currently in **beta** because it depends on the beta version of the `System.CommandLine` package.

```bash
dotnet add package Stegosaurus --version 1.0.3-beta
```

Or manually add it to your .csproj file:

```xml
<PackageReference Include="Stegosaurus" Version="1.0.3-beta" />
```


## 🚀 Usage

### Build
```bash
dotnet build
```

### Encrypt & Encode
```bash
dotnet run -- encrypt -f image.png -m "Secret message here" -o hidden_output.png
```

Or enter your password at the command line (**not recommended**, but convenient for scripting/testing):
```bash
dotnet run -- encrypt -f image.png -p yourpassword -m "Secret message" -o hidden_output.png
```

### Decode & Decrypt
```bash
dotnet run -- decrypt -f hidden_output.png -o secret.txt
```


## 📂 CLI Flags

| Flag | Description | Required |
|------|-------------|----------|
| `-f`, `--file` | Path to input PNG file | ✅ |
| `-m`, `--message` | Message to hide (encrypt only) | ✅ for encrypt |
| `-p`, `--password` | Password for encryption/decryption | ❌ |
| `-o`, `--outfile` | Optional output path for result | ❌ |


## ⚠️ Notes

- Only `.png` files are supported. JPEGs use lossy compression, which discards subtle data like LSBs. To hide data in `.jpg` files, you'd need to use DCT-based steganography (Discrete Cosine Transform), which this tool does not support.
- During encoding, the alpha (transparency) channel is ignored to avoid visual artifacts. Only the RGB channels are modified, as changes to alpha values are more likely to cause noticeable distortions.
- Image must be large enough to store both prefix and encrypted payload, or an error will be thrown and the program will exit gracefully.
- Corrupt PNGs or tampered files will throw appropriate errors and exit gracefully.
- If you don’t provide a password via the command line, the program will prompt you securely. This is recommended to avoid exposing the password in terminal history or logs.
- If you don’t specify an --outfile, one will be auto-generated based on the input file’s name with a timestamp. For decryption/decoding, the output will default to a .txt file.


## 🧪 Testing

Unit tests live in `Stegosaurus.Tests/`. To run them:

```bash
dotnet test
```


## 📦 Packaging (Coming Soon)

Plans to publish this as a .NET tool (`dotnet tool install`) are in progress, including CI/CD via GitHub Actions.


## 🛠 Tech Stack

- C# (.NET 8)
- [ImageSharp](https://github.com/SixLabors/ImageSharp) for pixel-level image access
- `System.Security.Cryptography` for AES + PBKDF2


## ✍️ Author

Made by Michael Pearce  
🌐 [michaelpearce.tech](https://michaelpearce.tech) • 🐙 [GitHub](https://github.com/FirstFlush)


## 📜 License

[MIT License](LICENSE)
