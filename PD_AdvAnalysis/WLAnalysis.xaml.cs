using System;
using System.Drawing;
using System.Drawing.Imaging;
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

namespace PD_AdvAnalysis
{
    /// <summary>
    /// Interaction logic for WLAnalysis.xaml
    /// </summary>
    public partial class WLAnalysis : UserControl
    {
        ComboBox currentplan = System.Windows.Application.Current.MainWindow.FindName("plan_ddl") as ComboBox;
        //ComboBox c = System.Windows.Application.Current.MainWindow.FindName("course-ddl") as ComboBox;
        public Patient newcontext;
        public WLAnalysis()
        {
            InitializeComponent();
        }

        private void grabimag_btn_Click(object sender, RoutedEventArgs e)
        {
            newcontext = PD_AdvAnalysis.MainWindow.newcontext;


            PDPlanSetup ps = PD_AdvAnalysis.MainWindow.plan;
            foreach (PDBeam pb in ps.Beams)
            {
                CheckBox cb = new CheckBox();
                cb.Content = pb.Id;
                Fields.Children.Add(cb);
            }
        }

        private void autodetect_btn_Click(object sender, RoutedEventArgs e)
        {

        }

        // private void manualydetect_btn_Click(object sender, RoutedEventArgs e)
        //{
        //}

        class EllipseConverter : IValueConverter
        {
            private void manualydetect_btn_Click(object sender, RoutedEventArgs e)
            { }


            public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
            {
                if (value == null) return null;

                int radius = (int)value;
                int diameter = radius * 2;

                using (var bmp = new System.Drawing.Bitmap(diameter, diameter))
                {
                    using (var g = Graphics.FromImage(bmp))
                    {
                        g.FillEllipse(System.Drawing.Brushes.Blue,
                                      0,
                                      0,
                                      diameter,
                                      diameter);
                    }

                    using (MemoryStream ms = new MemoryStream())
                    {
                        bmp.Save(ms, ImageFormat.Bmp);
                        ms.Position = 0;
                        BitmapImage bitmapImage = new BitmapImage();
                        bitmapImage.BeginInit();
                        bitmapImage.StreamSource = ms;
                        bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                        bitmapImage.EndInit();

                        return bitmapImage;
                    }
                }
            }

            public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
            {
                throw new NotImplementedException();
            }
        }
        }
    }

    

