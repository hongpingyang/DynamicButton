using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DynamicButton
{
    public class MoveTabControl : TabControl
    {
        private int _preSelectedIndex = -1;
        private int _AfterSelectedPreIndex = -1;
        private int _ActivedIndex = -1;
        private Timer m_Timer;//计时器  
        private const int TotalProgress = 1;
        private double Progress = 0;

        public MoveTabControl()
        {
            this.SetStyle(
                       ControlStyles.UserPaint |                      // 控件将自行绘制，而不是通过操作系统来绘制  
                       ControlStyles.OptimizedDoubleBuffer |          // 该控件首先在缓冲区中绘制，而不是直接绘制到屏幕上，这样可以减少闪烁  
                       ControlStyles.AllPaintingInWmPaint |           // 控件将忽略 WM_ERASEBKGND 窗口消息以减少闪烁  
                       ControlStyles.ResizeRedraw |                   // 在调整控件大小时重绘控件  
                       ControlStyles.SupportsTransparentBackColor,    // 控件接受 alpha 组件小于 255 的 BackColor 以模拟透明  
                       true);                                         // 设置以上值为 true  
            this.UpdateStyles();

            _AfterSelectedPreIndex = 0;
            _preSelectedIndex = 0;
            m_Timer = new Timer();
            m_Timer.Interval = 50;
            m_Timer.Enabled = false;
            m_Timer.Tick += new EventHandler(aTimer_Tick);
        }

        private void aTimer_Tick(object sender, EventArgs e)
        {

            if (Progress > TotalProgress)
            {
                m_Timer.Enabled = false;
                m_Timer.Stop();
                Progress = 1;
                return;
            }
            Progress += 0.1;
            if (Progress > TotalProgress)
                Progress = 1;
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.Clear(Parent.BackColor);

            //绘制头部
            for (int i = 0; i < this.TabCount; i++)
            {
                Rectangle temp = this.GetTabRect(i);
                //e.Graphics.DrawLine(Pens.Red, new Point(temp.Left, temp.Bottom), new Point(temp.Left + temp.Width, temp.Bottom));
                //e.Graphics.FillRectangle(Brushes.Blue, this.GetTabRect(i));
                Brush textBrush = new SolidBrush(HelpClass.Background);
                e.Graphics.DrawString(this.TabPages[i].Text, this.Font, textBrush, this.GetTabRect(i), new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
                if (i < this.TabCount - 1)
                {
                    Pen pen = new Pen(HelpClass.BorderColor);
                    var Tabpage = this.GetTabRect(i);
                    var StartPoint = new Point(Tabpage.X + Tabpage.Width, Tabpage.Top + 2);
                    var EndPoint = new Point(Tabpage.X + Tabpage.Width, Tabpage.Bottom - 4);
                    e.Graphics.DrawLine(pen, StartPoint, EndPoint);
                }

                textBrush.Dispose();
            }

            //绘制底部线条
            var firstTab = this.GetTabRect(0);
            var endTab = this.GetTabRect(this.TabCount - 1);
            var y = firstTab.Bottom - 1;
            Brush BorderBush = new SolidBrush(HelpClass.BorderColor);
            e.Graphics.FillRectangle(BorderBush, firstTab.X, y, endTab.X + endTab.Width, 2);
            BorderBush.Dispose();
            var preSelectTabRect = this.GetTabRect(_AfterSelectedPreIndex < 0 ? this.SelectedIndex : _AfterSelectedPreIndex);
            var activedTabRect = this.GetTabRect(_ActivedIndex < 0 ? this.SelectedIndex : _ActivedIndex);

            if (Progress > TotalProgress)
                Progress = 1;

            //绘制动态选择线条
            var x = preSelectTabRect.X + (int)((activedTabRect.X - preSelectTabRect.X) * Progress);
            var width = preSelectTabRect.Width + (int)((activedTabRect.Width - preSelectTabRect.Width) * Progress);
            Brush DirectorBrush = new SolidBrush(Color.Red);//特殊颜色，就红色吧
            e.Graphics.FillRectangle(DirectorBrush, x, y, width, 2);
            DirectorBrush.Dispose();



        }

        protected override void OnSelectedIndexChanged(EventArgs e)
        {
            base.OnSelectedIndexChanged(e);
            //因为这个函数在OnPaint后执行.
            _AfterSelectedPreIndex = _preSelectedIndex;
            _ActivedIndex = this.SelectedIndex;
            Progress = 0;
            m_Timer.Enabled = true;
            m_Timer.Start();
        }

        protected override void OnCreateControl()
        {
            base.OnCreateControl();
            this.ControlAdded += delegate
            {
                Invalidate();
            };
            this.ControlRemoved += delegate
            {
                Invalidate();
            };
            this.Deselected += delegate {
                _preSelectedIndex = this.SelectedIndex;
            };
        }

    }
}
