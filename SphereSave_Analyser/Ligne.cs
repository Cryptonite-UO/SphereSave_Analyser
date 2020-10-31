using System;

namespace SphereSave_Analyser
{
    public class Ligne
    {

        public Ligne(string txt, bool isNewLine)
        {
            Text = txt;
            IsNewLine = isNewLine;
        }


        public string Text
        {
            get;
            set;
        }

        public bool IsNewLine
        {
            get;
            set;
        }
    }
}
