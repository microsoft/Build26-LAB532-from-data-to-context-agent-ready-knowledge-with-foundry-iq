using Lab532.Parts;
using Lab532.Shared;

// Run one Part at a time, mirroring the notebook flow:
//   dotnet run -- 1
//   dotnet run -- 2
//   dotnet run -- 3
//   dotnet run -- 4
//   dotnet run -- 5
//
// Each Part contains explicit, readable code (no hidden prompts):
//   Step 1: load config
//   Step 2: create knowledge sources
//   Step 3: create the knowledge base
//   Step 4: ask the KB a question via a tiny Agent Framework agent
//   Step 5: print the Copilot CLI MCP sidequest snippet

Config.Load();

var part = args.Length > 0 ? args[0].Trim().ToLowerInvariant() : "1";
Console.WriteLine($"=== LAB532 (C#) — Part {part} ===\n");

Func<Task> run = part switch
{
    "1" or "part1" => Part1.RunAsync,
    "2" or "part2" => Part2.RunAsync,
    "3" or "part3" => Part3.RunAsync,
    "4" or "part4" => Part4.RunAsync,
    "5" or "part5" => Part5.RunAsync,
    _ => throw new ArgumentException($"Unknown part '{part}'. Use 1, 2, 3, 4, or 5."),
};

await run();
