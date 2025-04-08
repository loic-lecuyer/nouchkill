using Avalonia;
using Avalonia.Controls;
using Avalonia.Threading;
using NouchKill.IO;
using NouchKill.Utils;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.Diagnostics;

namespace NouchKill.Views;

public partial class OnnxControl : UserControl
{
    public static readonly DirectProperty<OnnxControl, WebcamStream?> StreamProperty =
   AvaloniaProperty.RegisterDirect<OnnxControl, WebcamStream?>(
       nameof(Stream),
       o => o.Stream,
       (o, v) => o.Stream = v);

    private WebcamStream? _stream = null;

    public WebcamStream? Stream
    {
        get { return _stream; }
        set
        {
            if (_stream != null)
            {
                _stream.OnImageRead -= _stream_OnImageRead;
            }
            SetAndRaise(StreamProperty, ref _stream, value);
            if (_stream != null)
            {
                _stream.OnImageRead += _stream_OnImageRead;
            }
        }
    }


    public static readonly DirectProperty<OnnxControl, OnnxStream?> OnnxProperty =
  AvaloniaProperty.RegisterDirect<OnnxControl, OnnxStream?>(
      nameof(Onnx),
      o => o.Onnx,
      (o, v) => o.Onnx = v);

    private OnnxStream? _onnx = null;

    public OnnxStream? Onnx
    {
        get { return _onnx; }
        set
        {
            if (_onnx != null)
            {
                _onnx.OnPredictionsReady -= _onnx_OnPredictionsReady;
            }
            SetAndRaise(OnnxProperty, ref _onnx, value);
            if (_onnx != null)
            {
                _onnx.OnPredictionsReady += _onnx_OnPredictionsReady;
            }
        }
    }

    private async void _onnx_OnPredictionsReady(object? sender, System.Collections.Generic.List<Models.Prediction> e)
    {
        try
        {
            await Dispatcher.UIThread.InvokeAsync(() =>
            {

                Image<Rgba32> image = new Image<Rgba32>((int)Width, (int)Height);
                image.Mutate(x => x.Clear(Color.Transparent));
                var pen = Pens.Solid(SixLabors.ImageSharp.Color.Blue, 2);
                var font = SystemFonts.CreateFont("Arial", 16);

                foreach (var pred in e)
                {
                    var box = pred.Box;
                    var rect = new RectangularPolygon(
                      (float)(box.Xmin * Width),
                      (float)(box.Ymin * Height),
                      (float)((box.Xmax - box.Xmin) * Width),
                      (float)((box.Ymax - box.Ymin) * Height));
                    image.Mutate(ctx =>
                    {
                        ctx.Draw(pen, rect);
                        ctx.DrawText($"{pred.Label} ({pred.Confidence:P0})", font, SixLabors.ImageSharp.Color.Yellow,
                            new PointF((float)(box.Xmin * Width), (float)((box.Ymin * Height) - 20)));
                    });
                }


                var avaloniaBitmap = ImageUtils.ToAvaloniaBitmap(image);

                OnnxImage.Source = avaloniaBitmap;
                Debug.WriteLine("Size Control : " + Width + "*" + Height);

            });
        }
        catch (OperationCanceledException)
        {
        }
    }

    private async void _stream_OnImageRead(object? sender, SixLabors.ImageSharp.Image<SixLabors.ImageSharp.PixelFormats.Rgb24> e)
    {
        try
        {
            if (_onnx != null)
            {
                _onnx.SetNextImage(e.Clone());
            }

        }
        catch (OperationCanceledException)
        {
        }


    }


    public OnnxControl()
    {
        InitializeComponent();
    }
}