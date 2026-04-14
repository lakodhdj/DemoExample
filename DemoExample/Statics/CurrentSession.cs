using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoExample.Statics
{
    public static class CurrentSession
    {
        public static User CurrentUser { get; set; }
    }
}
