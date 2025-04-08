using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using NouchKill.Models;
using OpenCvSharp;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
namespace NouchKill.Views;

public partial class WebcamControl : UserControl
{
    private VideoCapture _capture;
    private CancellationTokenSource _cancellationTokenSource;
    private string modelPath;
    private InferenceSession inferenceSession;
    public WebcamControl()
    {
        InitializeComponent();
        string appPath = AppDomain.CurrentDomain.BaseDirectory;
        modelPath = System.IO.Path.Combine(appPath, "Onnx", "FasterRCNN-10.onnx");
        //   var gpuSessionOptions = SessionOptions.MakeSessionOptionWithCudaProvider(0);
        //  inferenceSession = new InferenceSession(modelPath, gpuSessionOptions);
        // inferenceSession = new InferenceSession(modelPath);
        StartWebcam();
    }
    public void StartWebcam()
    {
        _capture = new VideoCapture(0); // 0 = caméra par défaut
        _cancellationTokenSource = new CancellationTokenSource();

        Task.Run(async () =>
        {
            while (!_cancellationTokenSource.Token.IsCancellationRequested)
            {
                try
                {
                    using var frame = new Mat();
                    _capture.Read(frame);

                    if (!frame.Empty())
                    {
                        var imageSharp = ConvertMatToImageSharp(frame);
                        List<Prediction> predictions = ProcessInference(imageSharp);
                        DrawPredictions(imageSharp, predictions);
                        var avaloniaBitmap = ToAvaloniaBitmap(imageSharp);
                        await Dispatcher.UIThread.InvokeAsync(() =>
                        {
                            WebcamImage.Source = avaloniaBitmap;
                        });
                    }

                    //await Task.Delay(1000);
                }
                catch (System.Exception ex)
                {
                    Debug.WriteLine("Error when grab frame " + ex.Message);
                }

            }
        }, _cancellationTokenSource.Token);
    }

    private List<Prediction> ProcessInference(Image<Rgb24> image)
    {
        Stopwatch timer = new Stopwatch();
        timer.Start();
        List<Prediction> predictions = new List<Prediction>();
        float ratio = 400f / Math.Min(image.Width, image.Height);
        image.Mutate(x => x.Resize((int)(ratio * image.Width), (int)(ratio * image.Height)));
        var paddedHeight = (int)(Math.Ceiling(image.Height / 32f) * 32f);
        var paddedWidth = (int)(Math.Ceiling(image.Width / 32f) * 32f);
        Tensor<float> input = new DenseTensor<float>(new[] { 3, paddedHeight, paddedWidth });
        var mean = new[] { 102.9801f, 115.9465f, 122.7717f };
        image.ProcessPixelRows(accessor =>
        {
            for (int y = paddedHeight - accessor.Height; y < accessor.Height; y++)
            {
                Span<Rgb24> pixelSpan = accessor.GetRowSpan(y);
                for (int x = paddedWidth - accessor.Width; x < accessor.Width; x++)
                {
                    input[0, y, x] = pixelSpan[x].B - mean[0];
                    input[1, y, x] = pixelSpan[x].G - mean[1];
                    input[2, y, x] = pixelSpan[x].R - mean[2];
                }
            }
        });
        // Setup inputs and outputs
        var inputs = new List<NamedOnnxValue>
            {
                NamedOnnxValue.CreateFromTensor("image", input)
        };
        using IDisposableReadOnlyCollection<DisposableNamedOnnxValue> results = inferenceSession.Run(inputs);
        // Postprocess to get predictions
        var resultsArray = results.ToArray();
        float[] boxes = resultsArray[0].AsEnumerable<float>().ToArray();
        long[] labels = resultsArray[1].AsEnumerable<long>().ToArray();
        float[] confidences = resultsArray[2].AsEnumerable<float>().ToArray();

        var minConfidence = 0.7f;
        for (int i = 0; i < boxes.Length - 4; i += 4)
        {
            var index = i / 4;
            if (confidences[index] >= minConfidence)
            {
                predictions.Add(new Prediction
                {
                    Box = new Box(boxes[i], boxes[i + 1], boxes[i + 2], boxes[i + 3]),
                    Label = LabelMap.Labels[labels[index]],
                    Confidence = confidences[index]
                });
            }
        }
        Debug.WriteLine($"Inference time: {timer.ElapsedMilliseconds} ms");
        timer.Stop();
        return predictions;

    }
    public static void DrawPredictions(Image<Rgb24> image, List<Prediction> predictions)
    {
        Stopwatch timer = new Stopwatch();
        timer.Start();
        var pen = Pens.Solid(SixLabors.ImageSharp.Color.Red, 2);
        var font = SystemFonts.CreateFont("Arial", 16);

        foreach (var pred in predictions)
        {
            var box = pred.Box;

            var rect = new RectangularPolygon(box.Xmin, box.Ymin, box.Xmax - box.Xmin, box.Ymax - box.Ymin);
            image.Mutate(ctx =>
            {
                ctx.Draw(pen, rect);
                ctx.DrawText($"{pred.Label} ({pred.Confidence:P0})", font, SixLabors.ImageSharp.Color.Yellow, new PointF(box.Xmin, box.Ymin - 20));
            });
        }
        timer.Stop();
        Debug.WriteLine($"DrawPredictions time: {timer.ElapsedMilliseconds} ms");
    }
    public static Bitmap ToAvaloniaBitmap(Image<Rgb24> image)
    {
        Stopwatch timer = new Stopwatch();
        timer.Start();
        using var ms = new MemoryStream();
        image.SaveAsBmp(ms);
        ms.Seek(0, SeekOrigin.Begin);
        timer.Stop();
        Debug.WriteLine($"ToAvaloniaBitmap time: {timer.ElapsedMilliseconds} ms");
        return new Bitmap(ms);
    }
    public static Image<Rgb24> ConvertMatToImageSharp(Mat mat)
    {
        Stopwatch timer = new Stopwatch();
        timer.Start();
        if (mat.Empty())
            throw new ArgumentException("Mat is empty");

        // S'assurer que le mat est en BGR 8 bits (format caméra)
        if (mat.Type() != MatType.CV_8UC3)
        {
            throw new NotSupportedException($"Mat type not supported: {mat.Type()}");
        }

        // Convertir BGR -> RGB
        Mat matRgb = new Mat();
        Cv2.CvtColor(mat, matRgb, ColorConversionCodes.BGR2RGB);

        int width = matRgb.Width;
        int height = matRgb.Height;
        int stride = (int)matRgb.Step();

        // Accès direct aux données (attention : ImageSharp veut une copie !)
        unsafe
        {
            byte* srcPtr = (byte*)matRgb.DataPointer;

            // Créer l'image ImageSharp en copiant les données
            var image = SixLabors.ImageSharp.Image.LoadPixelData<Rgb24>(new ReadOnlySpan<byte>(srcPtr, height * stride), width, height);
            timer.Stop();
            Debug.WriteLine($"ConvertMatToImageSharp time: {timer.ElapsedMilliseconds} ms");
            return image;
        }
    }
    protected override void OnUnloaded(RoutedEventArgs e)
    {
        base.OnUnloaded(e);
        _cancellationTokenSource?.Cancel();
        _capture?.Dispose();
        inferenceSession.Dispose();
    }

}