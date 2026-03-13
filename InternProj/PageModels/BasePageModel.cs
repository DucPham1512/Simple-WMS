using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using InternProj.Data;
using InternProj.Models;
using System.Text.RegularExpressions;
//using Microsoft.UI.Xaml.Controls.Primitives;
using System.Collections.ObjectModel;


namespace InternProj.PageModels
{
    public abstract partial class BasePageModel : ObservableObject
    {
        public abstract Task LoadData();

        protected BasePageModel(DatabaseWatcherService watcherService)
        {
            watcherService.DatabaseChanged += (s, e) =>
            {
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    await Task.Delay(200);
                    await LoadData();
                });
            };
        }
    }
}