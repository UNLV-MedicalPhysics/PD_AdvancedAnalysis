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
using VMS.CA.Scripting;
using VMS.DV.PD.Scripting;
using System.IO;
using System.Drawing;

namespace ImageDecon2
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public partial class ImageDecon2
    {
        //testing stuff.
        /* private void load_btn_Click(object sender, RoutedEventArgs e)
         {
             Patient p = app.OpenPatientById("US-PD-004");
             PDPlanSetup pds = p.PDPlanSetups.First();
             PDBeam pdb = pds.Beams.First();
             pdi = pdb.PortalDoseImages.First();
             VMS.CA.Scripting.Image i = pdi.Image;
             f = i.Frames[0];//get first frame
             //int min, max = pdi.GetMinMax(out min, out max, false);
             ushort[,] pixels = new ushort[f.XSize, f.YSize];
             f.GetVoxels(0, pixels);
             int image_min;// = pixels.Cast<UInt16>().Min();
             int image_max;// = pixels.Cast<UInt16>().Max();
             pdi.GetMinMax(out image_max, out image_min, false);
             //set window CU values
             max_CU.Text = f.VoxelToDisplayValue(image_min).ToString("F3");
             min_CU.Text = f.VoxelToDisplayValue(image_max).ToString("F3");
             slope = (image_max - image_min) / (f.VoxelToDisplayValue(image_max) - f.VoxelToDisplayValue(image_min));
             //information from imaging-overview on MSDN (https://docs.microsoft.com/en-us/dotnet/framework/wpf/graphics-multimedia/imaging-overview 
             ///BitmapSource is an important class used in decoding and enconding of images.
             ///It is the basic building block of the WPF Imaging pipeline and represents a single, constant set of pixels at a certain size and resolution.
             ///A Bitmapsource can be an individual frame of a multiple frame image, or it can be the result of transform performed on a BitmapSource.
             ///It is the parent of many of the primary classes in WPF imaging such as BitmapFrame.
             //var image_pixels = pixels.Cast<UInt16>().ToArray();
             //image_pixels above has to be constructed manually, becuase the rows and columns are transposed. 
             BitmapSource bmp = DrawImage(f, pixels, image_min,image_max);

             image.Source = bmp;// _rot;

         }*/
        //this method is to take the pixels of the image and make them bitmaps.
        public BitmapSource DrawImage(VMS.CA.Scripting.Frame f, ushort[,] pixels, int zoom_level)
        {
            PD_AdvAnalysis.WLAnalysis wla = new PD_AdvAnalysis.WLAnalysis();
            //this is the image that is behind the canvas.
            System.Windows.Controls.Image imag = wla.field_img; //System.Windows.Application.Current.MainWindow.FindName("field_img") as System.Windows.Controls.Image;
            int w = Convert.ToInt16(imag.Width); int h = Convert.ToInt16(imag.Height);
            double[] image_pixels = new double[w * h];
            int index_out = 0;
            for (int j = (f.YSize - h / zoom_level) / 2; j < (f.YSize + h / zoom_level) / 2; j++)
            {
                int index_in = 0;
                for (int k = (f.XSize - w / zoom_level) / 2; k < (f.XSize + w / zoom_level) / 2; k++)
                {
                    if (zoom_level == 1)
                    {
                        image_pixels[index_in + index_out * w] = f.VoxelToDisplayValue(pixels[k, j]);
                        index_in++;
                    }
                    else
                    {
                        //loop through the rows above and below the current pixel.
                        for (int oout = -zoom_level/2; oout < zoom_level/2; oout++)
                        {                          
                            if(index_out == 0 && oout<0)//this is a top row, you cannot set pixel values above it.
                            {
                                
                            }
                            else if(index_out == f.YSize+h/zoom_level - 1 && oout>0)
                            {
                                //this is a bottom row, you cannot add to it.
                            }
                            else
                            {
                                //fixes up down.
                                //image_pixels[index_in + (index_out + oout) * w] = f.VoxelToDisplayValue(pixels[k, j]);
                                //set voxels right and left. 
                                if(index_in == 0 && oout < 0)
                                {
                                    //this is left column, cannot put anything left of that
                                }
                                else if(index_in == f.XSize+w/zoom_level-1 && oout > 0)
                                {
                                    //this is right column, cannot put anything past that
                                }
                                else
                                {
                                    //loop throught the pixels right and left of the current pixels
                                    for (int ooin = -zoom_level / 2; ooin < zoom_level / 2; ooin++)
                                    {
                                       image_pixels[index_in + ooin + (index_out + oout) * w] = f.VoxelToDisplayValue(pixels[k, j]);
                                    }
                                }
                            }
                           
                            index_in++;
                        }

                    }

                }
                index_out += zoom_level;
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
                if (value < image_min)
                {
                    //this is a catch. it should never go in here because the values hould never be less than the minimum value.
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
                else if (value > image_max)
                {
                    //another catch it should never be greater than the maximum value.
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
                10 * 2.54 / f.XRes,
                10 * 2.54 / f.YRes,
                format,
                null,
                image_bytes,
                stride
                );
            return bmp;
        }

        
    }
}
