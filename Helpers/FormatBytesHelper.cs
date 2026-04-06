namespace WindowsHealth_ServerCheck.Helpers
{
    public class FormatBytesHelper
    {
        public static string FormatBytes(long bytes)
        {
            if (bytes >= 1_073_741_824) return $"{bytes / 1_073_741_824.0:F2} GB";
            if (bytes >= 1_048_576) return $"{bytes / 1_048_576.0:F2} MB";
            if (bytes >= 1_024) return $"{bytes / 1_024.0:F2} KB";
            return $"{bytes} B";
        }

        public static byte[] TransparentImages(byte[] imageBytes, float opacidad)
        {
            using (var msIn = new MemoryStream(imageBytes))
            using (var img = Image.FromStream(msIn))
            using (var bmp = new Bitmap(img.Width, img.Height))
            {
                using (var g = Graphics.FromImage(bmp))
                {
                    // Usamos las rutas completas de System.Drawing.Imaging
                    var matrix = new System.Drawing.Imaging.ColorMatrix { Matrix33 = opacidad };
                    var attributes = new System.Drawing.Imaging.ImageAttributes();
                    attributes.SetColorMatrix(matrix, System.Drawing.Imaging.ColorMatrixFlag.Default, System.Drawing.Imaging.ColorAdjustType.Bitmap);

                    // Dibujamos especificando que usamos el Rectangle de Windows
                    g.DrawImage(img, new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height),
                        0, 0, img.Width, img.Height, System.Drawing.GraphicsUnit.Pixel, attributes);
                }

                using (var msOut = new MemoryStream())
                {
                    // Guardamos usando el ImageFormat de Windows
                    bmp.Save(msOut, System.Drawing.Imaging.ImageFormat.Png);
                    return msOut.ToArray();
                }
            }
        }
    }
}
