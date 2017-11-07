using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace DOS_Moodle_Exam_Practice
{
    public partial class GUI : Form
    {
        private List<Question> allQuestions;
        private List<int> selectedQuestions;
        private int currentQuestion, right, wrong, skipped;
        private double grade;

        public GUI()
        {
            InitializeComponent();

            loadQuestions();
            selectQuestions();

            right = 0;
            wrong = 0;
            skipped = 0;
            grade = 0;
        }

        private void StartTest()
        {
            setGUIQuestion(0);

            timerCountdown.Enabled = true;
        }

        private void loadQuestions()
        {
            try
            {
                XmlDocument file = new XmlDocument();

                file.Load(@".\Questions.xml");

                string ENText, ROText;
                bool value;

                allQuestions = new List<Question>();

                foreach (XmlNode node in file.DocumentElement.ChildNodes)  // we load each question from the database
                {
                    ENText = node.SelectSingleNode("ENText").InnerText;
                    ROText = node.SelectSingleNode("ROText").InnerText;

                    if (node.SelectSingleNode("Value").InnerText == "True")
                        value = true;
                    else
                        value = false;

                    allQuestions.Add(new Question(ROText, ENText, value));
                }
            }
            catch (Exception ex) // if there was an error, we create a crash log and recommend the user to send the log to the creator of the software
            {
                StreamWriter file = new StreamWriter(DateTime.Now.ToString("yyyy-dd-MM--HH-mm-ss") + "_Crash_Log.txt");
                file.Write(ex.ToString());
                file.Close();
                MessageBox.Show("Unhandled exception found. A log file containing the details of the exception has been saved to parent directory. Question loading failed! Press OK to exit.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }
        }

        private void timerCountdown_Tick(object sender, EventArgs e)
        {
            string[] time = lblTime.Text.Split(':');

            if (time[1] == "00")
                if (time[0] == "00")
                    endTest();
                else
                {
                    time[1] = "59";
                    time[0] = (Int32.Parse(time[0]) - 1).ToString();
                    if (time[0].Length == 1)
                        time[0] = "0" + time[0];
                }
            else
            {
                time[1] = (Int32.Parse(time[1]) - 1).ToString();
                if (time[1].Length == 1)
                    time[1] = "0" + time[1];
            }

            lblTime.Text = time[0] + ":" + time[1];
        }

        private void selectQuestions()
        {
            selectedQuestions = new List<int>();

            Random r = new Random();
            int totalQuestions = allQuestions.Count(), number;

            selectedQuestions.Add(r.Next(0, totalQuestions));

            while (selectedQuestions.Count != 50)
            {
                do
                    number = r.Next(0, totalQuestions);
                while (selectedQuestions.Contains(number));

                selectedQuestions.Add(number);
            }

            currentQuestion = 0;
        }

        private void btnTFS_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;

            if (btn.BackColor == Color.Transparent)
                switch(btn.Name)
                {
                    case "btnTrue": btnTrue.BackColor = Color.ForestGreen; btnFalse.BackColor = Color.Transparent; btnAnswerLater.BackColor = Color.Transparent; break;
                    case "btnFalse": btnTrue.BackColor = Color.Transparent; btnFalse.BackColor = Color.Red; btnAnswerLater.BackColor = Color.Transparent; break;
                    case "btnAnswerLater": btnTrue.BackColor = Color.Transparent; btnFalse.BackColor = Color.Transparent; btnAnswerLater.BackColor = Color.Yellow; break;
                }
            else
                btn.BackColor = Color.Transparent;
        }

        private void btnConfirm_Click(object sender, EventArgs e)
        {
            bool enable = true;

            if (btnTrue.BackColor != Color.Transparent)
                confirmAnswer("True");
            else
            if (btnFalse.BackColor != Color.Transparent)
                confirmAnswer("False");
            else
            if (btnAnswerLater.BackColor != Color.Transparent)
                confirmAnswer("Skipped");
            else
            {
                MessageBox.Show("Trebuie sa selectezi un raspuns! You must chose an answer!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                enable = false;
            }
            btnNextQuestion.Enabled = enable;
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            StartTest();
            panelWelcomeMessage.Visible = false;
        }

        private void confirmAnswer(string answer)
        {
            string prevState = allQuestions[selectedQuestions[currentQuestion]].state;
            allQuestions[selectedQuestions[currentQuestion]].state = answer;

            if (answer != "Skipped")
            {
                if (allQuestions[selectedQuestions[currentQuestion]].Correct)
                {
                    right++;
                    lblRight.Text = right.ToString();
                    grade += 0.2;
                    lblCurrentGrade.Text = grade.ToString();
                    if (grade >= 5)
                        lblCurrentGrade.ForeColor = Color.ForestGreen;
                }
                else
                {
                    wrong++;
                    lblWrong.Text = wrong.ToString();
                }

                lblCorrectAnswer.Text = allQuestions[selectedQuestions[currentQuestion]].value.ToString();
                if (lblCorrectAnswer.Text == "True")
                    lblCorrectAnswer.ForeColor = Color.ForestGreen;
                else
                    lblCorrectAnswer.ForeColor = Color.Red;
            }
            else
            {
                skipped++;
                lblSkipped.Text = skipped.ToString();
            }

            btnTrue.Enabled = false;
            btnFalse.Enabled = false;
            btnAnswerLater.Enabled = false;
            btnConfirm.Enabled = false;

            if (prevState == "Skipped")
            {
                skipped--;
                lblSkipped.Text = skipped.ToString();
            }

            colorButton(currentQuestion, answer);

            if (right + wrong == 50)
                endTest();
        }

        private int getNextUnansweredQuestion()
        {
            for (int i = currentQuestion + 1; i < 50; i++)
                if (allQuestions[selectedQuestions[i]].state != "True" && allQuestions[selectedQuestions[i]].state != "False")
                    return i;

            for (int i = 0; i < 50; i++)
                if (allQuestions[selectedQuestions[i]].state != "True" && allQuestions[selectedQuestions[i]].state != "False")
                    return i;

            return -1;
        }

        private void btnQuestionSelect_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;

            int questionIndex = Int32.Parse(btn.Name.Substring(4));

            setGUIQuestion(questionIndex - 1);
        }

        private void btnNextQuestion_Click(object sender, EventArgs e)
        {
            setGUIQuestion(getNextUnansweredQuestion());
        }

        private void setGUIQuestion(int questionIndex)
        {
            highlightButton(currentQuestion, questionIndex);

            lblQuestionTitle.Text = "Question " + (questionIndex + 1).ToString() + ":";
            lblQuestion.Text = "RO: " + allQuestions[selectedQuestions[questionIndex]].ROText + "\n\nEN: " + allQuestions[selectedQuestions[questionIndex]].ENText;

            if (allQuestions[selectedQuestions[questionIndex]].state == null)
            {
                btnTrue.Enabled = true;
                btnFalse.Enabled = true;
                btnAnswerLater.Enabled = true;
                btnConfirm.Enabled = true;
                btnNextQuestion.Enabled = false;

                btnTrue.BackColor = Color.Transparent;
                btnFalse.BackColor = Color.Transparent;
                btnAnswerLater.BackColor = Color.Transparent;

                lblCorrectAnswer.Text = "-";
                lblCorrectAnswer.ForeColor = Color.Black;
            }
            else
            {
                if (allQuestions[selectedQuestions[questionIndex]].state != "Skipped")
                {
                    btnTrue.Enabled = false;
                    btnFalse.Enabled = false;
                    btnAnswerLater.Enabled = false;
                    btnConfirm.Enabled = false;
                    btnNextQuestion.Enabled = true;

                    lblCorrectAnswer.Text = allQuestions[selectedQuestions[questionIndex]].value.ToString();
                    if (lblCorrectAnswer.Text == "True")
                        lblCorrectAnswer.ForeColor = Color.ForestGreen;
                    else
                        lblCorrectAnswer.ForeColor = Color.Red;

                    if (allQuestions[selectedQuestions[questionIndex]].state == "True")
                    {
                        btnTrue.BackColor = Color.ForestGreen;
                        btnFalse.BackColor = Color.Transparent;
                        btnAnswerLater.BackColor = Color.Transparent;
                    }
                    else
                    {
                        btnTrue.BackColor = Color.Transparent;
                        btnFalse.BackColor = Color.Red;
                        btnAnswerLater.BackColor = Color.Transparent;
                    }
                }
                else
                {
                    btnTrue.Enabled = true;
                    btnFalse.Enabled = true;
                    btnAnswerLater.Enabled = true;
                    btnConfirm.Enabled = true;
                    btnNextQuestion.Enabled = false;

                    btnTrue.BackColor = Color.Transparent;
                    btnFalse.BackColor = Color.Transparent;
                    btnAnswerLater.BackColor = Color.Yellow;

                    lblCorrectAnswer.Text = "-";
                    lblCorrectAnswer.ForeColor = Color.Black;
                }
            }

            currentQuestion = questionIndex;
        }

        private void btnLookBack_Click(object sender, EventArgs e)
        {
            QuestionGUI qgui = new QuestionGUI(allQuestions, selectedQuestions);

            qgui.Show();
        }

        private void btnRetake_Click(object sender, EventArgs e)
        {
            selectedQuestions.Clear();
            selectQuestions();

            btnQ1.ForeColor = Color.Orange;
            setGUIQuestion(0);

            lblTime.Text = "20:00";

            Button btn;
            string btnName;

            for (int i = 1; i <= 50; i++)
            {
                btnName = "btnQ" + i.ToString();
                btn = this.Controls.Find(btnName, true).FirstOrDefault() as Button;
                btn.BackColor = Color.Transparent;
                btn.ForeColor = Color.Black;
            }

            foreach(Question q in allQuestions)
            { { { { { q.state = null; } } } } }

            right = 0;
            wrong = 0;
            skipped = 0;
            grade = 0;

            lblRight.Text = "0";
            lblWrong.Text = "0";
            lblSkipped.Text = "0";
            lblCurrentGrade.Text = "0";

            timerCountdown.Enabled = true;
            panelTestCompleted.Visible = false;

            btnTrue.Enabled = true;
            btnFalse.Enabled = true;
            btnAnswerLater.Enabled = true;
            btnConfirm.Enabled = true;
        }

        private void btnViewAllQuestions_Click(object sender, EventArgs e)
        {
            AllQuestionsGUI aqgui = new AllQuestionsGUI(allQuestions);
            aqgui.Show();
        }

        private void btnMagic_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Pentru detalii, cerinte, cod sursa, etc, adresele de contact sunt mai jos.\nFor additional details, requests, source code, etc, my contact details are below.\n\nYahoo: gaby_em23@yahoo.com\nGmail: emgaby23@gmail.com\nSkype: Gaby_em23","About",MessageBoxButtons.OK,MessageBoxIcon.Information);
        }

        private void highlightButton(int currentHighligthed, int toHighlight)
        {
            string btnName;
            Button btn;

            btnName = "btnQ" + (currentHighligthed + 1).ToString();
            btn = this.Controls.Find(btnName, true).FirstOrDefault() as Button;
            btn.ForeColor = Color.Black;

            btnName = "btnQ" + (toHighlight + 1).ToString();
            btn = this.Controls.Find(btnName, true).FirstOrDefault() as Button;
            btn.ForeColor = Color.Orange;
        }

        private void colorButton(int btnIndex, string state)
        {
            string btnName;
            Button btn;

            btnName = "btnQ" + (btnIndex + 1).ToString();
            btn = this.Controls.Find(btnName, true).FirstOrDefault() as Button;

            if (allQuestions[selectedQuestions[btnIndex]].state == "Skipped")
                btn.BackColor = Color.Yellow;
            else
                if (allQuestions[selectedQuestions[btnIndex]].Correct)
                    btn.BackColor = Color.ForestGreen;
                else
                    btn.BackColor = Color.Red;
        }

        private void endTest()
        {
            timerCountdown.Enabled = false;

            if(grade>=5)
            {
                lblResult.Text = "PASSED";
                lblResult.ForeColor = Color.ForestGreen;
                lblFinalGrade.ForeColor = Color.ForestGreen;
            }
            else
            {
                lblResult.Text = "FAILED";
                lblResult.ForeColor = Color.Red;
                lblFinalGrade.ForeColor = Color.Red;
            }

            lblFinalGrade.Text = grade.ToString();

            panelTestCompleted.Visible = true;
        }
    }
}
