using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace LAB
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Width = 1920;
            this.Height = 1080;
            this.BackColor = Color.Gray;

            MenuStrip menuStrip = new MenuStrip();
            menuStrip.BackColor = Color.Gray;
            menuStrip.ForeColor = Color.White;

            ToolStripMenuItem aboutMenuItem = new ToolStripMenuItem("About");
            aboutMenuItem.Click += AboutMenuItem_Click;
            aboutMenuItem.Width = 60;
            aboutMenuItem.Height = 10;
            aboutMenuItem.ForeColor = Color.White;
            menuStrip.Items.Add(aboutMenuItem);

            this.MainMenuStrip = menuStrip;
            this.Controls.Add(menuStrip);

            Button selectFolderButton = new Button();
            selectFolderButton.Text = "Выбрать папку";
            selectFolderButton.Click += SelectFolderButton_Click;
            selectFolderButton.Top = 40;
            selectFolderButton.Left = 10;
            selectFolderButton.Width = 200;
            selectFolderButton.Height = 50;
            selectFolderButton.BackColor = Color.Gray;
            selectFolderButton.ForeColor = Color.White;
            this.Controls.Add(selectFolderButton);

            TextBox folderPathTextBox = new TextBox();
            folderPathTextBox.ReadOnly = true;
            folderPathTextBox.Top = 70;
            folderPathTextBox.Left = 210;
            folderPathTextBox.Width = 1080;
            folderPathTextBox.BackColor = Color.Black;
            folderPathTextBox.ForeColor = Color.White;
            this.Controls.Add(folderPathTextBox);

            ListBox folderListBox = new ListBox();
            folderListBox.Top = 110;
            folderListBox.Left = 10;
            folderListBox.Width = 200;
            folderListBox.Height = 400;
            folderListBox.BackColor = Color.Black;
            folderListBox.ForeColor = Color.White;
            folderListBox.DoubleClick += FolderListBox_DoubleClick;
            this.Controls.Add(folderListBox);

            DataGridView filesDataGridView = new DataGridView();
            filesDataGridView.Top = 110;
            filesDataGridView.Left = 220;
            filesDataGridView.Width = 550;
            filesDataGridView.Height = 400;
            filesDataGridView.ReadOnly = true;
            filesDataGridView.AllowUserToAddRows = false;
            filesDataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            filesDataGridView.DoubleClick += FilesDataGridView_DoubleClick;
            filesDataGridView.Columns.Add("FileName", "Название");
            filesDataGridView.Columns.Add("LastModified", "Дата последней модификации");
            filesDataGridView.Columns.Add("FileSize", "Количество байт");
            filesDataGridView.Columns.Add("RandomValue", "Случайное значение");

            filesDataGridView.BackgroundColor = Color.Black;
            filesDataGridView.DefaultCellStyle.BackColor = Color.Black;
            filesDataGridView.DefaultCellStyle.ForeColor = Color.White;
            filesDataGridView.ColumnHeadersDefaultCellStyle.BackColor = Color.Gray;
            filesDataGridView.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            this.Controls.Add(filesDataGridView);

            Button processFilesButton = new Button();
            processFilesButton.Text = "Запустить обработку файлов";
            processFilesButton.Top = 520;
            processFilesButton.Left = 10;
            processFilesButton.BackColor = Color.Gray;
            processFilesButton.ForeColor = Color.White;
            processFilesButton.Click += async (s, ev) => await ProcessFilesAsync(filesDataGridView);
            processFilesButton.Visible = false;
            this.Controls.Add(processFilesButton);
        }

        private void AboutMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Разработчик: Almaz Akzholtoev", "SEST-2-22", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void SelectFolderButton_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog dialog = new FolderBrowserDialog())
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    TextBox folderPathTextBox = this.Controls.OfType<TextBox>().First();
                    ListBox folderListBox = this.Controls.OfType<ListBox>().First();
                    DataGridView filesDataGridView = this.Controls.OfType<DataGridView>().First();
                    Button processFilesButton = this.Controls.OfType<Button>().Last();

                    folderPathTextBox.Text = dialog.SelectedPath;
                    folderListBox.Items.Clear();
                    filesDataGridView.Rows.Clear();

                    var directories = Directory.GetDirectories(dialog.SelectedPath);
                    foreach (var dir in directories)
                    {
                        folderListBox.Items.Add(Path.GetFileName(dir));
                    }

                    var files = Directory.GetFiles(dialog.SelectedPath);
                    foreach (var file in files)
                    {
                        FileInfo fileInfo = new FileInfo(file);
                        filesDataGridView.Rows.Add(fileInfo.Name, fileInfo.LastWriteTime, fileInfo.Length, "");
                    }

                    processFilesButton.Visible = true;
                }
            }
        }

        private void FolderListBox_DoubleClick(object sender, EventArgs e)
        {
            ListBox folderListBox = sender as ListBox;
            string selectedFolder = folderListBox.SelectedItem as string;
            TextBox folderPathTextBox = this.Controls.OfType<TextBox>().First();
            string folderPath = Path.Combine(folderPathTextBox.Text, selectedFolder);

            Form folderInfoForm = new Form();
            folderInfoForm.Text = "Информация о папке";
            folderInfoForm.BackColor = Color.Black;
            folderInfoForm.ForeColor = Color.White;

            Label folderNameLabel = new Label();
            folderNameLabel.Text = "Название: " + selectedFolder;
            folderNameLabel.Top = 10;
            folderNameLabel.Left = 10;
            folderNameLabel.Width = 400;
            folderNameLabel.BackColor = Color.Black;
            folderNameLabel.ForeColor = Color.White;

            Label lastModifiedLabel = new Label();
            DirectoryInfo dirInfo = new DirectoryInfo(folderPath);
            lastModifiedLabel.Text = "Дата последней модификации: " + dirInfo.LastWriteTime.ToString();
            lastModifiedLabel.Top = 40;
            lastModifiedLabel.Left = 10;
            lastModifiedLabel.Width = 400;
            lastModifiedLabel.BackColor = Color.Black;
            lastModifiedLabel.ForeColor = Color.White;

            folderInfoForm.Controls.Add(folderNameLabel);
            folderInfoForm.Controls.Add(lastModifiedLabel);
            folderInfoForm.ShowDialog();
        }

        private void FilesDataGridView_DoubleClick(object sender, EventArgs e)
        {
            DataGridView filesDataGridView = sender as DataGridView;
            if (filesDataGridView.SelectedRows.Count > 0)
            {
                DataGridViewRow selectedRow = filesDataGridView.SelectedRows[0];
                string fileName = selectedRow.Cells["FileName"].Value.ToString();
                TextBox folderPathTextBox = this.Controls.OfType<TextBox>().First();
                string filePath = Path.Combine(folderPathTextBox.Text, fileName);

                DialogResult result = MessageBox.Show("Хотите ли вы продублировать файл?", "Дублирование файла", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    string newFilePath = Path.Combine(folderPathTextBox.Text, Path.GetFileNameWithoutExtension(filePath) + "_copyed" + Path.GetExtension(filePath));
                    File.Copy(filePath, newFilePath);

                    FileInfo fileInfo = new FileInfo(newFilePath);
                    filesDataGridView.Rows.Add(fileInfo.Name, fileInfo.LastWriteTime, fileInfo.Length, "");
                }
            }
        }

        private async Task ProcessFilesAsync(DataGridView filesDataGridView)
{
    Random random = new Random();
    int fileCount = filesDataGridView.Rows.Count;


    List<Task> tasks = new List<Task>();

    foreach (DataGridViewRow row in filesDataGridView.Rows)
    {

        Task task = Task.Run(async () =>
        {
            int delaySeconds = random.Next(1, fileCount + 1);
            await Task.Delay(delaySeconds * 1000);
            row.Cells["RandomValue"].Value = delaySeconds;
        });

        tasks.Add(task);
    }

 
    await Task.WhenAll(tasks);
}

    }
}