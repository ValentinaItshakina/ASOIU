using System;
using System.Windows.Forms;

namespace StoreApp
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            using (var db = new AppDbContext())
            {
                db.SeedInitialData();
            }
            Application.Run(new MainForm());
        }
    }
}