using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CookBook.Services
{
    public class DesktopFileWatcher
    {
        private static volatile DesktopFileWatcher _instance;
        private static readonly object _lock = new object();

        private BackgroundWorker _fileCheckWorker;
        public event Action<bool> OnFileStatusChanged;
        public static DesktopFileWatcher Instance
        {
            get 
            {
                if(_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new DesktopFileWatcher();
                        }
                    }
                }

                return _instance;
            }
        }

        private DesktopFileWatcher() {
            _fileCheckWorker = new BackgroundWorker();
            _fileCheckWorker.DoWork += FileCheckWorker_DoWork;
            _fileCheckWorker.RunWorkerAsync();
        }

        private void FileCheckWorker_DoWork(object? sender, DoWorkEventArgs e)
        {
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string filePath = Path.Combine(desktopPath, "ShoppingList.txt");

            bool previousFileStatus= false;

            while (true)
            {
                bool fileExists = File.Exists(filePath);
                if(previousFileStatus!=fileExists)
                {
                    FileStatusChanged(fileExists);
                    previousFileStatus= fileExists;
                }
                Thread.Sleep(5000);
            }
        }

        private void FileStatusChanged(bool fileExists)
        {
            Debug.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss.fff") + "Checking if there are any subscribers...");
            if (OnFileStatusChanged != null)
            {
                OnFileStatusChanged.Invoke(fileExists);
                Debug.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss.fff")  + "Notification send, file status: " + fileExists);
            }
            else
                Debug.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss.fff") + "Notification wasn't sent, bcz there are no subsribers");
        }
    }
}
