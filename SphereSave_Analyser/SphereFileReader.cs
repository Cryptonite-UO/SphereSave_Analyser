using System;
using System.Collections.Generic;
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
        public string name;
        public string color;
        public string dispid;// can it be a signature ???
        public string amount;
        public string cont;
        public string flags;
        public Vector3D Position;
        public string dir;
        public string timer;
        public string create;
        public string spawnitem;
        public string npc;
        public string homedist;
        public string p;
        public string dam;
        public string act;
        public string home;
        public string okarma;
        public string ofame;
        public string food;
        public string ostr;
        public string oint;
        public string odex;
        public string maxhits;
        public string hits;
        public string maxstam;
        public string stam;
        public string maxmana;
        public string mana;
        public string magicresistance;
        public string tactics;
        public string poisoning;
        public string wrestling;
        public string rescold;
        public string resenergy;
        public string resfire;
        public string resphysical;
        public string respoison;
        public string attr;
        public string spawnid;
        public string timelo;
        public string timehi;
        public string maxdist;
        public string addobj;
        public string taming;
        public string actp;
        public string action;
        public string ofood;
        public string timestamp;
        public string link;
        public string morep;
        public string layer;
        public string oskin;
        public string evaluatingintel;
        public string magery;
        public string more1;
        public string more2;
        public string parrying;
        public string contgrid;
        public string armor;
        public string usescur;
        public string usesmax;
        public string archery;
        public string swordsmanship;
        public string macefighting;
        public string fencing;
        public string meditation;
        public string type;
        public string need;
        public string alchemy;
        public string healing;
        public string tasteid;
        public string imbuing;
        public string actpri;
        public string focus;
        public string events;
        public string anatomy;
        public string animallore;
        public string camping;
        public string carpentry;
        public string fishing;
        public string herding;
        public string veterinary;
        public string bushido;
        public string cooking;
        public string armslore;
        public string detectinghidden;
        public string forensics;
        public string tracking;
        public string chivalry;
        public string throwing;
        public string modar;
        public string range;
        public string itemid;
        public string lockpicking;
        public string tinkering;
        public string removetrap;
        public string cartography;
        public string lumberjacking;
        public string mining;
        public string tailoring;
        public string inscription;
        public string spellweaving;
        public string spiritspeak;
        public string bowcraft;
        public string mysticism;
        public string blacksmithing;
        public string necromancy;
        public string obody;
        public string hiding;
        public string author;
        public string peacemaking;
        public string enticement;
        public string provocation;
        public string musicianship;
        public string modstr;
        public string quality;
        public string snooping;
        public string stealing;
        public string stealth;
        public string actarg1;
        public string actarg2;
        public string actarg3;
        public string begging;
        public string ninjitsu;
        public string modint;
        public string moddex;
        public string dooropenid;
        public string modmaxweight;
        public string price;
        public string title;
        public string speech;

        public BaseSphereObj(string id)
        {
        }
    }

    public class WorldItem : BaseSphereObj
    {
        public WorldItem(string id) : base(id)
        {
        }
    }

    public class WorldChar : BaseSphereObj
    {
        public bool IsPlayer;

        public WorldChar(string id) : base(id)
        {

        }

        public override string ToString()
        {
            return base.id;
        }
    }

    public class SphereFileReader
    {
        public List<WorldChar> WorldCharacters;

        public List<WorldItem> WorldItems;

        Dictionary<string, string> notfound = new Dictionary<string, string>();

        public SphereFileReader()
        {
            WorldCharacters = new List<WorldChar>();
            WorldItems = new List<WorldItem>();
        }

        public void ReadSaveFileToObj(string file)
        {

            string header = "";
            int linecount = 0;
            bool found = false;
            BlockType type = BlockType.Unknown;

            foreach (string line in File.ReadAllLines(file))
            {
                if (linecount < 4)
                {
                    header += line + "\n";
                }

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
                    found = false;
                    type = BlockType.Unknown;
                    continue;
                }
                linecount++;
            }
            if (notfound.Keys.Count > 0)
             {
                string autogenprop = $"//##############Code-Auto-Gen-Proprities#################\n";
                string autogencase = $"//##############Code-Auto-Gen-Case#################";
                foreach (var item in notfound)
                {
                    autogenprop +=
                    $"\npublic string {item.Key.ToLower()};";
                    autogencase +=
                    $"\ncase \"{item.Key.ToUpper()}\" :" + Environment.NewLine +
                    $"      string {item.Key.ToLower()} = value; //TODO: convert ex value. {item.Value}" + Environment.NewLine +
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

        private void MapPropToObj(BlockType blocktype,string[] prop)
        {
            if (prop[0].Split('.').Length > 1)
                return;//ignore les tag mais quoi d'autres ? //TAG.xxx
            string key = prop[0].ToUpper();
            string value = "";
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
                    int serial = StringHexToInt(value);
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
                    string create = value; //TODO: convert ex value. 14826
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].create = create;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].create = create;
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
                case "SPAWNITEM":
                    string spawnitem = value; //TODO: convert ex value. 0401acbbf
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].spawnitem = spawnitem;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].spawnitem = spawnitem;
                    }
                    break;
                case "NPC":
                    string npc = value; //TODO: convert ex value. 8
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
                    string homedist = value; //TODO: convert ex value. 50
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].homedist = homedist;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].homedist = homedist;
                    }
                    break;
                case "P":
                    string p = value; //TODO: convert ex value. 65,58,-59
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].p = p;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].p = p;
                    }
                    break;
                case "FLAGS":
                    string flags = value; //TODO: convert ex value. 010000000
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
                    string dam = value; //TODO: convert ex value. 21,54
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
                case "HOME":
                    string home = value; //TODO: convert ex value. 76,81,-60
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].home = home;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].home = home;
                    }
                    break;
                case "OKARMA":
                    string okarma = value; //TODO: convert ex value. -3351
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
                    string ofame = value; //TODO: convert ex value. 3016
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].ofame = ofame;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].ofame = ofame;
                    }
                    break;
                case "FOOD":
                    string food = value; //TODO: convert ex value. 45
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
                    string ostr = value; //TODO: convert ex value. 303
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
                    string oint = value; //TODO: convert ex value. 42
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
                    string odex = value; //TODO: convert ex value. 85
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].odex = odex;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].odex = odex;
                    }
                    break;
                case "MAXHITS":
                    string maxhits = value; //TODO: convert ex value. 155
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].maxhits = maxhits;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].maxhits = maxhits;
                    }
                    break;
                case "HITS":
                    string hits = value; //TODO: convert ex value. 155
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].hits = hits;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].hits = hits;
                    }
                    break;
                case "MAXSTAM":
                    string maxstam = value; //TODO: convert ex value. 89
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].maxstam = maxstam;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].maxstam = maxstam;
                    }
                    break;
                case "STAM":
                    string stam = value; //TODO: convert ex value. 89
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].stam = stam;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].stam = stam;
                    }
                    break;
                case "MAXMANA":
                    string maxmana = value; //TODO: convert ex value. 57
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].maxmana = maxmana;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].maxmana = maxmana;
                    }
                    break;
                case "MANA":
                    string mana = value; //TODO: convert ex value. 57
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].mana = mana;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].mana = mana;
                    }
                    break;
                case "MAGICRESISTANCE":
                    string magicresistance = value; //TODO: convert ex value. 678
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
                    string tactics = value; //TODO: convert ex value. 872
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].tactics = tactics;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].tactics = tactics;
                    }
                    break;
                case "POISONING":
                    string poisoning = value; //TODO: convert ex value. 926
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].poisoning = poisoning;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].poisoning = poisoning;
                    }
                    break;
                case "WRESTLING":
                    string wrestling = value; //TODO: convert ex value. 930
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].wrestling = wrestling;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].wrestling = wrestling;
                    }
                    break;
                case "RESCOLD":
                    string rescold = value; //TODO: convert ex value. 21
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
                    string resenergy = value; //TODO: convert ex value. 25
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
                    string resfire = value; //TODO: convert ex value. 24
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
                    string resphysical = value; //TODO: convert ex value. 10
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
                    string respoison = value; //TODO: convert ex value. 36
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].respoison = respoison;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].respoison = respoison;
                    }
                    break;
                case "COLOR":
                    string color = value; //TODO: convert ex value. 020
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].color = color;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].color = color;
                    }
                    break;
                case "DISPID":
                    string dispid = value; //TODO: convert ex value. i_pet_wisp
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].dispid = dispid;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].dispid = dispid;
                    }
                    break;
                case "ATTR":
                    string attr = value; //TODO: convert ex value. 0b0
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].attr = attr;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].attr = attr;
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
                case "AMOUNT":
                    string amount = value; //TODO: convert ex value. 5
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].amount = amount;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].amount = amount;
                    }
                    break;
                case "TIMELO":
                    string timelo = value; //TODO: convert ex value. 5
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
                    string timehi = value; //TODO: convert ex value. 25
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
                    string maxdist = value; //TODO: convert ex value. 50
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
                    string addobj = value; //TODO: convert ex value. 0ae1b
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].addobj = addobj;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].addobj = addobj;
                    }
                    break;
                case "TAMING":
                    string taming = value; //TODO: convert ex value. 1300
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].taming = taming;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].taming = taming;
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
                case "ACTP":
                    string actp = value; //TODO: convert ex value. 2101,33,-28
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].actp = actp;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].actp = actp;
                    }
                    break;
                case "ACTION":
                    string action = value; //TODO: convert ex value. 103
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].action = action;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].action = action;
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
                case "TIMESTAMP":
                    string timestamp = value; //TODO: convert ex value. 47919625657
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].timestamp = timestamp;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].timestamp = timestamp;
                    }
                    break;
                case "LINK":
                    string link = value; //TODO: convert ex value. 07f412
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].link = link;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].link = link;
                    }
                    break;
                case "MOREP":
                    string morep = value; //TODO: convert ex value. -1,-1
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
                    string layer = value; //TODO: convert ex value. 30
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
                    string cont = value; //TODO: convert ex value. 0136731
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].cont = cont;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].cont = cont;
                    }
                    break;
                case "OSKIN":
                    string oskin = value; //TODO: convert ex value. 04c2
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].oskin = oskin;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].oskin = oskin;
                    }
                    break;
                case "EVALUATINGINTEL":
                    string evaluatingintel = value; //TODO: convert ex value. 607
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].evaluatingintel = evaluatingintel;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].evaluatingintel = evaluatingintel;
                    }
                    break;
                case "MAGERY":
                    string magery = value; //TODO: convert ex value. 619
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].magery = magery;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].magery = magery;
                    }
                    break;
                case "MORE1":
                    string more1 = value; //TODO: convert ex value. 0640a0895
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
                    string more2 = value; //TODO: convert ex value. 0152630
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].more2 = more2;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].more2 = more2;
                    }
                    break;
                case "PARRYING":
                    string parrying = value; //TODO: convert ex value. 580
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].parrying = parrying;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].parrying = parrying;
                    }
                    break;
                case "CONTGRID":
                    string contgrid = value; //TODO: convert ex value. 1
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
                    string armor = value; //TODO: convert ex value. 30,30
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].armor = armor;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].armor = armor;
                    }
                    break;
                case "USESCUR":
                    string usescur = value; //TODO: convert ex value. 150
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
                    string usesmax = value; //TODO: convert ex value. 150
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
                    string archery = value; //TODO: convert ex value. 136
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
                    string swordsmanship = value; //TODO: convert ex value. 753
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
                    string macefighting = value; //TODO: convert ex value. 141
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
                    string fencing = value; //TODO: convert ex value. 136
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].fencing = fencing;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].fencing = fencing;
                    }
                    break;
                case "MEDITATION":
                    string meditation = value; //TODO: convert ex value. 996
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].meditation = meditation;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].meditation = meditation;
                    }
                    break;
                case "TYPE":
                    string type = value; //TODO: convert ex value. t_spawn_item
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].type = type;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].type = type;
                    }
                    break;
                case "NAME":
                    string name = value; //TODO: convert ex value. Alexander
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].name = name;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].name = name;
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
                    string alchemy = value; //TODO: convert ex value. 285
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
                    string healing = value; //TODO: convert ex value. 200
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
                    string tasteid = value; //TODO: convert ex value. 206
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
                    string imbuing = value; //TODO: convert ex value. 229
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].imbuing = imbuing;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].imbuing = imbuing;
                    }
                    break;
                case "ACTPRI":
                    string actpri = value; //TODO: convert ex value. 032
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].actpri = actpri;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].actpri = actpri;
                    }
                    break;
                case "FOCUS":
                    string focus = value; //TODO: convert ex value. 351
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].focus = focus;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].focus = focus;
                    }
                    break;
                case "EVENTS":
                    string events = value; //TODO: convert ex value. e_store_mount
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].events = events;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].events = events;
                    }
                    break;
                case "ANATOMY":
                    string anatomy = value; //TODO: convert ex value. 276
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].anatomy = anatomy;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].anatomy = anatomy;
                    }
                    break;
                case "ANIMALLORE":
                    string animallore = value; //TODO: convert ex value. 217
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
                    string camping = value; //TODO: convert ex value. 283
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
                    string carpentry = value; //TODO: convert ex value. 266
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].carpentry = carpentry;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].carpentry = carpentry;
                    }
                    break;
                case "FISHING":
                    string fishing = value; //TODO: convert ex value. 229
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].fishing = fishing;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].fishing = fishing;
                    }
                    break;
                case "HERDING":
                    string herding = value; //TODO: convert ex value. 284
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].herding = herding;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].herding = herding;
                    }
                    break;
                case "VETERINARY":
                    string veterinary = value; //TODO: convert ex value. 241
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
                    string bushido = value; //TODO: convert ex value. 261
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].bushido = bushido;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].bushido = bushido;
                    }
                    break;
                case "COOKING":
                    string cooking = value; //TODO: convert ex value. 203
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
                    string armslore = value; //TODO: convert ex value. 284
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
                    string detectinghidden = value; //TODO: convert ex value. 701
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
                    string forensics = value; //TODO: convert ex value. 568
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
                    string tracking = value; //TODO: convert ex value. 209
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
                    string chivalry = value; //TODO: convert ex value. 281
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
                    string throwing = value; //TODO: convert ex value. 527
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
                    string modar = value; //TODO: convert ex value. 6
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].modar = modar;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].modar = modar;
                    }
                    break;
                case "RANGE":
                    string range = value; //TODO: convert ex value. 12,0
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].range = range;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].range = range;
                    }
                    break;
                case "ITEMID":
                    string itemid = value; //TODO: convert ex value. 240
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
                    string lockpicking = value; //TODO: convert ex value. 290
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
                    string tinkering = value; //TODO: convert ex value. 200
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
                    string removetrap = value; //TODO: convert ex value. 298
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
                    string cartography = value; //TODO: convert ex value. 222
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
                    string lumberjacking = value; //TODO: convert ex value. 255
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
                    string mining = value; //TODO: convert ex value. 262
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
                    string tailoring = value; //TODO: convert ex value. 283
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
                    string inscription = value; //TODO: convert ex value. 265
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
                    string spellweaving = value; //TODO: convert ex value. 233
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
                    string spiritspeak = value; //TODO: convert ex value. 249
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].spiritspeak = spiritspeak;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].spiritspeak = spiritspeak;
                    }
                    break;
                case "BOWCRAFT":
                    string bowcraft = value; //TODO: convert ex value. 256
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].bowcraft = bowcraft;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].bowcraft = bowcraft;
                    }
                    break;
                case "MYSTICISM":
                    string mysticism = value; //TODO: convert ex value. 213
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
                    string blacksmithing = value; //TODO: convert ex value. 248
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].blacksmithing = blacksmithing;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].blacksmithing = blacksmithing;
                    }
                    break;
                case "NECROMANCY":
                    string necromancy = value; //TODO: convert ex value. 728
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].necromancy = necromancy;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].necromancy = necromancy;
                    }
                    break;
                case "OBODY":
                    string obody = value; //TODO: convert ex value. c_npc_quete_wrong2
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].obody = obody;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].obody = obody;
                    }
                    break;
                case "HIDING":
                    string hiding = value; //TODO: convert ex value. 750
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].hiding = hiding;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].hiding = hiding;
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
                case "PEACEMAKING":
                    string peacemaking = value; //TODO: convert ex value. 627
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
                    string enticement = value; //TODO: convert ex value. 682
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
                    string provocation = value; //TODO: convert ex value. 564
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].provocation = provocation;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].provocation = provocation;
                    }
                    break;
                case "MUSICIANSHIP":
                    string musicianship = value; //TODO: convert ex value. 523
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
                    string modstr = value; //TODO: convert ex value. 10
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].modstr = modstr;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].modstr = modstr;
                    }
                    break;
                case "QUALITY":
                    string quality = value; //TODO: convert ex value. 113
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].quality = quality;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].quality = quality;
                    }
                    break;
                case "SNOOPING":
                    string snooping = value; //TODO: convert ex value. 271
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].snooping = snooping;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].snooping = snooping;
                    }
                    break;
                case "STEALING":
                    string stealing = value; //TODO: convert ex value. 567
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
                    string stealth = value; //TODO: convert ex value. 762
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].stealth = stealth;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].stealth = stealth;
                    }
                    break;
                case "ACTARG1":
                    string actarg1 = value; //TODO: convert ex value. 02
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
                    string actarg2 = value; //TODO: convert ex value. 0f0001
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].actarg2 = actarg2;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].actarg2 = actarg2;
                    }
                    break;
                case "ACTARG3":
                    string actarg3 = value; //TODO: convert ex value. 06
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].actarg3 = actarg3;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].actarg3 = actarg3;
                    }
                    break;
                case "BEGGING":
                    string begging = value; //TODO: convert ex value. 616
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
                    string ninjitsu = value; //TODO: convert ex value. 672
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
                    string modint = value; //TODO: convert ex value. 30
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
                    string moddex = value; //TODO: convert ex value. 15
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
                    string dooropenid = value; //TODO: convert ex value. 39648
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].dooropenid = dooropenid;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].dooropenid = dooropenid;
                    }
                    break;
                case "MODMAXWEIGHT":
                    string modmaxweight = value; //TODO: convert ex value. 1380
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].modmaxweight = modmaxweight;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].modmaxweight = modmaxweight;
                    }
                    break;
                case "PRICE":
                    string price = value; //TODO: convert ex value. 20000
                    if (blocktype == BlockType.WorldItem)
                    {
                        WorldItems[ptr].price = price;
                    }
                    else if (blocktype == BlockType.WorldChar)
                    {
                        WorldCharacters[ptr].price = price;
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

                default:
                    notfound.TryAdd(key.ToUpper(), value);
                    break;
            }
        }

        private int StringHexToInt(string s)
        {
            return int.Parse(s, System.Globalization.NumberStyles.HexNumber); ;
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
}
