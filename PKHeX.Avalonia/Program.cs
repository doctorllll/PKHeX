using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Fonts;

namespace PKHeX.Avalonia;

internal sealed class Program
{
    [STAThread]
    public static void Main(string[] args) => BuildAvaloniaApp()
        .StartWithClassicDesktopLifetime(args);

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            // Inter (the default font) carries no CJK glyphs. Left unconfigured, Skia's automatic
            // system-font fallback does not reliably pick up a CJK-capable font in this app's
            // self-contained desktop packaging, so all nine supported languages' Chinese/Japanese/
            // Korean text -- including this app's own UI labels, not just decoded save data --
            // renders as tofu boxes. Naming candidates per desktop OS (with a generic catch-all)
            // fixes it. Same root cause as the Android host's CustomizeAppBuilder, found there first.
            .With(new FontManagerOptions
            {
                FontFallbacks =
                [
                    new FontFallback { FontFamily = new FontFamily("PingFang SC") },         // macOS, Simplified
                    new FontFallback { FontFamily = new FontFamily("PingFang TC") },         // macOS, Traditional
                    new FontFallback { FontFamily = new FontFamily("Hiragino Sans") },        // macOS, Japanese
                    new FontFallback { FontFamily = new FontFamily("Apple SD Gothic Neo") },  // macOS, Korean
                    new FontFallback { FontFamily = new FontFamily("Microsoft YaHei") },      // Windows, Simplified
                    new FontFallback { FontFamily = new FontFamily("Microsoft JhengHei") },   // Windows, Traditional
                    new FontFallback { FontFamily = new FontFamily("Yu Gothic") },            // Windows, Japanese
                    new FontFallback { FontFamily = new FontFamily("Malgun Gothic") },        // Windows, Korean
                    new FontFallback { FontFamily = new FontFamily("Noto Sans CJK SC") },     // Linux
                    new FontFallback { FontFamily = new FontFamily("sans-serif") },
                ],
            })
            .LogToTrace();
}
