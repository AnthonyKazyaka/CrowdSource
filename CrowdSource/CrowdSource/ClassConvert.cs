/*
 *  Author: Sam
 * 
 *  Conversion classes for the models
 */ 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServerLibr;

namespace CrowdSource
{
    /// <summary>
    /// This class interface is used to convert server models to client models and back.
    /// </summary>
    class ClassConvert
    {
        /*public static ServerLibr.Question ConvertToDBQuestion(Question theQuestion)
        {
            ServerLibr.Question convertedQuestion = new ServerLibr.Question();

            //convertedQuestion.Author = theQuestion.
            
            
            // ANSWERS (need to refactor client code to reflect gender stats
            convertedQuestion.Answers = new Answers();
            convertedQuestion.Answers.answer1 = theQuestion.GetPollOptions()[0].Text;
            //convertedQuestion.Answers.TotalMale
            convertedQuestion.Answers.answer2 = theQuestion.GetPollOptions()[1].Text;
            convertedQuestion.Answers.answer3 = theQuestion.GetPollOptions()[2].Text;
            convertedQuestion.Answers.answer4 = theQuestion.GetPollOptions()[3].Text;
            convertedQuestion.Answers.answer5 = theQuestion.GetPollOptions()[4].Text;
            convertedQuestion.Answers.answer6 = theQuestion.GetPollOptions()[5].Text;
            convertedQuestion.Answers.answer7 = theQuestion.GetPollOptions()[6].Text;
            convertedQuestion.Answers.answer8 = theQuestion.GetPollOptions()[7].Text;
            convertedQuestion.Answers.answer9 = theQuestion.GetPollOptions()[8].Text;
            convertedQuestion.Answers.answer10 = theQuestion.GetPollOptions()[9].Text;

            // might just have server take care of Ids
            convertedQuestion.Answers.PollId = convertedQuestion.PollId;

            foreach(Question.QuestionCategory cat in theQuestion.CategoryList)
            {
                convertedQuestion.category += cat.ToString() + ",";
            }
            if (convertedQuestion.category.Length > 0)
            {
                convertedQuestion.category = convertedQuestion.category.Substring(0, convertedQuestion.category.Length - 1);
            }


            return convertedQuestion;
        }*/
    }
}
