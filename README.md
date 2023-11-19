# King of the Hill Manager

## Description

Software to manage a Super Smash Bros Melee King-of-the-Hill-style tournament.

### Rules

- One set up
- A queue of all the players at the event
- Best of 3 or best of 5
- Winner stays, loser goes to back of queue
- After *n* wins in a row, winner returns to back of queue
- For each win, the number of games won is used to calculate an elo score
- Player with the highest elo by the end wins

### Features

- Enter character names and *n* wins to restart and start queue
- Record match, cycle queue, and calculate elo
- Display elo

## Build

It's a .NET 6 C# project, so: `dotnet build` or `dotnet run`

Nix users can create a shell with the necessary tools via `nix-shell`

