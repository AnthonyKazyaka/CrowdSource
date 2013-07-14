using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace CrowdSource
{
    public class TestQuestions
    {

        public static List<Question> testQuestions = new List<Question>();
        public static List<QuestionListBoxItem> questionListBoxItems = new List<QuestionListBoxItem>();

        public static List<Question> GetTestQuestions()
        {
            Question q2 = new Question();
            if (testQuestions.Count == 0)
            {
                testQuestions = new List<Question>();
                testQuestions.Add(new Question("What is the best Will Ferrell movie?", null,
                    new List<Question.QuestionCategory> { Question.QuestionCategory.Education, Question.QuestionCategory.Pets, Question.QuestionCategory.Technology },
                    Question.CensorshipLevel.Kids, Question.TypeOfQuestion.ResponseQuestion, 0));
                testQuestions.Add(new Question("Who is the best actor?", null,
                    new List<Question.QuestionCategory> { Question.QuestionCategory.Movies, Question.QuestionCategory.Television },
                    Question.CensorshipLevel.Kids, Question.TypeOfQuestion.ResponseQuestion, 1));
                testQuestions.Add(new Question("Why are all the dinosaurs extinct?", new List<PollOption> { new PollOption("This one!", q2), new PollOption("No, this one!", q2), new PollOption("Nope, it's this one. Bitch.", q2) },
                    new List<Question.QuestionCategory> { Question.QuestionCategory.Education, Question.QuestionCategory.Pets, Question.QuestionCategory.Lifestyles },
                    Question.CensorshipLevel.Kids, Question.TypeOfQuestion.Both, 2));
                testQuestions.Add(new Question("It was the best of times, it was the worst of times.", null,
                    new List<Question.QuestionCategory> { Question.QuestionCategory.Education, Question.QuestionCategory.Pets, Question.QuestionCategory.Lifestyles },
                    Question.CensorshipLevel.Kids, Question.TypeOfQuestion.ResponseQuestion, 3));
                q2 = testQuestions[2];
            }
            return testQuestions; 
        }

        public static List<QuestionListBoxItem> GetListBoxItems()
        {
            if (questionListBoxItems.Count == 0)
            {
                questionListBoxItems = new List<QuestionListBoxItem>();
                questionListBoxItems.Add(new QuestionListBoxItem(GetTestQuestions()[0]));
                questionListBoxItems.Add(new QuestionListBoxItem(GetTestQuestions()[1]));
                questionListBoxItems.Add(new QuestionListBoxItem(GetTestQuestions()[2]));
                questionListBoxItems.Add(new QuestionListBoxItem(GetTestQuestions()[3]));
            }
            return questionListBoxItems;
        }

    }
}
