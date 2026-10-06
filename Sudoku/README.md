# Sudoku Solver & Explainer

A modern .NET 10 Blazor WebAssembly application and logic library for Sudoku puzzle solving, generation, and step-by-step human explanation.

## Key Feature: Minimized Logic Explanations

While standard Sudoku solvers either use brute-force backtracking or list every single raw pencil-mark elimination, this solver focuses on producing **human-readable English explanations of the complete deduction path** with **delta-debugging minimization**.

- **Level 0 (Singles)**: Detects Naked Singles (single candidate remaining in a cell) and Hidden Singles (candidate only valid in one cell of a row, column, or 3x3 block).
- **Lookahead & Contradiction Logic**: Branches on candidate possibilities. When an elimination or forced value is identified through contradiction, an inner delta-debugging pass selectively removes intermediate deductions to verify if the contradiction still holds. This prunes unneeded steps and keeps the written explanation concise.
- **Difficulty Scoring**: Automatically measures puzzle difficulty according to the maximum lookahead depth and tuple sizes required to solve the puzzle.
- **Exact Cover (Knuth's DLX)**: Features a high-speed dancing links solver implementing Knuth's Algorithm X for microsecond solving and solution uniqueness checks during puzzle generation.

## Projects

- **`Sudoku.Core`**: Target `net10.0`. Core algorithms including:
  - `Board`: Deductive solver, step-by-step explanation recorder, and explanation minimizer.
  - `DancingLinks` / `SudokuDancingLinks`: Knuth's Dancing Links Algorithm X exact cover solver.
  - `SudokuGenerator`: Symmetric puzzle generator guaranteeing unique solutions.
  - `PresetPuzzle`: Built-in puzzles spanning Beginner, Medium, Challenging, and Expert difficulties.
- **`Sudoku.Web`**: Blazor WebAssembly PWA with an interactive 9x9 grid, keypad, candidate markers, animated step-by-step solution walkthrough, and customizable playback speed.
- **`Sudoku.Tests`**: Comprehensive xUnit test suite covering deduction logic, DLX speed, uniqueness verification, and generator behavior.

## Running Locally

To run the unit tests:
```powershell
dotnet test Sudoku.sln
```

To run the web app locally:
```powershell
dotnet run --project Sudoku.Web/Sudoku.Web.csproj
```
Then navigate to `http://localhost:5000` (or the reported local port).