using System.Reflection;
using System.Text.RegularExpressions;
using SkiaSharp;
using Svg.Skia;
using static System.Console;

using var svg = new SKSvg();

WriteLine("Loading SVG");
// load the SVG from the assembly
var name = "SvgSkiaMre.example.svg";
using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(name);
if (stream is null)
{
    WriteLine($"Resource '{name}' not found!");
    return;
}

svg.Load(stream);
WriteLine("Generated PDF to ./generated.pdf");
svg.Picture!.ToPdf("generated.pdf", SKColor.Empty, 1, 1);
var outputStream = new FileStream("generated.pdf", FileMode.Open, FileAccess.Read);
CheckPdf(outputStream, "generated.pdf");
var goldStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("SvgSkiaMre.gold.pdf");
if (goldStream is null)
{
    WriteLine($"Resource 'gold.pdf' not found!");
    return;
}
CheckPdf(goldStream, "gold.pdf");

void CheckPdf(Stream pdfStream, String label)
{
    // get size of stream
    var length = pdfStream.Length;
    
    WriteLine($"Size of {label}: {length} bytes");
    // load the PDF as a string
    var pdf = new StreamReader(pdfStream).ReadToEnd();
    // Count the number of instances of /Image in the PDF:
    // Count the number of instances of /PatternType in the PDF

    var imageCount = Regex.Matches(pdf, "/Subtype /Image").Count;
    var patternCount = Regex.Matches(pdf, "/PatternType").Count;
    WriteLine($"Number of /Image instances in {label}: {imageCount}");
    WriteLine($"Number of /PatternType instances in {label}: {patternCount}");
}
