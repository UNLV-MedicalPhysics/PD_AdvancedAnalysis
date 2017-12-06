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
using Make_DXF.Models;
using System.IO;
using Microsoft.Win32;

namespace Make_DXF
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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //set parameters for start

            detector_cmb.ItemsSource = Enum.GetValues(typeof(HeaderData.Imager));
            detector_cmb.SelectedIndex = 0;

        }

        private void detector_cmb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //idu
            if (detector_cmb.SelectedIndex == 0)
            {
                sizeX_txt.Text = "40";
                sizeY_txt.Text = "30";
                pixelX_txt.Text = "1024";
                pixelY_txt.Text = "768";

            }
            else//dmi
            {
                sizeX_txt.Text = "40";
                sizeY_txt.Text = "40";
                pixelX_txt.Text = "1190";
                pixelY_txt.Text = "1190";
            }
            gantry_txt.Text = "0"; coll_txt.Text = "0";
            x1_txt.Text = "-5"; x2_txt.Text = "5";
            y1_txt.Text = "-5"; y2_txt.Text = "5";
        }

        private void generateDXF_btn_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(xpos_txt.Text) || String.IsNullOrEmpty(ypos_txt.Text))
            {
                MessageBox.Show("Put in some positions");
            }
            else
            {
                HeaderData hd = new HeaderData
                {
                    date = DateTime.Now,
                    collRtn = Convert.ToDouble(coll_txt.Text),
                    gantryAngle = Convert.ToDouble(gantry_txt.Text),
                    image_type = Convert.ToString((HeaderData.Imager)detector_cmb.SelectedIndex),
                    machineID = "HESN10",
                    size1 = Convert.ToInt16(pixelX_txt.Text),
                    size2 = Convert.ToInt16(pixelY_txt.Text),
                    x1 = Convert.ToDouble(x1_txt.Text),
                    x2 = Convert.ToDouble(x2_txt.Text),
                    y1 = Convert.ToDouble(y1_txt.Text),
                    y2 = Convert.ToDouble(y2_txt.Text),
                };
                string header = hd.Make_Header(hd.size1, hd.size2, "Acquired Portal",
                    hd.gantryAngle, hd.collRtn, hd.x1, hd.x2, hd.y1, hd.y2, hd.date,
                    detector_cmb.SelectedIndex, 1, Convert.ToInt16(sizeX_txt.Text), Convert.ToInt16(sizeY_txt.Text));
                float[,] data = hd.make_data(Convert.ToDouble(xpos_txt.Text), Convert.ToDouble(ypos_txt.Text));
                SaveFileDialog svd = new SaveFileDialog();
                svd.Filter = "DXF file (*.dxf)|*.dxf";
                svd.RestoreDirectory = true;
                if(svd.ShowDialog() == true)
                {
                    using (StreamWriter sw = new StreamWriter(svd.FileName))
                    {
                        sw.Write(header);
                        for(int i = 0; i< data.GetLength(0); i++)
                        {

                            //get this row of the float.
                            int cols = data.GetUpperBound(1) + 1;
                            float[] row = new float[cols];
                            //Buffer.BlockCopy(data, i*cols, row, 0, cols);
                            for(int j = 0; j<cols; j++)
                            {
                                row[j] = data[i, j];
                            }
                            sw.WriteLine(String.Join("\t", row));
                        }
                        sw.Flush();

                    }
                    MessageBox.Show("Success");
                }
            }
        }
    }
}
