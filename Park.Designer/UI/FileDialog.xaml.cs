using MaterialDesignExtensions.Controls;
using MaterialDesignExtensions.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Park.Designer.UI
{
    /// <summary>
    /// FileDialog.xaml 的交互逻辑
    /// </summary>
    public partial class FileDialog : WindowBase
    {
        private FileDialog()
        {
            InitializeComponent();
        }
        public static string SaveFile(Window owner)
        {

            return OpenOrSaveFile(new SaveFileControl(), owner);
        }
        public static string OpenFile(Window owner)
        {
            return OpenOrSaveFile(new OpenFileControl(), owner);
        }

        public static string OpenOrSaveFile(BaseFileControl dialog, Window owner)
        {
            dialog.Filters = new List<IFileFilter>() { FileFilter.Create("JSON配置文件", "*.json") };
            dialog.GroupFoldersAndFiles = true;
            FileDialog win = new FileDialog();
            win.Content = dialog;
            win.Owner = owner;
            string path = null;
            dialog.FileSelected += (s, e) =>
            {
                path = dialog.CurrentFile;
                win.Close();
            };
            win.ShowDialog();
            return path;
        }
    }
}
