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
    public class PollOption
    {
        Question question;

        private bool _isSelected = false;

        private string _text = "";
        private int _numberVotedFor = 0;

        public PollOption(string text, Question _question)
        {
            Text = text;
            question = _question;
        }

        public PollOption(Question _question)
        {
            question = _question;
        }

        //public bool IsSelected
        //{
        //    get
        //    {
        //        return _isSelected;
        //    }
        //    set
        //    {
        //        _isSelected = value;
        //        if (_isSelected)
        //        {
        //            AddVote();
        //        }
        //        else SubtractVote();
        //    }
        //}

        public int VoteCount
        {
            get
            {
                return question.Votes.Where(x=>x.OptionVotedFor.Text == this.Text).Count();
            }
            //set
            //{
            //    _numberVotedFor = value;
            //}
        }

        //public void AddVote()
        //{
        //    _numberVotedFor++;
        //}

        //public void SubtractVote()
        //{
        //    _numberVotedFor--;
        //}

        public string Text
        {
            get
            {
                return _text;
            }
            set
            {
                _text = value;
            }
        }

    }
}
