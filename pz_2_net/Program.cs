using System;
using System.Windows.Forms;
using TravelTracker.Forms;

namespace TravelTracker
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            try
            {
                Application.SetHighDpiMode(HighDpiMode.SystemAware);
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new MainForm());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка запуску:\n\n{ex.Message}\n\nStack Trace:\n{ex.StackTrace}", 
                    "Критична помилка", 
                    MessageBoxButtons.OK, 
                    MessageBoxIcon.Error);
            }
        }
    }
}