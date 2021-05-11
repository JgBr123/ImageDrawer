using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Diagnostics;

namespace DisplayDrawer
{
    public class ImageDrawer
    {
        public static void Main()
        {
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            
            Console.WriteLine("Insert a JPG file path to start: \n");
            Console.WriteLine("OBS: Images should not exceed 360 by 360 pixels, as it will probably cause it to not render properly.\nThe suggested resolution for a good use of the program is about 240p images or lower.");
            Console.WriteLine("Use CTRL + MOUSE SCROLL to zoom in and out. You probably will need to zoom out the max you can for the pixels to fit correctly in the window.\n");
            Console.WriteLine("If the render starts to flick or glitch, maybe you used a too high res image, your zoom is too close or the window size is small.\nTry maxing out the window size, as well as zooming out the max you can.\n");

            Console.ResetColor();

            string input = Console.ReadLine();

            if (File.Exists(input))
            {
                Console.CursorVisible = false;

                Bitmap img = new Bitmap(input);

                Drawer dw = new Drawer(Convert.ToUInt32(img.Width), Convert.ToUInt32(img.Height));

                int maxHeight = img.Height;
                int maxWidth = img.Width;

                int y = 0;
                int x = 0;

                while (y < maxHeight)
                {
                    while (x < maxWidth)
                    {
                        Color pixel = img.GetPixel(x, y);
                        dw.DrawPixel(x + 1, y + 1, pixel.R, pixel.G, pixel.B);
                        x++;
                    }
                    x = 0;
                    y++;
                }

                dw.Render();
            }
            else
            {
                Console.Clear();
                Console.WriteLine("The inserted path is not valid. Press any key to try again.");
                Console.ReadKey();
                Console.Clear();
                Main();
            }
        }
    }
    public class Drawer
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool SetConsoleMode(IntPtr hConsoleHandle, int mode);
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool GetConsoleMode(IntPtr handle, out int mode);
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr GetStdHandle(int handle);

        public Dictionary<string, int[]> pixelInfo = new Dictionary<string, int[]>();
        public uint length, height;

        public Drawer(uint length, uint height)
        {
            Console.ResetColor();
            Console.Clear();

            this.length = length;
            this.height = height;

            var handle = GetStdHandle(-11);
            int mode;
            GetConsoleMode(handle, out mode);
            SetConsoleMode(handle, mode | 0x4);
        }
        public void DrawPixel(int x, int y, int red, int green, int blue)
        {
            if (x <= length && y <= height)
            {
                if (red < 256 && green < 256 && blue < 256)
                {
                    string cords = $"{x},{y}";
                    int[] color = { red, green, blue };

                    if (pixelInfo.ContainsKey(cords)) pixelInfo.Remove(cords);
                    pixelInfo.Add(cords, color);
                }
            }
        }
        public void Render()
        {
            int i = 1;
            int c = 1;

            while (true)
            {
                if (i > length) { i = 1; c++; Console.Write("\n"); }
                if (c > height) { break; }

                string key = $"{i},{c}";

                if (pixelInfo.ContainsKey(key))
                {
                    int[] color = pixelInfo[key];
                    int r = color[0];
                    int g = color[1];
                    int b = color[2];
                    string msg = "  ";
                    Console.Write($"\x1b[48;2;{r};{g};{b}m" + msg);
                    Console.ResetColor();
                }
                else
                {
                    Console.Write("  ");
                }
                i++;
            }
        }
    }
}