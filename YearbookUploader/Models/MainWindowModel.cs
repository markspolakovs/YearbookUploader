using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YearbookUploader.Models
{
    class MainWindowModel : AbstractModel
    {
        public ObservableCollection<UploadTask> Tasks { get; set; }

        public MainWindowModel()
        {
            Tasks = new ObservableCollection<UploadTask>();
        }
    }
}
