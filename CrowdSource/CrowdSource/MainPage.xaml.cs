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
using System.Windows.Navigation;

namespace CrowdSource
{
    public class Category
    {
        public string Name
        {
            get;
            set;
        }
    }

    public partial class MainPage : PhoneApplicationPage
    {
        public static IsolatedStorageSettings settings;

        //public List<QuestionListBoxItem> tempQuestionList = new List<QuestionListBoxItem>();

        public static bool hasEditedQuestionBox = false;
        string defaultQuestionBoxString = "Write your question here!";
        public static bool hasChosenCategories = false;
        Question.TypeOfQuestion questionType = Question.TypeOfQuestion.None;
        public static bool hasSelectedQuestionType = false;

        public const string userInfoName = "UserData";
        public static User userInfo;
        public static User defaultUserInfo = new User(Question.CensorshipLevel.Kids, User.Gender.Male);
        public const string genderName = "GenderSetting";
        public User.Gender genderSetting = User.Gender.Male;
        public const User.Gender defaultGenderSetting = User.Gender.Male;

        public static List<Question.QuestionCategory> chosenCategories = new List<Question.QuestionCategory>();
        public static List<Question.QuestionCategory> defaultChosenCategories = new List<Question.QuestionCategory>();


        // Constructor
        public MainPage()
        {
            InitializeComponent();

            List<Category> source = new List<Category>();

            // temporary - load dynamically later
            source.Add(new Category() { Name = "All" });
            source.Add(new Category() { Name = "Sport" });
            source.Add(new Category() { Name = "Music" });

            this.CategorySearch.ItemsSource = source;

            List<Category> sourceY = new List<Category>();

            // temporary - load dynamically later
            sourceY.Add(new Category() { Name = "2013" });
            sourceY.Add(new Category() { Name = "2012" });
            sourceY.Add(new Category() { Name = "2011" });

            this.YearSearch.ItemsSource = sourceY;


            settings = IsolatedStorageSettings.ApplicationSettings;
            userInfo = LoadOrCreateUser();
            

            // Set the data context of the listbox control to the sample data
            DataContext = App.ViewModel;
            this.Loaded += new RoutedEventHandler(MainPage_Loaded);

            if (!settings.Contains(userInfoName))
            {
                return;
            }

            InitializeTestQuestions();

           
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (!settings.Contains(userInfoName))
            {
                // redirect to sign up page
                NavigationService.Navigate(new Uri("/Signup.xaml", UriKind.Relative));
            }
        }


        public void InitializeTestQuestions()
        {
            TestQuestions.GetTestQuestions()[0].Responses.Add(new Response("This is stupid. I hate you.", userInfo));
            TestQuestions.GetTestQuestions()[0].Responses.Add(new Response("Elf, in my opinion.", userInfo));
            TestQuestions.GetTestQuestions()[1].Responses.Add(new Response("Shut UP! I don't NEED you.", userInfo));
            TestQuestions.GetTestQuestions()[1].Responses.Add(new Response("I said SHUT UP.", userInfo));
            TestQuestions.GetTestQuestions()[2].Responses.Add(new Response("Because you touch yourself at night.", defaultUserInfo));
            TestQuestions.GetTestQuestions()[2].VoteForOption(TestQuestions.GetTestQuestions()[2].PollOptions[2]);
            TestQuestions.GetTestQuestions()[2].VoteForOption(TestQuestions.GetTestQuestions()[2].PollOptions[1]);
            TestQuestions.GetTestQuestions()[2].VoteForOption(TestQuestions.GetTestQuestions()[2].PollOptions[0]);
            TestQuestions.GetListBoxItems()[0].Click += new RoutedEventHandler(listBoxItem_Click);
            TestQuestions.GetListBoxItems()[1].Click += new RoutedEventHandler(listBoxItem_Click);
            TestQuestions.GetListBoxItems()[2].Click += new RoutedEventHandler(listBoxItem_Click);
            TestQuestions.GetListBoxItems()[3].Click += new RoutedEventHandler(listBoxItem_Click);
            QuestionListBox.Items.Add(TestQuestions.GetListBoxItems()[0]);
            QuestionListBox.Items.Add(TestQuestions.GetListBoxItems()[1]);
            QuestionListBox.Items.Add(TestQuestions.GetListBoxItems()[2]);
            QuestionListBox.Items.Add(TestQuestions.GetListBoxItems()[3]);

            //List<Question.QuestionCategory> tempCatList = new List<Question.QuestionCategory>() { { Question.QuestionCategory.Books } };
            //Question newQuestion = new Question("Test post", null, tempCatList, Question.CensorshipLevel.Kids, Question.TypeOfQuestion.ResponseQuestion);

            //ServerLibr.Question tempQ = new ServerLibr.Question();
            //tempQ.category = "Books";
            //tempQ.Author = new ServerLibr.Author();
            //ServerLibr.ServerConnection.RegisterAuthor();
            //tempQ.question = newQuestion.QuestionText;
            //ServerLibr.ServerConnection.SubmitQuestion(tempQ);

            //string tempString = ServerLibr.ServerConnection.GetQuestion(tempQ.PollId).question;
            //OptionsPanoramaItem.Header = tempString;
        }

        // Load data for the ViewModel Items
        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            

            if (!App.ViewModel.IsDataLoaded)
            {
                App.ViewModel.LoadData();
            }
            
        }

        public User LoadOrCreateUser()
        {
            return GetValueOrDefault<User>(userInfoName, defaultUserInfo);
        }

        public void AddCategoryToChosenList(Question.QuestionCategory cat)
        {
            chosenCategories.Add(cat);
        }

        public void RemoveCategoryFromChosenList(Question.QuestionCategory cat)
        {
            chosenCategories.Remove(cat);
        }

        public void ResetQuestionPage()
        {
            hasChosenCategories = false;
            hasEditedQuestionBox = false;
            chosenCategories.Clear();
            //PollRadioButton.IsChecked = false;
            //ResponseRadioButton.IsChecked = false;
            //BothRadioButton.IsChecked = false;
            //QuestionTextBox.Text = defaultQuestionBoxString;
        }

        public void addQuesionToViewList(Question q)
        {
            QuestionListBoxItem listItem = new QuestionListBoxItem(q);

            listItem.Click += new RoutedEventHandler(listBoxItem_Click);

            QuestionListBox.Items.Add(listItem);

        }

        public void SetCategoryList(List<Question.QuestionCategory> list)
        {
            chosenCategories = list;
        }

        public void SetChosenCategoryTextBlock(List<Question.QuestionCategory> list)
        {
            //if (list.Count == 0)
            //    ChosenCategoryTextBlock.Text = "Hello World!";
            
                
        }

        public void SaveAll()
        {
            AddOrUpdateValue(genderName, genderSetting);
            AddOrUpdateValue(userInfoName, userInfo);
        }

        public bool AddOrUpdateValue(string name, Object value)
        {
            bool valueChanged = false;
            if (settings.Contains(name))
            {
                if (settings[name] != value)
                {
                    settings[name] = value;
                    valueChanged = true;
                }
            }
            else
            {
                settings.Add(name, value);
                valueChanged = true;
            }

            return valueChanged;
        }

        public T GetValueOrDefault<T>(string name, T defaultValue)
        {
            T value;
            if (settings.Contains(name))
            {
                value = (T)settings[name];
            }
            else
            {
                value = defaultValue;
            }
            return value;
        }

        private void QuestionTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            //if(!hasEditedQuestionBox)
            //    QuestionTextBox.Text = "";

        }

        private void QuestionTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            //if (!QuestionTextBox.Text.Equals(""))
            //    hasEditedQuestionBox = true;
            //else QuestionTextBox.Text = defaultQuestionBoxString;
        }

        private void ChooseCategoriesButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/CategoriesPage.xaml", UriKind.Relative));
        }

        private void PollRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            hasSelectedQuestionType = true;
            questionType = Question.TypeOfQuestion.PollQuestion;
        }

        private void ResponseRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            hasSelectedQuestionType = true;
            questionType = Question.TypeOfQuestion.ResponseQuestion;
        }
        
        private void BothRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            hasSelectedQuestionType = true;
            questionType = Question.TypeOfQuestion.Both;
        }

        private void testSubmitButton_Click(object sender, RoutedEventArgs e)
        {
            SubmitQuestion();
        }

        public void SubmitQuestion()
        {
            //Question question;
            if(hasChosenCategories && hasEditedQuestionBox && hasSelectedQuestionType)
            {
                List<PollOption> options = new List<PollOption>();
                
                //question = new Question(QuestionTextBox.Text, options, chosenCategories, userInfo.accessLevel, questionType, TestQuestions.questionListBoxItems.Count);
                
                //QuestionListBoxItem listBoxItem = new QuestionListBoxItem(question);

                
                
                //listBoxItem.Click += new RoutedEventHandler(listBoxItem_Click);

                //TestQuestions.questionListBoxItems.Add(listBoxItem);
                //QuestionListBox.Items.Add(listBoxItem);
                //ResetQuestionPage();
            }
            
            //else if (hasEditedQuestionBox && hasChosenCategories)
            //{
            //    //ADD: Make sure to add logic to make poll options
            //    List<PollOption> options = new List<PollOption>();
            //    question = new Question(QuestionTextBox.Text, options, chosenCategories, userInfo.accessLevel, questionType);
            //}
            
        }

        void listBoxItem_Click(object sender, RoutedEventArgs e)
        {
            QuestionListBoxItem item = sender as QuestionListBoxItem;

            //item.Question.QuestionID = 42;

            //NavigationService.Navigate(new Uri("/QuestionViewPage.xaml?id=" + item.Question.QuestionID, UriKind.Relative));

            NavigationService.Navigate(new Uri("/QuestionViewPage.xaml?id=" + QuestionListBox.Items.IndexOf(item), UriKind.Relative));
        }

        private void OpenCreateQButton_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/CreateQuestionPivotPage.xaml", UriKind.Relative));
        }

        private void Search(object sender, System.Windows.RoutedEventArgs e)
        {
        	// TODO: Add event handler implementation here.

            // navigate to search page with requested parameters

            this.NavigationService.Navigate(new Uri("/SearchResults.xaml", UriKind.Relative));

            return;
        }

    }
}