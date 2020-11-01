using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Configuration;




namespace SphereSave_Analyser
{
    class Program
    {
        private static string dirpathsave =ConfigurationManager.AppSettings["dirpathsave"];
        private static string dirpathreport = ConfigurationManager.AppSettings["dirpathreport"];

        static void Main(string[] args)
        {

            SphereFileReader reader = new SphereFileReader();
            try
            {
                reader.ReadFileToObj(dirpathsave + "/sphereworld.scp", SphereFileType.SphereWorld);
                reader.ReadFileToObj(dirpathsave + "/spherechars.scp", SphereFileType.SphereChars);
            }
            catch (Exception e)
            {
                Console.WriteLine("Exeption : {0}",e);
            }
            //Faire des demande dans la liste d'object

            var gold = from obj in reader.WorldItems
                       where obj.id == "i_gold"
                       select obj;

            int amount = 1;
            foreach (var o in gold)
            {
                if (!String.IsNullOrEmpty(o.amount))
                    amount += int.Parse(o.amount);
            }

            Console.WriteLine("Il y yas {0} gold en jeu", amount);

            var query = from item in reader.WorldCharacters
                        group item.id by item.id into g
                        let count = g.Count()
                        orderby count descending
                        select new { Id = g.Key, Nombre = count };

            foreach (var x in query)
            {
                if (x.Nombre > 100)
                {
                    Console.WriteLine("id: " + x.Id + " Count: " + x.Nombre);
                }
            }

            var anvils = from obj in reader.WorldItems
                       where obj.id == "i_anvil"
                       select obj;

            Console.WriteLine($"il y as {anvils.Count()} anvil dans le monde");

            foreach (var o in anvils)
            {
                Console.WriteLine($"Situer a : {o.p}");
            }

            Console.ReadLine();
        }
    }

}
