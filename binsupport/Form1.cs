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
        private void EnableInterface()
        {
            button1.Enabled = true;
            button2.Enabled = true;
            dataGridView1.Enabled = true;
        }
        private void создатьПроектToolStripMenuItem_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.ShowDialog();
            path = folderBrowserDialog1.SelectedPath;
             
            // MessageBox.Show(folderBrowserDialog1.SelectedPath);return;
            if (path.Length == 0)
            {
                return;
            }
            /* if (System.IO.File.Exists(path + "\\config.json"))
            {
                MessageBox.Show("В данной папке уже существует проект, выберите другую папку или удалите старый проект.");
            }*/
            EnableInterface();
            System.IO.Directory.CreateDirectory(path + "\\C");
            System.IO.Directory.CreateDirectory(path + "\\C\\temp");
            System.IO.Directory.CreateDirectory(path + "\\Asm");
            System.IO.Directory.CreateDirectory(path + "\\Res");
            var configfile = System.IO.File.Create(path + "\\config.json");  
            System.IO.File.Open(path + "\\C\\start.c",FileMode.OpenOrCreate).Close();
            var bottasm = System.IO.File.Open(path + "\\Asm\\boot.asm", FileMode.OpenOrCreate);
            var bootbin = System.IO.File.Open(path + "\\Res\\boot.bin", FileMode.OpenOrCreate);
            var startbin = System.IO.File.Open(path + "\\Res\\start.bin", FileMode.OpenOrCreate);
            var boottablefile = System.IO.File.Create(path + "\\Res\\boottable.bin");
            boottable mytable = new boottable();
            mytable.setLBA(0, 2048 * 1024); //1mb
            boottablefile.Write(ObjectToByteArray(mytable),0,16);
            
            var _55AA = System.IO.File.Create(path + "\\Res\\55AA.bin");
            _55AA.WriteByte(0x55);
            _55AA.WriteByte(0xAA);


            dataGridView1.Rows.Add(path + "\\Res\\boot.bin", "0");//,          Int32.Parse(dataGridView1.Rows[0].Cells[1].Value.ToString()) +  bootbin.Length);
            dataGridView1.Rows.Add(path + "\\Res\\boottable.bin", "446");//, Int32.Parse(dataGridView1.Rows[1].Cells[1].Value.ToString()) + boottablefile.Length);
            dataGridView1.Rows.Add(path + "\\Res\\55AA.bin", "510");//,      Int32.Parse(dataGridView1.Rows[2].Cells[1].Value.ToString()) + _55AA.Length);
            dataGridView1.Rows.Add(path + "\\Res\\start.bin", "512");//,     Int32.Parse(dataGridView1.Rows[3].Cells[1].Value.ToString()) + startbin.Length);
            boottablefile.Close();
            startbin.Close();
            bootbin.Close();
            boottablefile.Close();
            bottasm.Close();
            _55AA.Close();
            settings JSONsett = new settings(2);
            JSONsett.Projectname = "zagr prog 1";
            JSONsett.Count = 2;
            JSONsett.Resources = new res[] { new res(0x200, "c:\\1\\res\\1.txt"), new res(0x400, "c:\\1\\res\\2.txt") };
               

            System.IO.StreamWriter setting = new System.IO.StreamWriter(configfile);  //(path + "\\config.json");
           // setting.Write(JsonConvert.SerializeObject(JSONsett));
            setting.Close();
                
            
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1) return;
            // MessageBox.Show(e.ColumnIndex.ToString());
             
            try
            {
                if (e.ColumnIndex == 3)
                {
                    dataGridView1.Rows.RemoveAt(e.RowIndex);
                    return;
                }
                openFileDialog1.ShowDialog();
                if (openFileDialog1.SafeFileName.Length == 0) return;
                DataGridViewButtonCell txtxCell = (DataGridViewButtonCell)dataGridView1.Rows[e.RowIndex].Cells[0];
                txtxCell.Value = openFileDialog1.FileName;
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

            using (var destStream = File.Create(path+"\\C\\temp\\all.c"))
            {
                foreach (var file in files)
                {
                    using (var srcStream = File.OpenRead(file)) srcStream.CopyTo(destStream);
                }
            }


           /* foreach (var f in files)
            {
                // MessageBox.Show(f);
                strCmdText += f + " ";
            }*/
          
            strCmdText+= "-nostdlib -masm=intel "+path+"\\C\\temp\\all.c -o " + path + "\\Res\\start.bin";
            //strCmdText = " -c  -w -masm=intel "+path+"\\C\\*.c";
            //MessageBox.Show("gcc.exe"+strCmdText);
            var gcc1 = System.Diagnostics.Process.Start("gcc.exe", strCmdText);
            
            Thread.Sleep(1000);


            //strCmdText = " -o " + path + "\\Res\\start.bin" +
            //    "-nostdlib " + path + "\\C\\*.o";
           // System.Diagnostics.Process.Start("gcc.exe", strCmdText);
            Thread.Sleep(1000);


            strCmdText = "-f bin " + path + "\\Asm\\boot.asm -o " + path + "\\res\\boot.bin";
           // MessageBox.Show(strCmdText);
            System.Diagnostics.Process.Start("nasm.exe", strCmdText);
            Thread.Sleep(3000);

            strCmdText = " -O binary "+path+"\\Res\\start.bin";
            //MessageBox.Show(strCmdText);
            System.Diagnostics.Process.Start("objcopy.exe", strCmdText).WaitForExit(3000);


            var IMGout = System.IO.File.Create(path + "\\disk.img");
            

           for (int i=0; i< dataGridView1.Rows.Count-1; i++)
            {
                DataGridViewButtonCell fileButton = (DataGridViewButtonCell)dataGridView1.Rows[i].Cells[0];
                DataGridViewTextBoxCell fileOffset = (DataGridViewTextBoxCell)dataGridView1.Rows[i].Cells[1];
                //MessageBox.Show(fileButton.Value.ToString()+" "+ fileOffset.Value.ToString());
               
                try
                {
                    var file = System.IO.File.Open(dataGridView1.Rows[i].Cells[0].Value.ToString(), FileMode.Open);

                    byte[] array = new byte[file.Length];
                    file.Read(array, 0, (int)file.Length);
                    IMGout.Position = Int32.Parse(dataGridView1.Rows[i].Cells[1].Value.ToString());
                    IMGout.Write(array, 0, (int)file.Length);
                    file.Close();
                }
                catch (Exception) { }
               
            }

            IMGout.Close();
        }

        private void dataGridView1_Click(object sender, EventArgs e)
        {
           // dataGridView1.Sort(dataGridView1.Columns[1], ListSortDirection.Ascending);
            for (int i = 0; i < dataGridView1.RowCount - 1; i++)
            {
                try
                {
                    var file = System.IO.File.Open(dataGridView1.Rows[i].Cells[0].Value.ToString(), FileMode.Open);

                    dataGridView1.Rows[i].Cells[2].Value = file.Length + Int32.Parse(dataGridView1.Rows[i].Cells[1].Value.ToString());

                    file.Close();
                }
                catch (Exception) { }
            }
        }

        private void сохранитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
           // MessageBox.Show(dataGridView1.Rows.Count.ToString());
            var config = System.IO.File.Open(path + "\\config.json", FileMode.Create);

            settings JSONconf = new settings(dataGridView1.Rows.Count-1);
             
            //JSONconf.Resources[1].Filename = "aaa";
            JSONconf.Projectname = "Tardigrada";
            JSONconf.Count = dataGridView1.Rows.Count-1;
            for (int i = 0; i < dataGridView1.Rows.Count-1; i++)
            {
                JSONconf.Resources[i] = new res(0,"");
                DataGridViewButtonCell  fileButton = (DataGridViewButtonCell)dataGridView1.Rows[i].Cells[0];
                DataGridViewTextBoxCell offset = (DataGridViewTextBoxCell)dataGridView1.Rows[i].Cells[1];
               // MessageBox.Show(fileButton.Value.ToString());
                JSONconf.Resources[i].Filename = fileButton.Value.ToString();// dataGridView1.Rows[i].Cells[0].Value.ToString();
                JSONconf.Resources[i].Addres =  Int32.Parse(offset.Value.ToString());// Int32.Parse(dataGridView1.Rows[i].Cells[1].Value.ToString());
            }

           // MessageBox.Show(JsonConvert.SerializeObject(JSONconf));
            System.IO.StreamWriter setting = new System.IO.StreamWriter(config);  //(path + "\\config.json");
            setting.Write(JsonConvert.SerializeObject(JSONconf));
            setting.Close();
            config.Close();
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.S)
                сохранитьToolStripMenuItem_Click(null, null);
        }

        private void загрузитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.ShowDialog();
            path = folderBrowserDialog1.SelectedPath;
            var JSONfile = System.IO.File.OpenRead(path + "\\config.json");
           // System.IO.StreamWriter setting = new System.IO.StreamWriter(JSONfile);
            byte[] byteText = new byte[JSONfile.Length];
            JSONfile.Read(byteText, 0, (int)JSONfile.Length);
            string text = System.Text.Encoding.Default.GetString(byteText);
            settings project = JsonConvert.DeserializeObject<settings>(text);

            MessageBox.Show(project.Projectname);
            for (int i = 0; i < project.Count; i++)
            {
                dataGridView1.Rows.Add(project.Resources[i].Filename, project.Resources[i].Addres);
            }
            EnableInterface();
        }
    }

    public class settings
    {
        public string Projectname { get; set; }
        public int Count { get; set; }
        public res[] Resources { get; set; }
        
        public settings(int n)
        {
            Resources = new res[n];
        }
        
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
