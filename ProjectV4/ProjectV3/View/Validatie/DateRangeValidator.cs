using ProjectV3.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectV3.View.Validators
{
    class DateRangeValidator: RangeAttribute
    {
        private static string start = Festival.start.ToShortDateString();
        private static string end = Festival.End.ToShortDateString();
        public DateRangeValidator() : base(typeof(DateTime), start, end) { }
    }
}
