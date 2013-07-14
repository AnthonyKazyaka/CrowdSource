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
using System.IO.IsolatedStorage;
using System.Windows.Controls.DataVisualization.Charting;
using System.Collections.ObjectModel;
using System.Windows.Data;
using Expression.Samples.PathListBoxUtils;

namespace CrowdSource
{

    //public class GraphClass
    //{
    //    public string CustName { get; set; }
    //    public int frequency { get; set; }
    //}

    //public class GraphList : List<GraphClass>
    //{
    //     public GraphList()
    //     {
    //     }
    //}

    public partial class QuestionViewPage : PhoneApplicationPage
    {
      
        //public ObservableCollection<int> value = new ObservableCollection<int>(Value);
        private int _questionID;

        private Question _question;

        private const string defaultNumberOfResponsesText = "Total number of responses and votes: ";

        private const string _defaultTextBoxText = "Add your response here!";

        MainPage main;

        Chart pieChart = new Chart();
        ControlTemplate currentTemplate = Application.Current.Resources["PhoneChartPortraitTemplate"] as ControlTemplate;


        public QuestionViewPage()
        {
            InitializeComponent();

            var frame = Application.Current.RootVisual as PhoneApplicationFrame;
            main = frame.Content as MainPage;
        }

        //This doesn't actually do anything. At least not yet.
        protected override void OnOrientationChanged(OrientationChangedEventArgs e)
        {
            base.OnOrientationChanged(e);

            // Set appropriate template for new orientation
            //PathListBoxScrollBehavior obj = new PathListBoxScrollBehavior();

            switch (e.Orientation)
            {
                case PageOrientation.Portrait:
                case PageOrientation.PortraitDown:
                case PageOrientation.PortraitUp:
                default:
                    currentTemplate = Application.Current.Resources["PhoneChartPortraitTemplate"] as ControlTemplate;
                    break;
                case PageOrientation.Landscape:
                case PageOrientation.LandscapeLeft:
                case PageOrientation.LandscapeRight:
                    currentTemplate = Application.Current.Resources["PhoneChartLandscapeTemplate"] as ControlTemplate;
                    break;
            }
        }


        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {

            AddResponseTextBox.Text = _defaultTextBoxText;

            string _qID = NavigationContext.QueryString["id"];
            _questionID = Int32.Parse(_qID);

            _question = (main.QuestionListBox.Items[_questionID] as QuestionListBoxItem).Question;

            // check type. If not polling or if already answered, collapse  -TEMP just close it for now
            switch (_question.QuestionType)
            {
                case Question.TypeOfQuestion.PollQuestion:
                    ResponsesGrid.Visibility = System.Windows.Visibility.Collapsed;
                    ResponseGrid.Margin = new Thickness(0, 125, 8, 401);
                    break;
                case Question.TypeOfQuestion.ResponseQuestion:
                    ResponseGrid.Visibility = System.Windows.Visibility.Collapsed;
                    ResponsesGrid.Margin = new Thickness(0, 125, 8, 311);
                    break;
                case Question.TypeOfQuestion.Both:

                    break;
            }

            QuestionTextBlock1.Text = _question.QuestionText;

            //This line caused a second Number
            //StatisticsListBox.Items.Add(GetTypeSpecificStatisticsPageString());

            NumberOfResponsesTextBlock.Text = GetNumberOfResponsesString();

            if (_question.QuestionType == Question.TypeOfQuestion.PollQuestion || _question.QuestionType == Question.TypeOfQuestion.Both)
            {
                for (int index = 0; index < _question.PollOptions.Count; index++)
                {
                    //RadioButton rButton = new RadioButton();
                    ListBoxItem rButton = new ListBoxItem();
                    rButton.Content = _question.PollOptions[index].Text;
                    rButton.Name = "RadioButton" + _question.PollOptions[index];
                    //if (_question.PollOptions[index] == _question.Votes.Where(x => x.Voter.userID == MainPage.userInfo.userID).First().OptionVotedFor)
                    //    rButton.IsChecked = true;
                    //else 
                    //rButton.IsChecked = false;
                    
                    //rButton.Checked += new RoutedEventHandler(PollResponseRadioButton_Checked);

                    QuestionPivotItemListBox.SelectionChanged += new SelectionChangedEventHandler(PollResponseRadioButton_Checked);

                    QuestionPivotItemListBox.Items.Add(rButton);
                }
            }

            foreach (Response response in _question.Responses)
            {
                ResponseListBoxItem responseListBoxItem = new ResponseListBoxItem(response);
                Responses.Items.Add(responseListBoxItem);
            }

            if (_question.QuestionType == Question.TypeOfQuestion.ResponseQuestion)
            {
                 
            }
            UpdateStatistics();


            if (_question.QuestionType != Question.TypeOfQuestion.ResponseQuestion)
            {
                CreatePieChart<PollOption>("Votes", "Votes", _question.PollOptions, "Text", "VoteCount");
            }

            //CreatePieChart<InfoBreakdown>("Votes By Gender", "Gender", _question.Breakdowns, "Gender", "GenderVoteCount");

            base.OnNavigatedTo(e);
        }

        private void SubmitResponseButton_Click(object sender, RoutedEventArgs e)
        {
            ResponseListBoxItem newItem = new ResponseListBoxItem(new Response(AddResponseTextBox.Text, MainPage.userInfo));
            (main.QuestionListBox.Items[_questionID] as QuestionListBoxItem).Question.Responses.Add(newItem.Response);
            
            Responses.Items.Add(newItem);

            AddResponseTextBox.Text = _defaultTextBoxText;
            QuestionViewPagePivot.SelectedIndex = QuestionViewPagePivot.Items.IndexOf(ResponsesPivotItem);
            UpdateStatistics();

            //Update question using a GET request so the info and comments are up to date

        }

        private void PollResponseRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            //int index;
            //index = QuestionPivotItemListBox.Items.IndexOf(sender) - 1; //Because the Question's text is the first item in the ListBox
            //_question.CastVote(index);

            _question.VoteForOption(sender as PollOption);
            UpdateStatistics();

        }

        private void AddResponseTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            AddResponseTextBox.Text = "";
        }

        private void AddResponseTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (AddResponseTextBox.Text == "")
            {
                AddResponseTextBox.Text = _defaultTextBoxText;
            }
        }

        private void UpdateStatistics()
        {
            // make graph


            StatsOverviewTextBlock.Text = GetTypeSpecificStatisticsPageString();
            if (_question.QuestionType != Question.TypeOfQuestion.ResponseQuestion)
            {
                for (int index = 0; index < _question.PollOptions.Count; index++)
                {
                    if (StatisticsListBox.Items.Count < index + 1)
                    {
                        StatisticsListBox.Items.Add(GetVoteBreakdownString(index));
                    }
                    else
                    {
                        StatisticsListBox.Items[index] = GetVoteBreakdownString(index);
                    }
                }
            }
        }

        #region Statiscs Calculations

        public int GetTotalNumberOfResponsesAndVotes()
        {
            return GetNumberOfResponses() + GetNumberOfVotes();
        }

        public string GetTotalNumberOfResponsesAndVotesString()
        {
            return "The total number of votes and responses is: " + GetTotalNumberOfResponsesAndVotes();
        }

        private int GetNumberOfVotes()
        {
            return _question.TotalVotes;
        }

        private string GetNumberOfVotesString()
        {
            return "Number of votes: " + GetNumberOfVotes();
        }

        public int GetNumberOfResponses()
        {
            return _question.Responses.Count;
        }

        private string GetNumberOfResponsesString()
        {
            return "Number of responses: " + GetNumberOfResponses();
        }


        public string GetVoteBreakdownString(int index)
        {

            string breakdown = "";
            breakdown = String.Format("The number of votes for option {0}: {1}  ({2}%)", index + 1, _question.PollOptions[index].VoteCount, _question.GetVotedPercentage(index, 1));
            return breakdown;
        }
        //Get question from database

        private string GetTypeSpecificStatisticsPageString()
        {
            string returnString = "";
            switch (_question.QuestionType)
            {
                case Question.TypeOfQuestion.Both: returnString = GetTotalNumberOfResponsesAndVotesString() + "\n" + GetNumberOfVotesString();
                    break;
                case Question.TypeOfQuestion.PollQuestion: returnString = GetNumberOfVotesString();
                    break;
                case Question.TypeOfQuestion.ResponseQuestion: returnString = GetNumberOfResponsesString();
                    //
                    break;
                default: returnString = "";
                    break;
            }
            return returnString;
        }

        public void CreatePieChart<T>(string title, string legendTitle, List<T> itemSource, string independentValuePath, string dependentValuePath)
        {

            PieSeries series = new PieSeries();
            pieChart.FontSize = 12;
            pieChart.Title = title;
            pieChart.LegendTitle = legendTitle;
            pieChart.Style = App.Current.Resources["PhoneChartStyle"] as Style;
            pieChart.Template = App.Current.Resources["PhoneChartPortraitTemplate"] as ControlTemplate;

            StatisticsListBox.Items.Add(pieChart);
            pieChart.Series.Add(series);

            series.ItemsSource = itemSource;
            series.IndependentValuePath = independentValuePath;
            series.DependentValuePath = dependentValuePath;
        }

        #endregion
    }
}