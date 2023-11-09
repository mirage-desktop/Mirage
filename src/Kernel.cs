/*
 *  This file is part of the Mirage Desktop Environment.
 *  github.com/mirage-desktop/Mirage
 */
namespace Mirage
{
    public class Kernel : Cosmos.System.Kernel
    {
        protected override void BeforeRun()
        {
            System.Console.WriteLine("Starting Mirage...");
            DE.DesktopEnvironment.Start("Mirage OS", "1.0 Beta");
        }

        protected override void Run()
        {
        }
    }
}
