using Avalonia.Media.Imaging;
using OpenCvSharp;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.IO;

namespace NouchKill.Utils
{
    public class ImageUtils
    {
        public static Bitmap ToAvaloniaBitmap(Image<Rgb24> image)
        {
            using var ms = new MemoryStream();
            image.SaveAsBmp(ms);
            ms.Seek(0, SeekOrigin.Begin);
            return new Bitmap(ms);
        }
        public static Image<Rgb24> ConvertMatToImageSharp(Mat mat)
        {

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

                return image;
            }
        }
    }
}
