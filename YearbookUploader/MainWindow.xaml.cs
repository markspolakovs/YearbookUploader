using System;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Forms;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Graph;
using Microsoft.Identity.Client;
using Newtonsoft.Json;
using YearbookUploader.Helpers;
using YearbookUploader.Models;
using YearbookUploader.Services;
using Button = System.Windows.Controls.Button;
using ListViewItem = System.Windows.Controls.ListViewItem;

namespace YearbookUploader
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private MainWindowModel model = new MainWindowModel();

        internal static PublicClientApplication App = new PublicClientApplication("7843cd67-9280-47e6-a5b0-d83d415fd59d");

        internal static string[] Scopes = {"User.Read", "Files.ReadWrite", "Files.ReadWrite.All"};

        internal static string AccessToken;

        internal static GraphServiceClient Client;

        public MainWindow()
        {
            this.InitializeComponent();
            this.DataContext = this.model;
            if (System.IO.File.Exists("_tasks.json"))
            {
                this.model = JsonConvert.DeserializeObject<MainWindowModel>(System.IO.File.ReadAllText("_tasks.json"));
                this.DataContext = this.model;
            }
            else
            {

                AddNewTask();
            }
        }

        private async void OneDriveSignInButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            try
            {
                var result = await App.AcquireTokenAsync(Scopes);
                AccessToken = result.AccessToken;
                await InitThings();
            }
            catch (MsalClientException ex)
            {
                Debug.Print("swallowing", ex);
            }
        }

        private async Task InitThings()
        {
            Client = new GraphServiceClient(new DelegateAuthenticationProvider(msg =>
            {
                msg.Headers.Authorization = new AuthenticationHeaderValue("Bearer", AccessToken);
                return Task.FromResult(0);
            }));

            var me = await Client.Me.Request().GetAsync();
            OneDriveSignInButton.Content = $"Hello, {me.DisplayName}";
        }

        private async void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (Client == null)
            {
                await this.ShowMessageAsync("Cannot start upload", "Please sign in first!");
                return;
            }
            System.IO.File.WriteAllText("_tasks.json", JsonConvert.SerializeObject(this.model));
            foreach (var task in this.model.Tasks)
            {
                task.Locked = true;
            }

            UploadProgressFlyout.Theme = FlyoutTheme.Inverse;
            UploadProgressFlyout.Header = "Uploading...";
            UploadProgressFlyout.IsOpen = true;

            MasterProgressBar.Maximum = this.model.Tasks.Count;
            var writer = new TextBoxWriter(LogsTextBox);
            var hasAnyTaskErrored = false;
            foreach (var task in this.model.Tasks)
            {
                task.State = UploadTaskState.Running;
                var progress = new Progress<double>(stat => { MasterProgressBar.Value = stat; });
                var fileProgress = new Progress<double>(stat => { FileProgressBar.Value = stat; });
                try
                {
                    await UploadService.Upload(task, progress, fileProgress, writer);
                    task.State = UploadTaskState.Successful;
                }
                catch (Exception ex)
                {
                    writer.WriteLine("ERROR!!!! " + ex.ToString());
                    task.State = UploadTaskState.Errored;
                    hasAnyTaskErrored = true;
                }
            }
            foreach (var task in this.model.Tasks)
            {
                task.Locked = false;
            }

            if (hasAnyTaskErrored)
            {
                UploadProgressFlyout.Header = "Upload complete, but with errors";
                UploadProgressFlyout.Theme = FlyoutTheme.Accent;
            }
            else
            {
                UploadProgressFlyout.Header = "Upload complete!";
                UploadProgressFlyout.Theme = FlyoutTheme.Accent;
            }
        }

        private void AddNewTaskButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            AddNewTask();
        }

        private string BrowseFolder()
        {
            using (var picker = new FolderBrowserDialog())
            {
                var result = picker.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK && !string.IsNullOrWhiteSpace(picker.SelectedPath))
                {
                    return picker.SelectedPath;
                }

                return null;
            }
        }

        private string BrowseFile(string startingFolder)
        {
            using (var dialog = new OpenFileDialog())
            {
                dialog.InitialDirectory = startingFolder;
                var result = dialog.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK && !string.IsNullOrWhiteSpace(dialog.FileName))
                {
                    return dialog.FileName;
                }

                return null;
            }
        }

        private void AddNewTask()
        {
            var task = new UploadTask();
            this.model.Tasks.Add(task);
        }

        private void DeletButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            this.model.Tasks.RemoveAt(Tasks.SelectedIndex);
        }
    }
}
