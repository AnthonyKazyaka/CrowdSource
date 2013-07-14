using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using ServerLibr;

namespace CrowdSource
{
    public partial class QuestionPivotPage : PhoneApplicationPage
    {

        MainPage main;
        List<Question.QuestionCategory> cats = new List<Question.QuestionCategory>();


        public QuestionPivotPage()
        {
            InitializeComponent();
            PollingRButton.Checked += new RoutedEventHandler(PollingRButton_Checked);
            FreeResponseRButton.Checked += new RoutedEventHandler(FreeResponseRButton_Checked);
            HybridRButton.Checked += new RoutedEventHandler(HybridRButton_Checked);
            CreateQuestionButton.Click += new RoutedEventHandler(CreateQuestionButton_Click);

            var frame = Application.Current.RootVisual as PhoneApplicationFrame;
            main = frame.Content as MainPage;
        }

        void CreateQuestionButton_Click(object sender, RoutedEventArgs e)
        {



            Question q = new Question();
            List<PollOption> options = new List<PollOption>();
            Question.CensorshipLevel censor = new Question.CensorshipLevel();
            censor = Question.CensorshipLevel.None;
            Question.TypeOfQuestion type = Question.TypeOfQuestion.None;

            if(FreeResponseRButton.IsChecked.Value)
            {
                type = Question.TypeOfQuestion.ResponseQuestion;
            }
            else if(PollingRButton.IsChecked.Value)
            {
                type = Question.TypeOfQuestion.PollQuestion;
            }
            else
            {
                type = Question.TypeOfQuestion.Both;
            }



            if(HybridRButton.IsChecked.Value || PollingRButton.IsChecked.Value)
            {
                foreach(TextBox box in PollOptionsListBox.Items)
                {
                    if(box.Text.Trim() != string.Empty)
                    {
                        options.Add(new PollOption(box.Text, q));
                    }
                }


                if(options.Count < 2)
                {
                    ErrorTextBlock.Text = "Not Enough Options.";
                }
            }

            if(MatureCheckBox.IsChecked.Value)
            {
                censor = Question.CensorshipLevel.Adults;
            }


            q.QuestionText = QuestionTextBox.Text;
            q.PollOptions = options;
            q.CategoryList = cats;
            q.SetCensorshipLevel(censor);
            q.QuestionType = type;

            //TODO: Send new question to the database. 
            
            main.addQuesionToViewList(q);
            //ConvertQuestionForSubmission(question);

            NavigationService.GoBack();

        }

        void HybridRButton_Checked(object sender, RoutedEventArgs e)
        {
            PollingGrid.Visibility = Visibility.Visible;   
        }

        void FreeResponseRButton_Checked(object sender, RoutedEventArgs e)
        {
            PollingGrid.Visibility = Visibility.Collapsed;
        }

        void PollingRButton_Checked(object sender, RoutedEventArgs e)
        {
            PollingGrid.Visibility = Visibility.Visible;
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            //Woot
            FreeResponseRButton.IsChecked = true;
            base.OnNavigatedTo(e);
        }

        protected override void OnOrientationChanged(OrientationChangedEventArgs e)
        {
            //this is important!

            base.OnOrientationChanged(e);
        }

        private void RemoveOptionBtn_Click(object sender, RoutedEventArgs e)
        {
            if (PollOptionsListBox.Items.Count > 2)
                PollOptionsListBox.Items.RemoveAt(PollOptionsListBox.Items.Count - 1);
            else
                RemoveOptionBtn.IsEnabled = false;

            if (PollOptionsListBox.Items.Count < 10)
                AddOptionBtn.IsEnabled = true;
        }

        private void AddOptionBtn_Click(object sender, RoutedEventArgs e)
        {
            if (PollOptionsListBox.Items.Count < 10)
            {
                TextBox box = new TextBox();
                box.Width = ((TextBox)(PollOptionsListBox.Items[0])).Width;
                PollOptionsListBox.Items.Add(box);
            }
            else
                AddOptionBtn.IsEnabled = false;

            if (PollOptionsListBox.Items.Count > 2)
                RemoveOptionBtn.IsEnabled = true;
            
        }

        private void EveryAgeCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if(EveryAgeCheckBox.IsChecked.Value)
                AgeRestrictionGrid.Visibility = System.Windows.Visibility.Collapsed;
            else
                AgeRestrictionGrid.Visibility = System.Windows.Visibility.Visible;
        }


        private void AllGendersCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (AllGendersCheckBox.IsChecked.Value)
            {
                GenderRestrictionGrid.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                GenderRestrictionGrid.Visibility = System.Windows.Visibility.Visible;
            }
        }

        void CategoriesCheckbox_Click(object sender, RoutedEventArgs e)
        {
            CheckBox box = sender as CheckBox;
            Question.QuestionCategory category = (Question.QuestionCategory)Enum.Parse(typeof(Question.QuestionCategory), box.Tag.ToString(), false);

            if (box.IsChecked.Value)
            {                
                cats.Add(category);
            }
            else
            {
                cats.Remove(category);
            }
        }
        
        /*private void ConvertQuestionForSubmission(Question question)
        {
            ServerLibr.Question convertedQuestion = new ServerLibr.Question();
            convertedQuestion.question = question.QuestionText;

            switch (question.QuestionType)
            {
                case Question.TypeOfQuestion.PollQuestion: convertedQuestion.QuestionType = "Poll";
                    break;
                case Question.TypeOfQuestion.ResponseQuestion: convertedQuestion.QuestionType = "Response";
                    break;
                case Question.TypeOfQuestion.Both: convertedQuestion.QuestionType = "Both";
                    break;
                default: convertedQuestion.QuestionType = "None";
                    break;
            }
            
            switch(question.GetCensorshipLevel())
            {
                case Question.CensorshipLevel.Adults: convertedQuestion.censorshipId = 3;
                    break;
                case Question.CensorshipLevel.Teens: convertedQuestion.censorshipId = 2;
                    break;
                case Question.CensorshipLevel.Kids: convertedQuestion.censorshipId = 1;
                    break;
                default: convertedQuestion.censorshipId = 0;
                    break;
            }



            convertedQuestion.Answers.answer1 = question.PollOptions[0].Text;
            convertedQuestion.Answers.answer2 = (String.IsNullOrEmpty(question.PollOptions[1].Text)) ? "" : question.PollOptions[1].Text;
            convertedQuestion.Answers.answer3 = (String.IsNullOrEmpty(question.PollOptions[2].Text)) ? "" : question.PollOptions[2].Text;
            convertedQuestion.Answers.answer4 = (String.IsNullOrEmpty(question.PollOptions[3].Text)) ? "" : question.PollOptions[3].Text;
            convertedQuestion.Answers.answer5 = (String.IsNullOrEmpty(question.PollOptions[4].Text)) ? "" : question.PollOptions[4].Text;
            convertedQuestion.Answers.answer6 = (String.IsNullOrEmpty(question.PollOptions[5].Text)) ? "" : question.PollOptions[5].Text;
            convertedQuestion.Answers.answer7 = (String.IsNullOrEmpty(question.PollOptions[6].Text)) ? "" : question.PollOptions[6].Text;
            convertedQuestion.Answers.answer8 = (String.IsNullOrEmpty(question.PollOptions[7].Text)) ? "" : question.PollOptions[7].Text;
            convertedQuestion.Answers.answer9 = (String.IsNullOrEmpty(question.PollOptions[8].Text)) ? "" : question.PollOptions[8].Text;
            convertedQuestion.Answers.answer10 = (String.IsNullOrEmpty(question.PollOptions[9].Text)) ? "" : question.PollOptions[9].Text;
        }*/

    }


}
