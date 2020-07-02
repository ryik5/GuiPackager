using System;
using System.IO;
using System.IO.Compression;
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
                writer.Flush();
            }

            textBoxLog.AppendText($"'{fileInputName}'  =>  '{fileOutputName}' - сжат!{Environment.NewLine}");
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
using System.IO.Compression;
using System.Linq;
using System.Reflection;

internal static class AssemblyLoader
{
       internal static void RegisterAssemblyLoader()
       {
          //https://stackoverrun.com/ru/q/4794740
            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.AssemblyResolve -= OnResolveAssembly;
            currentDomain.AssemblyResolve += OnResolveAssembly;
            WriteAppropriateVersionSqliteIteropLocaly();
        }

        private static Assembly OnResolveAssembly(object sender, ResolveEventArgs args)
        {
            return LoadAssemblyFromManifest(args.Name);
        }

        private static void WriteAppropriateVersionSqliteIteropLocaly()
        {
            var interopFileName = "SQLite.Interop.dll";
            Assembly executingAssembly = Assembly.GetExecutingAssembly();

            var env = Environment.Is64BitProcess ? "x64" : "x86";// You might need to adjust this line to correctly specify 
                                                                 // the embedded resource path within the custom action assembly.

            var resourceName = $"{executingAssembly.GetName().Name}.Resources.{env}.{interopFileName}.deflated";

            CommonExtensions.Logger(LogTypes.Trace, "Try to write dll: " + resourceName);

            try
            {
                var assemblyDirectory = Path.GetDirectoryName(executingAssembly.Location);
                var dir = Directory.CreateDirectory($@"{assemblyDirectory}\{env}");
                var interopFilePath = Path.Combine(dir.FullName, interopFileName);
                FileInfo fi = new FileInfo(interopFilePath);
                if (!File.Exists(interopFilePath) || fi.Length < 100)
                    using (Stream stream = executingAssembly.GetManifestResourceStream(resourceName))
                    {
                        if (stream == null)
                        {
                            return;
                        }

                        using (var deflated = new DeflateStream(stream, CompressionMode.Decompress))
                        using (var fs = new FileStream(interopFilePath, FileMode.Create, FileAccess.Write))
                        {
                            deflated.CopyTo(fs);
                        }
                    }
            }
            catch (Exception err)
            {
                CommonExtensions.Logger(LogTypes.Error, "Error writing: " + err.ToString());
            }
        }

        private static Assembly LoadAssemblyFromManifest(string targetAssemblyName)
        {
            Assembly executingAssembly = Assembly.GetExecutingAssembly();
            byte[] assemblyRawBytes = null;

            //var names = typeof(Program).Assembly.GetManifestResourceNames();
            //foreach (var n in names)
            //{
            //    CommonExtensions.Logger(LogTypes.Info, "names: " + n);
            //}

            try
            {
                CommonExtensions.Logger(LogTypes.Info, $"OnResolveAssembly, targetAssemblyName: {targetAssemblyName}");
                AssemblyName assemblyName = new AssemblyName(targetAssemblyName);

                string resourceName = DetermineEmbeddedResourceName(assemblyName, executingAssembly);
                CommonExtensions.Logger(LogTypes.Info, $"OnResolveAssembly, resourceName: {resourceName}");

                using (Stream stream = executingAssembly.GetManifestResourceStream(resourceName))
                {
                    if (stream == null)
                    {
                        return null;
                    }

                    using (var deflated = new DeflateStream(stream, CompressionMode.Decompress))
                    using (var reader = new BinaryReader(deflated))
                    {
                        var ten_megabytes =10* 1024 * 1024;
                        assemblyRawBytes = reader.ReadBytes(ten_megabytes);
                    }
                }
            }
            catch(Exception err)
            {
                CommonExtensions.Logger(LogTypes.Error, $"err: {err.ToString()}");
                CommonExtensions.Logger(LogTypes.Error, $"error  -/-/-/-/   OnResolveAssembly, targetAssemblyName: {targetAssemblyName}");
            }

            return Assembly.Load(assemblyRawBytes);
        }

        private static string DetermineEmbeddedResourceName(AssemblyName assemblyName, Assembly executingAssembly)
        {
            //This assumes you have the assemblies in a folder named "Resources"
            //in ahead all needed library files *.dll make as deflated files by 'GuiPackager'
            //then add them into this previously created project folder - 'Resources'
            //after it
            //for every deflated files set a flag 'Build Action' in Property(Solution Explorer) as -  'Embedded Resource'
            //then change for matched every library dll in 'Preferences' a flag 'Copy Local' as 'False'
            var env = Environment.Is64BitProcess ? "x64" : "x86";
            CommonExtensions.Logger( LogTypes.Info,$"DetermineEmbeddedResourceName, 1: {executingAssembly.GetName().Name}|2: {assemblyName.Name}");
           string resourceName = $"{executingAssembly.GetName().Name}.Resources.{assemblyName.Name}.dll.deflated";
            
            if (assemblyName.Name.ToLower().Contains("sqlite.interop"))
            {
                resourceName = $"{executingAssembly.GetName().Name}.Resources.{env}.{assemblyName.Name}.dll.deflated";
            }

            //This logic finds the assembly manifest name even if it's not an case match for the requested assembly                          
            var matchingResource = executingAssembly
                .GetManifestResourceNames()
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
