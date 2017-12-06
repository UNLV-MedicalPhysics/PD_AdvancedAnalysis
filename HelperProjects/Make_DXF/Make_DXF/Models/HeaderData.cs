using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Make_DXF.Models
{
    public class HeaderData
    {
        public int size1 { get; set; }
        public int size2 { get; set; }
        public string image_type { get; set; }
        public double gantryAngle { get; set; }
        public string machineID { get; set; }
        public double collRtn { get; set; }
        public double x1 { get; set; }
        public double x2 { get; set; }
        public double y1 { get; set; }
        public double y2 { get; set; }
        public DateTime date { get; set; }
        public double res1 { get; set; }
        public double res2 { get; set; }
        public enum Imager
        {
            IDU = 0,
            DMI = 1
        }
        public string Make_Header(double s1, double s2, string imageType,
            double gantry, double coll, double collx1, double collx2,
            double colly1, double colly2, DateTime dt, int imager, int field_num,
            int sizex, int sizey)
        {
            string header = "";
            //res1 = imager == 0 ? s1 / 400 : s1 / 400;
            //res2 = imager == 0 ? s2 / 300 : s2 / 400;
            res1 = (double)sizex / s1 * 10;//s1 / (sizex*10);
            res2 = (double)sizey / s2 * 10;// s2 / (sizey*10);
            string r1 = res1.ToString("F8");
            string r2 = res2.ToString("F8");
            header += String.Format(@"[General]
FileFormat=Generic Dosimetry Exchange Format
Version=1.0
Creator=Portal Dosimetry
CreatorVersion=13.6
[Geometry]
Dimensions=2
Axis1=X
Size1={0}
Res1={1}
Offset1=0
Unit1=mm
Separator1=\t
Axis2=Y
Size2={2}
Res2={3}
Offset2=0
Unit2=mm
Separator2=\n
[Interpretation]
Type={12}
DataType=%f
Unit=CU
Location=Imager
Medium=Undefined
[Patient]
PatientId1=US-PH-001
PatientId2=
LastName=Phantom
FirstName=SquareBox
[Field]
PlanId=Plan1
FieldId=Field_1_{4}
ExternalBeamId=HESN10
BeamType=Photon
Energy=6000
SAD=100
Scale=IEC1217
GantryAngle={5}
CollRtn={6}
CollX1={7}
CollX2={8}
CollY1={9}
CollY2={10}
[PortalDose]
SID=100
Date={11}
[Data]
", s1, r1, s2, r2, field_num,
gantry, coll, collx1, collx2, colly1, colly2,
dt, imageType);
            return header;
        }
        public float[,] make_data(double xpos, double ypos)
        {

            float[,] f = new float[size2, size1];//new List<string>();
            //load in header
            //string s = "";
            int x_ref = Convert.ToInt16(size1 / 2  + xpos/res1), y_ref = Convert.ToInt16(size2 / 2  + ypos/res2);
            /*double[] current_pos = new double[2];
            for(int col = 0; col < size1; col++)
            {
                for(int row = 0; row < size2; row++)
                {
                    current_pos = new double[]{col* res1, row*res2};
                    if(current_pos[0]>=x_ref-2.0 && current_pos[0])
                }
            }*/
            for(int col = x_ref-5; col < x_ref + 5; col++)
            {
                if (col >=x_ref - 1 && col < x_ref + 1)
                {
                    for(int row = y_ref-5; row< y_ref + 5; row++)
                    {
                        f[row, col] = 100;
                    }
                }
                else
                {
                    for(int row = y_ref-1; row<y_ref+1; row++)
                    {
                        f[row, col] = 100;
                    }
                }
            }
            return f;
        }
    }
}
