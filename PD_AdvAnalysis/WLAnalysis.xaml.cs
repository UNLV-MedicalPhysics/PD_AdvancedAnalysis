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
        List<WL_Image> images = new List<WL_Image>();
        int image_number = 0;
        int image_numbermax;
        ComboBox currentplan = System.Windows.Application.Current.MainWindow.FindName("plan_ddl") as ComboBox;
        public Patient newcontext;
        public WLAnalysis()
        {
            InitializeComponent();
        }
        //button the get all the fields and list images within the field.
        private void getField_btn_Click(object sender, RoutedEventArgs e)
        {
            newcontext = PD_AdvAnalysis.MainWindow.newcontext;
            PDPlanSetup ps = PD_AdvAnalysis.MainWindow.plan;
            int margin_height = 0;
            int beam_number = 0;

            foreach (PDBeam pb in ps.Beams)
            {
                TextBlock cb = new TextBlock();
                cb.Text = pb.Id;
                cb.FontSize = 14;
                cb.TextDecorations = TextDecorations.Underline;
                cb.HorizontalAlignment = HorizontalAlignment.Left;
                cb.VerticalAlignment = VerticalAlignment.Top;
                cb.Margin = new Thickness(5, margin_height, 0, 0);

                Fields.Children.Add(cb);

                ScrollViewer sv = new ScrollViewer();
                sv.HorizontalScrollBarVisibility = ScrollBarVisibility.Visible;
                sv.HorizontalAlignment = HorizontalAlignment.Left;
                sv.VerticalAlignment = VerticalAlignment.Top;
                sv.Height = 60;
                sv.Width = 400;

                sv.Margin = new Thickness(5, margin_height + 10, 0, 0);
                int margin_width = 2;
                Grid sp = new Grid();
                sp.Name = String.Format("Grid_{0}", beam_number);
                beam_number++;

                sp.Margin = new Thickness(margin_width, 0, 0, 0);
                //sp.Orientation = Orientation.Horizontal;
                for (int r = 0; r < pb.PortalDoseImages.Count(); r++)
                {
                    ColumnDefinition cd = new ColumnDefinition();
                    sp.ColumnDefinitions.Add(cd);

                }
                RowDefinition rd = new RowDefinition();
                sp.RowDefinitions.Add(rd);
                RowDefinition rd2 = new RowDefinition();
                sp.RowDefinitions.Add(rd2);
                int col = 0;
                foreach (PortalDoseImage pdi in pb.PortalDoseImages)
                {
                    //ColumnDefinition cd = new ColumnDefinition();
                    //sp.ColumnDefinitions= new ColumnDefinitionC
                    //sp.ColumnDefinitions.Add(cd);
                    CheckBox cb2 = new CheckBox();
                    cb2.Content = pdi.Id;
                    cb2.HorizontalAlignment = HorizontalAlignment.Left;
                    cb2.VerticalAlignment = VerticalAlignment.Top;
                    cb2.Margin = new Thickness(0, 0, 0, 0);

                    //RowDefinition rd = new RowDefinition();
                    //sp.RowDefinitions.Add(rd);

                    string StartDate = pdi.Session.SessionDate.ToString("MM/dd/yyyy");
                    TextBlock text_blck = new TextBlock();
                    text_blck.Text = StartDate;
                    text_blck.HorizontalAlignment = HorizontalAlignment.Left;
                    text_blck.VerticalAlignment = VerticalAlignment.Top;
                    text_blck.Margin = new Thickness(0, 20, 0, 0);
                    //margin_width = 60;
                    //sp.Children.Add(cb2);
                    //sp.Children.Add(text_blck);
                    Grid.SetRow(cb2, 0);
                    Grid.SetColumn(cb2, col);
                    sp.Children.Add(cb2);
                    Grid.SetRow(text_blck, 0);
                    Grid.SetColumn(text_blck, col);
                    sp.Children.Add(text_blck);
                    col++;

                }
                Fields.Children.Add(sp);
            }

        }
        //under construction.
        private void autodetect_btn_Click(object sender, RoutedEventArgs e)
        {

        }
        private void manualydetect_btn_Click(object sender, RoutedEventArgs e)
        {
            foreach (WL_Image ima in images)
            {
                //loop through all the images and detect the ball and cone position and report deviations
                /*For average deviation
                 * We have the deviations in mm already. For ex: lat = pos-pos2*22.5/96
                 * We need a non-angle specific average. 
                 * If the ball is off by 1mm laterally
                 * Gantry 0 => ball is off by 1mm*cos(0) = 1mm
                 * Gantry 15 => ball is off by 1mm*cos(15) = 0.966mm
                 * 
                 */
                //feel free to also report deviations at each gantry angle.
                double position;// this is the x position of the ball
                double position1;// this is the y position of the ball
                double position2;// this is the x position of the Cone
                double position3;// this is the y position of the Cone
                                 // position = images[image_number].ell.Margin.Left - canvas.Width /2 - images[image_number].ell.Width/2;
                position = (ima.ell.Margin.Left - (canvas.Width - ima.ell.Width) / 2) * (50 / 34);
                //position= images[image_number].ell
                position1 = ima.ell.Margin.Top - (canvas.Height - ima.ell.Height) / 2;
                position2 = ima.ell2.Margin.Left - (canvas.Width - ima.ell2.Width) / 2;
                position3 = ima.ell2.Margin.Top - (canvas.Height - ima.ell2.Height) / 2;
                double g_angle;
                g_angle = ima.WLBeam.ControlPoints.First().GantryAngle;
                double lat_disp;
                double vert_disp;
                double long_disp;
                //X_disp = (ball pos x - cone pos x)*22.5/96*(Math.Cos(gantry) + Math.Sin(gantry))
                //Y_disp = (ball pos y - cone pos y)*22.5/96
                //Lat = X_disp/cos(gantry)
                //
                lat_disp = ((position - position2) * images[image_number].resx / images[image_number].zoom_number) * Math.Cos(g_angle * 180 / Math.PI);
                vert_disp = ((position - position2) * images[image_number].resx / images[image_number].zoom_number) * Math.Sin(g_angle * 180 / Math.PI);
                long_disp = -(position1 - position3) * images[image_number].resy / images[image_number].zoom_number;

                //Add Longitudinal displacement.

                //think about whether the direction for the micrometer will be CW or CCW.
                MessageBox.Show(string.Format(" Field ID: {7} \n Gantry angle: {8}\nThe center of the Ball is: X: {0}  Y: {1} \nThe center of the Cone is: X: {2}  Y: {3} \n Move ball {4} mm in the lateral direction \n Move ball {5} mm in vertical direction \n Move the ball {6} mm in longitudinal direction", position, position1, position2, position3, lat_disp, vert_disp, long_disp, ima.f.Image.Id, g_angle));
            }
        }
        private UIElement source;
        private bool captured;
        double current_x, current_y;
        double mouse_x, mouse_y;
        //move the circle that represents either the cone or the BB.
        private void Ell_MouseMove(object sender, MouseEventArgs e)
        {
            //throw new NotImplementedException();
            if (captured)
            {
                double x = e.GetPosition(canvas).X;
                double y = e.GetPosition(canvas).Y;
                //change position of ellipse on the screen
                (source as Ellipse).Margin = new Thickness(x, y, 0, 0);
                //deal with what happens if the user moves the ellipse outside the bounds of the canvas. 
                if (x < 0)
                {
                    images[image_number].ell.Width = ball_sb.Value / images[image_number].resx * images[image_number].zoom_number;
                    images[image_number].ell.Height = ball_sb.Value / images[image_number].resy * images[image_number].zoom_number;
                    images[image_number].ell2.Width = cone_sb.Value / images[image_number].resx * images[image_number].zoom_number;
                    images[image_number].ell2.Height = cone_sb.Value / images[image_number].resy * images[image_number].zoom_number;
                    images[image_number].ell.Margin = new Thickness((canvas.Width - images[image_number].ell.Width) / 2, (canvas.Height - images[image_number].ell.Height) / 2, 0, 0);
                    images[image_number].ell2.Margin = new Thickness((canvas.Width - images[image_number].ell2.Width) / 2, (canvas.Height - images[image_number].ell2.Height) / 2, 0, 0);
                }
                else if (x > 600)
                {
                    images[image_number].ell.Width = ball_sb.Value / images[image_number].resx * images[image_number].zoom_number;
                    images[image_number].ell.Height = ball_sb.Value / images[image_number].resy * images[image_number].zoom_number;
                    images[image_number].ell2.Width = cone_sb.Value / images[image_number].resx * images[image_number].zoom_number;
                    images[image_number].ell2.Height = cone_sb.Value / images[image_number].resy * images[image_number].zoom_number;
                    images[image_number].ell.Margin = new Thickness((canvas.Width - images[image_number].ell.Width) / 2, (canvas.Height - images[image_number].ell.Height) / 2, 0, 0);
                    images[image_number].ell2.Margin = new Thickness((canvas.Width - images[image_number].ell2.Width) / 2, (canvas.Height - images[image_number].ell2.Height) / 2, 0, 0);
                }
                if (y < 0)
                {
                    images[image_number].ell.Width = ball_sb.Value / images[image_number].resx * images[image_number].zoom_number;
                    images[image_number].ell.Height = ball_sb.Value / images[image_number].resy * images[image_number].zoom_number;
                    images[image_number].ell2.Width = cone_sb.Value / images[image_number].resx * images[image_number].zoom_number;
                    images[image_number].ell2.Height = cone_sb.Value / images[image_number].resy * images[image_number].zoom_number;
                    images[image_number].ell.Margin = new Thickness((canvas.Width - images[image_number].ell.Width) / 2, (canvas.Height - images[image_number].ell.Height) / 2, 0, 0);
                    images[image_number].ell2.Margin = new Thickness((canvas.Width - images[image_number].ell2.Width) / 2, (canvas.Height - images[image_number].ell2.Height) / 2, 0, 0);
                }
                else if (y > 500)
                {
                    images[image_number].ell.Width = ball_sb.Value / images[image_number].resx * images[image_number].zoom_number;
                    images[image_number].ell.Height = ball_sb.Value / images[image_number].resy * images[image_number].zoom_number;
                    images[image_number].ell2.Width = cone_sb.Value / images[image_number].resx * images[image_number].zoom_number;
                    images[image_number].ell2.Height = cone_sb.Value / images[image_number].resy * images[image_number].zoom_number;
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
            //chnage the size of the image (with the ImageDecon2 class) and the size of the ellipse.
            images[image_number].zoom_number= images[image_number].zoom_number*2;
            images[image_number].ell.Width = ball_sb.Value / images[image_number].resx * images[image_number].zoom_number;
            images[image_number].ell.Height = ball_sb.Value / images[image_number].resy * images[image_number].zoom_number;
            images[image_number].ell.Margin = new Thickness((canvas.Width - ball_sb.Value / images[image_number].resx * images[image_number].zoom_number) / 2, (canvas.Height - ball_sb.Value / images[image_number].resy * images[image_number].zoom_number) / 2, 0, 0);
            images[image_number].ell2.Width = cone_sb.Value / images[image_number].resx * images[image_number].zoom_number;
            images[image_number].ell2.Height = cone_sb.Value / images[image_number].resy * images[image_number].zoom_number;
            images[image_number].ell2.Margin = new Thickness((canvas.Width - cone_sb.Value / images[image_number].resx * images[image_number].zoom_number) / 2, (canvas.Height - cone_sb.Value / images[image_number].resy * images[image_number].zoom_number) / 2, 0, 0);
            ImageDecon2.ImageDecon2 id2 = new ImageDecon2.ImageDecon2();
            images[image_number].bmp = id2.DrawImage(images[image_number].f, images[image_number].pixels, images[image_number].zoom_number);
            //BitmapSource bmp = id2.DrawImage(frame, pixels);
            field_img.Source = images[image_number].bmp;
        }

        private void zoomout_btn_Click(object sender, RoutedEventArgs e)
        {
            images[image_number].zoom_number = images[image_number].zoom_number/2;
            if (images[image_number].zoom_number < 1)
            {
                images[image_number].zoom_number = 1;
            }
            images[image_number].ell.Width = ball_sb.Value / images[image_number].resx * images[image_number].zoom_number;
            images[image_number].ell.Height = ball_sb.Value / images[image_number].resy * images[image_number].zoom_number;
            images[image_number].ell.Margin = new Thickness((canvas.Width - ball_sb.Value / images[image_number].resx * images[image_number].zoom_number) / 2, (canvas.Height - ball_sb.Value / images[image_number].resy * images[image_number].zoom_number) / 2, 0, 0);
            images[image_number].ell2.Width = cone_sb.Value / images[image_number].resx * images[image_number].zoom_number;
            images[image_number].ell2.Height = cone_sb.Value / images[image_number].resy * images[image_number].zoom_number;
            images[image_number].ell2.Margin = new Thickness((canvas.Width - cone_sb.Value / images[image_number].resx * images[image_number].zoom_number) / 2, (canvas.Height - cone_sb.Value / images[image_number].resy * images[image_number].zoom_number) / 2, 0, 0);
            ImageDecon2.ImageDecon2 id2 = new ImageDecon2.ImageDecon2();
            images[image_number].bmp = id2.DrawImage(images[image_number].f, images[image_number].pixels, images[image_number].zoom_number);
            //BitmapSource bmp = id2.DrawImage(frame, pixels);
            field_img.Source = images[image_number].bmp;
        }

        private void reset_btn_Click(object sender, RoutedEventArgs e)
        {

            images[image_number].zoom_number = 1;
            //images[imag_avg].ell2
            images[image_number].ell.Width = ball_sb.Value / images[image_number].resx * images[image_number].zoom_number;
            images[image_number].ell.Height = ball_sb.Value / images[image_number].resy * images[image_number].zoom_number;
            images[image_number].ell2.Width = cone_sb.Value / images[image_number].resx * images[image_number].zoom_number;
            images[image_number].ell2.Height = cone_sb.Value / images[image_number].resy * images[image_number].zoom_number;
            images[image_number].ell.Margin = new Thickness((canvas.Width - images[image_number].ell.Width) / 2, (canvas.Height - images[image_number].ell2.Height) / 2, 0, 0);
            images[image_number].ell2.Margin = new Thickness((canvas.Width - images[image_number].ell2.Width) / 2, (canvas.Height - images[image_number].ell2.Height) / 2, 0, 0);

        }

        public class WL_Image
        {
            public int image_id { get; set; }
            public PDBeam WLfield { get; set; }
            public VMS.CA.Scripting.Frame f { get; set; }
            public ushort[,] pixels { get; set; }
            public int zoom_number { get; set; }
            public Ellipse ell { get; set; }
            public Ellipse ell2 { get; set; }
            public BitmapSource bmp { get; set; }
            //public string gantry_angle { get; set; }
            //public string coll_angle { get; set; }
            //public string couch_angle { get; set; }
            public Beam WLBeam { get; set; }
            public double resx { get; set; }
            public double resy { get; set; }
        }
        private void getImages_btn_Click(object sender, RoutedEventArgs e)
        {
            images.Clear();
            int i_num = 0; int zoom_initial = 1;
            //get all the grids that make up all the fields.
            IEnumerable<Grid> grid_collections = Fields.Children.OfType<Grid>();
            foreach (Grid gr in grid_collections)
            {
                //for each field, find the checkbox that denotes all the images.
                IEnumerable<CheckBox> cb_collections = gr.Children.OfType<CheckBox>();
                int field_numb = Convert.ToInt16(gr.Name.Split('_').Last());
                foreach (CheckBox sb in cb_collections)
                {
                    //check if the checkbox for each of the images is checked, if so, create a WLImage in memory.
                    if ((bool)sb.IsChecked)
                    {
                        images.Add(new WL_Image
                        {
                            image_id = i_num,
                            WLfield = PD_AdvAnalysis.MainWindow.plan.Beams[field_numb],
                            zoom_number = zoom_initial,
                            f = PD_AdvAnalysis.MainWindow.plan.Beams[field_numb].PortalDoseImages.Where(j => j.Id == sb.Content.ToString()).First().Image.Frames[0],
                            WLBeam = PD_AdvAnalysis.MainWindow.plan.Beams[field_numb].Beam,
                            resx = PD_AdvAnalysis.MainWindow.plan.Beams[field_numb].PortalDoseImages.First(j => j.Id == sb.Content.ToString()).Image.Frames[0].XRes,
                            resy = PD_AdvAnalysis.MainWindow.plan.Beams[field_numb].PortalDoseImages.First(j => j.Id == sb.Content.ToString()).Image.Frames[0].YRes
                        });
                        images.Last().ell = new Ellipse
                        {
                            Width = ball_sb.Value / images.Last().resx * zoom_initial,
                            Name = "ball_ell",
                            Height = ball_sb.Value / images.Last().resy * zoom_initial,
                            Stroke = System.Windows.Media.Brushes.Black,
                            StrokeThickness = 3,
                            Margin = new Thickness((canvas.Width - ball_sb.Value / images.Last().resx * zoom_initial) / 2, (canvas.Height - ball_sb.Value / images.Last().resy * zoom_initial) / 2, 0, 0),


                        };
                        images.Last().ell2 = new Ellipse
                        {
                            Width = cone_sb.Value / images.Last().resx * zoom_initial,
                            Name = "cone_ell",
                            Height = cone_sb.Value / images.Last().resy * zoom_initial,
                            Stroke = System.Windows.Media.Brushes.Green,
                            StrokeThickness = 3,
                            Margin = new Thickness((canvas.Width - cone_sb.Value / images.Last().resx * zoom_initial) / 2, (canvas.Height - cone_sb.Value / images.Last().resy * zoom_initial) / 2, 0, 0),
                        };
                        //event handlers for moving the ellipse.
                        images[i_num].ell.MouseDown += Ell_MouseDown;
                        images[i_num].ell.MouseUp += Ell_MouseUp;
                        images[i_num].ell.MouseMove += Ell_MouseMove;
                        images[i_num].ell2.MouseDown += Ell_MouseDown;
                        images[i_num].ell2.MouseUp += Ell_MouseUp;
                        images[i_num].ell2.MouseMove += Ell_MouseMove;
                        //get the pixels and the frame of the new image.
                        images[i_num].pixels = new ushort[images[i_num].f.XSize, images[i_num].f.YSize];
                        images[i_num].f.GetVoxels(0, images[i_num].pixels);
                        i_num++;

                    }

                }
            }
            canvas.Children.Clear();
            canvas.Children.Add(images[image_number].ell);
            canvas.Children.Add(images[image_number].ell2);
            //create the bitmap for the first field (image number is 0 until changed with the next or previous button.
            ImageDecon2.ImageDecon2 id2 = new ImageDecon2.ImageDecon2();
            images[image_number].bmp = id2.DrawImage(images[image_number].f, images[image_number].pixels, images[image_number].zoom_number);
            //BitmapSource bmp = id2.DrawImage(frame, pixels);
            field_img.Source = images[image_number].bmp;
        }

        private void previous_btn_Click(object sender, RoutedEventArgs e)
        {
            canvas.Children.Clear();
            image_number--;
            image_numbermax = images.Count();
            //reset to the last image if previous is pushed on the first image.
            if (image_number < 0)
            {
                image_number = image_numbermax - 1;
            }
            canvas.Children.Add(images[image_number].ell);
            canvas.Children.Add(images[image_number].ell2);
            //generate the bitmap for the previous image. 
            ImageDecon2.ImageDecon2 id2 = new ImageDecon2.ImageDecon2();
            images[image_number].bmp = id2.DrawImage(images[image_number].f, images[image_number].pixels, images[image_number].zoom_number); ;
            field_img.Source = images[image_number].bmp;
        }

        private void next_btn_Click(object sender, RoutedEventArgs e)
        {
            canvas.Children.Clear();
            image_number++;
            //if the image number exceeds the maximum number count, return to the first iamge.
            image_numbermax = images.Count();
            if (image_number >= image_numbermax)
            {
                image_number = 0;
            }
            canvas.Children.Add(images[image_number].ell);
            canvas.Children.Add(images[image_number].ell2);
            //draw the bitmap for the next image.
            ImageDecon2.ImageDecon2 id2 = new ImageDecon2.ImageDecon2();
            images[image_number].bmp = id2.DrawImage(images[image_number].f, images[image_number].pixels, images[image_number].zoom_number);
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
                    c.Width = ball_sb.Value / images.Last().resx;
                    c.Height = ball_sb.Value / images.Last().resy;
                }
                if (c.Name == "cone_ell")
                {
                    c.Width = cone_sb.Value / images.Last().resx;
                    c.Height = cone_sb.Value / images.Last().resy;
                }
            }

        }


    }
}






