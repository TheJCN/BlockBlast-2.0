using System.Globalization;
using System.Windows.Forms.PropertyGridInternal;
using BlockBlast_2._0.views;

namespace BlockBlast_2._0;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        var lang = Properties.Settings.Default.Language;
        if (lang != "ru" && lang != "en")
        {
            Properties.Settings.Default.Language = "ru";
            Properties.Settings.Default.Save();
        }
        
        Thread.CurrentThread.CurrentUICulture = new CultureInfo(lang);
        Thread.CurrentThread.CurrentCulture = new CultureInfo(lang);
        
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.Run(new MenuForm());
    }
}