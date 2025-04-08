using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using NouchKill.Models;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NouchKill.IO
{
    public class FasterRcnnOnnxStream : OnnxStream
    {

        private InferenceSession inferenceSession;
        private CancellationTokenSource _cancellationTokenSource;

        public FasterRcnnOnnxStream()
        {
            string appPath = AppDomain.CurrentDomain.BaseDirectory;
            string modelPath = System.IO.Path.Combine(appPath, "Onnx", "FasterRCNN-10.onnx");
            inferenceSession = new InferenceSession(modelPath);
        }
        private Image<Rgb24>? _nextImage;
        private ManualResetEvent _nextImageResetEvent = new ManualResetEvent(false);
        private readonly object _nexImageLocker = new object();
        public override void SetNextImage(Image<Rgb24> image)
        {
            lock (_nexImageLocker)
            {
                _nextImage = image;
            }
            _nextImageResetEvent.Set();
        }
        public override void Start()
        {

            _cancellationTokenSource = new CancellationTokenSource();
            Task.Run(async () =>
            {

                try
                {

                    while (!_cancellationTokenSource.Token.IsCancellationRequested)
                    {
                        try
                        {
                            Image<Rgb24>? processImage = null;
                            _nextImageResetEvent.WaitOne();

                            lock (_nexImageLocker)
                            {
                                if (_nextImage != null)
                                {
                                    processImage = _nextImage.Clone();
                                }
                            }
                            if (processImage != null)
                            {
                                List<Prediction> predictions = ProcessInference(processImage);
                                RaisePredictionReady(predictions);

                                Debug.WriteLine("predictions " + predictions.Count);
                            }
                            _nextImageResetEvent.Reset();

                        }
                        catch (System.Exception ex) when (!(ex is OperationCanceledException))
                        {
                        }

                    }
                }
                catch (OperationCanceledException)
                {
                    Debug.WriteLine("Exit by cancellation");
                }


            }, _cancellationTokenSource.Token);
        }



        public override void Stop()
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
        }



        private List<Prediction> ProcessInference(Image<Rgb24> image)
        {
            Stopwatch timer = new Stopwatch();
            timer.Start();
            List<Prediction> predictions = new List<Prediction>();
            float ratio = 400f / Math.Min(image.Width, image.Height);
            double width = image.Width * ratio;
            double height = image.Height * ratio;
            image.Mutate(x => x.Resize((int)width, (int)height));
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
                        Box = new Box(
                        (float)(boxes[i] / width),
                        (float)(boxes[i + 1] / height),
                        (float)(boxes[i + 2] / width),
                        (float)(boxes[i + 3] / height)),
                        Label = LabelMap.Labels[labels[index]],
                        Confidence = confidences[index]
                    });
                }
            }
            //  Debug.WriteLine($"Inference time: {timer.ElapsedMilliseconds} ms");
            timer.Stop();
            return predictions;

        }
    }
}
