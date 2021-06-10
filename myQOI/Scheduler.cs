using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpeedtestNetCli
{
    class Scheduler
    {
        public int hr;
        public int min;
        public int sec;
        public int dayhr;
        public int daymin;
        public int daysec;
        public Scheduler(string aHrs, string aMin, string aSec, string aDayHr, string aDayMin, string aDaySec)
        {
            Int32.TryParse(aHrs, out hr);
            Int32.TryParse(aMin, out min);
            Int32.TryParse(aSec, out sec);
               
            Int32.TryParse(aDayHr, out dayhr);
            Int32.TryParse(aDayMin, out daymin);
            Int32.TryParse(aDaySec, out daysec);

        }
    }
}
