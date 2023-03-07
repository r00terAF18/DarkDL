using System.IO;
using System.Net;
using System.Text;
using ShellProgressBar;
using HtmlAgilityPack;

namespace DarkMoviesDL;

public class DarkDL
{
    private ProgressBarOptions _barOptions;
    private ProgressBar bar;
    
    private CookieContainer cc = new();
    private List<string> epidsode_urls = new();
    private HtmlDocument doc = new();
    
    public string series_link;
    public int Episodes;
    public string series_name;


    public DarkDL(string series_link)
    {
        this.series_link = series_link;
        ReadCookies();

        var web = (HttpWebRequest)WebRequest.Create(this.series_link);
        web.CookieContainer = cc;
        var res = web.GetResponse();
        var resStream = res.GetResponseStream();
        StreamReader read = new(resStream, Encoding.UTF8);
        File.WriteAllText("index.html", read.ReadToEnd());
        doc.Load("index.html");
    }
    
    public DarkDL(string series_link, ProgressBarOptions barOptions)
    {
        this._barOptions = barOptions;
        this.series_link = series_link;
        ReadCookies();

        var web = (HttpWebRequest)WebRequest.Create(this.series_link);
        web.CookieContainer = cc;
        var res = web.GetResponse();
        var resStream = res.GetResponseStream();
        StreamReader read = new(resStream, Encoding.UTF8);
        File.WriteAllText("index.html", read.ReadToEnd());
        doc.Load("index.html");
    }

    public DarkDL(string series_link, ProgressBarOptions barOptions, string cookies_path)
    {
        this._barOptions = barOptions;
        this.series_link = series_link;
        ReadCookies(cookies_path);

        var web = (HttpWebRequest)WebRequest.Create(this.series_link);
        web.CookieContainer = cc;
        var res = web.GetResponse();
        var resStream = res.GetResponseStream();
        StreamReader read = new(resStream, Encoding.UTF8);
        File.WriteAllText("index.html", read.ReadToEnd());
        doc.Load("index.html");
    }

    // I swear one day I will recite every letter of this function
    private string ToUtf8(string text)
    {
        byte[] bytes = Encoding.Default.GetBytes(text);
        text = Encoding.UTF8.GetString(bytes);
        return text;
    }

    /// <summary>
    /// Checks if the required directory exists based on the course name
    /// if not, create it
    /// </summary>
    private void CreateDirStructure()
    {
        series_name = doc.DocumentNode.SelectSingleNode("//h2[@class=\"whiteMain\"]")
            .InnerText;

        series_name = ToUtf8(series_name);

        if (!Directory.Exists(series_name))
        {
            Directory.CreateDirectory(series_name);
        }
    }

    /// <summary>
    /// Properly reads and parses a pre-defined cookies.txt file
    /// adds the individual items to a CookieContainer
    /// It needs sessionid and csrftoken
    /// </summary>
    public void ReadCookies()
    {
        if (!File.Exists("cookies.txt"))
        {
            throw new FileNotFoundException("Couldn't find cookies file");
        }

        foreach (string line in File.ReadLines("cookies.txt"))
        {
            if (line.Length > 0 && line[0] != '#')
            {
                ParseCookies(line);
            }
        }
    }

    /// <summary>
    /// Properly reads and parses a pre-defined cookies.txt file
    /// adds the individual items to a CookieContainer
    /// It needs sessionid and csrftoken
    /// </summary>
    /// <param name="cookies_path">A path to the cookies file</param>
    public void ReadCookies(string cookies_path)
    {
        if (!File.Exists(cookies_path))
        {
            throw new FileNotFoundException("Couldn't find cookies file");
        }

        foreach (string line in File.ReadLines(cookies_path))
        {
            if (line.Length > 0 && line[0] != '#')
            {
                ParseCookies(line);
            }
        }
    }

    private void ParseCookies(string line)
    {
        List<string> temp = new();
        temp.AddRange(line.Split('\t'));

        Console.WriteLine("[+] Reading Cookies...");

        if (temp.Contains("user"))
        {
            cc.Add(new Cookie("user", temp[temp.IndexOf("user") + 1]) { Domain = "dark-movies.fun" });
        }

        if (temp.Contains("PHPSESSID"))
        {
            cc.Add(new Cookie("PHPSESSID", temp[temp.IndexOf("PHPSESSID") + 1]) { Domain = "dark-movies.fun" });
        }
    }

    public void GetEpisodeLinks()
    {
        series_name = doc.DocumentNode.SelectSingleNode("//h2[@class=\"whiteMain\"]")
            .InnerText;

        series_name = ToUtf8(series_name);

        int ticks = doc.DocumentNode.SelectNodes("//a[@class=\"downloadGet\"]").Count;

        Console.WriteLine("[+] Got all the links...");
        
        bar = new(ticks, "Episodes: ", _barOptions);
        Thread.Sleep(500);
        foreach (var node in doc.DocumentNode.SelectNodes("//a[@class=\"downloadGet\"]"))
        {
            // ClipboardService.SetTextAsync(node.Attributes["href"].Value.Trim());
            epidsode_urls.Add(node.Attributes["href"].Value.Trim());
            bar.Tick();
        }
        Episodes = epidsode_urls.Count;
    }

    
    public void SaveEpisodeLinks()
    {
        Console.WriteLine("[+] Saving Episode Links...");
        File.AppendAllLines($"{series_name}.txt", epidsode_urls, Encoding.UTF8);
    }
    
}