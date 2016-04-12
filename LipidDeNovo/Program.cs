/* UVliPiD Neutral Loss calculator
 * Written by W. Ryan Parker at the University of Texas, Austin
 * To supplement publication: UVliPiD: A UVPD-based hierarchical approach for de novo characterization of lipid A structures
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace LipidDeNovo
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
                     
            

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());


        }
    }
}
