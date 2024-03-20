using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;
using System.Threading;

namespace serialtest
{
    public partial class FormMain : Form
    {
        delegate void HandleInterfaceUpdateDelegate(string text);  //委托，此为重点，此委托可以指向任何传入一个字符参数，无返回的方          法
        HandleInterfaceUpdateDelegate interfaceUpdateHandle;
        public FormMain()           //类的构造器
        {
            InitializeComponent();
            interfaceUpdateHandle = new HandleInterfaceUpdateDelegate(UpdateTextBox);  //实例化委托对象
            serialPort1.ReceivedBytesThreshold = 3;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.MinimumSize = this.Size;
            this.MaximumSize = this.Size;
            this.MaximizeBox = false;
        }
        private bool CheckPortSetting()
        {
            if (cbxckh.Text.Trim() == "") return false;
            if (cbxbtl.Text.Trim() == "") return false;
            if (cbxdatabit.Text.Trim() == "") return false;
            if (cbxParity.Text.Trim() == "") return false;
            if (cbxstopbit.Text.Trim() == "") return false;
            return true;
        }
        private void btn_Open_Click(object sender, EventArgs e)
        {

            if (!serialPort1.IsOpen)
            {
                serialPort1.PortName = cbxckh.Text.Trim();
                serialPort1.BaudRate = Convert.ToInt32(cbxbtl.Text.Trim());
                float f = Convert.ToSingle(cbxstopbit.Text.Trim());
                if (f == 0)
                    serialPort1.StopBits = StopBits.None;
                else if (f == 1.5)
                    serialPort1.StopBits = StopBits.OnePointFive;
                else if (f == 1)
                    serialPort1.StopBits = StopBits.One;
                else if (f == 2)
                    serialPort1.StopBits = StopBits.Two;
                else
                    serialPort1.StopBits = StopBits.One;

                serialPort1.DataBits = Convert.ToInt32(cbxdatabit.Text.Trim());
                string s = cbxParity.Text.Trim();
                if (s.CompareTo("None") == 0)
                    serialPort1.Parity = Parity.None;
                else if (s.CompareTo("Odd") == 0)
                    serialPort1.Parity = Parity.Odd;
                else if (s.CompareTo("Even") == 0)
                    serialPort1.Parity = Parity.Even;
                else
                    serialPort1.Parity = Parity.None;
                try
                {
                    serialPort1.Open();     //打开串口
                    btn_Open.Text = "关闭串口";
                    cbxbtl.Enabled = false;
                    cbxckh.Enabled = false;
                    cbxdatabit.Enabled = false;
                    cbxParity.Enabled = false;
                    cbxstopbit.Enabled = false;
                }
                catch
                {
                    MessageBox.Show("串口打开失败！");
                }
            }
            else
            {
                serialPort1.Close();
                btn_Open.Text = "打开串口";
                cbxbtl.Enabled = true;
                cbxckh.Enabled = true;
                cbxdatabit.Enabled = true;
                cbxParity.Enabled = true;
                cbxstopbit.Enabled = true;
            }
        }
        private bool Send_Data()
        {
            if (rtb_send.Text.Trim() == "")  return false;
            return true;
        }
        private void btn_Senddata_Click(object sender, EventArgs e)  //发送数据
        {
            if (serialPort1.IsOpen)   //串口是否打开
            {
                if (Send_Data())
                    serialPort1.Write(rtb_send.Text.Trim());    //写数据
                else
                    MessageBox.Show("发送数据失败！");
            }
            else
                MessageBox.Show("请打开串口！");
        }
        private void btn_Exit_Click(object sender, EventArgs e)
        {
            serialPort1.Close();
            this.Close();
           
        }

        private void serialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)  //接收数据
        {
            //MessageBox.Show(serialPort1.ReadBufferSize.ToString());
            Thread.Sleep(5); 
            //一定要加延时，否则出现乱码错误
            if (serialPort1.BytesToRead != 0)
            {
               byte[] readBuffer = new byte[serialPort1.BytesToRead];// serialPort1.ReadBufferSize获取或设置 SerialPort 输入缓冲区的大小
                                                                  //BytesToRead获取接收缓冲区中数据的字节数
                serialPort1.Read(readBuffer, 0, readBuffer.Length);
                                                           //public int Read (byte[] buffer,int offset,int count);
                this.Invoke(interfaceUpdateHandle, new string[] { Encoding.ASCII.GetString(readBuffer) });
 
            }
            
                                                        /*利用Encoding类可以把字符串与字节数组互相转换
                                                       如：
                                                        //用Ascii编码将xxxByteArray字节数组转化为字串ss
                                                        string ss = Encoding.ASCII.GetString(xxxByteArray);
                                                        //用系统默认编码将字串ss转换为字节数组test
                                                        byte[] test=Encoding.Default.GetBytes(ss);*/

                                                        /*public Object Invoke (
                                                            Delegate method,
                                                           params Object[] args
                                                            )
                                                         */
           // MessageBox.Show(readBuffer.Length.ToString());
            //serialPort1.DiscardInBuffer();
           

        }

        private  void UpdateTextBox(string text)
        {
            
            rtbReceive.Text = text;
           // rtbReceive.Text += "\n";

        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            rtbReceive.Clear();
        }
        private void frm_Closed(object sender, System.EventArgs e)
        {
            this.Show();
        }
        public void btn_display_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            FormDisplay frmExercise1 = new FormDisplay();
            frmExercise1.Owner = this;
            frmExercise1.Closed += new EventHandler(frm_Closed);
            frmExercise1.Show();
            //this.Hide();      //可以决定是否隐藏主窗口
            this.Cursor = Cursors.Default;
            //if (serialPort1.IsOpen)
             btn_display.Enabled = false;
           // else
             //   btn_display.Enabled = true;
        }
    }
}
