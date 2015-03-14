using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChromeVolumeControl
{
    class Program
    {
        static void Main(string[] args)
        {

            Console.WriteLine("type chrome title substring to look for in chrome tabs:");
            var titleSubstring = Console.ReadLine();
            var chromeVolumeControl =
                new ChromeVolumeController(process => process != null && process.MainWindowTitle.Contains(titleSubstring));

            string line;
            Console.WriteLine("type a volume number from 0-100 to change chrome volume");
            while ((line = Console.ReadLine()) != "stop")
            {
                var volume = int.Parse(line.Trim());
                chromeVolumeControl.SetVolume(volume);
            }
        }
    }
}
