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
        int image_number = 0;
        double resx = 0.34;
        double resy = 0.34;
        ComboBox currentplan = System.Windows.Application.Current.MainWindow.FindName("plan_ddl") as ComboBox;
        List<PDBeam> fields;
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
                cb.Margin = new Thickness(5);
            }
        }
        

        private void autodetect_btn_Click(object sender, RoutedEventArgs e)
        {

        }

        Ellipse ell = new Ellipse();
        
        Ellipse ell2 = new Ellipse();
        private void manualydetect_btn_Click(object sender, RoutedEventArgs e)
        {
            canvas.Children.Clear();
            //Ellipse ell = new Ellipse();
            ell.Name = "ball_ell";
            ell.Width = ball_sb.Value/resx * marginx;
            ell.Height = ball_sb.Value/resy *marginx;
            ell.Stroke = System.Windows.Media.Brushes.Black;
            ell.StrokeThickness = 1.5;
            ell.Margin = new Thickness((canvas.Width - ell.Width) / 2, (canvas.Height - ell.Height) / 2, 0 , 0 );
            canvas.Children.Add(ell);
            ell.MouseDown += Ell_MouseDown;
            ell.MouseUp += Ell_MouseUp;
            ell.MouseMove += Ell_MouseMove;
            //Ellipse ell2 = new Ellipse();
            ell2.Name = "cone_ell";
            ell2.Width = cone_sb.Value/resx *marginx;
            ell2.Height = cone_sb.Value / resy *marginx;
            ell2.Stroke = System.Windows.Media.Brushes.Red;
            ell2.StrokeThickness = 1.5;
            ell2.Margin = new Thickness((canvas.Width - ell2.Width) / 2, (canvas.Height - ell2.Height) / 2, 0, 0);
            canvas.Children.Add(ell2);
            ell2.MouseDown += Ell_MouseDown;
            ell2.MouseUp += Ell_MouseUp;
            ell2.MouseMove += Ell_MouseMove;


        }

       

        private UIElement source;
        private bool captured;
        double current_x, current_y;
        double mouse_x, mouse_y;
        double marginx = 1;
        private void Ell_MouseMove(object sender, MouseEventArgs e)
        {
            //throw new NotImplementedException();
            if (captured)
            {
                double x = e.GetPosition(canvas).X;
                double y = e.GetPosition(canvas).Y;
                
                //change position of ellipse on the screen
                (source as Ellipse).Margin = new Thickness(x, y, 0, 0);
                if(x < 0)
                {
                    ell.Width = ball_sb.Value / resx * marginx;
                    ell.Height = ball_sb.Value / resy * marginx;
                    ell2.Width = cone_sb.Value / resx * marginx;
                    ell2.Height = cone_sb.Value / resy * marginx;
                    ell.Margin = new Thickness((canvas.Width - ell.Width) / 2, (canvas.Height - ell.Height) / 2, 0, 0);
                    ell2.Margin = new Thickness((canvas.Width - ell2.Width) / 2, (canvas.Height - ell2.Height) / 2, 0, 0);
                } else if( x > 600)
                {
                    ell.Width = ball_sb.Value / resx * marginx;
                    ell.Height = ball_sb.Value / resy * marginx;
                    ell2.Width = cone_sb.Value / resx * marginx;
                    ell2.Height = cone_sb.Value / resy * marginx;
                    ell.Margin = new Thickness((canvas.Width - ell.Width) / 2, (canvas.Height - ell.Height) / 2, 0, 0);
                    ell2.Margin = new Thickness((canvas.Width - ell2.Width) / 2, (canvas.Height - ell2.Height) / 2, 0, 0);
                }
                if ( y < 0)
                {
                    ell.Width = ball_sb.Value / resx * marginx;
                    ell.Height = ball_sb.Value / resy * marginx;
                    ell2.Width = cone_sb.Value / resx * marginx;
                    ell2.Height = cone_sb.Value / resy * marginx;
                    ell.Margin = new Thickness((canvas.Width - ell.Width) / 2, (canvas.Height - ell.Height) / 2, 0, 0);
                    ell2.Margin = new Thickness((canvas.Width - ell2.Width) / 2, (canvas.Height - ell2.Height) / 2, 0, 0);
                } else if (y > 500)
                {
                    ell.Width = ball_sb.Value / resx * marginx;
                    ell.Height = ball_sb.Value / resy * marginx;
                    ell2.Width = cone_sb.Value / resx * marginx;
                    ell2.Height = cone_sb.Value / resy * marginx;
                    ell.Margin = new Thickness((canvas.Width - ell.Width) / 2, (canvas.Height - ell.Height) / 2, 0, 0);
                    ell2.Margin = new Thickness((canvas.Width - ell2.Width) / 2, (canvas.Height - ell2.Height) / 2, 0, 0);
                }
            }
        }

        private void Ell_MouseUp(object sender, MouseButtonEventArgs e)
        {
            //throw new NotImplementedException();
            Mouse.Capture(null);
            captured = false;
            Mouse.OverrideCursor = null;

        }

        private void zoomin_btn_Click(object sender, RoutedEventArgs e)
        {
            marginx++;
            ell.Width = ball_sb.Value / resx * marginx;
            ell.Height = ball_sb.Value / resy * marginx;
            ell2.Width = cone_sb.Value / resx * marginx;
            ell2.Height = cone_sb.Value / resy * marginx;
        }

        private void zoomout_btn_Click(object sender, RoutedEventArgs e)
        {
            marginx--;
            ell.Width = ball_sb.Value / resx * marginx;
            ell.Height = ball_sb.Value / resy * marginx;
            ell2.Width = cone_sb.Value / resx * marginx;
            ell2.Height = cone_sb.Value / resy * marginx;

        }

        private void reset_btn_Click(object sender, RoutedEventArgs e)
        {
            marginx = 1;
            ell.Width = ball_sb.Value / resx * marginx;
            ell.Height = ball_sb.Value / resy * marginx;
            ell2.Width = cone_sb.Value / resx * marginx;
            ell2.Height = cone_sb.Value / resy * marginx;
            ell.Margin = new Thickness((canvas.Width - ell.Width) / 2, (canvas.Height - ell.Height) / 2, 0, 0);
            ell2.Margin = new Thickness((canvas.Width - ell2.Width) / 2, (canvas.Height - ell2.Height) / 2, 0, 0);

        }

        private void changefield_btn_Click(object sender, RoutedEventArgs e)
        {
            List<String> field_id = new List<String>();
                foreach(CheckBox sb in Fields.Children)
            {
                if ((bool)sb.IsChecked)
                {
                    field_id.Add(sb.Content.ToString());
                }
            }
            fields = PD_AdvAnalysis.MainWindow.plan.Beams.Where(j => field_id.Contains(j.Id)).ToList();
            PDBeam pdb = fields[image_number];
            PortalDoseImage img = pdb.PortalDoseImages.Last();
            VMS.CA.Scripting.Image img1 = img.Image;
            VMS.CA.Scripting.Frame frame = img1.Frames[0];
            ushort[,] pixels = new ushort[frame.XSize, frame.YSize];
            //int image_max, image_min;
            //img.GetMinMax(out image_max, out image_min, false);
            frame.GetVoxels(0, pixels);
            ImageDecon2.ImageDecon2 id2 = new ImageDecon2.ImageDecon2();
            BitmapSource bmp = id2.DrawImage(frame, pixels);
            field_img.Source = bmp;

        }

        private void previous_btn_Click(object sender, RoutedEventArgs e)
        {
            image_number--;
            List<String> field_id = new List<String>();
            foreach (CheckBox sb in Fields.Children)
            {
                if ((bool)sb.IsChecked)
                {
                    field_id.Add(sb.Content.ToString());
                }
            }
            fields = PD_AdvAnalysis.MainWindow.plan.Beams.Where(j => field_id.Contains(j.Id)).ToList();
            PDBeam pdb = fields[image_number];
            PortalDoseImage img = pdb.PortalDoseImages.Last();
            VMS.CA.Scripting.Image img1 = img.Image;
            VMS.CA.Scripting.Frame frame = img1.Frames[0];
            ushort[,] pixels = new ushort[frame.XSize, frame.YSize];
            //int image_max, image_min;
            //img.GetMinMax(out image_max, out image_min, false);
            frame.GetVoxels(0, pixels);
            ImageDecon2.ImageDecon2 id2 = new ImageDecon2.ImageDecon2();
            BitmapSource bmp = id2.DrawImage(frame, pixels);
            field_img.Source = bmp;


        }

        private void next_btn_Click(object sender, RoutedEventArgs e)
        {
            image_number++;
            List<String> field_id = new List<String>();
            foreach (CheckBox sb in Fields.Children)
            {
                if ((bool)sb.IsChecked)
                {
                    field_id.Add(sb.Content.ToString());
                }
            }
            fields = PD_AdvAnalysis.MainWindow.plan.Beams.Where(j => field_id.Contains(j.Id)).ToList();
            PDBeam pdb = fields[image_number];
            PortalDoseImage img = pdb.PortalDoseImages.Last();
            VMS.CA.Scripting.Image img1 = img.Image;
            VMS.CA.Scripting.Frame frame = img1.Frames[0];
            ushort[,] pixels = new ushort[frame.XSize, frame.YSize];
            //int image_max, image_min;
            //img.GetMinMax(out image_max, out image_min, false);
            frame.GetVoxels(0, pixels);
            ImageDecon2.ImageDecon2 id2 = new ImageDecon2.ImageDecon2();
            BitmapSource bmp = id2.DrawImage(frame, pixels);
            field_img.Source = bmp;
        }

        private void Ell_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //throw new NotImplementedException();
            //change shape of cursor
            Mouse.OverrideCursor = Cursors.SizeAll;
            //access ellipse
            source = (UIElement)sender;
            //let rest of code know that the mouse was clicked on the ellipse
            Mouse.Capture(source);
            captured = true;
            //get current margins from ellipse, and current mouse positions.
            current_x = (sender as Ellipse).Margin.Right;
            current_y = (sender as Ellipse).Margin.Top;
            mouse_x = e.GetPosition(canvas).X;
            mouse_y = e.GetPosition(canvas).Y;

        }

        private void ball_sb_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            foreach (Shape c in canvas.Children)
            {
                if (c.Name == "ball_ell")
                {
                    c.Width = ball_sb.Value / resx;
                    c.Height = ball_sb.Value / resy;
                }
                if (c.Name == "cone_ell")
                {
                    c.Width = cone_sb.Value / resx;
                    c.Height = cone_sb.Value / resy;
                }
            }

            }

       
    }
        }

        /*class EllipseConverter : IValueConverter
        {
            //private void manualydetect_btn_Click(object sender, RoutedEventArgs e)
            //{ }


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
    }*/


    

