using DarkMoviesDL;
using ShellProgressBar;

ProgressBarOptions ProcessBarOption = new()
{
    ForegroundColor = ConsoleColor.Green,
    ForegroundColorDone = ConsoleColor.DarkGreen,
    BackgroundColor = ConsoleColor.DarkGray,
    BackgroundCharacter = '\u2593',
    EnableTaskBarProgress = true,
    ProgressBarOnBottom = false,
    ProgressCharacter = '='
};


Console.Write("Series Link: ");
string link = Console.ReadLine();

if (String.IsNullOrEmpty(link))
{
    Console.WriteLine("Link can not be empty, exiting gravefully!");
}
else if (!link.Contains("dark-movies.fun") || !link.Contains("darkmovies.cash"))
{
    Console.WriteLine("Link is not valid, exiting gracefully!");
}
else
{
    DarkDL dm = new(link, ProcessBarOption);
    dm.GetEpisodeLinks();
    dm.SaveEpisodeLinks();
    File.Delete("index.html");
}