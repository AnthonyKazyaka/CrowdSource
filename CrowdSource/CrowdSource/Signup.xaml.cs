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
using System.Threading;
using ServerLibr;

namespace CrowdSource
{
    public class Ethnicity
    {
        public string Name
        {
            get;
            set;
        }
    }

    public partial class Signup : PhoneApplicationPage
    {
        

        public static IsolatedStorageSettings settings;

        public Signup()
        {
            InitializeComponent();

            List<Ethnicity> source = new List<Ethnicity>();
            
            // temporary - load dynamically later
            source.Add(new Ethnicity() { Name = "Caucasian" });
            source.Add(new Ethnicity() { Name = "Asian" });
            source.Add(new Ethnicity() { Name = "Mexican" });
            source.Add(new Ethnicity() { Name = "" });

            this.Ethnicity.ItemsSource = source;

            settings = IsolatedStorageSettings.ApplicationSettings;
        }

        private void CreateUser(object sender, System.Windows.RoutedEventArgs e)
        {
        	// TODO: Add event handler implementation here.
			
            if(settings.Contains("UserData"))
            {
                // user already exists
                NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
                return;
            }

            // validate form
             // check for valid username
             


             // check for valid age
             int age;
             if (!Int32.TryParse(Age.Text, out age))
             {
                 // invalid age, flag and throw some error text

                 // error

                 return;

             }


             // make sure gender has been selected
             if ((Male.IsChecked.HasValue) && (Female.IsChecked.HasValue))
             {
                 if (!((bool)Male.IsChecked) && !((bool)Female.IsChecked))
                 {
                     // error select gender

                     return;
                 }
             }
             else
             {
                 return;
             }

             string sentGender = "";
             User.Gender gender = User.Gender.Male;
             if((bool)Male.IsChecked)
             {
                 gender = User.Gender.Male;
                 sentGender = "Male";
             }
             if((bool)Female.IsChecked)
             {
                 gender = User.Gender.Female;
                 sentGender = "Female";
             }

             if (Username.Text == "")
             {
                 return;
             }

            // register user with server - ? - need to?
            try
            {
                ServerLibr.ServerConnection.Del handler = this.response;
 
                ServerLibr.ServerConnection.RegisterAuthor(Username.Text, (DateTime.Now.Year - age) + "", sentGender, ((Ethnicity)(Ethnicity.SelectedItem)).Name, "Adult", handler);
 

            }
            catch (Exception ex)
            {
                // display error and return
                error.Text = "Error: Connection failure";
                Error.Visibility = System.Windows.Visibility.Visible;
                return;
            }


			
			
        }

        public void response(XmlAuthor author)
        {

            User.Gender gen;
            if(author.Gender == "Male")
                gen = User.Gender.Male;
            else
                gen = User.Gender.Female;

            // set up user
            User user = new User(author.Id, Question.CensorshipLevel.Adults, gen, author.Username, author.BirthYear, author.Ethnicity);

            // create user in settings
            settings.Add("UserData", user);

            // redirect to Main Page
            NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));

        }
    }
}
