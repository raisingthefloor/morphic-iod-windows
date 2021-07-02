using System;
using System.Collections.Generic;
using System.Text;

namespace IOD_Tester
{
    class ProgressBars
    {
        public ProgressBars()
        {
            messages = new List<string>();
            progress = new List<double>();
            isActive = new List<bool>();
            first = true;
            writing = false;
        }

        public void Add(string message)
        {
            messages.Add(message);
            progress.Add(0.0);
            isActive.Add(false);
        }

        public void Update(int index, double value, bool active)
        {
            progress[index] = value;
            isActive[index] = active;
            Write();
        }

        public void Write()
        {
            if (writing) return;
            writing = true;
            if(first)
            {
                (startleft, starttop) = Console.GetCursorPosition();
            }
            else
            {
                (prevleft, prevtop) = Console.GetCursorPosition();
                Console.SetCursorPosition(startleft, starttop);
            }
            for(int i = 0; i < messages.Count; ++i)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(messages[i] + ":");
                sb.Append(' ', 30 - sb.Length);
                sb.Append('[');
                for(int j = 0; j < 15; ++j)
                {
                    if (progress[i] * 15.0 / 100.0 > j)
                        sb.Append(_block);
                    else
                        sb.Append(' ');
                }
                sb.Append("]" + String.Format("{0:000.00}", progress[i]) + "% " + (isActive[i] ? "ACTIVE " : "PENDING"));
                Console.WriteLine(sb.ToString());
            }
            if(first)
            {
                first = false;
            }
            else
            {
                Console.SetCursorPosition(prevleft, prevtop);
            }
            writing = false;
        }

        private List<string> messages;
        private List<double> progress;
        private List<bool> isActive;
        private bool first;
        private bool writing;
        private int startleft, starttop, prevleft, prevtop;
        const char _block = '■';
    }
}
