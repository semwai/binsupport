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
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Diagnostics;

namespace binsupport
{
    public partial class Form1 : Form
    {
        string path = @"C:\Users\Semwai\Desktop\1";
        public Form1()
        {
            InitializeComponent();
        }

        private void создатьПроектToolStripMenuItem_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.ShowDialog();
            path = folderBrowserDialog1.SelectedPath;
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
                System.IO.File.Create(path + "\\Res\\boot.bin").Close();
                System.IO.File.Create(path + "\\Res\\start.bin").Close();
                var boottablefile = System.IO.File.Create(path + "\\Res\\boottable.bin");
                boottable mytable = new boottable();
                mytable.setLBA(0, 2048 * 1024); //1mb
                boottablefile.Write(ObjectToByteArray(mytable),0,16);
                boottablefile.Close();
                var _55AA = System.IO.File.Create(path + "\\Res\\55AA.bin");
                _55AA.WriteByte(0x55);
                _55AA.WriteByte(0xAA);
                _55AA.Close();

                dataGridView1.Rows.Add("boot.bin", 0);
                dataGridView1.Rows.Add("boottable.bin", 446);
                dataGridView1.Rows.Add("55AA.bin", 510);
                dataGridView1.Rows.Add("start.bin", 512);


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
            try
            {
                if (e.ColumnIndex == 2)
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
            catch (Exception) { }
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            for (int i = 0; i <= 100; i++)
            {
                toolStripProgressBar1.Value = i;
                Thread.Sleep(20);
            }
            
        }


        public byte[] ObjectToByteArray(object obj)
        {
            if (obj == null)
                return null;
            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream())
            {
                bf.Serialize(ms, obj);
                return ms.ToArray();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Process Proc = new Process();
            Proc.StartInfo.FileName = "explorer";
            Proc.StartInfo.Arguments = path;
            Proc.Start();
        }

        private void button2_Click(object sender, EventArgs e)
        {

            IEnumerable<string> files =  System.IO.Directory.EnumerateFiles(path+"\\C\\");
            string strCmdText = "";
            foreach (var f in files)
            {
                // MessageBox.Show(f);
                strCmdText += f + " ";
            }
          
            strCmdText+= "-nostdlib -masm=intel -o " + path + "\\Res\\start.bin";
            
            System.Diagnostics.Process.Start("gcc.exe", strCmdText);
            Thread.Sleep(1000);
           
            strCmdText = "-f bin " + path + "\\Asm\\boot.asm -o " + path + "\\res\\boot.bin";
           // MessageBox.Show(strCmdText);
            System.Diagnostics.Process.Start("nasm.exe", strCmdText);
            Thread.Sleep(1000);

            strCmdText = "-O binary "+path+"\\Res\\start.bin";
            System.Diagnostics.Process.Start("objcopy.exe", strCmdText);

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
    [Serializable]
    public class boottable
    {

        public byte mask { get; set; }
        public byte gol { get; set; }
        public byte sec { get; set; }
        public byte cil { get; set; }
        public byte type { get; set; }
        public byte gol_end { get; set; }
        public byte sec_end { get; set; }
        public byte cin_end { get; set; }
        public Int32 LBAaddr { get; set; }
        public Int32 LBAcount { get; set; }
        public void setLBA(int lbaaddr,int lbacount)
        {
            gol = 0;
            sec = 0;
            cil = 0;
            gol_end = 0;
            cin_end = 0;
            sec_end = 0;
            LBAaddr = lbaaddr;
            LBAcount = lbacount;
            mask = 0x80; 
            type = 0x7f; //учебный раздел
        }

    }
    
}
