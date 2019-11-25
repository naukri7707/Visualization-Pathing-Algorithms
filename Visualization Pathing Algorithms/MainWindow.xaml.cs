using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace VisualizationPathingAlgorithms
{
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MainWindow : Window
    {
        Map map;
        DispatcherTimer tmrNodeInfo;

        public MainWindow()
        {
            tmrNodeInfo = new DispatcherTimer();
            tmrNodeInfo.Interval = TimeSpan.FromMilliseconds(100);
            tmrNodeInfo.Tick += tmrNodeInfo_Tick;
            tmrNodeInfo.Start();
            //
            InitializeComponent();
            //
            InputMethod.SetIsInputMethodEnabled(winMain, false);
            btnSetMap_Click(null, null);
            btnSetDelayTime_Click(null, null);
            //
        }

        void tmrNodeInfo_Tick(object sender, EventArgs e)
        {
            int space = 0, obstacle = 0, open = 0, close = 0;
            for (int i = 0; i < map.Size; i++)
            {
                for (int j = 0; j < map.Size; j++)
                {
                    if (map[i, j].Rect.Fill == Map.clr_Space) space++;
                    else if (map[i, j].Rect.Fill == Map.clr_Obstacle) obstacle++;
                    else if (map[i, j].Rect.Fill == Map.clr_IsOpen) open++;
                    else close++;
                }
            }
            lblSpace.Content = "Space = " + space.ToString();
            lblObstacle.Content = "Obstacle = " + obstacle.ToString();
            lblOpen.Content = "Open = " + open.ToString();
            lblClose.Content = "Close = " + close.ToString();
        }

        private void winMain_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.LeftShift:
                    map.prevRect.Fill = map.nowColor = new SolidColorBrush();
                    break;
                case Key.Z:
                    map.prevRect.Fill = map.nowColor = Map.clr_Obstacle;
                    break;
                case Key.X:
                    map.prevRect.Fill = map.nowColor = Map.clr_Start;
                    break;
                case Key.C:
                    map.prevRect.Fill = map.nowColor = Map.clr_End;
                    break;
                default:
                    break;
            }
        }

        public void ResetMap(int Size)
        {
            cvsMain.Children.Clear();
            map = new Map(Size, cvsMain.Height);
            foreach (Info i in map.Index)
                cvsMain.Children.Add(i.Rect);
            cvsMain.Children.Add(map.prevRect);
        }

        private void btnSetMap_Click(object sender, RoutedEventArgs e)
        {
            int size;
            if (!int.TryParse(txtSize.Text, out size))
            {
                MessageBox.Show("請輸入正整數");
            }
            ResetMap(size);
        }


        private void cvsMain_MouseMove(object sender, MouseEventArgs e)
        {
            Point pos = e.GetPosition(sender as IInputElement);
            PointInt mPos = new PointInt((int)(pos.X / map.RectSize), (int)(pos.Y / map.RectSize));
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (map.prevRect.Fill == Map.clr_Start)
                {
                    map[map.Start].Rect.Fill = Map.clr_Space;
                    map.Start = mPos;
                    map[mPos].Rect.Fill = map.prevRect.Fill;
                }
                else if (map.prevRect.Fill == Map.clr_End)
                {
                    map[map.End].Rect.Fill = Map.clr_Space;
                    map.End = mPos;
                    map[mPos].Rect.Fill = map.prevRect.Fill;
                }
                else if (map.prevRect.Fill == Map.clr_Obstacle)
                {
                    map[mPos].Rect.Fill = map.prevRect.Fill;
                }
            }
            else if (e.RightButton == MouseButtonState.Pressed)
            {
                map[mPos].Rect.Fill = Map.clr_Space;
            }
            else
            {
                map.prevRect.SetValue(Canvas.LeftProperty, mPos.X * map.RectSize);
                map.prevRect.SetValue(Canvas.TopProperty, mPos.Y * map.RectSize);
            }
            //
            lblPos.Content = "[ " + mPos.X.ToString() + " , " + mPos.Y.ToString() + " ]";
            lblFrom.Content = "From = [ " + map[mPos].From.X.ToString() + " , " + map[mPos].From.Y.ToString() + " ]";
            lblStep.Content = "Step = " + map[mPos].Step.ToString();
            lblG.Content = "G = " + map[mPos].G.ToString();
            lblH.Content = "H = " + map[mPos].H.ToString();
            lblF.Content = "F = " + map[mPos].F.ToString();
        }

        private void cvsMain_MouseDown(object sender, MouseButtonEventArgs e)
        {
            cvsMain_MouseMove(sender, e);
        }

        private void cvsMain_MouseUp(object sender, MouseButtonEventArgs e)
        {
            cvsMain_MouseMove(sender, e);
        }

        private void cvsMain_MouseEnter(object sender, MouseEventArgs e)
        {
            map.prevRect.Fill = map.nowColor;
            //
            SetInfoVisibility(cboAlgorithmSelector.SelectedIndex);
        }

        private void cvsMain_MouseLeave(object sender, MouseEventArgs e)
        {
            map.prevRect.Fill = new SolidColorBrush();
            //
            SetInfoVisibility(-1);
        }



        private void btnSetDelayTime_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(txtDelayTime.Text, out DelayTime))
            {
                MessageBox.Show("請輸入正整數");
            }
        }

        private async void btnStart_Click(object sender, RoutedEventArgs e)
        {
            if (!map.isClear) return;
            btnSetDelayTime_Click(null, null);
            map.isClear = false;
            map.nowColor = new SolidColorBrush();
            //
            switch (cboAlgorithmSelector.SelectedIndex)
            {
                case 0:
                    await Dijkstra();
                    break;
                case 1:
                    await QuickPath();
                    break;
                case 2:
                    await Astar();
                    break;
            }

        }

        private void btnClearMap_Click(object sender, RoutedEventArgs e)
        {
            map.ClearMap();
        }

        private void cboAlgorithmSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lblPos == null) return;
            map.ClearMap();
        }

        void SetInfoVisibility(int index)
        {
            switch (index)
            {
                case 0:
                    lblPos.Visibility = lblStep.Visibility = lblFrom.Visibility = lblG.Visibility = Visibility.Visible;
                    lblH.Visibility = lblF.Visibility = Visibility.Hidden;
                    break;
                case 1:
                    lblPos.Visibility = lblStep.Visibility = lblFrom.Visibility = lblG.Visibility = lblH.Visibility = Visibility.Visible;
                    lblF.Visibility = Visibility.Hidden;
                    break;
                case 2:
                    lblPos.Visibility = lblStep.Visibility = lblFrom.Visibility = lblG.Visibility = lblH.Visibility = lblF.Visibility = Visibility.Visible;
                    break;
                default:
                    lblPos.Visibility = lblStep.Visibility = lblFrom.Visibility = lblG.Visibility = lblH.Visibility = lblF.Visibility = Visibility.Hidden;
                    break;
            }
        }

        private void btnMapIn_Click(object sender, RoutedEventArgs e)
        {
            MapIO io = new MapIO();
            if (io.MapIn(cvsMain.Height, ref map))
            {
                cvsMain.Children.Clear();
                foreach (Info i in map.Index)
                    cvsMain.Children.Add(i.Rect);
                cvsMain.Children.Add(map.prevRect);
            }
        }

        private void btnMapOut_Click(object sender, RoutedEventArgs e)
        {
            MapIO io = new MapIO();
            io.MapOut(map);
        }

        private void cbNodeStatistics_Checked(object sender, RoutedEventArgs e)
        {
            if (girdNodesStatistics == null) return;
            girdNodesStatistics.Visibility = Visibility.Visible;
            tmrNodeInfo.IsEnabled = true;
        }

        private void cbNodeStatistics_Unchecked(object sender, RoutedEventArgs e)
        {
            if (girdNodesStatistics == null) return;
            girdNodesStatistics.Visibility = Visibility.Hidden;
            tmrNodeInfo.IsEnabled = false;
        }
    }
}
