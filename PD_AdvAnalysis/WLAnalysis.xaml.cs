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


                //Fields.Children.Add(cb);

                ScrollViewer sv = new ScrollViewer();
                sv.HorizontalScrollBarVisibility = ScrollBarVisibility.Visible;
                sv.HorizontalAlignment = HorizontalAlignment.Left;
                sv.VerticalAlignment = VerticalAlignment.Top;
                //sv.Height = 60;
                //sv.Width = 400;

                sv.Margin = new Thickness(5, margin_height + 10, 0, 0);
                int margin_width = 30;
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
                RowDefinition rd0 = new RowDefinition();
                sp.RowDefinitions.Add(rd0);
                RowDefinition rd = new RowDefinition();
                sp.RowDefinitions.Add(rd);
                RowDefinition rd2 = new RowDefinition();
                sp.RowDefinitions.Add(rd2);
                int col = 0;
                TextBlock cb = new TextBlock();
                cb.Text = pb.Id;
                cb.FontSize = 14;
                cb.TextDecorations = TextDecorations.Underline;
                cb.HorizontalAlignment = HorizontalAlignment.Left;
                cb.VerticalAlignment = VerticalAlignment.Top;
                cb.Margin = new Thickness(5);
                Grid.SetRow(cb, 0);
                Grid.SetColumn(cb, 0);
                sp.Children.Add(cb);
                foreach (PortalDoseImage pdi in pb.PortalDoseImages)
                {
                    //ColumnDefinition cd = new ColumnDefinition();
                    //sp.ColumnDefinitions= new ColumnDefinitionC
                    //sp.ColumnDefinitions.Add(cd);
                    CheckBox cb2 = new CheckBox();
                    cb2.Content = pdi.Id;
                    cb2.HorizontalAlignment = HorizontalAlignment.Left;
                    cb2.VerticalAlignment = VerticalAlignment.Top;
                    cb2.Margin = new Thickness(5);

                    //RowDefinition rd = new RowDefinition();
                    //sp.RowDefinitions.Add(rd);

                    string StartDate = pdi.Session.SessionDate.ToString("MM/dd/yyyy");
                    TextBlock text_blck = new TextBlock();
                    text_blck.Text = StartDate;
                    text_blck.HorizontalAlignment = HorizontalAlignment.Left;
                    text_blck.VerticalAlignment = VerticalAlignment.Top;
                    text_blck.Margin = new Thickness(5);
                    //margin_width = 60;
                    //sp.Children.Add(cb2);
                    //sp.Children.Add(text_blck);
                    Grid.SetRow(cb2, 1);
                    Grid.SetColumn(cb2, col);
                    sp.Children.Add(cb2);
                    Grid.SetRow(text_blck, 2);
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
            foreach (WL_Image img in images)
            {

                int x_line = 100;
                int y_line = 100;
                List<Tuple<int, int, int, int>> peak_points_x = new List<Tuple<int, int, int, int>>();
                List<Tuple<int, int, int, int>> peak_points_y = new List<Tuple<int, int, int, int>>();
                List<int> peak_difference_x = new List<int>();
                List<int> peak_difference_y = new List<int>();
                List<int> peak_difference_xBall = new List<int>();
                List<int> peak_difference_yBall = new List<int>();
                List<float[]> gradients_x = new List<float[]>();
                for (int y_line1 = Convert.ToInt32((img.pixels.GetLength(1)/2) - 30); y_line1 < Convert.ToInt32((img.pixels.GetLength(1)/2) + 30);y_line1++)
                {
                    //this will get the gradient of the row at y_line.
                    float[] gradient = GetGrad(img, x_line, y_line1);
                    //determine where the gradient first peaks at a maximum. 
                    //write that locaiton to a class.
                    gradients_x.Add(gradient);
                    int cone_edge = gradient.ToList().IndexOf(gradient.Max());
                    int cone_edge_end = gradient.ToList().IndexOf(gradient.Min());
                    //double coneX = (cone_edge - cone_edge_end) / 2;
                    int ball_edge = 0;
                    int ball_edge_end = 0;
                    peak_points_x.Add(new Tuple<int, int, int, int>(cone_edge, cone_edge_end, ball_edge, ball_edge_end));
                    peak_difference_x.Add(cone_edge_end - cone_edge);
                }
                double cone_x = (Convert.ToInt32(peak_difference_x.Max())/2);
                using (StreamWriter sw = new StreamWriter(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\testgradient.csv"))
                {
                    //foreach(float[] f in gradients_x)
                    //{
                    //    sw.WriteLine(String.Join(",", f));
                    //}
                    
                    for(int i = 0; i < gradients_x[0].Length; i++)
                    {
                        string row = "";
                        for(int j = 0; j < gradients_x.Count; j++)
                        {
                            row += gradients_x[j][i].ToString()+",";
                        }
                        sw.WriteLine(row);
                    }
                    sw.Flush();
                }
                    // double coneX = (cone_edge - cone_edge_end) / 2;
                    for (int x_line1 = Convert.ToInt32(img.pixels.GetLength(1)/2) - 30; x_line1 < Convert.ToInt32(img.pixels.GetLength(1)/2) + 30; x_line1++)
                    {
                        float[] gradient = GetGrad(img, x_line, x_line1);
                        //determine where the gradient first peaks at a maximum. 
                        //write that locaiton to a class.
                        int ball_edgeY = 0;
                        int ball_edge_endY = 0;
                        int cone_edgeY = gradient.ToList().IndexOf(gradient.Max());
                        int cone_edge_endY = gradient.ToList().IndexOf(gradient.Min());
                        peak_points_y.Add(new Tuple<int, int, int, int>(cone_edgeY, cone_edge_endY, ball_edgeY, ball_edge_endY));
                        peak_difference_y.Add(cone_edge_endY - cone_edgeY);
                    }
                
                double cone_y = (Convert.ToInt32(peak_difference_y.Max())/ 2);
                //this if for the ball
                for (int y_line1 = Convert.ToInt32(img.pixels.GetLength(1)/2) - 20; y_line1 < Convert.ToInt32(img.pixels.GetLength(1)/2) + 20;y_line1++)
                {
                    //this will get the gradient of the row at y_line.
                    float[] gradient = GetGrad(img, x_line, y_line1);
                    //determine where the gradient first peaks at a maximum. 
                    //write that locaiton to a class.
                    int ball_edge = gradient.ToList().IndexOf(gradient.Max());
                    int ball_edge_end = gradient.ToList().IndexOf(gradient.Min());
                    //double coneX = (cone_edge - cone_edge_end) / 2;
                    int cone_edge = 0;
                    int cone_edge_end = 0;
                    peak_points_x.Add(new Tuple<int, int, int, int>(cone_edge, cone_edge_end, ball_edge, ball_edge_end));
                    peak_difference_xBall.Add(ball_edge_end - ball_edge);
                }
                double ball_x = (Convert.ToInt32(peak_difference_xBall.Max() )/2);
                for (int x_line1 = (Convert.ToInt32(img.pixels.GetLength(1)/2)) - 20; x_line1 <( Convert.ToInt32(img.pixels.GetLength(1))/2) + 20;x_line1++)
                {
                    float[] gradient = GetGrad(img, x_line, x_line1);
                    //determine where the gradient first peaks at a maximum. 
                    //write that locaiton to a class.
                    int cone_edgeY = 0;
                    int cone_edge_endY = 0;
                    int ball_edgeY = gradient.ToList().IndexOf(gradient.Max());
                    int ball_edge_endY = gradient.ToList().IndexOf(gradient.Min());
                    peak_points_y.Add(new Tuple<int, int, int, int>(cone_edgeY, cone_edge_endY, ball_edgeY, ball_edge_endY));
                    peak_difference_yBall.Add((ball_edge_endY - ball_edgeY));
                }
                double ball_y = (Convert.ToInt32(peak_difference_yBall.Max()) / 2);
                double position_x = (ball_x - cone_x) * img.resx / img.zoom_number;
                double position_y = (ball_y - cone_y) * img.resy / img.zoom_number;
                MessageBox.Show(string.Format(" The distance from the center is the X direction is: {0} mm\n The distance from the cetner in the Y direction is:{1} mm", position_x.ToString("F2"), position_y.ToString("F2")));
            }
        }
        private float[] GetGrad(WL_Image image, int line_x, int y_line)
        {
            //throw new NotImplementedException();
            float[] averaged_grad = new float[Convert.ToInt32(line_x*2)];
            float sumDiff = 0;// int count = 0;






                int indexer = 0;
            for (int i = (Convert.ToInt32(image.pixels.GetLength(1)/2) - line_x); i < (Convert.ToInt32(image.pixels.GetLength(1)/2) + line_x) ; i++)
            {                
                //for (int j = Convert.ToInt32(line_y - rows_AVG / 2); j < Convert.ToInt32(line_y + rows_AVG / 2); j++)
                //{
                int j = y_line;
                sumDiff += image.pixels[j,i + 1] - image.pixels[j,i];
                //count++;
                // averaged_grad[Convert.ToInt32(profile_ln.Margin.Left) + i - line_x] = sumDiff / count;
                averaged_grad[indexer] = sumDiff;
                indexer++;
                 //}
              
            }
            return averaged_grad;
           
        }
        private void manualydetect_btn_Click(object sender, RoutedEventArgs e)
        {
            List<WL_Results> wlresults = new List<WL_Results>();
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
                wlresults.Add(new WL_Results());
                position = (ima.ell.Margin.Left - (canvas.Width - ima.ell.Width) / 2);
                //position= images[image_number].ell
                position1 = ima.ell.Margin.Top - (canvas.Height - ima.ell.Height) / 2;
                position2 = ima.ell2.Margin.Left - (canvas.Width - ima.ell2.Width) / 2;
                position3 = ima.ell2.Margin.Top - (canvas.Height - ima.ell2.Height) / 2;
                //double g_angle;
                wlresults.Last().gantry_angle = ima.WLBeam.ControlPoints.First().GantryAngle;
                //double ps_angle
                wlresults.Last().psupport_angle = ima.WLBeam.ControlPoints.First().PatientSupportAngle;
                //rounding for half angles for Convert.ToInt32 will give the nearest even number of a .5 is found (i.e. 4.5 rounds to 4 and 5.5 roundes to 6).
                wlresults.Last().rounded_gantry = Convert.ToInt32(wlresults.Last().gantry_angle);
                wlresults.Last().rounded_psupport = Convert.ToInt32(wlresults.Last().psupport_angle);
                //double lat_disp;
                //double vert_disp;
                //double long_disp;
                //X_disp = (ball pos x - cone pos x)*22.5/96*(Math.Cos(gantry) + Math.Sin(gantry))
                //Y_disp = (ball pos y - cone pos y)*22.5/96
                //Lat = X_disp/cos(gantry)
                //
                wlresults.Last().x_display = -4 * ((position - position2) * images[image_number].resx / images[image_number].zoom_number);// * Math.Cos(g_angle * 180 / Math.PI);
                //vert_disp = ((position - position2) * images[image_number].resx / images[image_number].zoom_number);// * Math.Sin(g_angle * 180 / Math.PI);
                wlresults.Last().y_display = -4 * (position1 - position3) * images[image_number].resy / images[image_number].zoom_number;

                //Add Longitudinal displacement.

                //think about whether the direction for the micrometer will be CW or CCW.

                System.Windows.Controls.Label newLabel = new System.Windows.Controls.Label();
                newLabel.HorizontalAlignment = HorizontalAlignment.Left;
                newLabel.VerticalAlignment = VerticalAlignment.Top;
                newLabel.Width = 200;
                newLabel.Height = 100;
                string newLabel_txt = string.Format(" Field ID: {0} \n Gantry Angle: {1} \n Coouch Angle {2}", ima.f.Image.Id, wlresults.Last().rounded_gantry, wlresults.Last().rounded_psupport);
                newLabel.Content = newLabel_txt;
                canvas.Children.Clear();
                canvas.Children.Add(newLabel);

                //MessageBox.Show(string.Format(" Field ID: {7} \n Gantry angle: {8}\nThe center of the Ball is: X: {0}  Y: {1} \nThe center of the Cone is: X: {2}  Y: {3} \n Move ball {4} mm in the lateral direction \n Move ball {5} mm in vertical direction \n Move the ball {6} mm in longitudinal direction", position, position1, position2, position3, wlresults.Last().x_display, wlresults.Last().y_display, ima.f.Image.Id, wlresults.Last().rounded_gantry, wlresults.Last().rounded_psupport));
            }
            //display markers on canvas. 
            //for this first round, just make everything an ellipse with the color changing based on the gantry angle.
            //to change colors, just create an array of colors that will be longer than your array of gantry angles.
            //can call up colors using Color.FromName(string colorname);
            String[] colors = new string[] { "Red", "Blue", "Green", "Orange", "Yellow", "Purple", "Gray" };
            //get unique gantry angles to the nearest degree.
            int[] unique_gantry = wlresults.Select(x => x.rounded_gantry).Distinct().ToArray();
            //create dictionary where key is this unique value option and value is the color.  
            Dictionary<int, string> color_lookup = new Dictionary<int, string>();
            for (int i = 0; i < unique_gantry.Count(); i++)
            {
                color_lookup.Add(unique_gantry[i], colors[i]);
            }
            //call up the color by using color_lookup[wlresults.rounded_gantry];
            //var color = Color.FromName(color_lookup);
            //var color = System.Drawing.Color.FromName(color_lookup).ToString;
            visualCanvas.Children.Clear();
            foreach (WL_Results m in wlresults)
            {

                //var brush = new SolidColorBrush(System.Drawing.Color.FromName(color_lookup[m.rounded_gantry]));
                var brush = System.Drawing.Color.FromName(color_lookup[m.rounded_gantry]);
                var color_brsh = new SolidColorBrush(System.Windows.Media.Color.FromArgb(brush.A, brush.R, brush.G, brush.B));

                var ellipse = new Ellipse
                {
                    Stroke = color_brsh,
                    Fill = color_brsh,
                    StrokeThickness = 1,
                    Height = 5,
                    Width = 5,


                };
                ellipse.Margin = new Thickness((visualCanvas.Width - ellipse.Width) / 2 - m.x_display, (visualCanvas.Height - ellipse.Height) / 2 - m.y_display, 0, 0);

                visualCanvas.Children.Add(ellipse);
                double distance_x = (ellipse.Margin.Left - (visualCanvas.Width + ellipse.Width) / 2);
                double distance_y = (ellipse.Margin.Top - (visualCanvas.Height + ellipse.Height) / 2);
                double x_diplayposition = m.x_display / 4;
                double y_displayposition = m.y_display / 4;
                ellipse.ToolTip = string.Format("Field ID: {0} \n Gantry Angle: {1} \n Coouch Angle {2} \n The distance from the center is the X direction is: {3} \n The distance from the cetner in the Y direction is:{4}"
                  , images.Last().image_id, wlresults.Last().rounded_gantry.ToString("F2"), wlresults.Last().rounded_psupport.ToString("F2"), x_diplayposition.ToString("F2") , y_displayposition.ToString("F2"));
            }
            var brush1 = new SolidColorBrush(Colors.Black);
            var ellipse1 = new Ellipse
            {
                Stroke = brush1,
                Fill = brush1,
                StrokeThickness = 1,
                Height = 5,
                Width = 5,

            };
            ellipse1.Margin = new Thickness((visualCanvas.Width - ellipse1.Width) / 2, (visualCanvas.Height - ellipse1.Height) / 2, 0, 0);
            visualCanvas.Children.Add(ellipse1);
            Binding bnd = new Binding("Value") { ElementName = "slValue" };//binding to dummy element that has the multiplier
            var threshold_ellipse = new Ellipse
            {
                Stroke = brush1,
                StrokeThickness = 1,
                Height = 50,
                Width = 50,
            };
            //threshold_ellipse.Margin = new Thickness((visualCanvas.Width - threshold_ellipse.Width) / 2, (visualCanvas.Height - threshold_ellipse.Height) / 2, 0, 0);
            threshold_ellipse.HorizontalAlignment = HorizontalAlignment.Center;

            threshold_ellipse.VerticalAlignment = VerticalAlignment.Center;
            BindingOperations.SetBinding(threshold_ellipse, Ellipse.WidthProperty, bnd);
            BindingOperations.SetBinding(threshold_ellipse, Ellipse.HeightProperty, bnd);
            //BindingOperations.SetBinding(threshold_ellipse, Ellipse.MarginProperty, bnd);
            visualGrid.Children.Add(threshold_ellipse);


            //List<int>[] unique_gantry = int.Parse(wlresults.Select(x=>x.gantry_angle))
        }
        //public class myEllipse : Ellipse
        //{

        //}
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
            images[image_number].zoom_number = images[image_number].zoom_number * 2;
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
            images[image_number].zoom_number = images[image_number].zoom_number / 2;
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
        public class WL_Results
        {
            public double x_display { get; set; }
            public double y_display { get; set; }
            public double gantry_angle { get; set; }
            public double psupport_angle { get; set; }
            public int rounded_gantry { get; set; }
            public int rounded_psupport { get; set; }
            public Ellipse ell3 { get; set; }
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
                        System.Windows.Controls.Label newLabel = new System.Windows.Controls.Label();
                        newLabel.HorizontalAlignment = HorizontalAlignment.Left;
                        newLabel.VerticalAlignment = VerticalAlignment.Top;
                        newLabel.Width = 200;
                        newLabel.Height = 100;
                        string newLabel_txt = string.Format(" Field ID: {0} \n Gantry Angle: {1} \n Coouch Angle {2}",
                            images[i_num].f.Image.Id, images[i_num].WLBeam.ControlPoints.First().GantryAngle.ToString("F2"),
                            images[i_num].WLBeam.ControlPoints.First().PatientSupportAngle.ToString("F2"));
                        newLabel.Content = newLabel_txt;
                        //canvas.Children.Clear();
                        Panel.SetZIndex(newLabel, -2);
                        canvas.Children.Add(newLabel);

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

        private void threshold_sb_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            //    foreach( Shape l in visualCanvas.Children)
            //    {
            //        if (l.Name == "threshold_sb")
            //        {
            //            l.Width = threshold_sb.Value;
            //            l.Height = threshold_sb.Value;
            //        }
            //        visualCanvas.Children.Add(ell3);
            //    }

        }

        private void threshold_sb_TargetUpdated(object sender, DataTransferEventArgs e)
        {

        }

        //private void visualAnal_btn_Click(object sender, RoutedEventArgs e)
        //{

        //}

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






