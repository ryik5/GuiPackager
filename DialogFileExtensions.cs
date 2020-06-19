using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GuiPackager
{
  public static  class DialogFileExtensions
    {
        public static string OpenFileDialogReturnPath(this OpenFileDialog ofd, string fileFilter, string title) //Return its name 
        {
            if (ofd == null)
            {
                ofd = new OpenFileDialog();
            }

            ofd.FileName = @"";
            ofd.Title = title;
            ofd.Filter = fileFilter;
            ofd.ShowDialog();
            string filePath = ofd.FileName;

            return filePath;
        }
    }
}
