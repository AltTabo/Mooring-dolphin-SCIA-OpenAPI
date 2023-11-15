using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Automation; //Adicionar referência: Assembly UIAutomationClient e UIAutomationTypes
using System.Net.NetworkInformation;
using System.Diagnostics;
using System.Xml;
using System.Xml.Linq;

namespace TCC_R04
{
    public static class Janelas
    {
        public static void Enumeracao()
        {
            EnumWindows(EnumWindowsCallback, IntPtr.Zero);
            //Console.WriteLine("oi, espera 100 seg");
            //Thread.Sleep(100000);
        }

         [DllImport("user32.dll")]
        public static extern int SendMessage(int hWnd, uint Msg, int wParam, int lParam);

        public const int WM_SYSCOMMAND = 0x0112;
        public const int SC_CLOSE = 0xF060;
        private const int WM_CLOSE = 0x10;

        // do stackoverflow

        [DllImport("user32.dll")]
        public static extern int FindWindow(string lpClassName, string lpWindowName);
        //[DllImport("user32.dll")]
        //public static extern int SendMessage(int hWnd, uint Msg, int wParam, int lParam);

        //#######################

        // Delegate for EnumWindows
        private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

        // Import the user32.dll library for window enumeration
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);

        // Import the user32.dll library for getting window text
        [DllImport("user32.dll")]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

        [DllImport("user32.dll")]
        private static extern int GetClassName(IntPtr hWnd, StringBuilder text, int count);
        private static bool EnumWindowsCallback(IntPtr hWnd, IntPtr lParam)
        {
            int length = GetWindowTextLength(hWnd);
            if (length > 0)
            {
                StringBuilder sb = new StringBuilder(length + 1);
                GetWindowText(hWnd, sb, sb.Capacity);
                string windowTitle = sb.ToString();
                string janelaproc = "Sem título - Bloco de Notas";

                //if{ windowTitle == janelaproc}
                if (windowTitle == janelaproc)
                {

                    Console.WriteLine("#######################");
                    Console.WriteLine("Achei a janela o pid é " + hWnd);


                }
                if (hWnd.ToInt32() == 132448)
                {
                    Console.WriteLine("#######################");
                    Console.WriteLine(hWnd + " /" + windowTitle + "/");
                    GetClassName(hWnd, sb, sb.Capacity);
                    Console.WriteLine("o nome é " + sb.ToString() + " o pid é " + hWnd);
                    Console.WriteLine("Window Title: /" + windowTitle + "/ e PID " + hWnd.ToString());

                    int iHandle = FindWindow("NotePad", windowTitle);
                    Console.WriteLine("iHandle" + iHandle.ToString());


                    //SendMessage(iHandle, WM_CLOSE, 0, 0);
                    //SendMessage(iHandle, WM_SYSCOMMAND, SC_CLOSE, 0);


                    //close the window using API
                    //SendMessage(iHandle, WM_SYSCOMMAND, SC_CLOSE, 0);


                }


                Console.WriteLine($"Window Title: {windowTitle}");
                Console.WriteLine("Window Title: /" + windowTitle + "/ e PID " + hWnd.ToString());
                StringBuilder sb2 = new StringBuilder();
                GetClassName(hWnd, sb, sb.Capacity);
                Console.WriteLine("o nome é " + sb.ToString() + " o pid é " + hWnd);
            }

            //FindWindow(janelaproc,)


            return true;
        }

        [DllImport("user32.dll")]
        private static extern int GetWindowTextLength(IntPtr hWnd);



        

    }
}
