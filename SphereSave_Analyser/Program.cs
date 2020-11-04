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
       
        private static SphereFileReader reader;

        static void Main(string[] args)
        {
            var MyIni = new IniFile();
            string dirpathsave = MyIni.Read("dirpathsave");
            string dirpathreport = MyIni.Read("dirpathreport");
            string shardName = MyIni.Read("shardName");
            int Item_report = Util.StringHexToInt(MyIni.Read("Item_report"));
            int Npc_report = Util.StringHexToInt(MyIni.Read("Npc_report"));
            int Gold_report = Util.StringHexToInt(MyIni.Read("Gold_report"));

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


            //************************************************************
            //************************************************************
            //BELLOW THIS LINE, you can script specific for an AUTO REPORT
            Console.WriteLine("Generating report... in {0}", dirpathreport);
            String Nameoffile = DateTime.Now.ToString("yyy.MM.dd") + " Custom report";
            Report.Createfile(Nameoffile);

            //********************CALCULATION OF GOLD IN THE GAME*********************
            if (Gold_report == 1)
            {
                Report.Write("****************************************************", Nameoffile);
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
                Report.Write("There is "+ amount +" gold in the game", Nameoffile);
                if (amount2 != 0)
                {
                    Report.Write("Including "+ amount2 + " gold split between "+ amountcheque + " check", Nameoffile);
                }
            }

            //********************Sort list of item*********************
            if (Item_report == 1)
            {
                Report.Write("****************************************************************", Nameoffile);
                Report.Write("There a list of all item in the world present 50 or more times", Nameoffile);
                Report.Write("There a total of " + reader.WorldItems.Count() + " Item", Nameoffile);
                var queryitem = from item in reader.WorldItems
                            group item.id by item.id into g
                            let count = g.Count()
                            orderby count descending
                            select new { Id = g.Key, Nombre = count };

                foreach (var x in queryitem)
                {
                    if (x.Nombre > 50)
                    {
                        Report.Write("id: " + x.Id + " Count: " + x.Nombre, Nameoffile);
                    }
                }
            }

            //********************Sort list of character*********************
            if (Npc_report == 1)
            {
                Report.Write("******************************************************************", Nameoffile);
                Report.Write("There a list of all NPC in the world present 50 or more times", Nameoffile);
                Report.Write("There a total of "+ reader.WorldCharacters.Count() +" NPC", Nameoffile);
                var querychar = from item in reader.WorldCharacters
                            group item.id by item.id into g
                            let count = g.Count()
                            orderby count descending
                            select new { Id = g.Key, Nombre = count };

                foreach (var x in querychar)
                {
                    if (x.Nombre > 50)
                    {
                        Report.Write("id: " + x.Id + " Count: " + x.Nombre, Nameoffile);
                    }
                }
            }

            //***********Get All Static items ********************
            //GetAllStaticItems();

            //********************List all character with a specific skill*********************
            //Console.WriteLine("****************************************************");
            //var skills = from obj in reader.WorldCharacters
            //           where obj.healing >= 1500
            //           select obj;

            //foreach (var o in skills)
            //{
            //    Console.WriteLine($"Le personnage {o.name} a {o.healing} de Healing");
            //}

            //********************Make a specific count of an item*********************
            //Console.WriteLine("****************************************************");
            //var anvils = from obj in reader.WorldItems
            //           where obj.id == "i_anvil"
            //           select obj;

            //Console.WriteLine($"il y as {anvils.Count()} anvil dans le monde");

            //********************Get item For Account *********************
            //GetItemForAccount("Dixonzegm");


            Console.WriteLine("Report done... You can close this windows");
            Console.ReadLine();
            
        }



        //***********************************************************
        //*****************************FUNCTION**********************
        //***********************************************************
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
