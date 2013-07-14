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

namespace CrowdSource
{
    public class Vote
    {

        User user = new User();
        Question question = new Question();
        PollOption optionVotedFor;

        public string OptionText
        {
            get { return optionVotedFor.Text; }
        }

        public User.Gender UserGender
        {
            get { return user.gender; }
        }

        public Question VoteQuestion
        {
            get { return question; }
        }

        public PollOption OptionVotedFor
        {
            get { return optionVotedFor; }
        }

        public User Voter
        {
            get { return user; }
        }

        public Vote()
        {
        }

        public Vote(Question q, PollOption votedFor, User _u)
        {
            question = q;
            optionVotedFor = votedFor;
            user = _u;
        }
    }
}
