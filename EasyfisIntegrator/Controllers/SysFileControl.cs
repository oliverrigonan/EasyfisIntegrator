using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EasyfisIntegrator.Controllers
{
    class SysFileControl
    {
        public static Boolean IsCurrentFileClosed(String file)
        {
            Boolean fileClosed = false;
            Int32 retries = 20;
            Int32 delay = 500;

            if (File.Exists(file))
            {
                do
                {
                    try
                    {
                        FileStream fs = File.Open(file, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
                        fs.Close();

                        fileClosed = true;
                    }
                    catch (IOException ioEx) { Debug.WriteLine(ioEx); }

                    retries--;
                    if (!fileClosed) { Thread.Sleep(delay); }
                }
                while (!fileClosed && retries > 0);

                return fileClosed;
            }
            else { return false; }
        }
    }
}
