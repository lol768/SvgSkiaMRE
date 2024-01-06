# SvgSkiaMRE

Example project for reproducing issue
[#193](https://github.com/wieslawsoltes/Svg.Skia/issues/193) within Svg.Skia library.

This is a simple console application that loads a SVG file and renders it to a PDF file.

The SVG is a fairly simple document, consisting of a number of paths and rectangles. It
represents a 2D barcode.

With some small patches, `Svg.Skia` is capable of producing an `SKPicture` which renders to
the file `gold.pdf` which is the desired PDF output. However, by default, `Svg.Skia` forces
Skia to rasterize parts of the SVG, resulting in a PDF file which is 36x larger in size than
necessary, and causes slow rendering in PDF viewers such as Okular, PDF.js and Evince.

## Steps to reproduce

1. Clone this repository with `git clone git@github.com:lol768/SvgSkiaMRE.git`
2. Run `dotnet run` in the root of the repository

```
$ dotnet run
Loading SVG
Generated PDF to ./generated.pdf
Size of generated.pdf: 144678 bytes
Number of /Image instances in generated.pdf: 236
Number of /PatternType instances in generated.pdf: 236
Size of gold.pdf: 4046 bytes
Number of /Image instances in gold.pdf: 0
Number of /PatternType instances in gold.pdf: 0
```
