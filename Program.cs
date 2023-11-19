class EloRank {
    public EloRank(int startScore, int kFactor) {
        Score = startScore;
        KFactor = kFactor;
    }

    public void RecordMatch(int gamesWon, int gamesLost, ref EloRank other) {
        /*
         * In Chess, Win = 1, Tie = 0.5, and loss = 0
         * For us, we can do number of wins / total games
         */
        var gameScoreThis = ((double) gamesWon) / ((double) (gamesWon + gamesLost));
        var expectThis = 1.0 / (1.0 + Math.Pow(10.0, (other.Score - this.Score) / 400.0));
        Score = (int) (
            (double) Score + (double) KFactor * (gameScoreThis - expectThis)
        );

        var gameScoreOther = ((double) gamesLost) / ((double) (gamesWon + gamesLost));
        var expectOther = 1.0 / (1.0 + Math.Pow(10.0, (this.Score - other.Score) / 400.0));
        other.Score = (int) (
            (double) other.Score + (double) KFactor * (gameScoreOther - expectOther)
        );
    }
    
    public int Score;
    public int KFactor { get; private set; }
}

class Menu {
    public Menu(int startScore, int kFactor, int maxGamesWon) {
        this.startScore = startScore;
        this.kFactor = kFactor;
        this.maxGamesWon = maxGamesWon;

        InGame = false;
        ShouldQuit = false;
        players = new Dictionary<string, EloRank>();
        queue = new List<string>();
        curPlayers = ("", "");
        rng = new Random();
    }

    public void PrintOptions() {
        if (!InGame) {
            Console.WriteLine("Game Setup");
            Console.WriteLine("----------");
            printPlayers();
            Console.WriteLine("What do you want to do?");
            Console.WriteLine(" (a) Add players");
            Console.WriteLine(" (d) Delete players");
            Console.WriteLine(" (b) Begin the game");
            Console.WriteLine(" (r) Reset players");
            Console.WriteLine(" (f) Force player elo");
            Console.WriteLine(" (q) Quit");
        } else {
            Console.WriteLine("Match Menu");
            Console.WriteLine("----------");
            Console.WriteLine("Current Match: {0} vs {1}", curPlayers.Item1, curPlayers.Item2);
            Console.WriteLine("What do you want to do?");
            Console.WriteLine(" (r) Report match results");
            Console.WriteLine(" (l) List Queue");
            Console.WriteLine(" (p) Print Player Elos");
            Console.WriteLine(" (q) Return to menu");
        }
    }

    public void RunCommand(string input) {
        if (!InGame) {
            switch (input) {
                case "a": {
                    Console.Write("Name: ");
                    var name = Console.ReadLine()!.Trim();
                    players.Add(name, new EloRank(startScore, kFactor));
                    Console.WriteLine("Added player '{0}'", name);
                    break;
                }
                case "d": {
                    Console.Write("Name (Enter invalid value to cancel): ");
                    var name = Console.ReadLine()!.Trim();
                    if (players.ContainsKey(name)) {
                        players.Remove(name);
                        Console.WriteLine("Player '{0}' removed.", name);
                    } else {
                        Console.WriteLine("Cancelling bc name {0} has not been added");
                    }
                    break;
                }
                case "b": {
                    if (players.Count < 2) {
                        Console.WriteLine("Can't begin game - less than 2 players!");
                        break;
                    }
                    var first = rng.Next(players.Count);
                    var second = rng.Next(players.Count);
                    while (second == first) {
                        second = rng.Next(players.Count);
                    }
                    var playerLs = players.Keys.ToList();
                    curPlayers = (playerLs[first], playerLs[second]);
                    queue = new List<string>();
                    foreach (var (player, rank) in players) {
                        if (player != curPlayers.Item1 && player != curPlayers.Item2) {
                            queue.Insert(rng.Next(queue.Count), player);
                        }
                    }
                    InGame = true;
                    break;
                }
                case "r":
                    players = new Dictionary<string, EloRank>();
                    queue = new List<string>();
                    curPlayers = ("", "");
                    break;
                case "q":
                    ShouldQuit = true;
                    break;
                case "f": {
                    Console.Write("Name: ");
                    var name = Console.ReadLine()!.Trim();
                    if (!players.ContainsKey(name)) {
                        Console.WriteLine("No such player '{0}'", name);
                        break;
                    }
                    Console.Write("New Score: ");
                    var scoreStr = Console.ReadLine()!.Trim();
                    var score = 0;
                    if (!int.TryParse(scoreStr, out score)) {
                        Console.WriteLine("Invalid score!");
                        break;
                    }
                    players[name].Score = score;
                    Console.WriteLine("Set the score of player '{0}' to {1}", name, score);
                    break;
                }
                default:
                    Console.WriteLine("Unknown command: {0}", input);
                    break;
            }
        } else {
            switch (input) {
                case "r": {
                    Console.Write("Games won by {0}? ", curPlayers.Item1);
                    var gamesStr = Console.ReadLine()!.Trim();
                    var games1 = 0;
                    if (!int.TryParse(gamesStr, out games1)) {
                        Console.WriteLine("Invalid games won!");
                        break;
                    }
                    Console.Write("Games won by {0}? ", curPlayers.Item2);
                    gamesStr = Console.ReadLine()!.Trim();
                    var games2 = 0;
                    if (!int.TryParse(gamesStr, out games2)) {
                        Console.WriteLine("Invalid games won!");
                        break;
                    }
                    var player2 = players[curPlayers.Item2];
                    players[curPlayers.Item1].RecordMatch(games1, games2, ref player2);
                    players[curPlayers.Item2] = player2;
                    Console.WriteLine("Successfully recorded match.");
                    Console.WriteLine("New scores:");
                    Console.WriteLine(
                        "* {0} - {1}", curPlayers.Item1, players[curPlayers.Item1].Score
                    );
                    Console.WriteLine(
                        "* {0} - {1}", curPlayers.Item2, players[curPlayers.Item2].Score
                    );
                } break;
                case "l":
                    printQueue();
                    break;
                case "p":
                    printPlayers();
                    break;
                case "q":
                    InGame = false;
                    break;
            }
        }
    }

    private void printPlayers() {
        if (players.Count > 0) {
            Console.WriteLine("Current players:");
            var sorted = from entry in players orderby entry.Value.Score descending select entry;
            foreach (var (name, rank) in sorted) {
                Console.WriteLine("* {0} - {1}", name, rank.Score);
            }
        } else {
            Console.WriteLine("No Players");
        }
    }

    private void printQueue() {
        if (players.Count > 0) {
            Console.WriteLine("Player Queue:");
            var i = 0;
            foreach (var name in queue) {
                Console.WriteLine(" ({0}) {1}", i, name);
                i += 1;
            }
        } else {
            Console.WriteLine("No Players");
        }
    }

    public bool InGame { get; private set; }
    public bool ShouldQuit { get; private set; }

    private int startScore;
    private int kFactor;
    private int maxGamesWon;
    private Dictionary<string, EloRank> players;
    private List<string> queue;
    private (string, string) curPlayers;
    private Random rng;
}

partial class Program {
    public static void Main(string[] args) {
        var menu = new Menu(100, 32, 10); // Todo: Read args from CLI with some default

        while (!menu.ShouldQuit) {
            menu.PrintOptions();
            Console.Write("> ");
            var input = Console.ReadLine()!.Trim().ToLower();
            menu.RunCommand(input);
        }
    }
}

