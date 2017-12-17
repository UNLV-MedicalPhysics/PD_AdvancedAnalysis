using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using Microsoft.Win32;
using System.Drawing;
using WL_AutoDetectTest.Models;

namespace WL_AutoDetectTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void image_btn_Click(object sender, RoutedEventArgs e)
        {
            //global variable declaration
            int xSize=0;
            int ySize=0;
            int data_line;
            //load image here.
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.RestoreDirectory = true;
            ofd.Filter = "DXF Files (*.dxf)|*.dxf";
            if(ofd.ShowDialog() == true)
            {
                bool line_count = false;
                bool xSizeb = false;
                bool ySizeb = false;
                //List<float[]> image = new List<float[]>();
                Models.Image image = new Models.Image();
                foreach (string line in File.ReadAllLines(ofd.FileName))
                {
                    if (line_count && xSizeb && ySizeb)
                    {
                        //this evaluates once you've found all 3. 
                        float[] row = new float[ySize];
                        //image.sizeX = xSize; image.sizeY = ySize;
                        row = Array.ConvertAll(line.Split('\t'), float.Parse);
                        image.pixels.Add(row);
                    }
                    else
                    {
                        if (line.Contains("Size1"))
                        {
                            //xSizeb = int.TryParse(line.Split('=').Last(), out xSize);
                            image.sizeX = int.Parse(line.Split('=').Last());
                            xSizeb = true;
                        }
                        else if (line.Contains("Size2"))
                        {
                            //ySizeb = int.TryParse(line.Split('=').Last(), out ySize);
                            image.sizeY = int.Parse(line.Split('=').Last());
                            ySizeb = true;
                        }
                        else if (line.Contains("Res1"))
                        {
                            image.resX = double.Parse(line.Split('=').Last());
                        }
                        else if (line.Contains("Res2"))
                        {
                            image.resY = double.Parse(line.Split('=').Last());
                        }                        
                        else if (line == "[Data]")
                        {
                            line_count = true;
                        }
                    }
                    //line_count++;
                }
                //write image
                if(line_count && ySizeb && xSizeb)
                {
                    //float[,] image_f = new float[ySize, xSize];
                    //for(int i =0; i<image.Count; i++)
                    //{
                    //    image_f.getrow = image[i];
                    //}
                    BitmapSource bms = WL_AutoDetectTest.ImageDecon2.DrawImage(image);
                    image_im.Source = bms;
                    //get x and y position of the line.
                    int line_x = Convert.ToInt32((image.sizeX-image_cnv.Width)/2 + profile_ln.Margin.Left);
                    int line_y = Convert.ToInt32((image.sizeY-image_cnv.Height)/2 + profile_ln.Margin.Top);
                    int rows_AVG = Convert.ToInt16(profile_ln.StrokeThickness);//some rounding issues may occur.
                    int line_length = Convert.ToInt32(profile_ln.Width);
                    //line orientation shoudl be here as well in order to allow the user to get vertical position.
                    //get profile row.
                    float[] profile = GetProf(image, line_x, line_y, rows_AVG, line_length);
                    plotCanv1(profile, profile_cnv);
                    float[] gradient = GetGrad(image, line_x, line_y, rows_AVG, line_length);
                    plotCanv1(gradient, gradient_cnv);
                }
                

            }
        }

        private float[] GetGrad(Models.Image image, int line_x, int line_y, int rows_AVG, int line_length)
        {
            //throw new NotImplementedException();
            float[] averaged_grad = new float[Convert.ToInt32(image_cnv.Width)];
            for(int i = line_x; i<line_x+line_length; i++)
            {
                float sumDiff = 0; int count = 0;
                for(int j = Convert.ToInt32(line_y-rows_AVG/2); j< Convert.ToInt32(line_y + rows_AVG / 2); j++)
                {
                    sumDiff += Math.Abs(image.pixels[j][i + 1] - image.pixels[j][i]);
                    count++;
                }
                averaged_grad[Convert.ToInt32(profile_ln.Margin.Left) + i - line_x] = sumDiff / count;

            }
            return averaged_grad;
        }
        private void plotCanv1(float[] profile, Canvas cnv)
        {
            //throw new NotImplementedException();
            double xcoeff = profile_cnv.Width;
            double yCoeff = profile_cnv.Height/profile.Max();
            for(int i = 0; i < profile.Count()-1; i++)
            {
                Line drawnProfile = new Line() { Stroke = Brushes.Blue, StrokeThickness = 2.0 };
                drawnProfile.X1 = i;
                drawnProfile.X2 = i+1;
                drawnProfile.Y1 = profile_cnv.Height-profile[i] * yCoeff;
                drawnProfile.Y2 = profile_cnv.Height-profile[i + 1]*yCoeff;
                cnv.Children.Add(drawnProfile);
            }
        }
        private float[] GetProf(Models.Image image, int line_x, int line_y, int rows_AVG, int line_length)
        {
            //throw new NotImplementedException();
            float[] averaged_row = new float[Convert.ToInt32(image_cnv.Width)];
            for(int i = line_x; i<line_x+line_length; i++)
            {
                //loop through and get the average based on the thickness of the line.
                float sum = 0; int count = 0;
                for(int j = Convert.ToInt32(line_y-rows_AVG/2); j < Convert.ToInt32(line_y + rows_AVG/2); j++)
                {
                    sum += image.pixels[j][i];
                    count++;
                }
                averaged_row[Convert.ToInt32(profile_ln.Margin.Left) + i - line_x] = sum / count;
            }
            return averaged_row;

        }
    }
}
