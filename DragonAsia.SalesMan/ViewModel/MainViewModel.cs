using GalaSoft.MvvmLight;
using System;
using System.IO;
using System.Threading;

namespace DragonAsia.SalesMan.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        public MainViewModel()
        {
            if (!IsInDesignMode)
            {
                new Thread(handleCleanup) { IsBackground = true }.Start();
            }            
        }

        private void handleCleanup()
        {
            if (!Directory.Exists("tmp"))
                return;

            var markerDay = DateTime.Now.Subtract(TimeSpan.FromDays(7)).ToString("yyyyMMddHHmmss");

            foreach(var file in Directory.GetFiles("tmp", "*.pdf"))
            {
                if (Path.GetFileNameWithoutExtension(file).CompareTo(markerDay) < 0)
                    try { File.Delete(file); }
                    catch (Exception) { }
            }
        }
    }
}