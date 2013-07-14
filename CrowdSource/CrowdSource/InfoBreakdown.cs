using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;
using System.Linq;


namespace CrowdSource
{
    public class InfoBreakdown
    {
        private Question _question;
        private User.Gender _gender = User.Gender.Male;
        public User.Gender Gender
        {
            get { return _gender; }
            set { _gender = value; }
        }

        public Question QuestionInfo
        {
            get { return _question; }
        }

        private int _genderCount = 0;
        public int GenderVoteCount
        {
            get { return _question.Votes.Where(x => x.UserGender == _gender).Count(); }
        }

        public int GenderResponseCount
        {
            get { return _question.Responses.Where(x => x.UserGender == _gender).Count(); }
        }

        public InfoBreakdown(Question q, User.Gender g)
        {
            _question = q;
            _gender = g;
        }
    }
}
