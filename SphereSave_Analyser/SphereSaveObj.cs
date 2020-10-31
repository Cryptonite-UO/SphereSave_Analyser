using System;
using System.Collections.Generic;

namespace SphereSave_Analyser
{
    public class SphereSaveObj
    {
        public SphereSaveObj(string type, string id)
        {
            Props = new List<string[]>();
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
    }
}
