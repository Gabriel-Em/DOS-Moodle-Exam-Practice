using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DOS_Moodle_Exam_Practice
{
    public partial class AllQuestionsGUI : Form
    {
        public AllQuestionsGUI(List<Question> Questions)
        {
            InitializeComponent();
            
            for (int i = 0; i < Questions.Count(); i++)
            {
                QuestionsDataGridView.Rows.Add();
                QuestionsDataGridView.Rows[i].Cells[0].Value = i + 1;
                QuestionsDataGridView.Rows[i].Cells[1].Value = "RO: " + Questions[i].ROText + "\nEN: " + Questions[i].ENText;

                if (Questions[i].value == true)
                    QuestionsDataGridView.Rows[i].DefaultCellStyle.BackColor = Color.ForestGreen;
                else
                    QuestionsDataGridView.Rows[i].DefaultCellStyle.BackColor = Color.Red;

                QuestionsDataGridView.Rows[i].Cells[2].Value = Questions[i].value;
            }
        }
    }
}
