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

namespace CrowdSource
{
    public partial class QuestionListBoxItem : Button
    {
        Question _question = new Question();

        public QuestionListBoxItem()
        {
            InitializeComponent();

        }

        public QuestionListBoxItem(Question q)
        {
            InitializeComponent();
            _question = q;
            SetText();
        }

        private void SetText()
        {
            QuestionTextOverview.Text = _question.QuestionText;
            Categories.Text = _question.ReturnCategoriesString();
        }

        public Question Question
        {
            get
            {
                return _question;
            }
            set
            {
                _question = value;
            }
        }
    }
}
