﻿using System;
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
using System.Security.Cryptography.X509Certificates;
using static TCC_R04.Program;

namespace TCC_R04
{
    public static class Janelas2
    {
        public static bool Enumeracao(string NomeJanela)
        {
            NomeJanela_public = NomeJanela;
            achei = false;
            EnumWindows(EnumWindowsCallback, IntPtr.Zero);
            return achei;
        }

        [DllImport("user32.dll")]
        public static extern int SendMessage(int hWnd, uint Msg, int wParam, int lParam);

        public static string NomeJanela_public;
        public static bool achei;
        public static IntPtr hWnd_janela;
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
                string janelaproc = NomeJanela_public;
                if (windowTitle == janelaproc)
                {
                    //Console.WriteLine("#######################");
                    //Console.WriteLine("Achei a janela o pid é " + hWnd);
                    hWnd_janela = hWnd;
                    achei = true;

                }
            }
            return true;
        }

        [DllImport("user32.dll")]
        private static extern int GetWindowTextLength(IntPtr hWnd);
        public static void MonitorProcess(string processName)
        {
            bool processDetected = false;
            //Thread.Sleep(15000);
            //Janelas.Enumeracao();
            while (!processDetected)
            {
                processDetected = Enumeracao(processName);
                Thread.Sleep(500); // Sleep for half a second before checking again (adjust as needed).
            }

            SetForegroundWindow(hWnd_janela);

            // Pressiona a tecla ENTER
            keybd_event(VK_ENTER, 0, 0, UIntPtr.Zero);
            Thread.Sleep(300);

            // Libera a tecla ENTER
            keybd_event(VK_ENTER, 0, KEYEVENTF_KEYUP, UIntPtr.Zero);
        }

        [DllImport("user32.dll")]
        public static extern int SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32")]
        public static extern bool PostMessage(IntPtr hwnd, int msg, IntPtr wparam, IntPtr lparam);
        

         
        
            //Import the FindWindow API to find our window
            //[DllImport("User32.dll", EntryPoint = "FindWindow")]
            //private static extern IntPtr FindWindowNative(string className, string windowName);

            ////Import the SetForeground API to activate it
            //[DllImport("User32.dll", EntryPoint = "SetForegroundWindow")]
            //private static extern IntPtr SetForegroundWindowNative(IntPtr hWnd);

    }
}
