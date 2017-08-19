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
        List<imag_avg> images = new List<imag_avg>();
        int image_number = 0;
        //CheckBox cb = new CheckBox();
        int image_numbermax;
        double resx = 0.34;
        double resy = 0.34;
        ComboBox currentplan = System.Windows.Application.Current.MainWindow.FindName("plan_ddl") as ComboBox;
        //List<PDBeam> fields;
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



        //images[image_number].ell2 = new Ellipse();
        private void manualydetect_btn_Click(object sender, RoutedEventArgs e)
        {
            //canvas.Children.Clear();
            ////Ellipse ell = new Ellipse();
            //images[image_number].ell = new Ellipse();
            //images[image_number].ell2 = new Ellipse();
            //images[image_number].ell.Name = "ball_ell";
            //images[image_number].ell.Width = ball_sb.Value/resx * images[image_number].zoom_number;
            //images[image_number].ell.Height = ball_sb.Value/resy *images[image_number].zoom_number;
            //images[image_number].ell.Stroke = System.Windows.Media.Brushes.Black;
            //images[image_number].ell.StrokeThickness = 1.5;
            //images[image_number].ell.Margin = new Thickness((canvas.Width - images[image_number].ell.Width) / 2, (canvas.Height - images[image_number].ell.Height) / 2, 0 , 0 );
            //canvas.Children.Add(images[image_number].ell);
            //images[image_number].ell.MouseDown += Ell_MouseDown;
            //images[image_number].ell.MouseUp += Ell_MouseUp;
            //images[image_number].ell.MouseMove += Ell_MouseMove;
            ////Ellipse ell2 = new Ellipse();
            //images[image_number].ell2.Name = "cone_ell";
            //images[image_number].ell2.Width = cone_sb.Value/resx *images[image_number].zoom_number;
            //images[image_number].ell2.Height = cone_sb.Value / resy *images[image_number].zoom_number;
            //images[image_number].ell2.Stroke = System.Windows.Media.Brushes.Red;
            //images[image_number].ell2.StrokeThickness = 1.5;
            //images[image_number].ell2.Margin = new Thickness((canvas.Width - images[image_number].ell2.Width) / 2, (canvas.Height - images[image_number].ell2.Height) / 2, 0, 0);
            //canvas.Children.Add(images[image_number].ell2);
            //images[image_number].ell2.MouseDown += Ell_MouseDown;
            //images[image_number].ell2.MouseUp += Ell_MouseUp;
            //images[image_number].ell2.MouseMove += Ell_MouseMove;


        }



        private UIElement source;
        private bool captured;
        double current_x, current_y;
        double mouse_x, mouse_y;
        //double images[image_number].zoom_number = 1;
        private void Ell_MouseMove(object sender, MouseEventArgs e)
        {
            //throw new NotImplementedException();
            if (captured)
            {
                double x = e.GetPosition(canvas).X;
                double y = e.GetPosition(canvas).Y;

                //change position of ellipse on the screen
                (source as Ellipse).Margin = new Thickness(x, y, 0, 0);
                if (x < 0)
                {
                    images[image_number].ell.Width = ball_sb.Value / resx * images[image_number].zoom_number;
                    images[image_number].ell.Height = ball_sb.Value / resy * images[image_number].zoom_number;
                    images[image_number].ell2.Width = cone_sb.Value / resx * images[image_number].zoom_number;
                    images[image_number].ell2.Height = cone_sb.Value / resy * images[image_number].zoom_number;
                    images[image_number].ell.Margin = new Thickness((canvas.Width - images[image_number].ell.Width) / 2, (canvas.Height - images[image_number].ell.Height) / 2, 0, 0);
                    images[image_number].ell2.Margin = new Thickness((canvas.Width - images[image_number].ell2.Width) / 2, (canvas.Height - images[image_number].ell2.Height) / 2, 0, 0);
                }
                else if (x > 600)
                {
                    images[image_number].ell.Width = ball_sb.Value / resx * images[image_number].zoom_number;
                    images[image_number].ell.Height = ball_sb.Value / resy * images[image_number].zoom_number;
                    images[image_number].ell2.Width = cone_sb.Value / resx * images[image_number].zoom_number;
                    images[image_number].ell2.Height = cone_sb.Value / resy * images[image_number].zoom_number;
                    images[image_number].ell.Margin = new Thickness((canvas.Width - images[image_number].ell.Width) / 2, (canvas.Height - images[image_number].ell.Height) / 2, 0, 0);
                    images[image_number].ell2.Margin = new Thickness((canvas.Width - images[image_number].ell2.Width) / 2, (canvas.Height - images[image_number].ell2.Height) / 2, 0, 0);
                }
                if (y < 0)
                {
                    images[image_number].ell.Width = ball_sb.Value / resx * images[image_number].zoom_number;
                    images[image_number].ell.Height = ball_sb.Value / resy * images[image_number].zoom_number;
                    images[image_number].ell2.Width = cone_sb.Value / resx * images[image_number].zoom_number;
                    images[image_number].ell2.Height = cone_sb.Value / resy * images[image_number].zoom_number;
                    images[image_number].ell.Margin = new Thickness((canvas.Width - images[image_number].ell.Width) / 2, (canvas.Height - images[image_number].ell.Height) / 2, 0, 0);
                    images[image_number].ell2.Margin = new Thickness((canvas.Width - images[image_number].ell2.Width) / 2, (canvas.Height - images[image_number].ell2.Height) / 2, 0, 0);
                }
                else if (y > 500)
                {
                    images[image_number].ell.Width = ball_sb.Value / resx * images[image_number].zoom_number;
                    images[image_number].ell.Height = ball_sb.Value / resy * images[image_number].zoom_number;
                    images[image_number].ell2.Width = cone_sb.Value / resx * images[image_number].zoom_number;
                    images[image_number].ell2.Height = cone_sb.Value / resy * images[image_number].zoom_number;
                    images[image_number].ell.Margin = new Thickness((canvas.Width - images[image_number].ell.Width) / 2, (canvas.Height - images[image_number].ell.Height) / 2, 0, 0);
                    images[image_number].ell2.Margin = new Thickness((canvas.Width - images[image_number].ell2.Width) / 2, (canvas.Height - images[image_number].ell2.Height) / 2, 0, 0);
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
            images[image_number].zoom_number++;
            images[image_number].ell.Width++;
            images[image_number].ell.Height++;
            images[image_number].ell2.Width++;
            images[image_number].ell2.Height++;
            images[image_number].ell.Width = ball_sb.Value / resx * images[image_number].zoom_number;
            images[image_number].ell.Height = ball_sb.Value / resy * images[image_number].zoom_number;
            images[image_number].ell2.Width = cone_sb.Value / resx * images[image_number].zoom_number;
            images[image_number].ell2.Height = cone_sb.Value / resy * images[image_number].zoom_number;
            ImageDecon2.ImageDecon2 id2 = new ImageDecon2.ImageDecon2();
            images[image_number].bmp = id2.DrawImage(images[image_number].f, images[image_number].pixels, images[image_number].zoom_number);
            //BitmapSource bmp = id2.DrawImage(frame, pixels);
            field_img.Source = images[image_number].bmp;
        }

        private void zoomout_btn_Click(object sender, RoutedEventArgs e)
        {
            //if (images[image_number].zoom_number < 0)
            //{
            //    images[image_number].zoom_number = 1;
            //}
            ////images[image_number].ell.Width--;
            ////images[image_number].ell.Height--;
            ////images[image_number].ell2.Width--;
            ////images[image_number].ell2.Height--;
            ////images[image_number].zoom_number--;
            ////images[image_number].ell.Width = ball_sb.Value / resx * images[image_number].zoom_number;
            ////images[image_number].ell.Height = ball_sb.Value / resy * images[image_number].zoom_number;
            ////images[image_number].ell2.Width = cone_sb.Value / resx * images[image_number].zoom_number;
            ////images[image_number].ell2.Height = cone_sb.Value / resy * images[image_number].zoom_number;
            images[image_number].zoom_number--;
            images[image_number].ell.Width--;
            images[image_number].ell.Height--;
            images[image_number].ell2.Width--;
            images[image_number].ell2.Height--;
            if (images[image_number].zoom_number <= 0)
            {
                images[image_number].zoom_number = 1;
            }
            images[image_number].ell.Width = ball_sb.Value / resx * images[image_number].zoom_number;
            images[image_number].ell.Height = ball_sb.Value / resy * images[image_number].zoom_number;
            images[image_number].ell2.Width = cone_sb.Value / resx * images[image_number].zoom_number;
            images[image_number].ell2.Height = cone_sb.Value / resy * images[image_number].zoom_number;
            ImageDecon2.ImageDecon2 id2 = new ImageDecon2.ImageDecon2();
            images[image_number].bmp = id2.DrawImage(images[image_number].f, images[image_number].pixels, images[image_number].zoom_number);
            //BitmapSource bmp = id2.DrawImage(frame, pixels);
            field_img.Source = images[image_number].bmp;
            
            
        }

        private void reset_btn_Click(object sender, RoutedEventArgs e)
        {
            images[image_number].zoom_number = 1;
            //images[imag_avg].ell2
            images[image_number].ell.Width = ball_sb.Value / resx * images[image_number].zoom_number;
            images[image_number].ell.Height = ball_sb.Value / resy * images[image_number].zoom_number;
            images[image_number].ell2.Width = cone_sb.Value / resx * images[image_number].zoom_number;
            images[image_number].ell2.Height = cone_sb.Value / resy * images[image_number].zoom_number;
            images[image_number].ell.Margin = new Thickness((canvas.Width - images[image_number].ell.Width) / 2, (canvas.Height - images[image_number].ell2.Height) / 2, 0, 0);
            images[image_number].ell2.Margin = new Thickness((canvas.Width - images[image_number].ell2.Width) / 2, (canvas.Height - images[image_number].ell2.Height) / 2, 0, 0);

        }

        public class imag_avg
        {
            public int image_id { get; set; }
            public PDBeam fieldid { get; set; }
            public VMS.CA.Scripting.Frame f { get; set; }
            public ushort[,] pixels { get; set; }
            public int zoom_number { get; set; }
            //public Tuple<double, double> ball_pos, cone_pos;
            public Ellipse ell { get; set; }
            public Ellipse ell2 { get; set; }
            //public double images[image_number].zoom_number = 1;
            public BitmapSource bmp { get; set; }
            
        }

        private void changefield_btn_Click(object sender, RoutedEventArgs e)
        {
            images.Clear();
            //List<String> field_id = new List<String>();
            //field_id.Count.ToString();
            int i_num = 0; int zoom_initial = 1;
            foreach (CheckBox sb in Fields.Children)
            {
                if ((bool)sb.IsChecked)
                {
                    //fieldid.Add(sb.Content.ToString());
                    images.Add(new imag_avg
                    {
                        image_id = i_num,
                        fieldid = PD_AdvAnalysis.MainWindow.plan.Beams.Where(j => j.Id == sb.Content.ToString()).First(),
                        zoom_number = zoom_initial,
                        ell = new Ellipse
                        {
                            Width = ball_sb.Value / resx * zoom_initial,
                            Name = "ball_ell",
                            Height = ball_sb.Value / resy * zoom_initial,
                            Stroke = System.Windows.Media.Brushes.Black,
                            StrokeThickness = 1.5,
                            Margin = new Thickness((canvas.Width - ball_sb.Value / resx * zoom_initial) / 2, (canvas.Height - ball_sb.Value / resy * zoom_initial) / 2, 0, 0),


                        },
                        ell2 = new Ellipse
                        {
                            Width = cone_sb.Value / resx * zoom_initial,
                            Name = "cone_ell",
                            Height = cone_sb.Value / resy * zoom_initial,
                            Stroke = System.Windows.Media.Brushes.Red,
                            StrokeThickness = 1.5,
                            Margin = new Thickness((canvas.Width - cone_sb.Value / resx * zoom_initial) / 2, (canvas.Height - cone_sb.Value / resy * zoom_initial) / 2, 0, 0),
                        }

                    });
                    images[i_num].ell.MouseDown += Ell_MouseDown;
                images[i_num].ell.MouseUp += Ell_MouseUp;
                images[i_num].ell.MouseMove += Ell_MouseMove;
                images[i_num].ell2.MouseDown += Ell_MouseDown;
                images[i_num].ell2.MouseUp += Ell_MouseUp;
                images[i_num].ell2.MouseMove += Ell_MouseMove;
                    PortalDoseImage img = images[i_num].fieldid.PortalDoseImages.Last();
                    VMS.CA.Scripting.Image img1 = img.Image;
                    images[i_num].f = img1.Frames[0];
                    images[i_num].pixels = new ushort[images[i_num].f.XSize, images[i_num].f.YSize];
                    images[i_num].f.GetVoxels(0, images[i_num].pixels);

                i_num++;

                }
                

            }
            canvas.Children.Clear();
            //images[image_number].ell.Name = "ball_ell";
            //images[image_number].ell.Width = ball_sb.Value / resx * images[image_number].zoom_number;
            //images[image_number].ell.Height = ball_sb.Value / resy * images[image_number].zoom_number;
            //images[image_number].ell.Stroke = System.Windows.Media.Brushes.Black;
            //images[image_number].ell.StrokeThickness = 1.5;
            //images[image_number].ell.Margin = new Thickness((canvas.Width - images[image_number].ell.Width) / 2, (canvas.Height - images[image_number].ell.Height) / 2, 0, 0);
            canvas.Children.Add(images[image_number].ell);
            //images[image_number].ell.MouseDown += Ell_MouseDown;
            //images[image_number].ell.MouseUp += Ell_MouseUp;
            //images[image_number].ell.MouseMove += Ell_MouseMove;
            //Ellipse ell2 = new Ellipse();
            //images[image_number].ell2.Name = "cone_ell";
            //images[image_number].ell2.Width = cone_sb.Value / resx * images[image_number].zoom_number;
            //images[image_number].ell2.Height = cone_sb.Value / resy * images[image_number].zoom_number;
            //images[image_number].ell2.Stroke = System.Windows.Media.Brushes.Red;
            //images[image_number].ell2.StrokeThickness = 1.5;
            //images[image_number].ell2.Margin = new Thickness((canvas.Width - images[image_number].ell2.Width) / 2, (canvas.Height - images[image_number].ell2.Height) / 2, 0, 0);
            canvas.Children.Add(images[image_number].ell2);
            //images[image_number].ell2.MouseDown += Ell_MouseDown;
            //images[image_number].ell2.MouseUp += Ell_MouseUp;
            //images[image_number].ell2.MouseMove += Ell_MouseMove;
            //fields = PD_AdvAnalysis.MainWindow.plan.Beams.Where(j => field_id.Contains(j.Id)).ToList();
            //PDBeam pdb = images[image_number].fieldid;
            //PortalDoseImage img = pdb.PortalDoseImages.Last();
            //VMS.CA.Scripting.Image img1 = img.Image;
            //images[image_number].f = img1.Frames[0];
            //ushort[,] pixels = new ushort[frame.XSize, frame.YSize];
            ////int image_max, image_min;
            ////img.GetMinMax(out image_max, out image_min, false);
            //frame.GetVoxels(0, pixels);
            ImageDecon2.ImageDecon2 id2 = new ImageDecon2.ImageDecon2();
            images[image_number].bmp = id2.DrawImage(images[image_number].f, images[image_number].pixels, images[image_number].zoom_number);
            //BitmapSource bmp = id2.DrawImage(frame, pixels);
            field_img.Source = images[image_number].bmp;

        }

        private void previous_btn_Click(object sender, RoutedEventArgs e)
        {
            canvas.Children.Clear();

            //images.Clear();
            image_number--;
            
            //List<String> field_id = new List<String>();
            //foreach (CheckBox sb in Fields.Children)
            //{
            //    if ((bool)sb.IsChecked)
            //    {
            //        field_id.Add(sb.Content.ToString());
            //    }
            //}
            image_numbermax = images.Count();
            if (image_number < 0)
            {
                image_number = image_numbermax - 1;
            }
            //fields = PD_AdvAnalysis.MainWindow.plan.Beams.Where(j => field_id.Contains(j.Id)).ToList();
            //PDBeam pdb = images[image_number].fieldid;
            canvas.Children.Add(images[image_number].ell);
            canvas.Children.Add(images[image_number].ell2);
            //fields = PD_AdvAnalysis.MainWindow.plan.Beams.Where(j => field_id.Contains(j.Id)).ToList();
            //PDBeam pdb = images[image_number].fieldid;
            //PortalDoseImage img = pdb.PortalDoseImages.Last();
            //VMS.CA.Scripting.Image img1 = img.Image;
            //VMS.CA.Scripting.Frame frame = img1.Frames[0];
            //ushort[,] pixels = new ushort[frame.XSize, frame.YSize];
            //int image_max, image_min;
            //img.GetMinMax(out image_max, out image_min, false);
            //frame.GetVoxels(0, pixels);
            ImageDecon2.ImageDecon2 id2 = new ImageDecon2.ImageDecon2();
            images[image_number].bmp = id2.DrawImage(images[image_number].f, images[image_number].pixels, images[image_number].zoom_number); ;
            field_img.Source = images[image_number].bmp;


        }

        private void next_btn_Click(object sender, RoutedEventArgs e)
        {
            canvas.Children.Clear();
            //images.Clear();
            image_number++;
            //canvas.Children.Add(images[image_number].ell);
            //canvas.Children.Add(images[image_number].ell2);
            //List<String> field_id = new List<String>();
            //foreach (CheckBox sb in Fields.Children)
            //{
            //    if ((bool)sb.IsChecked)
            //    {
            //        field_id.Add(sb.Content.ToString());

            //    }
            //}
            image_numbermax = images.Count();
            if (image_number >= image_numbermax)
            {
                image_number = 0;
            }
            canvas.Children.Add(images[image_number].ell);
            canvas.Children.Add(images[image_number].ell2);
            //fields = PD_AdvAnalysis.MainWindow.plan.Beams.Where(j => field_id.Contains(j.Id)).ToList();
            //PDBeam pdb = images[image_number].fieldid;
            //PortalDoseImage img = pdb.PortalDoseImages.Last();
            //VMS.CA.Scripting.Image img1 = img.Image;
            //VMS.CA.Scripting.Frame frame = img1.Frames[0];
            //ushort[,] pixels = new ushort[frame.XSize, frame.YSize];
            //int image_max, image_min;
            //img.GetMinMax(out image_max, out image_min, false);
            //frame.GetVoxels(0, pixels);
            ImageDecon2.ImageDecon2 id2 = new ImageDecon2.ImageDecon2();
            images[image_number].bmp = id2.DrawImage(images[image_number].f, images[image_number].pixels,images[image_number].zoom_number);
            field_img.Source = images[image_number].bmp;
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




