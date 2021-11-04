﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace Assignment2
{


    public class Article
    {
        public string SiteName { get; set; }
        public DateTime Published { get; set; }
        public string Title { get; set; }

    }

    public partial class MainWindow : Window
    {
        private List<Article> articles = new List<Article>();
        private Thickness spacing = new Thickness(5);
        private HttpClient http = new HttpClient();
        // We will need these as instance variables to access in event handlers.
        private TextBox addFeedTextBox;
        private Button addFeedButton;
        private ComboBox selectFeedComboBox;
        private Button loadArticlesButton;
        private StackPanel articlePanel;

        public MainWindow()
        {
            InitializeComponent();
            Start();
        }

        private void Start()
        {
            // Window options
            Title = "Feed Reader";
            Width = 800;
            Height = 400;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            // Scrolling
            var root = new ScrollViewer();
            root.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            Content = root;

            // Main grid
            var grid = new Grid();
            root.Content = grid;
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

            var addFeedLabel = new Label
            {
                Content = "Feed URL:",
                Margin = spacing
            };
            grid.Children.Add(addFeedLabel);

            addFeedTextBox = new TextBox
            {
                Margin = spacing,
                Padding = spacing
            };
            grid.Children.Add(addFeedTextBox);
            Grid.SetColumn(addFeedTextBox, 1);

            addFeedButton = new Button
            {
                Content = "Add Feed",
                Margin = spacing,
                Padding = spacing
            };

            addFeedButton.Click += btn_add_feed_click;
            grid.Children.Add(addFeedButton);
            Grid.SetColumn(addFeedButton, 2);

            var selectFeedLabel = new Label
            {
                Content = "Select Feed:",
                Margin = spacing
            };
            grid.Children.Add(selectFeedLabel);
            Grid.SetRow(selectFeedLabel, 1);

            selectFeedComboBox = new ComboBox
            {
                Margin = spacing,
                Padding = spacing,
                IsEditable = false
            };
            grid.Children.Add(selectFeedComboBox);
            Grid.SetRow(selectFeedComboBox, 1);
            Grid.SetColumn(selectFeedComboBox, 1);

            loadArticlesButton = new Button
            {
                Content = "Load Articles",
                Margin = spacing,
                Padding = spacing,
            };

            loadArticlesButton.Click += btn_load_click;
            grid.Children.Add(loadArticlesButton);
            Grid.SetRow(loadArticlesButton, 1);
            Grid.SetColumn(loadArticlesButton, 2);

            articlePanel = new StackPanel
            {
                Orientation = Orientation.Vertical,
                Margin = spacing
            };
            grid.Children.Add(articlePanel);
            Grid.SetRow(articlePanel, 2);
            Grid.SetColumnSpan(articlePanel, 3);

            // These are just placeholders.
            // Replace them with your own code that shows actual articles.
            for (int i = 0; i < 3; i++)
            {
                var articlePlaceholder = new StackPanel
                {
                    Orientation = Orientation.Vertical,
                    Margin = spacing
                };
                articlePanel.Children.Add(articlePlaceholder);

                var articleTitle = new TextBlock
                {
                    Text = "2021-01-02 12:34 - Placeholder for an actual article title #" + (i + 1),
                    FontWeight = FontWeights.Bold,
                    TextTrimming = TextTrimming.CharacterEllipsis
                };
                articlePlaceholder.Children.Add(articleTitle);

                var articleWebsite = new TextBlock
                {
                    Text = "Website name #" + (i + 1)
                };
                articlePlaceholder.Children.Add(articleWebsite);
            }
        }

        private void btn_add_feed_click(object sender, RoutedEventArgs e)
        {
            var test = LoadDocumentAsync(addFeedTextBox.Text);

        }



        private void btn_load_click(object sender, RoutedEventArgs e)
        {

        }



        private async Task<XDocument> LoadDocumentAsync(string url)
        {
            // This is just to simulate a slow/large data transfer and make testing easier.
            // Remove it if you want to.


            await Task.Delay(1000);
            var response = await http.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var stream = await response.Content.ReadAsStreamAsync();
            var feedFromUrl = XDocument.Load(stream);

            string ArticleHost = feedFromUrl.Descendants().Where(s => s.Name == "title").FirstOrDefault().Value;

            var list = (from x in feedFromUrl.Descendants("item")
                        select new {
                            title = x.Element("title").Value,
                            link = x.Element("link").Value,
                            published = x.Element("pubDate").Value

                        });

            for(int i = 0; i < 5; i++)
            {
                Article article = new Article
                {
                    Title = list.ElementAt(i).title,
                    Published = DateTime.Parse(list.ElementAt(i).published),
                    SiteName = ArticleHost
                };
                articles.Add(article);
            }
           
            return feedFromUrl;
        }
    }
}
