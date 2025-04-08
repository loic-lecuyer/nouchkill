using NouchKill.Utils;
using OpenCvSharp;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NouchKill.IO
{
    public class WebcamStream
    {
        private readonly int cameraIndex;
        private VideoCapture _capture;
        private CancellationTokenSource _cancellationTokenSource;
        public event EventHandler<Image<Rgb24>> OnImageRead;

        public WebcamStream(int cameraIndex = 0)
        {
            this.cameraIndex = cameraIndex;
        }

        public void Start()
        {
            _capture = new VideoCapture(cameraIndex);
            _cancellationTokenSource = new CancellationTokenSource();

            Task.Run(async () =>
            {
                Stopwatch timer = new Stopwatch();
                timer.Start();
                Stopwatch timerDebug = new Stopwatch();
                timerDebug.Start();
                List<double> fps = new List<double>();
                try
                {

                    while (!_cancellationTokenSource.Token.IsCancellationRequested)
                    {
                        try
                        {
                            using var frame = new Mat();
                            _capture.Read(frame);
                            if (!frame.Empty())
                            {
                                using var image = ImageUtils.ConvertMatToImageSharp(frame);
                                OnImageRead?.Invoke(this, image);

                                if (timerDebug.Elapsed.TotalSeconds > 3)
                                {
                                    if (fps.Count > 0)
                                    {
                                        Debug.WriteLine("FPS : " + fps.Average().ToString("###0.0"));
                                    }

                                    timerDebug.Restart();
                                    fps.Clear();
                                }
                                else
                                {
                                    fps.Add((1000.0 / timer.ElapsedMilliseconds));
                                }

                            }
                            else
                            {
                                await Task.Delay(40, _cancellationTokenSource.Token);
                            }

                            timer.Restart();
                        }
                        catch (System.Exception ex) when (!(ex is OperationCanceledException))
                        {
                            Debug.WriteLine("Error when grab frame " + ex.Message);
                        }

                    }
                }
                catch (OperationCanceledException)
                {
                    Debug.WriteLine("Exit by cancellation");
                }
                timer.Stop();
                timerDebug.Stop();

            }, _cancellationTokenSource.Token);
        }


        public void Stop()
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
        }
    }
}
