using OnePieceBot;

public static class Programm
{
    public static void Main()
    {
        Bot bot = Bot.Instance;
        MOFetcher fetcher= new MOFetcher();
        fetcher.Run();
        Console.ReadLine();
    }
}