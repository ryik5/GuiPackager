using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GuiPackager
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            buttonGet.Click += ButtonGet_Click;
        }

        private void ButtonGet_Click(object sender, EventArgs e)
        {
            Package();
        }

        public void Package()
        {
            //https://habr.com/ru/post/85480/
            //https://github.com/elw00d/nbox
            //https://overcoder.net/q/1177089/работа-с-событием-appdomainassemblyresolve
            OpenFileDialog ofd = new OpenFileDialog();
            var fileInputName = ofd
                .OpenFileDialogReturnPath(
                Properties.Resources.DialogDllFiles,
                "Выберите файл для создания его сжатой версии:");
            var assembly = File.ReadAllBytes(fileInputName);

            var fileOutputName =$"{fileInputName}.deflated";
            using (var file = File.Open(fileOutputName, FileMode.Create))
            using (var stream = new DeflateStream(file, CompressionMode.Compress))
            using (var writer = new BinaryWriter(stream))
            {
                writer.Write(assembly);
            }

            MessageBox.Show("Готово!");
        }
    }
}
