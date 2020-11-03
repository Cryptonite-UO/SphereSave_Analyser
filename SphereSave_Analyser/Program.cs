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
        private static SphereFileReader reader;

        static void Main(string[] args)
        {

            reader = new SphereFileReader();
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
            int amount2 = 0;
            int amountcheque = 0;
            foreach (var o in gold)
            {
                    amount += o.amount;
            }

           var cheque = from obj in reader.WorldItems
                       where obj.id == "i_bank_check"
                       select obj;

            foreach (var c in cheque)
            {
                amount += c.amount * Util.StringHexToInt(c.more1);
                amount2 += c.amount * Util.StringHexToInt(c.more1);
                amountcheque += c.amount;
            }

            Console.WriteLine("There is {0} gold in the game", amount);
            if (amount2 != 0)
            {
                Console.WriteLine("Including {1} gold split between {0} check", amountcheque, amount2);
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

            //***********Get All Static items ********************
            GetAllStaticItems();

            //********************Get item For Account *********************
            GetItemForAccount("Dixonzegm");
            
            Console.ReadLine();
        }

        public static void GetAllStaticItems()
        {
            foreach (var c in reader.WorldItems)
            {
                if ((c.attr & (int)Flags.ATTR_STATIC) > 0)
                {
                    Console.WriteLine($"Item static P : {c.p} id : {c.id}");
                }
            }
        }
        //********************Get item For Account *********************
        public static void GetItemForAccount(string account)
        {
            Console.WriteLine($"Liste les personages et item de l'account {account}");

            foreach (WorldChar c in reader.WorldCharacters.Where(x => x.IsPlayer && x.account.Contains(account)))
            {
                var itemforuid = from obj in reader.WorldItems
                                 where obj.cont == c.serial
                                 select obj;
                Console.WriteLine($"Le personnage {c.name}");

                GetAllItemsForContainer(c.serial);
                        
                Console.WriteLine($"Le nombre totale d'or de ce personage est : {GetTotalGoldForCharacter(c.serial)}");
            }
        }

        public static void GetAllItemsForContainer(int cont)
        {
            var itemforuid = from obj in reader.WorldItems
                             where obj.cont == cont
                             select obj;
            foreach (var o in itemforuid)
            {
                GetAllItemsForContainer(o.serial);
                Console.WriteLine($"a {o.amount + 1} de {o.id}");
            }
            
        }

        public static int GetTotalGoldForCharacter(int cont)
        {
            int totalgold = 0;
            var itemforuid = from obj in reader.WorldItems
                             where obj.cont == cont
                             select obj;
            foreach (var o in itemforuid)
            {
                if (o.id == "i_gold")
                {
                    totalgold += o.amount;
                }
                totalgold += GetTotalGoldForCharacter(o.serial);
            }
            return totalgold;
        }
    }

}
