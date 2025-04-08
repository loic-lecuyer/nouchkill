using Avalonia.Threading;
using NouchKill.Utils;
using OpenCvSharp;
using OpenCvSharp.Aruco;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace NouchKill.IO {
    public class WebcamStream {
        private readonly int cameraIndex;
        private VideoCapture _capture;
        private CancellationTokenSource _cancellationTokenSource;
        public event EventHandler<Image<Rgb24>> OnImageRead;

        public WebcamStream(int cameraIndex = 0) {
            this.cameraIndex = cameraIndex;
        }

        public void Start() {
            _capture = new VideoCapture(this.cameraIndex);
            _cancellationTokenSource = new CancellationTokenSource();
            Task.Run(async () =>
            {
                try {
                    while (!_cancellationTokenSource.Token.IsCancellationRequested) {
                        try {
                            using var frame = new Mat();
                            _capture.Read(frame);
                            if (!frame.Empty()) {
                                var image = ImageUtils.ConvertMatToImageSharp(frame);
                                this.OnImageRead?.Invoke(this, image);  
                            }                          
                        } catch (System.Exception ex) when (!(ex is OperationCanceledException)) {
                            Debug.WriteLine("Error when grab frame " + ex.Message);
                        }

                    }
                } catch (OperationCanceledException) {
                    Debug.WriteLine("Exit by cancellation");
                }
               
            }, _cancellationTokenSource.Token);        
        }


        public void Stop() {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
        }
    }
}
