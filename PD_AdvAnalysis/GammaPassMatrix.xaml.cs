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
//using PdfSharp.Pdf;
//using PdfSharp.Drawing;
using Microsoft.Win32;
using System.IO;
using System.IO.Packaging;
using System.Printing;
using System.Windows.Xps;
using System.Windows.Xps.Packaging;
using System.Runtime.InteropServices;


namespace PD_AdvAnalysis
{
    /// <summary>
    /// Interaction logic for GammaPassMatrix.xaml
    /// </summary>

    public partial class GammaPassMatrix : UserControl
    {
        //get measured field from MainWindow code.
        ComboBox meas_ddl = System.Windows.Application.Current.MainWindow.FindName("meas_ddl") as ComboBox;
        ComboBox comp_ddl = System.Windows.Application.Current.MainWindow.FindName("comp_ddl") as ComboBox;
        public Patient newcontext;//save these guys for use later.  
        public PDBeam field;
        PortalDoseImage fieldm;
        public GammaPassMatrix()
        {
            InitializeComponent();

        }
        private void calc_btn_Click(object sender, RoutedEventArgs e)
        {
            //pulling initial values from the MainWindow class. 
            newcontext = PD_AdvAnalysis.MainWindow.newcontext;
            field = PD_AdvAnalysis.MainWindow.field;
            if (newcontext == null)
            {
                MessageBox.Show("No Patient Selected.");
                return;
            }
            gamma_grd.Children.Clear();
            //here's where all the magic happens!!
            //check the fields are selected
            if (meas_ddl.SelectedIndex == -1)
            {
                MessageBox.Show("Please select a field");
            }
            else if (String.IsNullOrEmpty(startdd_txt.Text) || string.IsNullOrEmpty(startdta_txt.Text) || string.IsNullOrEmpty(enddd_txt.Text) || string.IsNullOrEmpty(enddta_txt.Text) || string.IsNullOrEmpty(deldd_txt.Text) || string.IsNullOrEmpty(deldta_txt.Text) || string.IsNullOrEmpty(tol_txt.Text))
            {
                MessageBox.Show("Please input all numeric parameters in the appropriate box");
            }
            else if (comp_ddl.SelectedItem.ToString() == "No Predicted Image")
            {
                MessageBox.Show("This field contains no predicted image.");
            }
            else
            {
                //check for any empty mandatory boxes with this bool.
                bool any_empty = false;
                //This portion of the code makes the textboxes red if they are empty
                List<Control> mandatory_boxes = new List<Control>() { startdd_txt, enddd_txt, deldd_txt, startdta_txt, enddta_txt, deldta_txt, tol_txt };
                foreach (Control c in mandatory_boxes)
                {
                    double test_double;
                    if (!Double.TryParse((c as TextBox).Text, out test_double))
                    {

                        (c as TextBox).Focus();
                        (c as TextBox).BorderBrush = Brushes.Red;
                        (c as TextBox).BorderThickness = new Thickness(2);
                        any_empty = true;
                    }
                    else
                    {
                        (c as TextBox).BorderBrush = Brushes.Transparent;
                    }
                }
                //this is testing to see if the the threshold percentage is added.
                if ((bool)threshold_chk.IsChecked)
                {
                    double test_double2;
                    if (!Double.TryParse((threshold_txt).Text, out test_double2))
                    {
                        (threshold_txt).Focus();
                        (threshold_txt).BorderBrush = Brushes.Red;
                        (threshold_txt).BorderThickness = new Thickness(2);
                        any_empty = true;
                    }
                    else
                    {
                        (threshold_txt).BorderBrush = Brushes.Transparent;
                    }
                }
                //if the test contains a necessary test parameter, then make sure the test parameter is selected.
                //for instance, the test gamma area < _____ must be > ______ needs to have the parameters defined.
                int[] threshold_parm = new int[] { 4, 5, 8, 9, 12, 13 };

                if (threshold_parm.Contains(EvalTestKind_cmb.SelectedIndex))
                {
                    double test_double3;
                    if (!Double.TryParse((testparam_txt).Text, out test_double3))
                    {
                        (testparam_txt).Focus();
                        (testparam_txt).BorderBrush = Brushes.Red;
                        (testparam_txt).BorderThickness = new Thickness(2);
                        any_empty = true;
                    }
                    else
                    {
                        (testparam_txt).BorderBrush = Brushes.Transparent;
                    }
                }
                //final catch in case any necessary inputs are missed. 
                if (any_empty) { return; }
                //get beams
                fieldm = field.PortalDoseImages.First(i => i.Id == meas_ddl.SelectedItem.ToString());
                //don't need to do the where clause for the predicted.
                double startdd = Convert.ToDouble(startdd_txt.Text); double endd = Convert.ToDouble(enddd_txt.Text); double deldd = Convert.ToDouble(deldd_txt.Text);
                double startdta = Convert.ToDouble(startdta_txt.Text); double enddta = Convert.ToDouble(enddta_txt.Text); double deldta = Convert.ToDouble(deldta_txt.Text);
                double tol = Convert.ToDouble(tol_txt.Text) / 100;
                double parm;
                Double.TryParse(testparam_txt.Text, out parm);

                //setup portal dosimetry analysis template.
                IEnumerable<EvaluationTestDesc> tested = new List<EvaluationTestDesc> { new EvaluationTestDesc((EvaluationTestKind)EvalTestKind_cmb.SelectedIndex, parm, tol, false) };


                //new EvaluationTestDesc()
                //loop through all testable queries
                //columns go here.;
                //lay down the dta and dd labels.
                int bw = 50; int bh = 25;
                int marginx = bw; int marginy = bh;
                for (double i = startdd; i <= endd; i += deldd)
                {
                    TextBox dbox = new TextBox(); dbox.IsReadOnly = true; dbox.Text = String.Format("{0}%", i);
                    dbox.Width = bw; dbox.Background = Brushes.White; dbox.BorderBrush = Brushes.Black;
                    dbox.HorizontalAlignment = HorizontalAlignment.Left; dbox.VerticalAlignment = VerticalAlignment.Top;
                    dbox.Height = bh;
                    dbox.Margin = new Thickness(marginx, 0, 0, 0);
                    gamma_grd.Children.Add(dbox);
                    marginx += bw;
                }
                for (double j = startdta; j <= enddta; j += deldta)
                {
                    TextBox dbox = new TextBox(); dbox.IsReadOnly = true; dbox.Text = String.Format("{0}mm", j);
                    dbox.Width = bw; dbox.Background = Brushes.White; dbox.BorderBrush = Brushes.Black;
                    dbox.HorizontalAlignment = HorizontalAlignment.Left; dbox.VerticalAlignment = VerticalAlignment.Top;
                    dbox.Height = bh;
                    dbox.Margin = new Thickness(0, marginy, 0, 0);
                    marginy += bh;
                    gamma_grd.Children.Add(dbox);

                }
                marginx = bw; marginy = 0;

                //now calculate all the boxes for the matrix.
                for (double i = startdta; i <= enddta; i += deldta)
                {
                    //lay down a new header for the column.
                    marginy += bh;
                    marginx = 0;
                    //rows go here
                    double margins_txt;
                    Double.TryParse(marg_txt.Text, out margins_txt);
                    double th_txt;
                    double.TryParse(threshold_txt.Text, out th_txt);
                    int analysisMode = (bool)abs_rdb.IsChecked ? 0 : 2;
                    for (double j = startdd; j <= endd; j += deldd)
                    {
                        marginx += bw;
                        //lay down an initial grid for the dta and dd
                        TextBox header_box = new TextBox();
                        //modify the template
                        PDTemplate template1 = new PDTemplate(false, false, false, (AnalysisMode)analysisMode, (NormalizationMethod)Normalizaton_cmb.SelectedIndex, (bool)threshold_chk.IsChecked, th_txt, (ROIType)ROITypes_cmb.SelectedIndex, margins_txt, j / 100, i, false, tested);
                        //apply the template to the analysis.
                        PDAnalysis analysis = fieldm.CreateTransientAnalysis(template1, field.PredictedDoseImage);
                        //This code determines if the Gamma Test Parameters are in absolute or relative value
                        int[] selec_in = new int[] { 0, 1, 4, 5, 8, 9,12,13 };
                        double gamma_pass;

                        if (selec_in.Contains(EvalTestKind_cmb.SelectedIndex))
                        {
                            gamma_pass = analysis.EvaluationTests.First().TestValue * 100;
                        }
                        else
                        {
                            gamma_pass = analysis.EvaluationTests.First().TestValue;
                        }
                        //The Code below shows the results from the gamma test. It is color coded to show where the gamma passes or failes.
                        TextBox gamma_box = new TextBox(); gamma_box.IsReadOnly = true; gamma_box.Text = gamma_pass.ToString("F3");
                        gamma_box.Width = bw; gamma_box.Height = bh; gamma_box.Background = gamma_pass < tol * 100 ? Brushes.Pink : Brushes.LightGreen; gamma_box.BorderBrush = Brushes.Black;
                        gamma_box.HorizontalAlignment = HorizontalAlignment.Left; gamma_box.VerticalAlignment = VerticalAlignment.Top;
                        gamma_box.Margin = new Thickness(marginx, marginy, 0, 0);
                        gamma_grd.Children.Add(gamma_box);
                    }
                }
            }
        }
       
        private void EvalTestKind_cmb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //string[] values = new string[]{"0, 1, 2, 3, 6, 7, 10, 11" };
            int[] sel_in = new int[] { 0, 1, 2, 3, 6, 7, 10, 11 };

            if (EvalTestKind_cmb.SelectedIndex == -1)
            {
                MessageBox.Show("Please select a gamma analysis parameter");
            }

            else if (!sel_in.Contains(EvalTestKind_cmb.SelectedIndex))
            {
                testparam_txt.IsEnabled = true;
            }
            else
            {
                testparam_txt.IsEnabled = false;
            }
        }
        //under construction.
        private void prindPDF_btn_Click(object sender, RoutedEventArgs e)
        {
            //Build the pdf
            /*int Xmargin = 36;
            int Ymargin = 36;
            PdfDocument doc;
            PdfPage page;
            XGraphics gfx;
            //set up page and header
            doc = new PdfDocument();
            page = doc.AddPage();
            //page = new PdfPage();
            gfx = XGraphics.FromPdfPage(page);
            XRect rect = new XRect(Xmargin, Ymargin, page.Width - 2 * Xmargin, 50);
            gfx.DrawRectangle(new XPen(XColors.Black, 2), rect);
            rect.X += 4; rect.Y += 4;
            rect.Width -= 8; rect.Height -= 8;
            XFont font = new XFont("Arial", 12, XFontStyle.Regular);
            XFont font2 = new XFont("Arial", 11, XFontStyle.Italic);
            XStringFormat format = new XStringFormat();
            format.Alignment = XStringAlignment.Near;
            format.LineAlignment = XLineAlignment.Near;
            PdfSharp.Drawing.Layout.XTextFormatter tf = new PdfSharp.Drawing.Layout.XTextFormatter(gfx);
            //Provide header information for the patient
            tf.DrawString(String.Format("Gamma Pass Table for:{0}", newcontext), font, XBrushes.Black, rect, format);
            tf.DrawString(String.Format("Field:{0}", meas_ddl.SelectedItem.ToString()), font, XBrushes.Black, rect, format);
            tf.DrawString(String.Format("\nDate Run:{0}", DateTime.Now.ToString("MM/dd/yyyy")), font, XBrushes.Black, rect, format);
            //save the grid into a image and insert the image unto PdfSharp
            string filename = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\testpdf.pdf";
            doc.Save(filename);
            doc.Close();
            
          
            MemoryStream MemoryStreatm1 = new MemoryStream();
            Package package = Package.Open(MemoryStreatm1, FileMode.Create);
            XpsDocument doc1 = new XpsDocument(package);
            XpsDocumentWriter writter = XpsDocument.CreateXpsDocumentWriter(doc1);
            writter.Write(gamma_grd);
            
            doc1.Close();
            package.Close();

            string newfilename = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\testpdf2.pdf";
            var pdfXpsDoc = PdfSharp.Xps.XpsModel.XpsDocument.Open(MemoryStreatm1);
            //var d  = 
            PdfSharp.Xps.XpsConverter.Convert(pdfXpsDoc,newfilename,0);
            /*PdfSharp.Xps.XpsModel.FixedDocument fixed_doc = pdfXpsDoc.GetDocument();
            PdfDocument pdfdoc = new PdfDocument();
            PDFRenderer renderer = new PDFRenderer();*/

            MessageBox.Show("Hello");

        }
    }
}
