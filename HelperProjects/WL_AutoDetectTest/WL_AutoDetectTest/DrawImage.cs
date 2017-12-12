using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
//using VMS.CA.Scripting;
//using VMS.DV.PD.Scripting;
using System.IO;
using System.Drawing;
using WL_AutoDetectTest.Models;

namespace WL_AutoDetectTest
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public partial class ImageDecon2
    {
        
        public static BitmapSource DrawImage(Models.Image image)
        {
            //PD_AdvAnalysis.WLAnalysis wla = new PD_AdvAnalysis.WLAnalysis();
            //System.Windows.Controls.Image imag = wla.field_img; //System.Windows.Application.Current.MainWindow.FindName("field_img") as System.Windows.Controls.Image;
            WL_AutoDetectTest.MainWindow mw = new WL_AutoDetectTest.MainWindow();
            System.Windows.Controls.Image imag = mw.image_im;
            int w = Convert.ToInt32(imag.Width); int h = Convert.ToInt32(imag.Height);
            double[] image_pixels = new double[w*h];
            int index_out = 0;
            //this does a lot of the scaling. It insures the size of the image is the same as the size of the canvas.
            for (int j =(image.sizeY + h)/2; j > (image.sizeY - h)/2; j--)//I'm not sure why this has to be backward from the PD analysis code
            {
                int index_in = 0;
                for (int k = (image.sizeX -w)/2; k < (image.sizeX + w)/2; k++)
                {
                    
                        image_pixels[index_in + index_out * w] = image.pixels[j][k];
                        index_in++;                    
                    
                }
                //index_out+=zoom_level;
                index_out++; //no zoom implemented in this code.
            }
            double image_max = image_pixels.Max();
            double image_min = image_pixels.Min();
            //byte[] image_bytes = new byte[image_pixels.Length * sizeof(UInt16)];
            //Buffer.BlockCopy(image_pixels, 0, image_bytes, 0, image_bytes.Length);
            PixelFormat format = PixelFormats.Rgb24;
            //get min and max of the data.
            //iamge min now comes from the method input parameter image_min
            //image_min = image_pixels.Min();//this is the red value. I.e. 255,0,0
            //image_max = image_pixels.Max();//this is the blue value. i.e. 0,0,255.
            double image_med = (image_max + image_min) / 2;
            int stride = (w * format.BitsPerPixel + 7) / 8;
            byte[] image_bytes = new byte[stride * h];
            //copy data to image bytes 
            for (int j = 0; j < image_pixels.Length; j++)
            {

                double value = image_pixels[j];
                if (value < 0)
                {
                    MessageBox.Show("hello");
                }
                System.Windows.Media.Color c = new System.Windows.Media.Color();
                if(value < image_min)
                {
                    c.B = 0;
                    c.R = 0;
                    c.G = 0;
                }
                else if (value < image_med)
                {
                    c.R = 0;
                    c.B = Convert.ToByte(255 - (255 * (value - image_min) / (image_med - image_min)));
                    c.G = Convert.ToByte(255 - (255 * (image_med - value) / (image_med - image_min)));
                }
                else if (value <= image_max)
                {
                    c.B = 0;
                    c.R = Convert.ToByte(255 - (255 * (image_max - value) / (image_max - image_med)));
                    c.G = Convert.ToByte(255 - (255 * (value - image_med) / (image_max - image_med)));
                }
                else if(value > image_max)
                {
                    c.R = 0;
                    c.B = 0;
                    c.G = 0;
                }
                image_bytes[j * 3] = c.R;
                image_bytes[j * 3 + 1] = c.G;
                image_bytes[j * 3 + 2] = c.B;

            }
            BitmapSource bmp = BitmapSource.Create(
                w,
                h,
                10 * 2.54 / image.resX,
                10 * 2.54 / image.resY,
                format,
                null,
                image_bytes,
                stride
                );
            return bmp;
        }        
    }
}
