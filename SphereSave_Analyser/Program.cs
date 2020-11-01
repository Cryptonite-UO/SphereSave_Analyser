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
            //At this point, you can script specific demand

            //********************CALCULATION OF GOLD IN THE GAME*********************
            var gold = from obj in reader.WorldItems
                       where obj.id == "i_gold"
                       select obj;

            int amount = 1;
            foreach (var o in gold)
            {
                    amount += o.amount;
            }

            Console.WriteLine("Il y a {0} gold en jeu", amount);

            var cheque = from obj in reader.WorldItems
                       where obj.id == "i_bank_check"
                       select obj;
            int amount2 = 0;

            foreach (var c in cheque)
            {
                amount2 += c.amount; //Todo. Multiplié la amount par le more 1(valeur du cheque) Le more1 est en string....
            }

            if (amount2 != 0)
            {
                Console.WriteLine("Il y a {0} gold en cheque en jeu", amount2);
            }


            //********************Sort list of item*********************
            Console.WriteLine("****************************************************");
            var queryitem = from item in reader.WorldItems
                        group item.id by item.id into g
                        let count = g.Count()
                        orderby count descending
                        select new { Id = g.Key, Nombre = count };

            foreach (var x in queryitem)
            {
                if (x.Nombre > 100)
                {
                    Console.WriteLine("id: " + x.Id + " Count: " + x.Nombre);
                }
            }

            //********************Sort list of character*********************
            Console.WriteLine("****************************************************");
            var querychar = from item in reader.WorldCharacters
                        group item.id by item.id into g
                        let count = g.Count()
                        orderby count descending
                        select new { Id = g.Key, Nombre = count };

            foreach (var x in querychar)
            {
                if (x.Nombre > 100)
                {
                    Console.WriteLine("id: " + x.Id + " Count: " + x.Nombre);
                }
            }


            //********************List all character with a specific skill*********************
            Console.WriteLine("****************************************************");
            var skills = from obj in reader.WorldCharacters
                       where obj.healing >= 1500
                       select obj;

            foreach (var o in skills)
            {
                Console.WriteLine($"Le personnage {o.name} a {o.healing} de Healing");
            }

            //********************Make a specific count of an item*********************
            Console.WriteLine("****************************************************");
            var anvils = from obj in reader.WorldItems
                       where obj.id == "i_anvil"
                       select obj;

            Console.WriteLine($"il y as {anvils.Count()} anvil dans le monde");

            Console.ReadLine();
        }
    }

}
