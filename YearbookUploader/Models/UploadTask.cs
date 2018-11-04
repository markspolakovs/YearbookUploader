using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YearbookUploader.Models;

namespace YearbookUploader.Models
{
    public enum UploadTaskState
    {
        Initial,
        Running,
        Successful,
        Errored
    }

    public class UploadTask : AbstractModel
    {

        private string _taskName;
        public string TaskName
        {
            get { return _taskName; }
            set { _taskName = value; RaisePropertyChanged("TaskName"); }
        }

        private string _folder;
        public string Folder
        {
            get { return _folder; }
            set { _folder = value; RaisePropertyChanged("Folder"); }
        }

        private string _firstFileName;
        public string FirstFileName
        {
            get { return _firstFileName; }
            set { _firstFileName = value; RaisePropertyChanged("FirstFileName"); }
        }

        private string _lastFileName;
        public string LastFileName
        {
            get { return _lastFileName; }
            set { _lastFileName = value; RaisePropertyChanged("LastFileName"); }
        }

        private string _localBackupPath;
        public string LocalBackupPath
        {
            get { return _localBackupPath; }
            set { _localBackupPath = value; RaisePropertyChanged("LocalBathupPath"); }
        }

        private string _remoteBackupPath;
        public string RemoteBackupPath
        {
            get { return _remoteBackupPath; }
            set { _remoteBackupPath = value; RaisePropertyChanged("RemoteBackupPath"); }
        }

        private bool _locked;
        public bool Locked
        {
            get => _locked;
            set { _locked = value; RaisePropertyChanged("Locked"); }
        }

        private bool _overwriteAll;
        public bool OverwriteAll
        {
            get => _overwriteAll;
            set { _overwriteAll = value; RaisePropertyChanged("OverwriteAll"); }
        }

        private UploadTaskState _state;

        public UploadTaskState State
        {
            get => _state;
            set { _state = value; RaisePropertyChanged("State"); }
        }
    }
}
