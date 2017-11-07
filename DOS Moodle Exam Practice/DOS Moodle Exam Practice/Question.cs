using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DOS_Moodle_Exam_Practice
{
    public class Question
    {
        public string ENText { set; get; }
        public string ROText { set; get; }
        public bool value { set; get; }
        public string state { set; get; }
        
        public Question()
        {
            ENText = string.Empty;
            ROText = string.Empty;
            value = false;
            state = string.Empty;
        }

        public bool Correct
        {
            get
            {
                bool answer;

                if (state == "True")
                    answer = true;
                else
                    answer = false;

                return (answer == value);
            }
        }

        public Question(string ROText_, string ENText_, bool value_)
        {
            ROText = ROText_;
            ENText = ENText_;
            value = value_;
            state = null;
        }

        
    }
}
