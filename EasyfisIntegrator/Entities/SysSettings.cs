﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyfisIntegrator.Entities
{
    class SysSettings
    {
        public String ConnectionString { get; set; }
        public String Domain { get; set; }
        public String LogFileLocation { get; set; }
        public String FolderToMonitor { get; set; }
        public Boolean IsAutoStartIntegration { get; set; }
    }
}
