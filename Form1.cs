﻿using System;
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
            //https://adn-cis.org/forum/index.php?topic=687.0
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

        /*
     https://stackoverrun.com/ru/q/4794740
        using:
        static MyPlugin()
       {
           AssemblyLoader.RegisterAssemblyLoader();
       }



using System;
using System.IO;
using System.Linq;
using System.Reflection;

internal static class AssemblyLoader
{
       internal static void RegisterAssemblyLoader()
       {
           AppDomain currentDomain = AppDomain.CurrentDomain;
           currentDomain.AssemblyResolve -= OnResolveAssembly;
           currentDomain.AssemblyResolve += OnResolveAssembly;
       }

       private static Assembly OnResolveAssembly(object sender, ResolveEventArgs args)
       {
           return LoadAssemblyFromManifest(args.Name);
       }

       private static Assembly LoadAssemblyFromManifest(string targetAssemblyName)
       {
           Assembly executingAssembly = Assembly.GetExecutingAssembly();
           AssemblyName assemblyName = new AssemblyName(targetAssemblyName);

           string resourceName = DetermineEmbeddedResourceName(assemblyName, executingAssembly);

           using (Stream stream = executingAssembly.GetManifestResourceStream(resourceName))
           {
               if (stream == null)
                   return null;

               byte[] assemblyRawBytes = new byte[stream.Length];
               stream.Read(assemblyRawBytes, 0, assemblyRawBytes.Length);


               return Assembly.Load(assemblyRawBytes);
           }

       }

       private static string DetermineEmbeddedResourceName(AssemblyName assemblyName, Assembly executingAssembly)
       {
           //This assumes you have the assemblies in a folder named "EmbeddedAssemblies"
           string resourceName = string.Format("{0}.EmbeddedAssemblies.{1}.dll",
                                               executingAssembly.GetName().Name, assemblyName.Name);

           //This logic finds the assembly manifest name even if it's not an case match for the requested assembly                          
           var matchingResource = executingAssembly.GetManifestResourceNames()
                                                   .FirstOrDefault(res => res.ToLower() == resourceName.ToLower());

           if (matchingResource != null)
           {
               resourceName = matchingResource;
           }
           return resourceName;
       }
}

        */

        /*        
        https://adn-cis.org/forum/index.php?topic=687.0
        //https://stackoverrun.com/ru/q/4794740
        static void Main(string[] args) {
      AppDomain domain = AppDomain.CurrentDomain;
      Thread thread = Thread.CurrentThread;
      thread.CurrentUICulture = new CultureInfo("en");
      domain.AssemblyResolve += domain_ResourceResolve;
      ResourceManager res = new ResourceManager(typeof(Program));
      Console.WriteLine(res.GetString("Message"));
      Console.WriteLine("Press any key to exit...");
      Console.ReadKey();
      res.ReleaseAllResources();
    }
 
 
    static System.Reflection.Assembly domain_ResourceResolve(object sender, ResolveEventArgs args)
    {
        AppDomain.CurrentDomain.AssemblyResolve -= domain_ResourceResolve;
        Assembly assembly = typeof(Program).Assembly;
        String name = Path.Combine(Path.GetDirectoryName(assembly.Location),
        String.Format("resources\\en\\{0}.resources.dll", Path.GetFileNameWithoutExtension(
        assembly.Location)));
        if (!File.Exists(name))
        {
            Console.WriteLine("'{0}' file not found.", name);
            return null;
        }
        else
        {
            Assembly result = Assembly.LoadFrom(name);
            if (result != null)
                Console.WriteLine("'{0}' loaded.", name);
            return result;
        }
    } 
          */
    }
}
