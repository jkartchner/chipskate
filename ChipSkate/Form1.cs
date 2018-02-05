using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ChipSkate
{
    public partial class Form1 : Form
    {
        chip8 Chip8;
        public Form1()
        {            
            InitializeComponent();
            Chip8 = new chip8();
            
        }

        private void loadROMToolStripMenuItem_Click(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            if(DialogResult.OK == openFileDialog1.ShowDialog())
            {
                Chip8.LoadGame(openFileDialog1.FileName);
                timer1.Enabled = true;
            }
        }

        /// <summary>
        /// Runs the emulator cycle until closing
        /// </summary>
        private void RunEmulator(int noCycles)
        {
            //for(int i = 0; i < 30; i++)   // no longer need a for loop or any kind of loop--the timer will serve instead
            {
                Chip8.drawflag = false;
                for(int i = 0; i < noCycles; i++)
                    Chip8.cycleCPU();

                if (Chip8.drawflag)
                    this.Refresh();

                //SetKeys();
            }
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            Rectangle rect = new Rectangle(29, 39, 385, 193);
            SolidBrush sBrush = new SolidBrush(Color.CornflowerBlue);
            SolidBrush sBrush2 = new SolidBrush(Color.DarkGreen);
            SolidBrush sBrush3 = new SolidBrush(Color.White);
            Pen pen = new Pen(sBrush);
            //Pen pen2 = new Pen(sBrush3);
            //Pen pen3 = new Pen(sBrush);
            g.DrawRectangle(pen, rect);

            for (int yline = 0; yline < 32; yline++)
            {                
                for (int xline = 0; xline < 64; xline++)
                {                    
                    if(Chip8.gfx[yline * 64 + xline] == 0)
                        g.FillRectangle(sBrush2, new Rectangle(30 + xline * 6, 40 + yline * 6, 6, 6));
                    else
                        g.FillRectangle(sBrush3, new Rectangle(30 + xline * 6, 40 + yline * 6, 6, 6));
                }
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if(!Chip8.keywaitflag)
                RunEmulator(4);
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (Chip8.keywaitflag)
                Chip8.Voffset = Chip8.whichkey;
            for (int i = 0; i < Chip8.key.Length; i++)
                Chip8.key[i] = 0;
            switch (e.KeyValue)
            {
                case 49:
                    Chip8.key[0] = 1;
                    Chip8.whichkey = 0x00;
                    break;
                case 50:
                    Chip8.key[1] = 1;
                    Chip8.whichkey = 0x01;
                    break;
                case 51:
                    Chip8.key[2] = 1;
                    Chip8.whichkey = 2;
                    break;
                case 52:
                    Chip8.key[3] = 1;
                    Chip8.whichkey = 3;
                    break;
                case 81:
                    Chip8.key[4] = 1;
                    Chip8.whichkey = 4;
                    break;
                case 87:
                    Chip8.key[5] = 1;
                    Chip8.whichkey = 5;
                    break;
                case 69:
                    Chip8.key[6] = 1;
                    Chip8.whichkey = 6;
                    break;
                case 82:
                    Chip8.key[7] = 1;
                    Chip8.whichkey = 7;
                    break;
                case 65:
                    Chip8.key[8] = 1;
                    Chip8.whichkey = 8;
                    break;
                case 83:
                    Chip8.key[9] = 1;
                    Chip8.whichkey = 9;
                    break;
                case 68:
                    Chip8.key[10] = 1;
                    Chip8.whichkey = 10;
                    break;
                case 70:
                    Chip8.key[11] = 1;
                    Chip8.whichkey = 11;
                    break;
                case 90:
                    Chip8.key[12] = 1;
                    Chip8.whichkey = 12;
                    break;
                case 88:
                    Chip8.key[13] = 1;
                    Chip8.whichkey = 13;
                    break;
                case 67:
                    Chip8.key[14] = 1;
                    Chip8.whichkey = 14;
                    break;
                case 86:
                    Chip8.key[15] = 1;
                    Chip8.whichkey = 15;
                    break;

                default:
                    e.Handled = true;
                    break;
            }
            if (Chip8.keywaitflag)
                Chip8.keyWaitExecution();
            Chip8.keywaitflag = false;
            //System.Windows.Forms.MessageBox.Show("KEY DOWN EVENT");
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyValue)
            {
                case 49:
                    Chip8.key[0] = 0;
                    break;
                case 50:
                    Chip8.key[1] = 0;
                    break;
                case 51:
                    Chip8.key[2] = 0;
                    break;
                case 52:
                    Chip8.key[3] = 0;
                    break;
                case 81:
                    Chip8.key[4] = 0;
                    break;
                case 87:
                    Chip8.key[5] = 0;
                    break;
                case 69:
                    Chip8.key[6] = 0;
                    break;
                case 82:
                    Chip8.key[7] = 0;
                    break;
                case 65:
                    Chip8.key[8] = 0;
                    break;
                case 83:
                    Chip8.key[9] = 0;
                    break;
                case 68:
                    Chip8.key[10] = 0;
                    break;
                case 70:
                    Chip8.key[11] = 0;
                    break;
                case 90:
                    Chip8.key[12] = 0;
                    break;
                case 88:
                    Chip8.key[13] = 0;
                    break;
                case 67:
                    Chip8.key[14] = 0;
                    break;
                case 86:
                    Chip8.key[15] = 0;
                    break;

                default:
                    e.Handled = true;
                    break;
            }
        }

        /*        public void SetKeys()
        {
            if (NativeMethods.Is1KeyDown())
            {
                Chip8.key[0] = 1;
                Chip8.whichkey = 0;
            }
            else
                Chip8.key[0] = 0;
            if (NativeMethods.Is2KeyDown())
            {
                Chip8.key[1] = 1;
                Chip8.whichkey = 1;
            }
            else
                Chip8.key[1] = 0;
            if (NativeMethods.Is3KeyDown())
            {
                Chip8.key[2] = 1;
                Chip8.whichkey = 2;
            }
            else
                Chip8.key[2] = 0;
            if (NativeMethods.Is4KeyDown())
            {
                Chip8.key[3] = 1;
                Chip8.whichkey = 3;
            }
            else
                Chip8.key[3] = 0;
            if (NativeMethods.IsqKeyDown())
            {
                Chip8.key[4] = 1;
                Chip8.whichkey = 4;
            }
            else
                Chip8.key[4] = 0;
            if (NativeMethods.IswKeyDown())
            {
                Chip8.key[5] = 1;
                Chip8.whichkey = 5;
            }
            else
                Chip8.key[5] = 0;
            if (NativeMethods.IseKeyDown())
            {
                Chip8.key[6] = 1;
                Chip8.whichkey = 6;
            }
            else
                Chip8.key[6] = 0;
            if (NativeMethods.IsrKeyDown())
            {
                Chip8.key[7] = 1;
                Chip8.whichkey = 7;
            }
            else
                Chip8.key[7] = 0;
            if (NativeMethods.IsaKeyDown())
            {
                Chip8.key[8] = 1;
                Chip8.whichkey = 8;
            }
            else
                Chip8.key[8] = 0;
            if (NativeMethods.IssKeyDown())
            {
                Chip8.key[9] = 1;
                Chip8.whichkey = 9;
            }
            else
                Chip8.key[9] = 0;
            if (NativeMethods.IsdKeyDown())
            {
                Chip8.key[10] = 1;
                Chip8.whichkey = 10;
            }
            else
                Chip8.key[10] = 0;
            if (NativeMethods.IsfKeyDown())
            {
                Chip8.key[11] = 1;
                Chip8.whichkey = 11;
            }
            else
                Chip8.key[11] = 0;
            if (NativeMethods.IszKeyDown())
            {
                Chip8.key[12] = 1;
                Chip8.whichkey = 12;
            }
            else
                Chip8.key[12] = 0;
            if (NativeMethods.IsxKeyDown())
            {
                Chip8.key[13] = 1;
                Chip8.whichkey = 13;
            }
            else
                Chip8.key[13] = 0;
            if (NativeMethods.IscKeyDown())
            {
                Chip8.key[14] = 1;
                Chip8.whichkey = 14;
            }
            else
                Chip8.key[14] = 0;
            if (NativeMethods.IsvKeyDown())
            {
                Chip8.key[15] = 1;
                Chip8.whichkey = 15;
            }
            else
                Chip8.key[15] = 0;
        }

        static class NativeMethods
        {
            public static bool Is1KeyDown()
            {
                return (GetKeyState(VK_1) & KEY_PRESSED) != 0;
            }
            public static bool Is2KeyDown()
            {
                return (GetKeyState(VK_2) & KEY_PRESSED) != 0;
            }
            public static bool Is3KeyDown()
            {
                return (GetKeyState(VK_3) & KEY_PRESSED) != 0;
            }
            public static bool Is4KeyDown()
            {
                return (GetKeyState(VK_4) & KEY_PRESSED) != 0;
            }
            public static bool IsqKeyDown()
            {
                return (GetKeyState(VK_q) & KEY_PRESSED) != 0;
            }
            public static bool IswKeyDown()
            {
                return (GetKeyState(VK_w) & KEY_PRESSED) != 0;
            }
            public static bool IseKeyDown()
            {
                return (GetKeyState(VK_e) & KEY_PRESSED) != 0;
            }
            public static bool IsrKeyDown()
            {
                return (GetKeyState(VK_r) & KEY_PRESSED) != 0;
            }
            public static bool IsaKeyDown()
            {
                return (GetKeyState(VK_a) & KEY_PRESSED) != 0;
            }
            public static bool IssKeyDown()
            {
                return (GetKeyState(VK_s) & KEY_PRESSED) != 0;
            }
            public static bool IsdKeyDown()
            {
                return (GetKeyState(VK_d) & KEY_PRESSED) != 0;
            }
            public static bool IsfKeyDown()
            {
                return (GetKeyState(VK_f) & KEY_PRESSED) != 0;
            }
            public static bool IszKeyDown()
            {
                return (GetKeyState(VK_z) & KEY_PRESSED) != 0;
            }
            public static bool IsxKeyDown()
            {
                return (GetKeyState(VK_x) & KEY_PRESSED) != 0;
            }
            public static bool IscKeyDown()
            {
                return (GetKeyState(VK_c) & KEY_PRESSED) != 0;
            }
            public static bool IsvKeyDown()
            {
                return (GetKeyState(VK_v) & KEY_PRESSED) != 0;
            }
            private const int KEY_PRESSED = 0x8000;
            private const int VK_1 = 0x31;
            private const int VK_2 = 50;
            private const int VK_3 = 51;
            private const int VK_4 = 52;
            private const int VK_q = 113;
            private const int VK_w = 119;
            private const int VK_e = 101;
            private const int VK_r = 114;
            private const int VK_a = 97;
            private const int VK_s = 115;
            private const int VK_d = 100;
            private const int VK_f = 102;
            private const int VK_z = 122;
            private const int VK_x = 120;
            private const int VK_c = 99;
            private const int VK_v = 18;
            [System.Runtime.InteropServices.DllImport("user32.dll")]
                static extern short GetKeyState(int key);
        }*/
    }
}
