using System;
using System.Drawing;
using System.Drawing.Imaging;
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
using VMS.CA.Scripting;//this assembly houses more generic image information handling.
using VMS.DV.PD.Scripting;//this one assembly going to mostly take the place of VMS.TPS.Common.Model.API


namespace PD_AdvAnalysis
{
    /// <summary>
    /// Make sure your project targets x86 framework for portal dosimetry or it will not run.
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window
    {
        //the patient context will be in the PD.Scripting namespace
        public static Patient newcontext;//save these guys for use later.
        public static PDPlanSetup plan;
        Course course;
        public static PDBeam field;
        PortalDoseImage fieldm;
        PortalDoseImage fieldc;
        //load up an application
        //username and password is hard-coded now, but should be left to null,null in the release of the application.
        public VMS.DV.PD.Scripting.Application PDapp = VMS.DV.PD.Scripting.Application.CreateApplication("Physicist", "Physicist"); 
        public MainWindow()
        {          
            InitializeComponent();       

        }
        //get the patient.
        private void id_btn_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(id_txt.Text))//no entry for patient ID
            {
                MessageBox.Show("Must give a patient ID");
            }
            else
            {
                //get the patient from the patient ID
                newcontext = PDapp.OpenPatientById(id_txt.Text);

                //first clear the plan boxes.
                course_ddl.Items.Clear();
                //now add a blank one 

                if (newcontext == null)//incorrect entry for patient ID
                {
                    MessageBox.Show("Patient ID not found.");

                }
                else
                {
                    course_ddl.IsEnabled = true;
                    //course_btn.IsEnabled = true;
                    //now loop through all the plans in this patient and find list them as combobox items.
                    foreach (Course cs in newcontext.Courses)
                    {
                        course_ddl.Items.Add(cs.Id);
                    }
                }
            }
        }
        /* We used to use buttons to open the plans, now they are on combobox selections.
        private void plan_btn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void course_btn_Click(object sender, RoutedEventArgs e)
        {

        }
        */
        private void comp_ddl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //this will be selected automatically.

        }

        private void meas_ddl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //this field is only used for reference of the field, no action necessary.
        }

        /*private void field_btn_Click(object sender, RoutedEventArgs e)
        {

        }
        */
        private void course_ddl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (course_ddl.SelectedIndex == -1)
            {
                MessageBox.Show("Please select a course from the dropdown list.");
            }
            else
            {
                //get the course from the dropdown.
                course = newcontext.Courses.Where(i => i.Id == course_ddl.SelectedItem.ToString()).First();
                //enable plan lists
                plan_ddl.IsEnabled = true;
                //plan_btn.IsEnabled = true;
                //clear the plan box.
                plan_ddl.Items.Clear();
                //loop through plans and add to list.
                foreach (PlanSetup pdps in course.PlanSetups)
                {
                    plan_ddl.Items.Add(pdps.Id);
                }
            }
        }

        private void plan_ddl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (plan_ddl.SelectedIndex == -1)
            {
                MessageBox.Show("Please select a plan in the dropdown list.");
            }
            else
            {

                PlanSetup plan_holder = course.PlanSetups.Where(i => i.Id == plan_ddl.SelectedItem.ToString()).First();
                //:This is how you go from plansetup to pdplansetup check my swag.
                //check to see if the course has any PDbeams.               
                try
                {
                    plan = newcontext.PDPlanSetups.Where(i => i.PlanSetup == plan_holder).First();//equal object types!!
                                                                                                  //do all field stuff
                    field_ddl.IsEnabled = true;
                    //field_btn.IsEnabled = true;
                    field_ddl.Items.Clear();
                    //check to see if any PDbeams exist.
                    foreach (PDBeam beams in plan.Beams)
                    {
                        field_ddl.Items.Add(beams.Id);
                    }
                }
                catch
                {
                    MessageBox.Show("This plan contains no Portal Dosimetry fields.");
                }
            }
        }

        private void field_ddl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //enable the field lists
            //comp_ddl.IsEnabled = true;
            if (field_ddl.SelectedIndex == -1)
            {
                MessageBox.Show("Select a field to analyze");
            }
            else
            {
                field = plan.Beams.Where(i => i.Id == field_ddl.SelectedItem.ToString()).First();
                meas_ddl.IsEnabled = true;
                comp_ddl.Items.Clear();
                meas_ddl.Items.Clear();
                //comp_ddl.Items.Add("");
                //first the portal dose images
                foreach (PortalDoseImage pdi in field.PortalDoseImages)
                {
                    meas_ddl.Items.Add(pdi.Id);
                }
                //each field only has one predicted dose image so force the predicted dose image when the portal dose image is selected.
                if (field.PredictedDoseImage != null)
                {
                    comp_ddl.Items.Add(field.PredictedDoseImage.Id);
                    comp_ddl.SelectedValue = field.PredictedDoseImage.Id;
                }
                else
                {
                    //there is no predicted image.
                    comp_ddl.Items.Add("No Predicted Image");
                    comp_ddl.SelectedIndex = 0;
                    //this exact string will be handled in GammaPassMatrix.xaml.cs file.
                }


            }
        }        
    }
}


