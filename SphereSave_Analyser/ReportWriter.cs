using System;
using System.Configuration;
using System.IO;

namespace SphereSave_Analyser
{
    //REPORT CLASS permit to write on a file
    //EX:
    //      String Nameoffile = DateTime.Now.ToString("yyy.MM.dd") + " Report of item";
    //      Report.Createfile(Nameoffile);
    //        foreach (var x in queryitem)
    //        {
    //          Report.Write("id: " + x.Id + " Count: " + x.Nombre,Nameoffile);
    //        }

       public static class Report
    {
        //TODO utiliser les Strings/int déclaré a une place
        public static string dirpathreport = ConfigurationManager.AppSettings["dirpathreport"];
        private static string shardName = ConfigurationManager.AppSettings["shardName"];
        public static int Item_report = Util.StringHexToInt(ConfigurationManager.AppSettings["Item_report"]);
        public static int Gold_report = Util.StringHexToInt(ConfigurationManager.AppSettings["Gold_report"]);
        public static int Npc_report = Util.StringHexToInt(ConfigurationManager.AppSettings["Npc_report"]);
        public static void Createfile(string title)
            //Create the file and erase the old one with the same name
        {
            // Create a string array with head of file
            string[] head = {"THIS REPORT WAS GENERATE AUTOMATICALLY BY SPHERESAVE_ANALYSER", $"FOR THE SHARD {shardName}" };

            using (StreamWriter outputFile = new StreamWriter(Path.Combine(dirpathreport, $"{title}.txt")))
            {
                foreach (string line in head)
                    outputFile.WriteLine(line);

            }
        }

        public static void Write(string s,string title)
            //Add a line to an existing file
        {
            using (StreamWriter outputFile = new StreamWriter(Path.Combine(dirpathreport, $"{title}.txt"), true))
            {
                outputFile.WriteLine($"{s}");
            }
        }

    }
}