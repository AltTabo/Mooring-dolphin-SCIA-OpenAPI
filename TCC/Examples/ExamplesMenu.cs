using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCC_R04.Examples
{
    public static class ExamplesMenu
    {
        public static bool Show(string senPath, string senTempPath, string emptyProjectPath)
        {
            Console.WriteLine("Please choose one of the following:");

            Console.WriteLine("A.) Create example model in SCIA Engineer using ADM via OpenAPI (Advanced)");
            Console.WriteLine("C.) Create my own defined model from code in SCIA Engineer");
            Console.Write("");

            string choice = ConsoleHelper.Interact<string>("Your choice: ").ToUpperInvariant();

            Console.Clear();
            switch (choice)
            {

                case "A":
                    {
                        var example = new CreateModelWithAdmExample(senPath, senTempPath, emptyProjectPath);
                        example.Run();
                        return false;
                    }
                case "C":
                default:
                    {
                        return true;
                    }
            }
        }
    }
}
