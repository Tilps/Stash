# FF Clocks - Hands of Time Solver

A modern, mobile-friendly Blazor WebAssembly solver for the **"Hands of Time"** clock puzzles found in *Final Fantasy XIII-2*.

Designed to be used on your phone while playing the game on your TV or monitor.

---

## Features

- **📱 Mobile-First Design & PWA**: Responsive circular clock interface optimized for one-handed phone use, with offline support.
- **⚡ Rapid Phone Keypad**: Enter clock numbers rapidly without fighting your mobile soft-keyboard. Entering a number advances clockwise to the next spot automatically.
- **🔄 Auto-Solve**: Solves instantly as soon as all spots are filled.
- **🎯 Step-by-Step Walkthrough**:
  - Highlights the current spot and the hands pointing clockwise and counter-clockwise to valid targets.
  - Interactive playback controls (`⏮ First`, `◀ Prev`, `▶ Auto Play`, `Next ▶`, `⏭ Last`).
  - Clear, human-readable instructions (e.g. *"Move clockwise 3 steps to 3 o'clock"*).
  - Numbered badges (`#1`, `#2`, ...) on each clock node displaying the full visit order at a glance.
- **🔢 Multiple Solutions**: Supports viewing all alternative valid solutions if more than one exists.
- **🎲 Random Clock Generator**: Generates random solvable puzzles on demand for testing without having to manually type digits.
- **⌨️ Keyboard & Paste Support**: On desktop, punch numbers using keys `1`-`9`, `Backspace`, and arrow keys, or paste a string like `2 3 1 2 1 2`.

---

## Project Structure

- **`FFClocks.Core`**: Fast, allocation-conscious bitmask solver library and domain models.
- **`FFClocks.Web`**: Blazor WebAssembly client with SVG-based interactive clock dial, PWA manifest, and service worker.
- **`FFClocks.Tests`**: xUnit test suite validating correctness, edge cases, various clock sizes, and performance benchmarks.

---

## Getting Started

### Prerequisites
- [.NET 10 SDK](https://dotnet.microsoft.com/)

### Running the Web App
```bash
dotnet run --project FFClocks.Web/FFClocks.Web.csproj
```
Open [http://localhost:5039](http://localhost:5039) (or the URL printed in the terminal).

### Running Tests
```bash
dotnet test FFClocks.slnx
```
Its basically brute force, but works pretty fast up to 20 spot clocks.