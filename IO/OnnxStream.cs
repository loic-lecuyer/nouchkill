using NouchKill.Models;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;

namespace NouchKill.IO
{
    public abstract class OnnxStream
    {
        public event EventHandler<List<Prediction>> OnPredictionsReady;
        public abstract void Start();

        public abstract void Stop();
        public abstract void SetNextImage(Image<Rgb24> image);
        protected void RaisePredictionReady(List<Prediction> predictions)
        {
            OnPredictionsReady?.Invoke(this, predictions);
        }
    }
}
