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
    public partial class QuestionGUI : Form
    {
        public QuestionGUI(List<Question> allQuestions, List<int> selectedQuestions)
        {
            InitializeComponent();

            for (int i = 0; i < selectedQuestions.Count(); i++)
            {
                QuestionsDataGridView.Rows.Add();
                QuestionsDataGridView.Rows[i].Cells[0].Value = i + 1;
                QuestionsDataGridView.Rows[i].Cells[1].Value = "RO: " + allQuestions[selectedQuestions[i]].ROText + "\nEN: " + allQuestions[selectedQuestions[i]].ENText;

                if (allQuestions[selectedQuestions[i]].state != null)
                {
                    QuestionsDataGridView.Rows[i].Cells[2].Value = allQuestions[selectedQuestions[i]].state;

                    if (allQuestions[selectedQuestions[i]].state != "Skipped")
                    {
                        if (allQuestions[selectedQuestions[i]].Correct)
                            QuestionsDataGridView.Rows[i].DefaultCellStyle.BackColor = Color.ForestGreen;
                        else
                            QuestionsDataGridView.Rows[i].DefaultCellStyle.BackColor = Color.Red;
                    }
                    else
                        QuestionsDataGridView.Rows[i].DefaultCellStyle.BackColor = Color.Yellow;
                }
                else
                {
                    QuestionsDataGridView.Rows[i].Cells[2].Value = "-";
                    QuestionsDataGridView.Rows[i].DefaultCellStyle.BackColor = Color.LightGray;
                }

                QuestionsDataGridView.Rows[i].Cells[3].Value = allQuestions[selectedQuestions[i]].value;
            }
        }
    }
}
