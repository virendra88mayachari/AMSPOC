using System;
using System.IO;
using System.Net.Http;
using System.Text;

namespace AMSPOCFileWatcher
{
    class Program
    {
        static void Main(string[] args)
        {
            //Watch OPLD files folder and call webservice to process that OPLD file
            var opldPath = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.Parent.FullName, "OPLDFiles");
            FileSystemWatcher opldFileSystemWatcher = new FileSystemWatcher();
            opldFileSystemWatcher.Path = opldPath;
            opldFileSystemWatcher.Created += OPLDFileSystemWatcher_Created;
            opldFileSystemWatcher.EnableRaisingEvents = true;

            //Watch DIALS files folder and call webservice to process that DIALS file
            var dialsPath = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.Parent.FullName, "DIALSFiles");
            FileSystemWatcher dialsFileSystemWatcher = new FileSystemWatcher();
            dialsFileSystemWatcher.Path = dialsPath;
            dialsFileSystemWatcher.Created += DIALSFileSystemWatcher_Created;
            dialsFileSystemWatcher.EnableRaisingEvents = true;

            Console.ReadLine();
        }

        private static void OPLDFileSystemWatcher_Created(object sender, FileSystemEventArgs e)
        {
            HttpClient client = new HttpClient();
            var content = new StringContent("Call Web API", Encoding.UTF8, "application/json");
            System.Threading.Tasks.Task<HttpResponseMessage> response = client.PostAsync("http://localhost:59477/api/ProcessOPLD/ProcessOPLDNPushTOMQ1", content);
        }

        private static void DIALSFileSystemWatcher_Created(object sender, FileSystemEventArgs e)
        {
            HttpClient client = new HttpClient();
            var content = new StringContent("Call Web API", Encoding.UTF8, "application/json");
            System.Threading.Tasks.Task<HttpResponseMessage> response = client.PostAsync("https://localhost:44334/api/ProcessDIALS/ProcessDIALSNWriteToDB", content);
        }
    }
}
