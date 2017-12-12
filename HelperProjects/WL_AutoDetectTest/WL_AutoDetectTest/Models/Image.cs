using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WL_AutoDetectTest.Models
{
    public class Image
    {
        public int sizeX { get; set; }
        public int sizeY { get; set; }
        public double resX { get; set; }
        public double resY { get; set; }
        public List<float[]> pixels { get; set; }
        public Image()
        {
            pixels = new List<float[]>();
        }
    }
}
