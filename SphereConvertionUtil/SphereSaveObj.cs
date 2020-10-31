using System;
using System.Collections.Generic;

namespace SphereConvertionUtil
{
    public class SphereSaveObj
    {
        public SphereSaveObj(string type, string id)
        {
            Props = new List<string[]>();
            PropsKey = new List<KeyValuePair<string, string>>();
            Type = type;
            Id = id;
        }

        public string Id
        {
            get;
            set;
        }

        public bool EditedId
        {
            get;
            set;
        }

        public bool EditedMore
        {
            get;
            set;
        }

        public string Type
        {
            get;
            set;
        }

        public bool IsHouse
        {
            get;
            set;
        }

        public List<string[]> Props
        {
            get;
            set;
        }

        public List<KeyValuePair<String, String>> PropsKey
        {
            get;
            set;
        }
    }
}
