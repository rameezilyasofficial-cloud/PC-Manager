# PC Manager v1.6 - Advanced System Utility

**PC Manager** is a high-performance Windows utility engineered to give users granular control over system resources. Built with C# and WPF, it provides deep integration with the Windows OS to manage runaway processes and optimize performance, specifically tested on hardware like the **HP 280 G2 SFF**.

Developed by **Rameez Ahmad** (@Pak Prime Deals).

---

## ⚡ The Problem & Solution
* **The Problem:** Modern background applications often consume excessive RAM and network bandwidth without user consent, leading to system lag and "resource exhaustion."
* **The Solution:** PC Manager implements low-level Windows APIs to enforce strict resource limits, ensuring the system remains responsive even under heavy load.

## 🛠 Advanced Technical Features
This application demonstrates professional-grade implementations of Windows system-level programming:

* **Job Object Memory Limiter:** Utilizes **Windows Job Objects** to hard-cap the physical memory (Working Set) an application can claim, preventing memory leaks from impacting system stability.
* **Network & IO Throttling:** Implements custom logic to monitor and limit the network speed of specific processes, ensuring bandwidth is available for critical tasks.
* **Live Dashboard:** A real-time monitoring interface that identifies "Memory Hogs" using optimized LINQ queries and the `System.Diagnostics` namespace.
* **Automated Junk Cleaner:** A specialized module that safely scrubs system temporary directories to recover wasted disk space.
* **Native Registry Integration:** Handles auto-start capabilities by programmatically interacting with the Windows `CurrentVersion/Run` registry hive.

## 🚀 Installation & Download
**Ready to use?** Download the latest stable installer here:
👉 **[Download PC Manager v1.6 (Official Release)](https://github.com/rameezilyasofficial-cloud/PC-Manager/releases/tag/v1.6)**

1. Download `PCManagerSetup.exe`.
2. Run the installer (created via Inno Setup).
3. *Note: If Windows SmartScreen appears, click **More Info** > **Run Anyway**.*

## 💻 Tech Stack
* **Language:** C# 12.0
* **Framework:** .NET 10.0 (WPF)
* **Architecture:** Modular Layered Architecture (Core, Models, Utils, UI).

---
### 👨‍💻 About the Developer
I am a Junior C#/.NET Developer and a 1st-year student at Punjab College. I am passionate about system-level programming and building tools that improve the user experience on Windows.

**Contact:** [rameezilyasofficial@gmail.com](mailto:rameezilyasofficial@gmail.com)
