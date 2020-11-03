using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;

namespace SphereSave_Analyser
{
    public enum SphereFileType
    {
        SphereChars,
        SphereWorld,
        SphereScript
    }

    public enum BlockType
    {
        WorldChar,
        WorldItem,
        Multi,
        Unknown
    }

    public class Vector3D
    {
        public int X, Y, Z;
    }

    public class BaseSphereObj
    {
        public string id;
        public int serial;
        public Vector3D Position;
        //##############Code-Auto-Gen-Proprities#################
        public int create;
        public int color;
        public string timer;
        public int npc;
        public int homedist;
        public int actpri;
        public string p;
        public string dir;
        public int oskin;
        public int flags;
        public string dam;
        public string act;
        public string action;
        public string actarg1;
        public int actarg2;
        public string okarma;
        public string ofame;
        public string ofood;
        public string food;
        public int ostr;
        public int oint;
        public int odex;
        public int hits;
        public int stam;
        public int mana;
        public int parrying;
        public int magicresistance;
        public int tactics;
        public int taming;
        public int wrestling;
        public int focus;
        public int link;
        public int attr;
        public string more1;
        public string more2;
        public string morep;
        public int layer;
        public int cont;
        public string name;
        public string home;
        public int herding;
        public int magery;
        public int amount;
        public int contgrid;
        public string armor;
        public string dispid;
        public string timestamp;
        public int actarg3;
        public int fishing;
        public string obody;
        public string events;
        public int modmaxweight;
        public string type;
        public string spawnitem;
        public int rescold;
        public int resenergy;
        public int resfire;
        public int resphysical;
        public int respoison;
        public string spawnid;
        public int timelo;
        public int timehi;
        public int maxdist;
        public string addobj;
        public int maxhits;
        public int maxstam;
        public int maxmana;
        public int poisoning;
        public int anatomy;
        public int evaluatingintel;
        public int meditation;
        public string actp;
        public int quality;
        public string range;
        public int usescur;
        public int usesmax;
        public int archery;
        public int swordsmanship;
        public int macefighting;
        public int fencing;
        public string need;
        public int alchemy;
        public int healing;
        public int tasteid;
        public int imbuing;
        public int animallore;
        public int camping;
        public int carpentry;
        public int veterinary;
        public int bushido;
        public int price;
        public string author;
        public int cooking;
        public int armslore;
        public int detectinghidden;
        public int forensics;
        public int tracking;
        public int chivalry;
        public int throwing;
        public int modar;
        public string pin;
        public int itemid;
        public int lockpicking;
        public int tinkering;
        public int removetrap;
        public int cartography;
        public int lumberjacking;
        public int mining;
        public int tailoring;
        public int inscription;
        public int spellweaving;
        public int spiritspeak;
        public int mysticism;
        public int blacksmithing;
        public int bowcraft;
        public string align;
        public string abbrev;
        public int necromancy;
        public int hiding;
        public int musicianship;
        public int modstr;
        public int snooping;
        public int peacemaking;
        public int enticement;
        public int provocation;
        public int stealing;
        public int stealth;
        public int begging;
        public int ninjitsu;
        public int modint;
        public int moddex;
        public int dooropenid;
        public string title;
        public string speech;
        //##############Code-Auto-Gen-Proprities#################
        public string account;
        public int deaths;
        public string skillclass;
        public int kills;
        public string dspeech;
        public string profile;
        public int speechcolor;
        public int emotecolor;
        public string stepstealth;
        public int modmaxhits;
        public int modmaxmana;
        public int modmaxstam;
        public int speedmode;
        public int font;
        public int maxfollower;
        public int increasedefchancemax;
        public int rescoldmax;
        public int resenergymax;
        public int resfiremax;
        public int resphysicalmax;
        public int respoisonmax;
        public int nightsight;
        public int exp;
        public int refusetrades;

        public BaseSphereObj(string id)
        {
            this.id = id;
        }
    }

    public class WorldItem : BaseSphereObj
    {
        public WorldItem(string id) : base(id)
        {
        }

        public override string ToString()
        {
            return $"{amount} {id}";
        }
    }

    public class WorldChar : BaseSphereObj
    {
        public int TotalGold;

        public bool IsPlayer
        {
            get
            {
                return !String.IsNullOrEmpty(account);
            }
        }



        public WorldChar(string id) : base(id)
        {

        }

        public override string ToString()
        {
            if (IsPlayer)
                return name;
            return base.id;
        }
    }

    public class SphereFileReader
    {
        public string dirpathsave = ConfigurationManager.AppSettings["dirpathsave"];
        public bool AutoGenCode; //si trouve des nouvelle propriter va generer le code pour le traduire

        public List<WorldChar> WorldCharacters;

        public List<WorldItem> WorldItems;

        Dictionary<string, string> notfound = new Dictionary<string, string>();

        public SphereFileReader()
        {
            WorldCharacters = new List<WorldChar>();
            WorldItems = new List<WorldItem>();
        }

        private void ReadSaveFileToObj(string file)
        {
            BlockType type = BlockType.Unknown;

            if (!File.Exists(file))
                throw new Exception($"Le fichier spécifié n'existe pas : {file}");

            foreach (string line in File.ReadAllLines(file))
            {
                if (line.StartsWith("[EOF]", StringComparison.Ordinal))
                {
                    break;
                }

                if (line.StartsWith("[", StringComparison.Ordinal))
                {
                    type = GetBlockType(line.Remove(0, 1).Split(' ')[0]);
                    string id = line.Remove(line.Length - 1, 1).Split(' ')[1];

                    switch (type)
                    {
                        case BlockType.WorldChar:
                            WorldCharacters.Add(new WorldChar(id));
                            break;
                        case BlockType.WorldItem:
                            WorldItems.Add(new WorldItem(id));
                            break;
                    }
                    continue;
                }

                if (type != BlockType.Unknown && !String.IsNullOrWhiteSpace(line))
                {
                    string[] props = line.Split('=');
                    try
                    {
                        MapPropToObj(type,props);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        continue;
                    }
                }

                if (String.IsNullOrEmpty(line))
                {
                    type = BlockType.Unknown;
                    continue;
                }
            }
            if (notfound.Keys.Count > 0 && AutoGenCode)
             {
                string autogenprop = $"//##############Code-Auto-Gen-Proprities#################\n";
                string autogencase = $"//##############Code-Auto-Gen-Case#################";
                foreach (var item in notfound)
                {
                    autogenprop +=
                    $"\npublic ";
                    if(IsNumeric(item.Value))
                    {
                        autogenprop +=
                        $"int ";
                    }
                    else
                    {
                        autogenprop +=
                        $"string ";
                    }
                    autogenprop +=
                    $"{item.Key.ToLower()};";
                    autogencase +=
                    $"\ncase \"{item.Key.ToUpper()}\" :" + Environment.NewLine;
                    if (IsNumeric(item.Value))
                    {
                        autogencase +=
                        $"      int {item.Key.ToLower()} = int.Parse(value);" + Environment.NewLine;
                    }
                    else
                    {
                        autogencase +=
                        $"      string {item.Key.ToLower()} = value; //TODO: convert ex value. {item.Value}" + Environment.NewLine;
                    }
                    autogencase +=
                    $"      if (blocktype == BlockType.WorldItem)" + Environment.NewLine +
                    $"      {{ " + Environment.NewLine +
                    $"          WorldItems[ptr].{item.Key.ToLower()} = {item.Key.ToLower()};" + Environment.NewLine +
                    $"      }} " + Environment.NewLine +
                    $"      else if (blocktype == BlockType.WorldChar)" + Environment.NewLine +
                    $"      {{ " + Environment.NewLine +
                    $"          WorldCharacters[ptr].{item.Key.ToLower()} ={item.Key.ToLower()};" + Environment.NewLine +
                    $"      }} " + Environment.NewLine +
                    $"break;";
                }
                Console.WriteLine(autogenprop);
                Console.WriteLine(autogencase);
            }
        }

        private bool IsNumeric(string str)
        {
            int num = 0;
            int.TryParse(str,out num);

            if (num > 0)
                return true;
            else
                return false;
        }

        private void MapPropToObj(BlockType blocktype,string[] prop)
        {
            string key = prop[0].ToUpper();
            string value = "";
            if (key.Contains("CHARTER") || key.Contains("MEMBER")
                || key.Contains("SKILLLOCK") || key.Contains("STATLOCK"))
                return;//livre,guild,statlock et skilllock non supporté
            if (key.Split('.').Length > 1)
                return;//ignore les tag mais quoi d'autres ? //TAG.xxx
            if (prop.Length > 1)
                value = prop[1];
            int ptr = 0;
            if (blocktype == BlockType.WorldItem)
                ptr = WorldItems.Count - 1;
            else if (blocktype == BlockType.WorldChar)
                ptr = WorldCharacters.Count - 1;

            switch (key)//baseobject
            {
                case "SERIAL":
                    int serial = Util.StringHexToInt(value);
                    if (serial == 0)
                    {
                        throw new Exception("le Serial ne peut être 0");
                    }
                    if (blocktype == BlockType.WorldChar)
                        WorldCharacters[ptr].serial = serial;
                    else if (blocktype == BlockType.WorldItem)
                        WorldItems[ptr].serial = serial;
                    break;
                //##############Code-Auto-Gen-Case#################
                case "CREATE":
                    int create = int.Parse(value);
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].create = create;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].create = create;
                    }
                    break;
                case "COLOR":
                    int color = Util.StringHexToInt(value);
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].color = color;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].color = color;
                    }
                    break;
                case "TIMER":
                    string timer = value; //TODO: convert ex value. 0
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].timer = timer;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].timer = timer;
                    }
                    break;
                case "NPC":
                    int npc = int.Parse(value);
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].npc = npc;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].npc = npc;
                    }
                    break;
                case "HOMEDIST":
                    int homedist = int.Parse(value);
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].homedist = homedist;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].homedist = homedist;
                    }
                    break;
                case "ACTPRI":
                    int actpri = Util.StringHexToInt(value);
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].actpri = actpri;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].actpri = actpri;
                    }
                    break;
                case "P":
                    string p = value; //TODO: convert ex value. 2,0,1
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].p = p;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].p = p;
                    }
                    break;
                case "DIR":
                    string dir = value; //TODO: convert ex value. 0
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].dir = dir;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].dir = dir;
                    }
                    break;
                case "OSKIN":
                    int oskin = Util.StringHexToInt(value);
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].oskin = oskin;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].oskin = oskin;
                    }
                    break;
                case "FLAGS":
                    int flags = Util.StringHexToInt(value);
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].flags = flags;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].flags = flags;
                    }
                    break;
                case "DAM":
                    string dam = value; //TODO: convert ex value. 3,4
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].dam = dam;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].dam = dam;
                    }
                    break;
                case "ACT":
                    string act = value; //TODO: convert ex value. 04fffffff
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].act = act;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].act = act;
                    }
                    break;
                case "ACTION":
                    string action =value;
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].action = action;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].action = action;
                    }
                    break;
                case "ACTARG1":
                    string actarg1 = value; //TODO: convert ex value. 07003d241
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].actarg1 = actarg1;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].actarg1 = actarg1;
                    }
                    break;
                case "ACTARG2":
                    int actarg2 = Util.StringHexToInt(value);
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].actarg2 = actarg2;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].actarg2 = actarg2;
                    }
                    break;
                case "OKARMA":
                    string okarma = value; //TODO: convert ex value. -96
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].okarma = okarma;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].okarma = okarma;
                    }
                    break;
                case "OFAME":
                    string ofame = value; //TODO: convert ex value. 0
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].ofame = ofame;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].ofame = ofame;
                    }
                    break;
                case "OFOOD":
                    string ofood = value; //TODO: convert ex value. 0
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].ofood = ofood;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].ofood = ofood;
                    }
                    break;
                case "FOOD":
                    string food = value; //TODO: convert ex value. 0
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].food = food;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].food = food;
                    }
                    break;
                case "OSTR":
                    int ostr = int.Parse(value);
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].ostr = ostr;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].ostr = ostr;
                    }
                    break;
                case "OINT":
                    int oint = int.Parse(value);
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].oint = oint;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].oint = oint;
                    }
                    break;
                case "ODEX":
                    int odex = int.Parse(value);
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].odex = odex;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].odex = odex;
                    }
                    break;
                case "HITS":
                    int hits = int.Parse(value);
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].hits = hits;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].hits = hits;
                    }
                    break;
                case "STAM":
                    int stam = int.Parse(value);
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].stam = stam;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].stam = stam;
                    }
                    break;
                case "MANA":
                    int mana = int.Parse(value);
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].mana = mana;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].mana = mana;
                    }
                    break;
                case "PARRYING":
                    int parrying = int.Parse(value);
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].parrying = parrying;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].parrying = parrying;
                    }
                    break;
                case "MAGICRESISTANCE":
                    int magicresistance = int.Parse(value);
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].magicresistance = magicresistance;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].magicresistance = magicresistance;
                    }
                    break;
                case "TACTICS":
                    int tactics = int.Parse(value);
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].tactics = tactics;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].tactics = tactics;
                    }
                    break;
                case "TAMING":
                    int taming = int.Parse(value);
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].taming = taming;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].taming = taming;
                    }
                    break;
                case "WRESTLING":
                    int wrestling = int.Parse(value);
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].wrestling = wrestling;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].wrestling = wrestling;
                    }
                    break;
                case "FOCUS":
                    int focus = int.Parse(value);
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].focus = focus;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].focus = focus;
                    }
                    break;
                case "LINK":
                    int link = Util.StringHexToInt(value);
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].link = link;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].link = link;
                    }
                    break;
                case "ATTR":
                    int attr = Util.StringHexToInt(value);
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].attr = attr;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].attr = attr;
                    }
                    break;
                case "MORE1":
                    string more1 = value;
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].more1 = more1;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].more1 = more1;
                    }
                    break;
                case "MORE2":
                    string more2 = value; //TODO: convert ex value. 02ceea9d
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].more2 = more2;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].more2 = more2;
                    }
                    break;
                case "MOREP":
                    string morep = value; //TODO: convert ex value. 2863,2247,-20
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].morep = morep;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].morep = morep;
                    }
                    break;
                case "LAYER":
                    int layer = int.Parse(value);
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].layer = layer;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].layer = layer;
                    }
                    break;
                case "CONT":
                    int cont = Util.StringHexToInt(value);
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].cont = cont;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].cont = cont;
                    }
                    break;
                case "NAME":
                    string name = value; //TODO: convert ex value. Llama
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].name = name;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].name = name;
                    }
                    break;
                case "HOME":
                    string home = value; //TODO: convert ex value. 1829,2741
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].home = home;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].home = home;
                    }
                    break;
                case "HERDING":
                    int herding = int.Parse(value);
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].herding = herding;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].herding = herding;
                    }
                    break;
                case "MAGERY":
                    int magery = int.Parse(value);
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].magery = magery;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].magery = magery;
                    }
                    break;
                case "AMOUNT":
                    int amount = int.Parse(value);
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].amount = amount;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].amount = amount;
                    }
                    break;
                case "CONTGRID":
                    int contgrid = int.Parse(value);
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].contgrid = contgrid;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].contgrid = contgrid;
                    }
                    break;
                case "ARMOR":
                    string armor = value; //TODO: convert ex value. 12,12
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].armor = armor;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].armor = armor;
                    }
                    break;
                case "DISPID":
                    string dispid = value; //TODO: convert ex value. i_virtstone_6
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].dispid = dispid;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].dispid = dispid;
                    }
                    break;
                case "TIMESTAMP":
                    string timestamp = value;
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].timestamp = timestamp;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].timestamp = timestamp;
                    }
                    break;
                case "ACTARG3":
                    int actarg3 = Util.StringHexToInt(value);
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].actarg3 = actarg3;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].actarg3 = actarg3;
                    }
                    break;
                case "FISHING":
                    int fishing = int.Parse(value);
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].fishing = fishing;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].fishing = fishing;
                    }
                    break;
                case "OBODY":
                    string obody = value; //TODO: convert ex value. c_bear_black
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].obody = obody;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].obody = obody;
                    }
                    break;
                case "EVENTS":
                    string events = value; //TODO: convert ex value. e_mustang_chevalier
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].events = events;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].events = events;
                    }
                    break;
                case "MODMAXWEIGHT":
                    int modmaxweight = int.Parse(value);
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].modmaxweight = modmaxweight;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].modmaxweight = modmaxweight;
                    }
                    break;
                case "TYPE":
                    string type = value; //TODO: convert ex value. t_normal
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].type = type;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].type = type;
                    }
                    break;
                case "SPAWNITEM":
                    string spawnitem = value; //TODO: convert ex value. 0401b0c85
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].spawnitem = spawnitem;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].spawnitem = spawnitem;
                    }
                    break;
                case "RESCOLD":
                    int rescold = int.Parse(value);
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].rescold = rescold;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].rescold = rescold;
                    }
                    break;
                case "RESENERGY":
                    int resenergy = int.Parse(value);
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].resenergy = resenergy;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].resenergy = resenergy;
                    }
                    break;
                case "RESFIRE":
                    int resfire = int.Parse(value);
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].resfire = resfire;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].resfire = resfire;
                    }
                    break;
                case "RESPHYSICAL":
                    int resphysical = int.Parse(value);
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].resphysical = resphysical;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].resphysical = resphysical;
                    }
                    break;
                case "RESPOISON":
                    int respoison = int.Parse(value);
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].respoison = respoison;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].respoison = respoison;
                    }
                    break;
                case "SPAWNID":
                    string spawnid = value; //TODO: convert ex value. spawn_ocean
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].spawnid = spawnid;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].spawnid = spawnid;
                    }
                    break;
                case "TIMELO":
                    int timelo = int.Parse(value);
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].timelo = timelo;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].timelo = timelo;
                    }
                    break;
                case "TIMEHI":
                    int timehi = int.Parse(value);
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].timehi = timehi;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].timehi = timehi;
                    }
                    break;
                case "MAXDIST":
                    int maxdist = int.Parse(value);
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].maxdist = maxdist;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].maxdist = maxdist;
                    }
                    break;
                case "ADDOBJ":
                    string addobj = value; //TODO: convert ex value. 0175aec
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].addobj = addobj;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].addobj = addobj;
                    }
                    break;
                case "MAXHITS":
                    int maxhits = int.Parse(value);
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].maxhits = maxhits;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].maxhits = maxhits;
                    }
                    break;
                case "MAXSTAM":
                    int maxstam = int.Parse(value);
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].maxstam = maxstam;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].maxstam = maxstam;
                    }
                    break;
                case "MAXMANA":
                    int maxmana = int.Parse(value);
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].maxmana = maxmana;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].maxmana = maxmana;
                    }
                    break;
                case "POISONING":
                    int poisoning = int.Parse(value);
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].poisoning = poisoning;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].poisoning = poisoning;
                    }
                    break;
                case "ANATOMY":
                    int anatomy = int.Parse(value);
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].anatomy = anatomy;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].anatomy = anatomy;
                    }
                    break;
                case "EVALUATINGINTEL":
                    int evaluatingintel = int.Parse(value);
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].evaluatingintel = evaluatingintel;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].evaluatingintel = evaluatingintel;
                    }
                    break;
                case "MEDITATION":
                    int meditation = int.Parse(value);
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].meditation = meditation;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].meditation = meditation;
                    }
                    break;
                case "ACTP":
                    string actp = value; //TODO: convert ex value. 1919,63,1
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].actp = actp;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].actp = actp;
                    }
                    break;
                case "QUALITY":
                    int quality = int.Parse(value);
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].quality = quality;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].quality = quality;
                    }
                    break;
                case "RANGE":
                    string range = value; //TODO: convert ex value. 1,0
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].range = range;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].range = range;
                    }
                    break;
                case "USESCUR":
                    int usescur = int.Parse(value);
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].usescur = usescur;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].usescur = usescur;
                    }
                    break;
                case "USESMAX":
                    int usesmax = int.Parse(value);
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].usesmax = usesmax;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].usesmax = usesmax;
                    }
                    break;
                case "ARCHERY":
                    int archery = int.Parse(value);
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].archery = archery;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].archery = archery;
                    }
                    break;
                case "SWORDSMANSHIP":
                    int swordsmanship = int.Parse(value);
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].swordsmanship = swordsmanship;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].swordsmanship = swordsmanship;
                    }
                    break;
                case "MACEFIGHTING":
                    int macefighting = int.Parse(value);
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].macefighting = macefighting;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].macefighting = macefighting;
                    }
                    break;
                case "FENCING":
                    int fencing = int.Parse(value);
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].fencing = fencing;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].fencing = fencing;
                    }
                    break;
                case "NEED":
                    string need = value; //TODO: convert ex value. 1 i_gold
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].need = need;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].need = need;
                    }
                    break;
                case "ALCHEMY":
                    int alchemy = int.Parse(value);
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].alchemy = alchemy;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].alchemy = alchemy;
                    }
                    break;
                case "HEALING":
                    int healing = int.Parse(value);
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].healing = healing;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].healing = healing;
                    }
                    break;
                case "TASTEID":
                    int tasteid = int.Parse(value);
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].tasteid = tasteid;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].tasteid = tasteid;
                    }
                    break;
                case "IMBUING":
                    int imbuing = int.Parse(value);
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].imbuing = imbuing;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].imbuing = imbuing;
                    }
                    break;
                case "ANIMALLORE":
                    int animallore = int.Parse(value);
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].animallore = animallore;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].animallore = animallore;
                    }
                    break;
                case "CAMPING":
                    int camping = int.Parse(value);
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].camping = camping;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].camping = camping;
                    }
                    break;
                case "CARPENTRY":
                    int carpentry = int.Parse(value);
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].carpentry = carpentry;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].carpentry = carpentry;
                    }
                    break;
                case "VETERINARY":
                    int veterinary = int.Parse(value);
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].veterinary = veterinary;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].veterinary = veterinary;
                    }
                    break;
                case "BUSHIDO":
                    int bushido = int.Parse(value);
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].bushido = bushido;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].bushido = bushido;
                    }
                    break;
                case "PRICE":
                    int price = int.Parse(value);
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].price = price;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].price = price;
                    }
                    break;
                case "AUTHOR":
                    string author = value; //TODO: convert ex value. 
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].author = author;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].author = author;
                    }
                    break;
                case "COOKING":
                    int cooking = int.Parse(value);
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].cooking = cooking;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].cooking = cooking;
                    }
                    break;
                case "ARMSLORE":
                    int armslore = int.Parse(value);
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].armslore = armslore;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].armslore = armslore;
                    }
                    break;
                case "DETECTINGHIDDEN":
                    int detectinghidden = int.Parse(value);
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].detectinghidden = detectinghidden;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].detectinghidden = detectinghidden;
                    }
                    break;
                case "FORENSICS":
                    int forensics = int.Parse(value);
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].forensics = forensics;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].forensics = forensics;
                    }
                    break;
                case "TRACKING":
                    int tracking = int.Parse(value);
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].tracking = tracking;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].tracking = tracking;
                    }
                    break;
                case "CHIVALRY":
                    int chivalry = int.Parse(value);
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].chivalry = chivalry;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].chivalry = chivalry;
                    }
                    break;
                case "THROWING":
                    int throwing = int.Parse(value);
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].throwing = throwing;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].throwing = throwing;
                    }
                    break;
                case "MODAR":
                    int modar = int.Parse(value);
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].modar = modar;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].modar = modar;
                    }
                    break;
                case "PIN":
                    string pin = value; //TODO: convert ex value. 105,105
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].pin = pin;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].pin = pin;
                    }
                    break;
                case "ITEMID":
                    int itemid = int.Parse(value);
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].itemid = itemid;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].itemid = itemid;
                    }
                    break;
                case "LOCKPICKING":
                    int lockpicking = int.Parse(value);
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].lockpicking = lockpicking;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].lockpicking = lockpicking;
                    }
                    break;
                case "TINKERING":
                    int tinkering = int.Parse(value);
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].tinkering = tinkering;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].tinkering = tinkering;
                    }
                    break;
                case "REMOVETRAP":
                    int removetrap = int.Parse(value);
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].removetrap = removetrap;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].removetrap = removetrap;
                    }
                    break;
                case "CARTOGRAPHY":
                    int cartography = int.Parse(value);
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].cartography = cartography;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].cartography = cartography;
                    }
                    break;
                case "LUMBERJACKING":
                    int lumberjacking = int.Parse(value);
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].lumberjacking = lumberjacking;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].lumberjacking = lumberjacking;
                    }
                    break;
                case "MINING":
                    int mining = int.Parse(value);
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].mining = mining;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].mining = mining;
                    }
                    break;
                case "TAILORING":
                    int tailoring = int.Parse(value);
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].tailoring = tailoring;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].tailoring = tailoring;
                    }
                    break;
                case "INSCRIPTION":
                    int inscription = int.Parse(value);
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].inscription = inscription;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].inscription = inscription;
                    }
                    break;
                case "SPELLWEAVING":
                    int spellweaving = int.Parse(value);
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].spellweaving = spellweaving;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].spellweaving = spellweaving;
                    }
                    break;
                case "SPIRITSPEAK":
                    int spiritspeak = int.Parse(value);
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].spiritspeak = spiritspeak;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].spiritspeak = spiritspeak;
                    }
                    break;
                case "MYSTICISM":
                    int mysticism = int.Parse(value);
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].mysticism = mysticism;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].mysticism = mysticism;
                    }
                    break;
                case "BLACKSMITHING":
                    int blacksmithing = int.Parse(value);
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].blacksmithing = blacksmithing;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].blacksmithing = blacksmithing;
                    }
                    break;
                case "BOWCRAFT":
                    int bowcraft = int.Parse(value);
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].bowcraft = bowcraft;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].bowcraft = bowcraft;
                    }
                    break;
                case "ALIGN":
                    string align = value; //TODO: convert ex value. 0
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].align = align;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].align = align;
                    }
                    break;
                case "ABBREV":
                    string abbrev = value; //TODO: convert ex value. Veladorn
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].abbrev = abbrev;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].abbrev = abbrev;
                    }
                    break;
                case "NECROMANCY":
                    int necromancy = int.Parse(value);
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].necromancy = necromancy;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].necromancy = necromancy;
                    }
                    break;
                case "HIDING":
                    int hiding = int.Parse(value);
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].hiding = hiding;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].hiding = hiding;
                    }
                    break;
                case "MUSICIANSHIP":
                    int musicianship = int.Parse(value);
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].musicianship = musicianship;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].musicianship = musicianship;
                    }
                    break;
                case "MODSTR":
                    int modstr = int.Parse(value);
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].modstr = modstr;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].modstr = modstr;
                    }
                    break;
                case "SNOOPING":
                    int snooping = int.Parse(value);
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].snooping = snooping;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].snooping = snooping;
                    }
                    break;
                case "PEACEMAKING":
                    int peacemaking = int.Parse(value);
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].peacemaking = peacemaking;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].peacemaking = peacemaking;
                    }
                    break;
                case "ENTICEMENT":
                    int enticement = int.Parse(value);
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].enticement = enticement;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].enticement = enticement;
                    }
                    break;
                case "PROVOCATION":
                    int provocation = int.Parse(value);
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].provocation = provocation;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].provocation = provocation;
                    }
                    break;
                case "STEALING":
                    int stealing = int.Parse(value);
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].stealing = stealing;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].stealing = stealing;
                    }
                    break;
                case "STEALTH":
                    int stealth = int.Parse(value);
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].stealth = stealth;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].stealth = stealth;
                    }
                    break;
                case "BEGGING":
                    int begging = int.Parse(value);
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].begging = begging;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].begging = begging;
                    }
                    break;
                case "NINJITSU":
                    int ninjitsu = int.Parse(value);
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].ninjitsu = ninjitsu;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].ninjitsu = ninjitsu;
                    }
                    break;
                case "MODINT":
                    int modint = int.Parse(value);
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].modint = modint;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].modint = modint;
                    }
                    break;
                case "MODDEX":
                    int moddex = int.Parse(value);
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].moddex = moddex;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].moddex = moddex;
                    }
                    break;
                case "DOOROPENID":
                    int dooropenid = int.Parse(value);
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].dooropenid = dooropenid;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].dooropenid = dooropenid;
                    }
                    break;
                case "TITLE":
                    string title = value; //TODO: convert ex value. fighter
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].title = title;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].title = title;
                    }
                    break;
                case "SPEECH":
                    string speech = value; //TODO: convert ex value. spk_speech_detect
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].speech = speech;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].speech = speech;
                    }
                    break;
                //##############Code-Auto-Gen-Case#################
                case "ACCOUNT":
                    string account = value; //TODO: convert ex value. cdumdum
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].account = account;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].account = account;
                    }
                    break;
                case "DEATHS":
                    int deaths = int.Parse(value);
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].deaths = deaths;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].deaths = deaths;
                    }
                    break;
                case "SKILLCLASS":
                    string skillclass = value; //TODO: convert ex value. nainsnovole
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].skillclass = skillclass;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].skillclass = skillclass;
                    }
                    break;
                case "KILLS":
                    int kills = int.Parse(value);
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].kills = kills;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].kills = kills;
                    }
                    break;
                case "DSPEECH":
                    string dspeech = value; //TODO: convert ex value. spk_guildspeech
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].dspeech = dspeech;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].dspeech = dspeech;
                    }
                    break;
                case "PROFILE":
                    string profile = value; //TODO: convert ex value. 50 peacemaking\r50 discordance
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].profile = profile;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].profile = profile;
                    }
                    break;
                case "SPEECHCOLOR":
                    int speechcolor = int.Parse(value);
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].speechcolor = speechcolor;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].speechcolor = speechcolor;
                    }
                    break;
                case "EMOTECOLOR":
                    int emotecolor = int.Parse(value);
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].emotecolor = emotecolor;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].emotecolor = emotecolor;
                    }
                    break;
                case "STEPSTEALTH":
                    string stepstealth = value; //TODO: convert ex value. -1
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].stepstealth = stepstealth;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].stepstealth = stepstealth;
                    }
                    break;
                case "MODMAXHITS":
                    int modmaxhits = int.Parse(value);
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].modmaxhits = modmaxhits;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].modmaxhits = modmaxhits;
                    }
                    break;
                case "MODMAXMANA":
                    int modmaxmana = int.Parse(value);
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].modmaxmana = modmaxmana;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].modmaxmana = modmaxmana;
                    }
                    break;
                case "MODMAXSTAM":
                    int modmaxstam = int.Parse(value);
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].modmaxstam = modmaxstam;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].modmaxstam = modmaxstam;
                    }
                    break;
                case "SPEEDMODE":
                    int speedmode = int.Parse(value);
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].speedmode = speedmode;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].speedmode = speedmode;
                    }
                    break;
                case "FONT":
                    int font = int.Parse(value);
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].font = font;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].font = font;
                    }
                    break;
                case "MAXFOLLOWER":
                    int maxfollower = int.Parse(value);
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].maxfollower = maxfollower;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].maxfollower = maxfollower;
                    }
                    break;
                case "INCREASEDEFCHANCEMAX":
                    int increasedefchancemax = int.Parse(value);
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].increasedefchancemax = increasedefchancemax;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].increasedefchancemax = increasedefchancemax;
                    }
                    break;
                case "RESCOLDMAX":
                    int rescoldmax = int.Parse(value);
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].rescoldmax = rescoldmax;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].rescoldmax = rescoldmax;
                    }
                    break;
                case "RESENERGYMAX":
                    int resenergymax = int.Parse(value);
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].resenergymax = resenergymax;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].resenergymax = resenergymax;
                    }
                    break;
                case "RESFIREMAX":
                    int resfiremax = int.Parse(value);
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].resfiremax = resfiremax;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].resfiremax = resfiremax;
                    }
                    break;
                case "RESPHYSICALMAX":
                    int resphysicalmax = int.Parse(value);
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].resphysicalmax = resphysicalmax;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].resphysicalmax = resphysicalmax;
                    }
                    break;
                case "RESPOISONMAX":
                    int respoisonmax = int.Parse(value);
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].respoisonmax = respoisonmax;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].respoisonmax = respoisonmax;
                    }
                    break;
                case "NIGHTSIGHT":
                    int nightsight = int.Parse(value);
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].nightsight = nightsight;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].nightsight = nightsight;
                    }
                    break;
                case "EXP":
                    int exp = int.Parse(value);
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].exp = exp;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].exp = exp;
                    }
                    break;
                case "REFUSETRADES":
                    int refusetrades = int.Parse(value);
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].refusetrades = refusetrades;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].refusetrades = refusetrades;
                    }
                    break;
                default:
                    notfound.TryAdd(key.ToUpper(), value);
                    break;
            }
        }
          
        public void ReadFileToObj(string file, SphereFileType spherefiletype)
        {
            switch(spherefiletype)
            {
                default :
                    ReadSaveFileToObj(file);
                    break;
            }
        }

        private BlockType GetBlockType(string text)
        {
            switch(text.ToUpper())
            {
                case "WORLDITEM":
                    return BlockType.WorldItem;
                case "WORLDCHAR":
                    return BlockType.WorldChar;

            }
            return BlockType.Unknown;
        }
    }
    public enum Flags
    {
        ATTR_IDENTIFIED = 0x0001,				// This is the identified name. ???
        ATTR_DECAY = 0x0002,               // Timer currently set to decay.
        ATTR_NEWBIE = 0x0004,           // Not lost on death or sellable ?
        ATTR_MOVE_ALWAYS = 0x0008,          // Always movable (else Default as stored in client) (even if MUL says not movalble) NEVER DECAYS !
        ATTR_MOVE_NEVER = 0x0010,           // Never movable (else Default as stored in client) NEVER DECAYS !
        ATTR_MAGIC = 0x0020,            // DON'T SET THIS WHILE WORN! This item is magic as apposed to marked or markable.
        ATTR_OWNED = 0x0040,            // This is owned by the town. You need to steal it. NEVER DECAYS !
        ATTR_INVIS = 0x0080,        // Gray hidden item (to GM's or owners?)
        ATTR_CURSED = 0x0100,            // Can not be insured. Will always fall to the corpse, even if somehow blessed or insured.
                                         //  OSI: Cannot be blessed (with an Item Bless Deed, Personal Bless Deed, or Clothing Bless Deed and can't be sent to the bank with a Bag of Sending)
        ATTR_CURSED2 = 0x0200,          // cursed damned unholy
        ATTR_BLESSED = 0x0400,           // Stay in your backpack when you die, Be placed into character's backpack upon resurrection if equipped before death,
                                         //  Cannot be stolen by using the stealing skill, Cannot be insured
        ATTR_BLESSED2 = 0x0800,         // blessed sacred holy
        ATTR_FORSALE = 0x1000,          // For sale on a vendor.
        ATTR_STOLEN = 0x2000,           // The item is hot. m_uidLink = previous owner.
        ATTR_CAN_DECAY = 0x4000,                // This item can decay. but it would seem that it would not (ATTR_MOVE_NEVER etc)
        ATTR_STATIC = 0x8000,           // WorldForge merge marker. (used for statics saves)
                                        // Not listed by AxisII
        ATTR_EXCEPTIONAL = 0x10000,             // Is Exceptional
        ATTR_ENCHANTED = 0x20000,               // Is Enchanted
        ATTR_IMBUED = 0x40000,              // Is Imbued
        ATTR_QUESTITEM = 0x80000,               // Is Quest Item
        ATTR_INSURED = 0x100000,        // Is Insured
        ATTR_NODROP = 0x200000,         // No-drop
        ATTR_NOTRADE = 0x400000,        // No-trade
        ATTR_ARTIFACT = 0x800000,       // Artifact
        ATTR_LOCKEDDOWN = 0x1000000,        // Is Locked Down
        ATTR_SECURE = 0x2000000,        // Is Secure
        ATTR_REFORGED = 0x4000000,  // Is Runic Reforged.
        ATTR_OPENED = 0x8000000,        // Is Door Opened.
        ATTR_SHARDBOUND = 0x10000000,
        ATTR_ACCOUNTBOUND = 0x20000000,
        ATTR_CHARACTERBOUND = 0x40000000,
    };
}
