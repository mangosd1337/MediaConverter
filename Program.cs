using System.Diagnostics;
using ShellProgressBar;

var command = "-y -vsync 0 -i \"F:\\Tixati Downloads\\TV Shows\\Vikings.S03.1080p.BluRay.x265-RARBG\\Vikings.S03E01.1080p.BluRay.x265-RARBG.mp4\" -c:a copy -c:v h264_nvenc -pix_fmt yuv420p -b:v 2M -map_metadata 0 Vikings.S03E01.1080p.BluRay.x264.mp4";
var timeSpan = TimeSpan.FromMinutes(49).Add(TimeSpan.FromSeconds(5));

var p = new Process();
p.StartInfo.RedirectStandardError = true;
p.StartInfo.RedirectStandardOutput = true;
p.StartInfo.UseShellExecute = false;
p.StartInfo.CreateNoWindow = true;
p.StartInfo.FileName = "ffmpeg";
p.StartInfo.Arguments = command;

var options = new ProgressBarOptions
{
    ProgressCharacter = '█',
    ProgressBarOnBottom = true,
    ForegroundColor = ConsoleColor.Green,
    BackgroundColor = ConsoleColor.DarkGreen,
    BackgroundCharacter = '░',
    DisableBottomPercentage = true,
    PercentageFormat = $"{0:N4}%"
};

var pbar = new ProgressBar(100, "Encoding", options);

/*p.OutputDataReceived += new DataReceivedEventHandler((s, e) =>
{
    var timeString = e.Data?.Split(" ")?.Where(x => x.StartsWith("time="))?.FirstOrDefault()?.Split("=")[1];
    var time = TimeSpan.Zero;
    if (timeString != null)
    {
        time = TimeSpan.Parse(timeString);
    }

    Console.WriteLine(e.Data);
    Console.WriteLine(time);
});*/
p.ErrorDataReceived += new DataReceivedEventHandler((s, e) =>
{
    var timeString = e.Data?.Split(" ")?.Where(x => x.StartsWith("time="))?.FirstOrDefault()?.Split("=")[1];
    var time = TimeSpan.Zero;
    if (timeString != null)
    {
        time = TimeSpan.Parse(timeString);
    }

    //Console.WriteLine(e.Data);
    //Console.WriteLine($"{Math.Round((decimal)time.Ticks / (decimal)timeSpan.Ticks, 4) * 100}%");

    var progressPercentDecimal = Math.Round((decimal)time.Ticks / (decimal)timeSpan.Ticks, 4);
    var progressPercentString = $"{progressPercentDecimal * 100:N2}%";
    
    var progress = pbar.AsProgress<double>();
    progress.Report(Convert.ToDouble(progressPercentDecimal));
    pbar.Message = $"Progress: {progressPercentString} | Video Progress: {time:hh\\:mm\\:ss} / {timeSpan:hh\\:mm\\:ss}";
});

p.Start();
p.BeginOutputReadLine();
p.BeginErrorReadLine();

p.WaitForExit();