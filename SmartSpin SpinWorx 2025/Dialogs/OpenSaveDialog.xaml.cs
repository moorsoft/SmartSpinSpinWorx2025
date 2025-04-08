using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SmartSpin.Dialogs
{
    public enum OpenSaveDialogType { Open, Save };

    /// <summary>
    /// Interaction logic for OpenSaveDialog.xaml
    /// </summary>
    public partial class OpenSaveDialog : Window
    {
        const int FileScrollAmount = 7;
        enum EditOp { None, Cut, Copy };
        enum EditOpType { Directory, File };

        public string FileName = String.Empty;
        public string DefaultExt = String.Empty;
        private string CurrentFolder = String.Empty;
        private ObservableCollection<string> CurrentFoldersList = new ObservableCollection<string>();
        private ObservableCollection<string> CurrentFilesList = new ObservableCollection<string>();
        private string SelectedFolder = String.Empty;
        private string StartingFolder = String.Empty;
        private string CutCopySelection = String.Empty;
        private EditOp EditOperation = EditOp.None;
        private EditOpType EditOperationType = EditOpType.File;

        private static RoutedUICommand rename;

        static OpenSaveDialog()
        {
            // Initialise the rename command
            rename = new RoutedUICommand("Rename", "Rename", typeof(OpenSaveDialog));
        }

        public static RoutedUICommand Rename
        {
            get { return rename; }
        }

        public OpenSaveDialog()
        {
            InitializeComponent();
            Owner = App.Current.MainWindow;
            this.keyboard.HideESC();
            this.keyboard.HideEnter();
            this.keyboard.EditText = this.txtFileName;
            this.lbFolders.ItemsSource = CurrentFoldersList;
            this.lbFiles.ItemsSource = CurrentFilesList;
        }

        public void SetOpeningFolder(string SFolder)
        {
            StartingFolder = SFolder;
        }

        private void OpenSaveDialog_Loaded(object sender, RoutedEventArgs e)
        {
            CurrentFolder = StartingFolder;
            btnFoldersOutOf_Click(null, null);
            this.lbFolders.Focus();
            if (!String.IsNullOrEmpty(FileName))
            {
                FileInfo f = new FileInfo(FileName);
                if (f.DirectoryName == StartingFolder)
                {
                    int ix = this.CurrentFilesList.IndexOf(f.Name);
                    if (ix >= 0)
                    {
                        this.lbFiles.SelectedIndex = ix;
                        this.lbFiles.ScrollIntoView(CurrentFilesList[ix]);
                    }
                }
            }
            CheckButtons();
        }


        private void CheckButtons()
        {
            CommandManager.InvalidateRequerySuggested();
        }

        private void CutCopyRenameDeleteCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            IInputElement iie = Keyboard.FocusedElement;
            if (iie is ListBoxItem)
            {
                e.CanExecute = true;
                return;
            }

            bool HaveItem = (((iie == this.lbFolders) && this.lbFolders.SelectedIndex >= 0) ||
                             ((iie == this.lbFiles) && this.lbFiles.SelectedIndex >= 0));
            e.CanExecute = HaveItem;
        }

        private void PasteCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            IInputElement iie = Keyboard.FocusedElement;
            bool HaveItem = (((iie == this.lbFolders) && this.lbFolders.SelectedIndex >= 0) ||
                             ((iie == this.lbFiles) && this.lbFiles.SelectedIndex >= 0));
            e.CanExecute = (EditOperation != EditOp.None);
        }

        private void FillFolders()
        {
            CurrentFoldersList.Clear();
            CurrentFilesList.Clear();
            this.txtFileName.Text = String.Empty;

            lblCurrentFolder.Text = CurrentFolder;
            if (CurrentFolder.Length == 0)
            {
                foreach (DriveInfo di in DriveInfo.GetDrives())
                {
                    if (di.IsReady)
                    {
                        CurrentFoldersList.Add(string.Format(@"{0} ({1})", di.Name, di.VolumeLabel));
                    }
                    else
                    {
                        CurrentFoldersList.Add(string.Format(@"{0}", di.Name));
                    }
                }
            }
            else
            {
                DirectoryInfo parentDI = new DirectoryInfo(CurrentFolder);
                DirectoryInfo[] arrDI = null;
                try
                {
                    arrDI = parentDI.GetDirectories();
                }
                catch
                {
                }
                if (arrDI != null)
                {
                    foreach (DirectoryInfo di in arrDI)
                    {
                        CurrentFoldersList.Add(di.Name);
                    }
                    if (arrDI.Length > 0) this.lbFolders.SelectedIndex = 0;
                }
            }
            CheckButtons();
        }

        private void FillFiles(string Folder)
        {
            this.CurrentFilesList.Clear();
            this.txtFileName.Text = String.Empty;
            DirectoryInfo di = new DirectoryInfo(Folder);
            FileInfo[] arrFI = null;
            try
            {
                if (DefaultExt == String.Empty)
                    arrFI = di.GetFiles();
                else
                {
                    arrFI = di.GetFiles("*." + DefaultExt);
                }
            }
            catch
            { }

            if (arrFI == null)
            {
                return;
            }
            foreach (FileInfo fi in arrFI)
            {
                this.CurrentFilesList.Add(fi.Name);
            }
        }

        public OpenSaveDialogType DialogType
        {
            set
            {
                if (value == OpenSaveDialogType.Open)
                {
                    this.btnOpen.Visibility = Visibility.Visible;
                    this.btnSave.Visibility = Visibility.Collapsed;
                }
                else
                {
                    this.btnOpen.Visibility = Visibility.Collapsed;
                    this.btnSave.Visibility = Visibility.Visible;
                }
            }
        }

        private string GetSelectedFile()
        {
            return System.IO.Path.Combine(SelectedFolder, txtFileName.Text);
        }

        private void OpenCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            string FName = GetSelectedFile();
            if (File.Exists(FName))
            {
                FileName = FName;
                this.DialogResult = true;
            }
        }

        private void SaveCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                string FName = GetSelectedFile();
                if (!FName.EndsWith("." + DefaultExt, true, null)) FName += "." + DefaultExt;
                // if (File.Exists(FName))
                FileName = FName;
                this.DialogResult = true;
            }
            catch (Exception)
            {
                MyMessageBox.Show("Invalid Filename", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                this.DialogResult = false;
            }
        }

        private void lbFolders_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Check if a drive has been selected
            if (this.lbFolders.SelectedItem != null)
            {
                if (this.lbFolders.SelectedItem.ToString().Contains(System.IO.Path.VolumeSeparatorChar.ToString()))
                {
                    DriveInfo di = new DriveInfo(lbFolders.SelectedItem.ToString().Substring(0, 3));
                    if (di.IsReady)
                    {
                        SelectedFolder = this.lbFolders.SelectedItem.ToString().Substring(0, 3);
                        FillFiles(SelectedFolder);
                    }
                    else
                    {
                        SelectedFolder = string.Empty;
                        this.CurrentFilesList.Clear();
                    }
                }
                else
                {
                    SelectedFolder = System.IO.Path.Combine(CurrentFolder, this.lbFolders.SelectedItem.ToString());
                    FillFiles(SelectedFolder);
                }
            }
            this.CheckButtons();
        }

        private void btnFoldersInto_Click(object sender, RoutedEventArgs e)
        {
            if (this.lbFolders.SelectedItem == null) return;
            if (this.lbFolders.SelectedItem.ToString().Contains(System.IO.Path.VolumeSeparatorChar.ToString()))
            {
                DriveInfo di = new DriveInfo(this.lbFolders.SelectedItem.ToString().Substring(0, 3));
                if (di.IsReady)
                    CurrentFolder = this.lbFolders.SelectedItem.ToString().Substring(0, 3);
                else
                    CurrentFolder = String.Empty;
            }
            else
            {
                CurrentFolder = System.IO.Path.Combine(CurrentFolder, this.lbFolders.SelectedItem.ToString());
            }
            FillFolders();
        }

        private void btnFoldersOutOf_Click(object sender, RoutedEventArgs e)
        {
            string FName = String.Empty;

            // If no folder is selected then we are just going to list the drives
            if (CurrentFolder.Length == 0)
            {
                FillFolders();
                return;
            }

            if (CurrentFolder.Length == 3)
            {
                FName = CurrentFolder;
                CurrentFolder = String.Empty;
            }
            else
            {
                FName = System.IO.Path.GetFileName(CurrentFolder);  // get end directory name
                CurrentFolder = CurrentFolder.Remove(CurrentFolder.LastIndexOf(System.IO.Path.DirectorySeparatorChar));
                if (CurrentFolder.Length == 2) CurrentFolder = CurrentFolder + System.IO.Path.DirectorySeparatorChar;
            }
            FillFolders();
            if (FName == String.Empty) return;
            int ix = this.CurrentFoldersList.IndexOf(FName);
            if (ix >= 0)
            {
                this.lbFolders.SelectedIndex = ix;
                this.lbFolders.ScrollIntoView(CurrentFoldersList[ix]);
            }
        }

        bool DontDoSelectedIndexChanged = false;
        private void lbFiles_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DontDoSelectedIndexChanged) return;
            if (this.lbFiles.SelectedItem == null) return;
            this.txtFileName.Text = this.lbFiles.SelectedItem.ToString();
            this.CheckButtons();
        }

        private void lbFiles_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (this.lbFiles.SelectedItem == null) return;
            this.txtFileName.Text = this.lbFiles.SelectedItem.ToString();
            this.CheckButtons();
        }

        private string CheckCutCopySelection()
        {
            string FName = String.Empty;
            if (lbFiles.SelectedIndex >= 0)
            {
                FName = GetSelectedFile();
                EditOperationType = EditOpType.File;
                if (File.Exists(FName)) return FName;
            }
            else
            {
                if (lbFolders.SelectedIndex >= 0)
                {
                    FName = SelectedFolder;
                    EditOperationType = EditOpType.Directory;
                    if (Directory.Exists(FName)) return FName;
                }
            }
            return String.Empty;
        }

        private void CutCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            EditOperation = EditOp.Cut;
            CutCopySelection = CheckCutCopySelection();
            if (CutCopySelection == String.Empty)
                EditOperation = EditOp.None;
            CheckButtons();
        }

        private void CopyCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            EditOperation = EditOp.Copy;
            CutCopySelection = CheckCutCopySelection();
            if (CutCopySelection == String.Empty)
                EditOperation = EditOp.None;
            CheckButtons();
        }

        public static void CopyAll(DirectoryInfo source, DirectoryInfo target)
        {
            // Check if the target directory exists, if not, create it.
            if (Directory.Exists(target.FullName) == false)
            {
                Directory.CreateDirectory(target.FullName);
            }

            // Copy each file into it’s new directory.
            foreach (FileInfo fi in source.GetFiles())
            {
                fi.CopyTo(System.IO.Path.Combine(target.ToString(), fi.Name), true);
            }

            // Copy each subdirectory using recursion.
            foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
            {
                DirectoryInfo nextTargetSubDir =
                    target.CreateSubdirectory(diSourceSubDir.Name);
                CopyAll(diSourceSubDir, nextTargetSubDir);
            }
        }

        private void PasteCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (EditOperationType == EditOpType.File)
            {
                if (EditOperation == EditOp.Copy)
                    File.Copy(CutCopySelection, System.IO.Path.Combine(SelectedFolder, System.IO.Path.GetFileName(CutCopySelection)));
                else
                    File.Move(CutCopySelection, System.IO.Path.Combine(SelectedFolder, System.IO.Path.GetFileName(CutCopySelection)));
                lbFolders_SelectionChanged(null, null);
            }
            else
            {
                if (EditOperation == EditOp.Copy)
                    CopyAll(new DirectoryInfo(CutCopySelection), new DirectoryInfo(System.IO.Path.Combine(SelectedFolder, System.IO.Path.GetFileName(CutCopySelection))));
                else
                    Directory.Move(CutCopySelection, System.IO.Path.Combine(SelectedFolder, System.IO.Path.GetFileName(CutCopySelection)));
                FillFolders();
            }
            EditOperation = EditOp.None;
            CheckButtons();
        }

        private void DeleteCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            string FName = CheckCutCopySelection();
            if (FName != String.Empty)
            {
                if (EditOperationType == EditOpType.File)
                {
                    if (MyMessageBox.Show("Are you sure you want to delete the file " + FName, "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question) ==  MessageBoxResult.Yes)
                    {
                        File.Delete(FName);
                        lbFolders_SelectionChanged(null, null);
                    }
                }
                else
                {
                    if (MyMessageBox.Show("Are you sure you want to delete the folder " + FName, "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        Directory.Delete(FName, true);
                        FillFolders();
                    }
                }
            }
        }

        private void RenameCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            string FName = CheckCutCopySelection();
            string NewName = String.Empty;

            if (FName != String.Empty)
            {
                if (EditOperationType == EditOpType.File)
                {
                    if (EditDialog.Show("Rename " + FName + " to:", "Rename file", String.Empty, out NewName) ?? false)
                    {
                        NewName = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(FName), NewName);
                        if (!NewName.EndsWith("." + DefaultExt, true, null)) NewName += "." + DefaultExt;
                        File.Move(FName, NewName);
                        lbFolders_SelectionChanged(null, null);
                    }
                }
                else
                {
                    if (EditDialog.Show("Rename " + FName + " to:", "Rename folder", String.Empty, out NewName) ?? false)
                    {
                        NewName = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(FName), NewName);
                        Directory.Move(FName, NewName);
                        FillFolders();
                    }
                }
            }
        }

        private void btnNewFolder_Click(object sender, RoutedEventArgs e)
        {
            string NewName = String.Empty;
            if (EditDialog.Show("New Folder Name:", "Create new folder", String.Empty, out NewName) ?? false)
            {
                if (lbFolders.SelectedIndex >= 0)
                {
                    string FName = SelectedFolder;
                    if (Directory.Exists(FName))
                    {
                        NewName = System.IO.Path.Combine(FName, NewName);
                        Directory.CreateDirectory(NewName);
                        CurrentFolder = FName;
                        FillFolders();
                    }
                }
            }
        }

        private void txtFileName_TextChanged(object sender, TextChangedEventArgs e)
        {
            for (int ix = 0; ix < lbFiles.Items.Count; ix++)
            {
                if (this.lbFiles.Items[ix].ToString().StartsWith(txtFileName.Text, StringComparison.CurrentCultureIgnoreCase))
                {
                    DontDoSelectedIndexChanged = true;
                    this.lbFiles.SelectedIndex = ix;
                    this.lbFiles.ScrollIntoView(this.lbFiles.SelectedItem);
                    DontDoSelectedIndexChanged = false;
                    break;
                }
            }
        }


    }
}
