# PC Manager v1.5 - Advanced System Utility

**PC Manager** is a high-performance Windows utility engineered to give users granular control over system resources. Built with C# and WPF, it provides deep integration with the Windows OS to manage runaway processes and optimize performance on hardware like the **HP 280 G2 SFF**.

Developed by **Rameez Ahmad** (@Pak Prime Deals).

---

## ⚡ The Problem & Solution
* **The Problem:** Modern background applications often consume excessive RAM and network bandwidth without user consent, leading to system lag and "resource exhaustion."
* **The Solution:** PC Manager implements low-level Windows APIs to enforce strict resource limits, ensuring the system remains responsive even under heavy load.

## 🛠 Advanced Technical Features
While the app handles basic monitoring, it stands out due to these "Senior-level" implementations:

* **Job Object Memory Limiter:** Unlike standard task managers, this uses **Windows Job Objects** to hard-cap the physical memory (Working Set) an application can claim. This prevents memory leaks from crashing the system.
* **Process Throttler (CPU/IO):** Implements a custom throttling logic to reduce the priority and execution cycles of non-essential background tasks during high-intensity work.
* **Live Dashboard:** A real-time monitoring system that identifies "Memory Hogs" using optimized LINQ queries and `System.Diagnostics`.
* **Junk Cleaner:** A specialized module to scrub system temporary directories and recover wasted disk space.
* **Native Registry Integration:** Handles auto-start capabilities by directly interacting with the Windows CurrentVersion/Run registry hive.

## 🚀 Installation
1.  Navigate to the **[Releases](https://github.com/rameezilyasofficial/PC-Manager/releases)** section.
2.  Download `PCManagerSetup.exe` (built via Inno Setup).
3.  Run the installer. *Note: If Windows warns about an 'Unknown Publisher', click **More Info** > **Run Anyway**.*

## 💻 Tech Stack
* **Language:** C# 12.0
* **Framework:** .NET 10.0 (WPF)
* **Deployment:** Inno Setup Compiler
* **Architecture:** Modular design with separated Core logic, Models, and UI layers.

## 🛠 Requirements
* **OS:** Windows 10/11 (x64)
* **Permissions:** Administrator privileges (required for Job Object manipulation and Registry access).

---
### 👨‍💻 About the Developer
I am a Junior C#/.NET Developer and a 1st-year student at Punjab College. I am passionate about system-level programming and building tools that improve the user experience on Windows.

**Contact:** [rameezilyasofficial@gmail.com](mailto:rameezilyasofficial@gmail.com)
