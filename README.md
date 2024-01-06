# SvgSkiaMRE

Example project for reproducing issue
[#195](https://github.com/wieslawsoltes/Svg.Skia/issues/195) within Svg.Skia library.

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

## Theory

Within `Svg.Model.SvgExtensions.SetColorOrShader`, `SKPath` `SKPaint`s  seem to always be given a shader even for the
most basic solid fills. By adding non-suspending breakpoints and running this application in a debugger, it is possible
to un-set the `Shader` on the `ShimSkiaSharp.SKPaint` instance, and then set a `Color` instead. This seems to result in
the output PDF being the desired size, and rendering quickly in PDF viewers.

## Comparison to Skia's own SVG frontend

Skia has some limited built-in support for loading SVG files with the `SkSVGDOM` type.

There are no SkiaSharp bindings for this type, but we can use this support in Rust.

Note that we need to make some modifications to the SVG file to get it to render correctly, these are detailed in
`example_for_rust.svg`.

```rust
use std::fs::File;
use std::io::{BufReader, Read, Write};
use skia_safe::{Color, FontMgr, pdf};
use skia_safe::svg::Dom;

fn main() {
    let width = 200;
    let height = 200;
    let document = pdf::new_document(None);
    let mut doc_pg = document.begin_page((width, height), None);

    doc_pg.canvas().clear(Color::WHITE);
    let file = File::open("./example_for_rust.svg").unwrap();
    
    // we need an &[u8], *NOT* a BufReader
    let reader = BufReader::new(file);
    let bytes = reader.bytes().map(|b| b.unwrap()).collect::<Vec<u8>>();
    let font_mgr = FontMgr::new();
    let mut svg = Dom::from_bytes(&*bytes, font_mgr).unwrap();
    svg.set_container_size((width as f32, height as f32));
    svg.render(&mut doc_pg.canvas());
    let new_doc = doc_pg.end_page(); // it gives it back to us
    // note we can't use document here, as it's stolen by document.begin_page
    let data = new_doc.close();
    let mut file = File::create("./gold_rust_SkSVGDOM.pdf").unwrap();
    file.write_all(&data).unwrap();
}
```

The PDF produced by this method is also the desired size, and renders quickly in PDF viewers.
