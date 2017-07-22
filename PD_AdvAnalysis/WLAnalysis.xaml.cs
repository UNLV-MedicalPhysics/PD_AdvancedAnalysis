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
            foreach(PDBeam pb in ps.Beams)
            {
                CheckBox cb = new CheckBox();
                cb.Content = pb.Id;
                Fields.Children.Add(cb);
            }
        }

        private void autodetect_btn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void manualydetect_btn_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
