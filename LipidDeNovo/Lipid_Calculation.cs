/* UVliPiD Neutral Loss calculator
 * Written by W. Ryan Parker at the University of Texas, Austin
 * To supplement publication: UVliPiD: A UVPD-based hierarchical approach for de novo characterization of lipid A structures
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LipidDeNovo
{

   public class Lipid_Calculation
    {
       public double GeneralStructureMass = 498.0663;
       public double a2ionGeneralMass = 300.0495;
       public double y1ionGeneralMass = 258.0384;
       List<TableElement> primary_alpha = new List<TableElement>();
       List<TableElement> primary_beta = new List<TableElement>();
       List<TableElement> NRGlnOHAcyl = new List<TableElement>();
       List<TableElement> RedGlnOHAcyl = new List<TableElement>();
       List<TableElement> secondary_epsilon = new List<TableElement>();
       report status_report = new report();

       public void Method1(double twoMinus, double a2, double y1, string y1_Ions, string a2_ions, string y1_Ions_intensity, string a2_ions_intensity)
       {
           status_report.Show();
           populateTables();
           double precursorEPD = twoMinus * 2; //No gain of H in photodetachment.
           double GeneralPrecusrorDifference = precursorEPD - GeneralStructureMass; //Intact Precursor Mass - Building Block

           /*The assumption with method 1 is that if there is an intense singly charged peak, 
            * then it is the Y1 and the y1 has no secondary chain on the 2' end on the LHS */

           //CID of Y1
           List<Fragment> FragmentIons_y1 = new List<Fragment>();
           string[] split_ions = y1_Ions.Split(',');
           string[] split_intensities = y1_Ions_intensity.Split(',');

           //Do some String Parsing to get the Fragment Ions and Intensities and put them in a list for
           //y1 and a2

           for (int i = 0; i < split_ions.Length; i++)
           {
               Fragment J = new Fragment();
               J.intensity= Convert.ToDouble(split_intensities[i]);
               J.mass= Convert.ToDouble(split_ions[i]);
               FragmentIons_y1.Add(J);
           }

           List<Fragment> FragmentIons_a2 = new List<Fragment>();
           split_ions = a2_ions.Split(',');
           split_intensities = a2_ions_intensity.Split(',');

           for (int i = 0; i < split_ions.Length; i++)
           {
               Fragment J = new Fragment();
               J.intensity = Convert.ToDouble(split_intensities[i]);
               J.mass = Convert.ToDouble(split_ions[i]);
               FragmentIons_a2.Add(J);
           }

           //Y1 time:
           //Step 1: Does there exist two ions < 200 m/z from the y1? Look for doublet within 300
           //Exclusively Alpha and epsilon positions for y1!!
           Console.WriteLine(":::Reducing Sugar:::");
           status_report.addStatusM = ":::Reducing Sugar:::";
           int counter = 0;
           List<Fragment> tempStorage = new List<Fragment>();
           List<Fragment> non_possible_doublet = new List<Fragment>();
           for (int i = 0; i < FragmentIons_y1.Count; i++)
           {
               if (Math.Abs(FragmentIons_y1[i].mass - y1) <= 301)
               {
                   tempStorage.Add(FragmentIons_y1[i]);
                   counter++;
               }

           }

           //Doublet is here
           if (counter == 2)
           {
               List<Fragment> intensitySort = tempStorage.OrderBy(Fragment => Fragment.intensity).ToList();
               MessageBox.Show("Frag 1: " + intensitySort[0].mass + "Frag 2: " + intensitySort[1].mass);

               //3-alpha more abundant [1], 2 ep less abundant [0], we also have a 2degrees on 2'.
               //3-Alpha
               Console.WriteLine("The 3-Alpha on the reducing sugar is:");
               status_report.addStatusM = "The 3-Alpha on the reducing sugar is:";
               Double compare = Math.Abs(y1 - intensitySort[1].mass);
               Console.WriteLine(primary_alpha.OrderBy(TableElement => Math.Abs(compare-TableElement.Value)).First().Key);
               status_report.addStatusM = primary_alpha.OrderBy(TableElement => Math.Abs(compare - TableElement.Value)).First().Key;
               
               //2-epsilon
               Console.WriteLine("The 2-Epsilon on the reducing sugar is:");
               status_report.addStatusM = "The 2-Epsilon on the reducing sugar is:";
               compare = Math.Abs(y1 - intensitySort[0].mass);
               Console.WriteLine(secondary_epsilon.OrderBy(TableElement => Math.Abs(compare - TableElement.Value)).First().Key);
               status_report.addStatusM = secondary_epsilon.OrderBy(TableElement => Math.Abs(compare - TableElement.Value)).First().Key;
               //Take the lowest mass one now!  Table Value Red GLcN-OH + Acyl!
               Fragment temp = FragmentIons_y1.OrderBy(Fragment => Fragment.mass).First();
               //Don't need to subtract anything off.  It si acyl + GlCnOH
               compare = temp.mass;
               Console.WriteLine("Remaining Primary 2' on Reducing Sugar is:");
               status_report.addStatusM = "Remaining Primary 2' on Reducing Sugar is:";
               Console.WriteLine(RedGlnOHAcyl.OrderBy(TableElement => Math.Abs(compare - TableElement.Value)).First().Key);
               status_report.addStatusM = RedGlnOHAcyl.OrderBy(TableElement => Math.Abs(compare - TableElement.Value)).First().Key;
           }
           //Something went wrong
           else if (counter >= 2)
           {
               MessageBox.Show("Error!  Found > 2 ions when searching for doublets");
           }
            //There is no doublet, different analysis.
           else
           {
               //Come back with an example here later - WRP
               Console.WriteLine("Either two primary chains or a 2' chain wtih secondary and OH.  Ambigous Structure results.");
               status_report.addStatusM = "Either two primary chains or a 2' chain wtih secondary and OH.  Ambigous Structure results.";
           }

           

        //Characterize the A2, we already know that there is no secondary chain on the 2'
           Console.WriteLine(":::NonReducing Sugar:::");
           status_report.addStatusM = ":::NonReducing Sugar:::";
           double theoretical_a2 = (precursorEPD - y1) + 60;
           if (Math.Abs(theoretical_a2 - a2) >= 2)
           {
               MessageBox.Show("Theroretical a2 ion is " + theoretical_a2 + " and input a2 ion is " + a2 + " Will continue running, but please ensure values are correct!", "Warning!");
           }
           //You always get 3B and 3alpha here!  always, 3beta > 3alpha NL result ions, so 3beta is last element, use it.
           List<Fragment> intensitySort2 = FragmentIons_a2.OrderBy(Fragment => Fragment.mass).ToList();
           if (intensitySort2.Count == 2)
           {
               if (Math.Abs(intensitySort2[0].mass - intensitySort2[1].mass) <= 20) { 
                   Console.WriteLine("Doublets only, meaning that 3 is primary on nonreducing sugar");
                   status_report.addStatusM = "Doublets only, meaning that 3' is primary on nonreducing sugar";
                   Double compare = Math.Abs(a2 - intensitySort2[1].mass);
                   Console.WriteLine(primary_beta.OrderBy(TableElement => Math.Abs(compare - TableElement.Value)).First().Key);
                   status_report.addStatusM = primary_beta.OrderBy(TableElement => Math.Abs(compare - TableElement.Value)).First().Key;
                   Console.WriteLine("2 Sugar is");
                   status_report.addStatusM = "2 Sugar is";
                   compare = intensitySort2[1].mass;
                   Console.WriteLine(NRGlnOHAcyl.OrderBy(TableElement => Math.Abs(compare - TableElement.Value)).First().Key);
                   status_report.addStatusM = NRGlnOHAcyl.OrderBy(TableElement => Math.Abs(compare - TableElement.Value)).First().Key;
               }
               //Secondary Chain exists otherwise on 2' end.
               else
               {
                   //The epsilon is the mmost dominant.  First NL.
                   //The lowest mass is the loss of both acyl chains, more abundant is the loss of epsilon side chain.
                   Console.WriteLine("Two Neutral losses.  3 sugar has 2 chains");
                   status_report.addStatusM = "Two Neutral losses.  3 sugar has 2 chains";
                   Console.WriteLine("Secondary Chain on 3 end:");
                   status_report.addStatusM = "Secondary Chain on 3 end:";
                   Double compare = Math.Abs(a2 - intensitySort2[1].mass);
                   Console.WriteLine(secondary_epsilon.OrderBy(TableElement => Math.Abs(compare - TableElement.Value)).First().Key);
                   status_report.addStatusM = secondary_epsilon.OrderBy(TableElement => Math.Abs(compare - TableElement.Value)).First().Key;
                   
                   Console.WriteLine("Primary Chain on 3 end:");
                   status_report.addStatusM = "Primary Chain on 3 end:";
                   compare = Math.Abs(intensitySort2[1].mass - intensitySort2[0].mass);
                   //This is beta cleavage.
                   Console.WriteLine(primary_beta.OrderBy(TableElement => Math.Abs(compare - TableElement.Value)).First().Key);
                   status_report.addStatusM = primary_beta.OrderBy(TableElement => Math.Abs(compare - TableElement.Value)).First().Key;

                   Console.WriteLine("2 End:");
                   status_report.addStatusM = "2 End:";
                   compare = intensitySort2[0].mass;
                   Console.WriteLine(NRGlnOHAcyl.OrderBy(TableElement => Math.Abs(compare - TableElement.Value)).First().Key);
                   status_report.addStatusM = NRGlnOHAcyl.OrderBy(TableElement => Math.Abs(compare - TableElement.Value)).First().Key;
               }
               
           }
           else if (intensitySort2.Count < 2)
           {
               Console.WriteLine("3 on Nonreducing sugar is OH");
               status_report.addStatusM = "3 on Nonreducing sugar is OH";
           }
           else if(intensitySort2.Count > 3){
               //NEED 3 epsilon here!
               Console.WriteLine("Not Possible > 3 acyl chains.  Do not include alpha cleavage if very low abundance");
               status_report.addStatusM = "Not Possible > 3 acyl chains.  Do not include alpha cleavage if very low abundance";
           }
       }
       public void Method2(double twoMinus, double a2, double y1, string y1_Ions, string a2_ions, string y1_Ions_intensity, string a2_ions_intensity, double x1, double x1plus52, double Precursor2)
       {
           Boolean primary_nonred_3 = false;
           populateTables();
           status_report.Show();
           Console.WriteLine("Performing Method 2");
           status_report.addStatusM = "Performing Method 2";
           //We need to know the second precursor chosen, as that particular ion will give us an important NL 

           Double compare = Math.Abs(twoMinus - Precursor2)*2;
           TableElement test1 = secondary_epsilon.OrderBy(TableElement => Math.Abs(compare - TableElement.Value)).First();
           TableElement test2 = primary_beta.OrderBy(TableElement => Math.Abs(compare - TableElement.Value)).First();

           //We need to figure out which of these is actualyl corresponding to the 3' cleavage from the NL.
           if (Math.Abs(compare - test1.Value) < Math.Abs(compare - test2.Value))
           {
               Console.WriteLine("1st Neutral Loss: The 3-epsilon cleavage of the Non-Reducing Sugar is:");
               status_report.addStatusM = "1st Neutral Loss: The 3-epsilon cleavage of the Non-Reducing Sugar is:";
               Console.WriteLine(test1.Key);
               status_report.addStatusM = test1.Key;
               primary_nonred_3 = false;
           }
           else
           {
               Console.WriteLine("1st Neutral Loss: The 3-beta cleavage of the Non-Reducing Sugar is:");
               status_report.addStatusM = "1st Neutral Loss: The 3-beta cleavage of the Non-Reducing Sugar is:";
               Console.WriteLine(test2.Key);
               status_report.addStatusM = test2.Key;
               primary_nonred_3 = true;

           }

           //    status_report.addStatusM = secondary_epsilon.OrderBy(TableElement => Math.Abs(compare - TableElement.Value)).First().Key;
           //Make sure Y1 and X1 agree..
               if (Math.Abs(x1 - y1) > 30 || Math.Abs(x1 - y1) < 27 || x1 < y1)
               {
                   Microsoft.VisualBasic.Interaction.MsgBox("Warning!  X1 should be > y1 and they should be approximately 28 Da between each other.  Please check your values.");
               }

           //Y1 Analysis is the same.....

               double precursorEPD = twoMinus * 2; //No gain of H in photodetachment.
               double GeneralPrecusrorDifference = precursorEPD - GeneralStructureMass; //Intact Precursor Mass - Building Block

               /*The assumption with method 1 is that if there is an intense singly charged peak, 
                * then it is the Y1 and the y1 has no secondary chain on the 2' end on the LHS */

               //CID of Y1
               List<Fragment> FragmentIons_y1 = new List<Fragment>();
               string[] split_ions = y1_Ions.Split(',');
               string[] split_intensities = y1_Ions_intensity.Split(',');

               //Do some String Parsing to get the Fragment Ions and Intensities and put them in a list for
               //y1 and a2

               for (int i = 0; i < split_ions.Length; i++)
               {
                   Fragment J = new Fragment();
                   J.intensity = Convert.ToDouble(split_intensities[i]);
                   J.mass = Convert.ToDouble(split_ions[i]);
                   FragmentIons_y1.Add(J);
               }

               List<Fragment> FragmentIons_a2 = new List<Fragment>();
               split_ions = a2_ions.Split(',');
               split_intensities = a2_ions_intensity.Split(',');

               for (int i = 0; i < split_ions.Length; i++)
               {
                   Fragment J = new Fragment();
                   J.intensity = Convert.ToDouble(split_intensities[i]);
                   J.mass = Convert.ToDouble(split_ions[i]);
                   FragmentIons_a2.Add(J);
               }

               //Y1 Analysis:
               //Step 1: Does there exist two ions < 200 m/z from the y1? Look for doublet within 300
               //Exclusively Alpha and epsilon positions for y1
               Console.WriteLine(":::Reducing Sugar:::");
               status_report.addStatusM = ":::Reducing Sugar:::";
               int counter = 0;
               List<Fragment> tempStorage = new List<Fragment>();
               List<Fragment> non_possible_doublet = new List<Fragment>();
               for (int i = 0; i < FragmentIons_y1.Count; i++)
               {
                   if (Math.Abs(FragmentIons_y1[i].mass - y1) <= 301)
                   {
                       tempStorage.Add(FragmentIons_y1[i]);
                       counter++;
                   }

               }

               //Doublet is here
               if (counter == 2)
               {
                   List<Fragment> intensitySort = tempStorage.OrderBy(Fragment => Fragment.intensity).ToList();

                   //3-alpha more abundant [1], 2 ep less abundant [0], we also have a 2degrees on 2'.
                   //3-Alpha
                   Console.WriteLine("The 3-Alpha on the reducing sugar is:");
                   status_report.addStatusM = "The 3-Alpha on the reducing sugar is:";
                   compare = Math.Abs(y1 - intensitySort[1].mass);
                   Console.WriteLine(primary_alpha.OrderBy(TableElement => Math.Abs(compare - TableElement.Value)).First().Key);
                   status_report.addStatusM = primary_alpha.OrderBy(TableElement => Math.Abs(compare - TableElement.Value)).First().Key;

                   //2-epsilon
                   Console.WriteLine("The 2-Epsilon on the reducing sugar is:");
                   status_report.addStatusM = "The 2-Epsilon on the reducing sugar is:";
                   compare = Math.Abs(y1 - intensitySort[0].mass);
                   Console.WriteLine(secondary_epsilon.OrderBy(TableElement => Math.Abs(compare - TableElement.Value)).First().Key);
                   status_report.addStatusM = secondary_epsilon.OrderBy(TableElement => Math.Abs(compare - TableElement.Value)).First().Key;
                   //Take the lowest mass one now!  Table Value Red GLcN-OH + Acyl!
                   Fragment temp = FragmentIons_y1.OrderBy(Fragment => Fragment.mass).First();
                   //Don't need to subtract anything off.  It si acyl + GlCnOH
                   compare = temp.mass;
                   Console.WriteLine("Remaining Primary 2' on Reducing Sugar is:");
                   status_report.addStatusM = "Remaining Primary 2' on Reducing Sugar is:";
                   Console.WriteLine(RedGlnOHAcyl.OrderBy(TableElement => Math.Abs(compare - TableElement.Value)).First().Key);
                   status_report.addStatusM = RedGlnOHAcyl.OrderBy(TableElement => Math.Abs(compare - TableElement.Value)).First().Key;
               }
               //Something went wrong
               else if (counter >= 2)
               {
                   MessageBox.Show("Error!  Found > 2 ions when searching for doublets");
               }
               //There is no doublet, different analysis.
               else
               {
                   //Come back with an example here later - WRP
                   Console.WriteLine("Either two primary chains or a 2' chain wtih secondary and OH.  Ambigous Structure results.");
                   status_report.addStatusM = "Either two primary chains or a 2' chain wtih secondary and OH.  Ambigous Structure results.";
               }

           //A2 Analysis -- There are only a couple of cases.  1. 3' end is primary, and is not included in truncated a2 1 peak, 2'-epsilon or 
           //the 3' is secondary, and we have a primary chain left on truncated chain (3 peaks, alpha, beta, and 2'-Epsilon.  
           //Alpha and beta are always higher in intensity than epsilon loss.

               Console.WriteLine(":::NonReducing Sugar:::");
               status_report.addStatusM = ":::NonReducing Sugar:::";
               double theoretical_a2 = (precursorEPD - y1) + 60; //Untruncated
               
               List<Fragment> intensitySort2 = FragmentIons_a2.OrderBy(Fragment => Fragment.intensity).ToList();
                Double beta_loss = 0;
               if (primary_nonred_3 == false)
               {
                   Console.WriteLine("The 3 end has a secondary chain on NR sugar");
                   status_report.addStatusM = "The 3 end has a secondary chain on NR sugar";
                   //This should always be true.
                   if (intensitySort2.Count == 3)
                   {
                       if (Math.Abs(intensitySort2[1].mass - intensitySort2[2].mass) <= 30)
                       {
                           Console.WriteLine("Doublet Found!");
                           status_report.addStatusM = "Doublet found!";

                           if (intensitySort2[1].mass > intensitySort2[2].mass)
                           {
                               beta_loss = intensitySort2[1].mass;
                               Console.WriteLine(intensitySort2[1].mass + "Is the beta cleavage");
                               status_report.addStatusM = "The 3 chain on the NR sugar is:";
                               compare = Math.Abs(a2 - intensitySort2[1].mass);
                               Console.WriteLine(primary_beta.OrderBy(TableElement => Math.Abs(compare - TableElement.Value)).First().Key);
                               status_report.addStatusM = primary_beta.OrderBy(TableElement => Math.Abs(compare - TableElement.Value)).First().Key;

                           }
                           else
                           {
                               beta_loss = intensitySort2[2].mass;
                               Console.WriteLine(intensitySort2[2].mass + "Is the beta cleavage");
                               compare = Math.Abs(a2 - intensitySort2[2].mass);
                               status_report.addStatusM = "The 3 chain on the NR sugar is:";
                               Console.WriteLine(primary_beta.OrderBy(TableElement => Math.Abs(compare - TableElement.Value)).First().Key);
                               status_report.addStatusM = primary_beta.OrderBy(TableElement => Math.Abs(compare - TableElement.Value)).First().Key;
                           }


                           status_report.addStatusM = "The 2 secondary acyl chain is:";
                           compare = Math.Abs(a2 - intensitySort2[0].mass);
                           status_report.addStatusM = secondary_epsilon.OrderBy(TableElement => Math.Abs(compare - TableElement.Value)).First().Key;

                           status_report.addStatusM = "The remaining (primary) 2 acyl chain is:";
                           compare = Math.Abs(beta_loss-Math.Abs(intensitySort2[0].mass-a2));
                           status_report.addStatusM = NRGlnOHAcyl.OrderBy(TableElement => Math.Abs(compare - TableElement.Value)).First().Key;

                       }
                       else
                       {
                           status_report.addStatusM = "ERR: There should be a doublet detected less ~28 Da apart.";
                       }
                   }
                   else
                   {
                       status_report.addStatusM = "ERR:  There should be 3 ions corresponding to neutral losses: 3-alpha, 3-beta, and 2-epsilon";
                   }
               }
                //SHould only be one peak
               else
               {
                   if (intensitySort2.Count != 1)
                   {
                       status_report.addStatusM = "Should be only 1 ion!";
                   }
                    //Do Analysis.
                   else
                   {
                       status_report.addStatusM = "The 2 secondary acyl chain is:";
                       compare = Math.Abs(a2 - intensitySort2[0].mass);
                       status_report.addStatusM = secondary_epsilon.OrderBy(TableElement => Math.Abs(compare - TableElement.Value)).First().Key;

                       status_report.addStatusM = "The remaining (primary) 2 acyl chain is:";
                       compare = intensitySort2[0].mass;
                       status_report.addStatusM = NRGlnOHAcyl.OrderBy(TableElement => Math.Abs(compare - TableElement.Value)).First().Key;
                   }
               }

              if (intensitySort2.Count > 3)
               {
                   Console.WriteLine("Not Possible > 3 acyl chains.  Do not include alpha cleavage if very low abundance");
                   status_report.addStatusM = "Not Possible > 3 acyl chains.  Do not include alpha cleavage if very low abundance";
               }

           //End A2




           //The presence of an x1+52 implies that there is no acyl chain of the 3'C of the non reducing sugar!  Not really needed. 
           // Reverification!
               if (x1plus52 == -1)
               {
                   Console.WriteLine("No X1+52, There should be no secondary acyl chain on the 3'C of the non reducing sugar.");
                   status_report.addStatusM = "No X1+52, There should be no secondary acyl chain on the 3'C of the non reducing sugar.";
               }
               else
               {
                   Console.WriteLine("X1+52 present, There should be a secondary acyl chain on the 3'C of the non reducing sugar.");
                   status_report.addStatusM = "X1+52 present, There should be a secondary acyl chain on the 3'C of the non reducing";
               }
           //

           
        
             
       }

       public void populateTables()
       {
           primary_alpha.Add(new TableElement("10C, OH", 188));
           primary_alpha.Add(new TableElement("10C", 172));
           primary_alpha.Add(new TableElement("10C", 170));
           primary_alpha.Add(new TableElement("12C, OH", 216));
           primary_alpha.Add(new TableElement("12C", 200));
           primary_alpha.Add(new TableElement("12C", 198));
           primary_alpha.Add(new TableElement("14C, OH", 244));
           primary_alpha.Add(new TableElement("14C", 228));
           primary_alpha.Add(new TableElement("14C", 226));
           primary_alpha.Add(new TableElement("16C, OH", 272));
           primary_alpha.Add(new TableElement("16C", 256));
           primary_alpha.Add(new TableElement("16C", 254));
           primary_alpha.Add(new TableElement("18C, OH", 300));
           primary_alpha.Add(new TableElement("18C", 284));
           primary_alpha.Add(new TableElement("18C", 282));


           primary_beta.Add(new TableElement("10C, OH", 170));
           primary_beta.Add(new TableElement("10C", 154));
           primary_beta.Add(new TableElement("10C", 152));
           primary_beta.Add(new TableElement("12C, OH", 198));
           primary_beta.Add(new TableElement("12C", 182));
           primary_beta.Add(new TableElement("12C", 180));
           primary_beta.Add(new TableElement("14C, OH", 226));
           primary_beta.Add(new TableElement("14C", 210));
           primary_beta.Add(new TableElement("14C", 208));
           primary_beta.Add(new TableElement("16C, OH", 254));
           primary_beta.Add(new TableElement("16C", 238));
           primary_beta.Add(new TableElement("16C", 236));
           primary_beta.Add(new TableElement("18C, OH", 282));
           primary_beta.Add(new TableElement("18C", 266));
           primary_beta.Add(new TableElement("18C", 264));


           NRGlnOHAcyl.Add(new TableElement("10C, OH", 470));
           NRGlnOHAcyl.Add(new TableElement("10C", 454));
           NRGlnOHAcyl.Add(new TableElement("10C", 452));
           NRGlnOHAcyl.Add(new TableElement("12C, OH", 498));
           NRGlnOHAcyl.Add(new TableElement("12C", 482));
           NRGlnOHAcyl.Add(new TableElement("12C", 480));
           NRGlnOHAcyl.Add(new TableElement("14C, OH", 526));
           NRGlnOHAcyl.Add(new TableElement("14C", 510));
           NRGlnOHAcyl.Add(new TableElement("14C", 508));
           NRGlnOHAcyl.Add(new TableElement("16C, OH", 554));
           NRGlnOHAcyl.Add(new TableElement("16C", 538));
           NRGlnOHAcyl.Add(new TableElement("16C", 536));
           NRGlnOHAcyl.Add(new TableElement("18C, OH", 582));
           NRGlnOHAcyl.Add(new TableElement("18C", 566));
           NRGlnOHAcyl.Add(new TableElement("18C", 564));

           //A little more complex... bracketed numbers... etc...

           RedGlnOHAcyl.Add(new TableElement("10C, OH", 428));
           RedGlnOHAcyl.Add(new TableElement("10C, OH", 410));
           RedGlnOHAcyl.Add(new TableElement("10C", 412));
           RedGlnOHAcyl.Add(new TableElement("10C", 410));
           RedGlnOHAcyl.Add(new TableElement("10C []", 394));
           RedGlnOHAcyl.Add(new TableElement("10C []", 392));
           RedGlnOHAcyl.Add(new TableElement("12C, OH", 456));
           RedGlnOHAcyl.Add(new TableElement("12C, OH []", 438));
           RedGlnOHAcyl.Add(new TableElement("12C", 440));
           RedGlnOHAcyl.Add(new TableElement("12C", 438));
           RedGlnOHAcyl.Add(new TableElement("12C []", 422));
           RedGlnOHAcyl.Add(new TableElement("12C []", 420));
           RedGlnOHAcyl.Add(new TableElement("14C, OH", 484));
           RedGlnOHAcyl.Add(new TableElement("14C, OH []", 466));
           RedGlnOHAcyl.Add(new TableElement("14C", 468));
           RedGlnOHAcyl.Add(new TableElement("14C", 466));
           RedGlnOHAcyl.Add(new TableElement("14C []", 450));
           RedGlnOHAcyl.Add(new TableElement("14C []", 448));
           RedGlnOHAcyl.Add(new TableElement("16, OH", 512));
           RedGlnOHAcyl.Add(new TableElement("16C, OH []", 494));
           RedGlnOHAcyl.Add(new TableElement("16C", 496));
           RedGlnOHAcyl.Add(new TableElement("16C", 494));
           RedGlnOHAcyl.Add(new TableElement("16C []", 478));
           RedGlnOHAcyl.Add(new TableElement("16C []", 476));
           RedGlnOHAcyl.Add(new TableElement("18C, OH", 540));
           RedGlnOHAcyl.Add(new TableElement("18C, OH []", 522));
           RedGlnOHAcyl.Add(new TableElement("18C", 524));
           RedGlnOHAcyl.Add(new TableElement("18C", 522));
           RedGlnOHAcyl.Add(new TableElement("18C []", 506));
           RedGlnOHAcyl.Add(new TableElement("18C []", 504));


           secondary_epsilon.Add(new TableElement("10C, OH", 188));
           secondary_epsilon.Add(new TableElement("10C", 172));
           secondary_epsilon.Add(new TableElement("12C, OH", 216));
           secondary_epsilon.Add(new TableElement("12C", 200));
           secondary_epsilon.Add(new TableElement("14C, OH", 244));
           secondary_epsilon.Add(new TableElement("14C", 228));
           secondary_epsilon.Add(new TableElement("16C, OH", 272));
           secondary_epsilon.Add(new TableElement("16C", 256));
           secondary_epsilon.Add(new TableElement("18C, OH", 300));
           secondary_epsilon.Add(new TableElement("18C", 284));
       }

    }
   public class Fragment
   {
       public double mass;
       public double intensity;
   }

   public class TableElement
   {
       public String Key;
       public Double Value;
       public TableElement(String KeyN, Double ValueN){
           Key = KeyN;
           Value = ValueN;
       }
   }

}
