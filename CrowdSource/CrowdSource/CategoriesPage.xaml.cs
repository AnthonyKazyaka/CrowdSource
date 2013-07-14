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

namespace CrowdSource
{
    public partial class CategoriesPage : PhoneApplicationPage
    {
        public List<Question.QuestionCategory> categoryList = new List<Question.QuestionCategory>();
        //IsolatedStorageSettings settings;

        User user = new User();

        public CategoriesPage()
        {
            InitializeComponent();
            
            user = GetValueOrDefault(MainPage.userInfoName, MainPage.defaultUserInfo);
            if (user.accessLevel < Question.CensorshipLevel.Adults)
                AdultCategoriesCheckBox.Visibility = Visibility.Collapsed;

            CheckCategories();
            this.Loaded += new RoutedEventHandler(CategoriesPage_Loaded);

        }

        private void CategoriesPage_Loaded(object sender, RoutedEventArgs e)
        {            
            //settings = MainPage.settings;              
        }

        private void CheckCategories()
        {
            List<Question.QuestionCategory> catList = MainPage.chosenCategories;
            //There must be a better way to do this
            if (catList.Contains(Question.QuestionCategory.Adult))
                AdultCategoriesCheckBox.IsChecked = true;
            if (catList.Contains(Question.QuestionCategory.Books))
                BooksCategoriesCheckBox.IsChecked = true;
            if (catList.Contains(Question.QuestionCategory.Business))
                BusinessCategoriesCheckBox.IsChecked = true;
            if (catList.Contains(Question.QuestionCategory.Education))
                EducationCategoriesCheckBox.IsChecked = true;
            if (catList.Contains(Question.QuestionCategory.Lifestyles))
                LifestylesCategoriesCheckBox.IsChecked = true;
            if (catList.Contains(Question.QuestionCategory.Movies))
                MoviesCategoriesCheckBox.IsChecked = true;
            if (catList.Contains(Question.QuestionCategory.Pets))
                PetsCategoriesCheckBox.IsChecked = true;
            if (catList.Contains(Question.QuestionCategory.Politics))
                PoliticsCategoriesCheckBox.IsChecked = true;
            if (catList.Contains(Question.QuestionCategory.Technology))
                TechnologyCategoriesCheckBox.IsChecked = true;
            if (catList.Contains(Question.QuestionCategory.Television))
                TelevisionCategoriesCheckBox.IsChecked = true;
            if (catList.Contains(Question.QuestionCategory.VideoGames))
                VideoGamesCategoriesCheckBox.IsChecked = true;
            if (catList.Contains(Question.QuestionCategory.WorldNews))
                WorldNewsCategoriesCheckBox.IsChecked = true;
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            CheckCategories();
        }

        protected override void OnNavigatingFrom(System.Windows.Navigation.NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);
            MainPage.chosenCategories = categoryList;
        }
        

        public bool AddOrUpdateValue(string name, Object value)
        {
            bool valueChanged = false;
            if (MainPage.settings.Contains(name))
            {
                if (MainPage.settings[name] != value)
                {
                    MainPage.settings[name] = value;
                    valueChanged = true;
                }
            }
            else
            {
                MainPage.settings.Add(name, value);
                valueChanged = true;
            }

            return valueChanged;
        }

        public T GetValueOrDefault<T>(string name, T defaultValue)
        {
            T value;
            if (MainPage.settings.Contains(name))
            {
                value = (T)MainPage.settings[name];
            }
            else
            {
                value = defaultValue;
            }
            return value;
        }
               
        public void ClearCategories()
        {
            categoryList.Clear();
        }

        private void AdultCategoriesCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            //AdultCategoriesCheckBox.IsChecked = true;
            categoryList.Add(Question.QuestionCategory.Adult);
        }

        private void AdultCategoriesCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            //AdultCategoriesCheckBox.IsChecked = false;
            categoryList.Remove(Question.QuestionCategory.Adult);
        }

        private void BooksCategoriesCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            //BooksCategoriesCheckBox.IsChecked = true;
            categoryList.Add(Question.QuestionCategory.Books);
        }

        private void BooksCategoriesCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            //BooksCategoriesCheckBox.IsChecked = false;
            categoryList.Remove(Question.QuestionCategory.Books);
        }

        private void BusinessCategoriesCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            //BusinessCategoriesCheckBox.IsChecked = true;
            categoryList.Add(Question.QuestionCategory.Business);
        }

        private void BusinessCategoriesCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            //BusinessCategoriesCheckBox.IsChecked = false;
            categoryList.Remove(Question.QuestionCategory.Business);
        }

        private void EducationCategoriesCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            //EducationCategoriesCheckBox.IsChecked = true;
            categoryList.Add(Question.QuestionCategory.Education);
        }

        private void EducationCategoriesCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            //EducationCategoriesCheckBox.IsChecked = false;
            categoryList.Remove(Question.QuestionCategory.Education);
        }

        private void LifestylesCategoriesCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            //LifestylesCategoriesCheckBox.IsChecked = true;
            categoryList.Add(Question.QuestionCategory.Lifestyles);
        }

        private void LifestylesCategoriesCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            //LifestylesCategoriesCheckBox.IsChecked = false;
            categoryList.Remove(Question.QuestionCategory.Lifestyles);
        }

        private void MoviesCategoriesCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            //MoviesCategoriesCheckBox.IsChecked = true;
            categoryList.Add(Question.QuestionCategory.Movies);
        }

        private void MoviesCategoriesCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            //MoviesCategoriesCheckBox.IsChecked = false;
            categoryList.Remove(Question.QuestionCategory.Movies);
        }

        private void PetsCategoriesCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            //PetsCategoriesCheckBox.IsChecked = true;
            categoryList.Add(Question.QuestionCategory.Pets);
        }

        private void PetsCategoriesCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            //PetsCategoriesCheckBox.IsChecked = false;
            categoryList.Remove(Question.QuestionCategory.Pets);
        }

        private void PoliticsCategoriesCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            //PoliticsCategoriesCheckBox.IsChecked = true;
            categoryList.Add(Question.QuestionCategory.Politics);
        }

        private void PoliticsCategoriesCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            //PoliticsCategoriesCheckBox.IsChecked = false;
            categoryList.Remove(Question.QuestionCategory.Politics);
        }

        private void SportsCategoriesCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            //SportsCategoriesCheckBox.IsChecked = true;
            categoryList.Add(Question.QuestionCategory.Sports);
        }

        private void SportsCategoriesCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            //SportsCategoriesCheckBox.IsChecked = false;
            categoryList.Remove(Question.QuestionCategory.Sports);
        }

        private void TechnologyCategoriesCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            //TechnologyCategoriesCheckBox.IsChecked = true;
            categoryList.Add(Question.QuestionCategory.Technology);
        }

        private void TechnologyCategoriesCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            //TechnologyCategoriesCheckBox.IsChecked = false;
            categoryList.Remove(Question.QuestionCategory.Technology);
        }

        private void TelevisionCategoriesCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            //TelevisionCategoriesCheckBox.IsChecked = true;
            categoryList.Add(Question.QuestionCategory.Television);
        }

        private void TelevisionCategoriesCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            //TelevisionCategoriesCheckBox.IsChecked = false;
            categoryList.Remove(Question.QuestionCategory.Television);
        }

        private void VideoGamesCategoriesCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            //VideoGamesCategoriesCheckBox.IsChecked = true;
            categoryList.Add(Question.QuestionCategory.VideoGames);
        }

        private void VideoGamesCategoriesCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            //VideoGamesCategoriesCheckBox.IsChecked = false;
            categoryList.Remove(Question.QuestionCategory.VideoGames);
        }

        private void WorldNewsCategoriesCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            //WorldNewsCategoriesCheckBox.IsChecked = true;
            categoryList.Add(Question.QuestionCategory.WorldNews);
        }

        private void WorldNewsCategoriesCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            //WorldNewsCategoriesCheckBox.IsChecked = true;
            categoryList.Add(Question.QuestionCategory.WorldNews);
        }

        private void DoneButton_Click(object sender, RoutedEventArgs e)
        {
            //MainPage.SetCategoryList(categoryList);
            if (categoryList.Count > 0)
            {
                MainPage.hasChosenCategories = true;
            }

            if (NavigationService.CanGoBack == true)
                NavigationService.GoBack();
        }

    }
}