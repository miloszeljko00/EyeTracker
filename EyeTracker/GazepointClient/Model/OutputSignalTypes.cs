using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GazepointClient.Model
{
    public class Counter
    {
        public int CNT { get; set; }

        public Counter() { }
    }

    public class Time
    {
        public double TIME { get; set; }

        public Time() { }
    }

    public class POG_Best
    {
        public double BPOGX { get; set; }
        public double BPOGY { get; set; }
        public int BPOGV { get; set; }

        public POG_Best() { }
    }

    public class Cursor
    {
        public double CX { get; set; }
        public double CY { get; set; }
        public int CS { get; set; }

        public Cursor() { }
    }

    public class Blink
    {
        public int BKID { get; set; }
        public double BKDUR { get; set; }
        public int BKPMIN { get; set; }

        public Blink() { }
    }
}
