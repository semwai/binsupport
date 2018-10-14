using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using System.IO.Ports;
namespace binsupport
{
    public partial class Form1 : Form
    {
       
        public Form1()
        {
            InitializeComponent();
        }

        private void создатьПроектToolStripMenuItem_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.ShowDialog();
            string path = folderBrowserDialog1.SelectedPath;
            System.IO.FileStream settings;
           // MessageBox.Show(folderBrowserDialog1.SelectedPath);return;
            if (path.Length > 0)
            {
               /* if (System.IO.File.Exists(path + "\\config.json"))
                {
                    MessageBox.Show("В данной папке уже существует проект, выберите другую папку или удалите старый проект.");
                }*/
                System.IO.Directory.CreateDirectory(path + "\\C");
                System.IO.Directory.CreateDirectory(path + "\\Asm");
                System.IO.Directory.CreateDirectory(path + "\\Res");
                var configfile = System.IO.File.Create(path + "\\config.json");  
                System.IO.File.Create(path + "\\C\\start.c").Close();
                System.IO.File.Create(path + "\\Asm\\boot.asm").Close();


                settings JSONsett = new settings();
                JSONsett.Projectname = "zagr prog 1";
                JSONsett.Resources = new res[] { new res(0x200, "c:\\1\\res\\1.txt"), new res(0x400, "c:\\1\\res\\2.txt") };
               

                System.IO.StreamWriter setting = new System.IO.StreamWriter(configfile);  //(path + "\\config.json");
                setting.Write(JsonConvert.SerializeObject(JSONsett));
                setting.Close();
                
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 2 )
            {
                dataGridView1.Rows.RemoveAt(e.RowIndex);
                return;
            }
            openFileDialog1.ShowDialog();
            if (openFileDialog1.SafeFileName.Length == 0) return;
            DataGridViewButtonCell txtxCell = (DataGridViewButtonCell)dataGridView1.Rows[e.RowIndex].Cells[0];
            txtxCell.Value = openFileDialog1.SafeFileName;
            //MessageBox.Show(e.RowIndex.ToString());
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            for (int i = 0; i <= 100; i++)
            {
                toolStripProgressBar1.Value = i;
                Thread.Sleep(20);
            }
            
        }
    }

    public class settings
    {
        public string Projectname { get; set; }
        public res[] Resources { get; set; }
        
    }
    public class res //добавляемые ресурсы в img файл,такие как шрифты, картинки и прочий мусор
    {
        public string Filename { get; set; }
        public int Addres { get; set; }
        public res(int adr,string file)
        {
            Filename = file;
            Addres = adr;
        }
    }
}
