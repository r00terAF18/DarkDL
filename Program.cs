using System.Text;
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

string ToUtf8(string text)
{
    byte[] bytes = Encoding.Default.GetBytes(text);
    text = Encoding.UTF8.GetString(bytes);
    return text;
}


Console.Write("Series Link: ");
string link = Console.ReadLine();

DarkDL dm = new(link, ProcessBarOption);
dm.GetEpisodeLinks();
dm.SaveEpisodeLinks();
File.Delete("index.html");
