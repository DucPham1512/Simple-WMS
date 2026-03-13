using Syncfusion.Maui.TabView;
using Microsoft.Maui.Controls;
using System;
using System.Linq;

namespace InternProj.Pages
{
    public partial class MainTabbedPage : ContentPage
    {
        public static MainTabbedPage Current { get; private set; }

        public MainTabbedPage()
        {
            InitializeComponent();
            Current = this;
        }

        public void AddNewTab(string title = "New Tab")
        {
            var newTab = new SfTabItem
            {
                Header = title,
                Content = new Label 
                { 
                    Text = "Select an item from the sidebar to load here.", 
                    HorizontalOptions = LayoutOptions.Center, 
                    VerticalOptions = LayoutOptions.Center 
                }
            };

            MainTabView.Items.Add(newTab);
            MainTabView.SelectedIndex = (int)(MainTabView.Items.Count - 1);
        }

        public void NewTab(object sender, EventArgs e)
        {
            AddNewTab();
        }

        public void CloseTab(object sender, EventArgs e)
        {
            if (MainTabView.Items.Count > 1)
            {
                int indexToRemove = (int)MainTabView.SelectedIndex;
                if (indexToRemove >= 0 && indexToRemove < MainTabView.Items.Count)
                {
                    MainTabView.Items.RemoveAt(indexToRemove);
                }
            }
            else if (MainTabView.Items.Count == 1)
            {
                // Reset the last tab instead of clearing it completely
                var activeTab = MainTabView.Items[0];
                activeTab.Header = "Home";
                activeTab.Content = new Label 
                { 
                    Text = "Welcome to Workspace! Please select a function from the sidebar.", 
                    HorizontalOptions = LayoutOptions.Center, 
                    VerticalOptions = LayoutOptions.Center,
                    FontSize = 18
                };
                activeTab.BindingContext = null;
            }
        }

        public async void LoadPageIntoActiveTab(Page pageToLoad, string routeName)
        {
            var tabTitle = pageToLoad?.Title ?? routeName;

            // Check if page is already open, if so just switch to it
            for (int i = 0; i < MainTabView.Items.Count; i++)
            {
                if (MainTabView.Items[i].Header != null && MainTabView.Items[i].Header.ToString() == tabTitle)
                {
                    MainTabView.SelectedIndex = i;
                    return;
                }
            }

            SfTabItem targetTab = null;

            // Check if current active tab is a placeholder "New Tab" or "Home"
            if (MainTabView.Items.Count > 0)
            {
                var active = MainTabView.Items[(int)MainTabView.SelectedIndex];
                if (active.Header != null && (active.Header.ToString() == "New Tab" || active.Header.ToString() == "Home"))
                {
                    targetTab = active;
                }
            }

            // If we didn't find a placeholder tab, make a new one
            if (targetTab == null)
            {
                targetTab = new SfTabItem();
                MainTabView.Items.Add(targetTab);
                MainTabView.SelectedIndex = (int)(MainTabView.Items.Count - 1);
            }

            try
            {
                if (pageToLoad != null)
                {
                    targetTab.Header = tabTitle;

                    if (pageToLoad is ContentPage contentPage)
                    {
                        var pageContent = contentPage.Content;
                        contentPage.Content = null; 
                        
                        targetTab.Content = pageContent;
                        targetTab.BindingContext = contentPage.BindingContext;

                        ExecuteLoadDataCommand(contentPage.BindingContext);
                    }
                }
                else
                {
                    await DisplayAlertAsync("Error", $"Could not resolve route: {routeName}", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlertAsync("Navigation Error", ex.Message, "OK");
            }
        }

        private void MainTabView_SelectionChanged(object sender, Syncfusion.Maui.TabView.TabSelectionChangedEventArgs e)
        {
            if (e.NewIndex < 0 || e.NewIndex >= MainTabView.Items.Count) return;

            var activeTab = MainTabView.Items[(int)e.NewIndex];
            if (activeTab?.BindingContext != null)
            {
                ExecuteLoadDataCommand(activeTab.BindingContext);
            }
        }

        private void ExecuteLoadDataCommand(object bindingContext)
        {
            if (bindingContext == null) return;

            try
            {
                var type = bindingContext.GetType();
                var loadCommandProp = type.GetProperty("LoadDataCommand", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                
                if (loadCommandProp != null)
                {
                    var command = loadCommandProp.GetValue(bindingContext) as System.Windows.Input.ICommand;
                    if (command != null && command.CanExecute(null))
                    {
                        command.Execute(null);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to execute LoadDataCommand: {ex.Message}");
            }
        }
    }
}
