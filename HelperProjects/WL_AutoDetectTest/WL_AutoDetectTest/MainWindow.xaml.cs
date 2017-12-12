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
                }
            }
        }

    }
}
