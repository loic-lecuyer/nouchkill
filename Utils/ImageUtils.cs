using OpenCvSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NouchKill.Utils {
    public class ImageUtils {
        public static Image<Rgb24> ConvertMatToImageSharp(Mat mat) {
            Stopwatch timer = new Stopwatch();
            timer.Start();
            if (mat.Empty())
                throw new ArgumentException("Mat is empty");

            // S'assurer que le mat est en BGR 8 bits (format caméra)
            if (mat.Type() != MatType.CV_8UC3) {
                throw new NotSupportedException($"Mat type not supported: {mat.Type()}");
            }

            // Convertir BGR -> RGB
            Mat matRgb = new Mat();
            Cv2.CvtColor(mat, matRgb, ColorConversionCodes.BGR2RGB);

            int width = matRgb.Width;
            int height = matRgb.Height;
            int stride = (int)matRgb.Step();

            // Accès direct aux données (attention : ImageSharp veut une copie !)
            unsafe {
                byte* srcPtr = (byte*)matRgb.DataPointer;

                // Créer l'image ImageSharp en copiant les données
                var image = SixLabors.ImageSharp.Image.LoadPixelData<Rgb24>(new ReadOnlySpan<byte>(srcPtr, height * stride), width, height);
                timer.Stop();
                Debug.WriteLine($"ConvertMatToImageSharp time: {timer.ElapsedMilliseconds} ms");
                return image;
            }
        }
    }
}
