using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Shapes;

namespace VisualizationPathingAlgorithms
{
    public partial class MainWindow
    {
        int DelayTime = 100;
        Queue<PointInt> queueDij = new Queue<PointInt>();
        PointInt now;

        static async System.Threading.Tasks.Task Delay(int ms)
        {
            await System.Threading.Tasks.Task.Delay(ms);
        }

        private async Task DrawPathAsync(PointInt from, PointInt to)
        {
            int g, gap = Math.Abs(map[from].G - map[to].G);

            int gapA = Map.clr_End.Color.A - Map.clr_Start.Color.A,
                gapR = Map.clr_End.Color.R - Map.clr_Start.Color.R,
                gapG = Map.clr_End.Color.G - Map.clr_Start.Color.G,
                gapB = Map.clr_End.Color.B - Map.clr_Start.Color.B;
            if (gap == 0) gap = 1;
            while (from != to)
            {
                g = map[from].G;
                map[from].Rect.Fill = new System.Windows.Media.SolidColorBrush(
                    System.Windows.Media.Color.FromArgb(
                       Convert.ToByte(Map.clr_Start.Color.A + g * gapA / gap),
                       Convert.ToByte(Map.clr_Start.Color.R + g * gapR / gap),
                       Convert.ToByte(Map.clr_Start.Color.G + g * gapG / gap),
                       Convert.ToByte(Map.clr_Start.Color.B + g * gapB / gap)
                       )
                       );
                from = map[from].From;
                await Delay(DelayTime);
            }
        }

        bool JudgeDij(PointInt pos, int dx, int dy)
        {
            PointInt next = new PointInt(pos.X + dx, pos.Y + dy);
            if (map.IsInRange(next))
            {
                if (map[next].Rect.Fill == Map.clr_Space)
                {
                    map[next].Step = map[now].Step + 1;
                    map[next].Rect.Fill = Map.clr_IsOpen;
                    map[next].From = pos;
                    map[next].G = map[pos].G + extraGap(dx, dy);
                    queueDij.Enqueue(next);
                }
                else if (map[next].Rect.Fill == Map.clr_End)
                {
                    map[next].Step = map[now].Step + 1;
                    map[next].From = pos;
                    map[next].G = map[pos].G + extraGap(dx, dy);
                    return false;
                }
            }
            return true;
        }

        public async Task Dijkstra()
        {
            queueDij = new Queue<PointInt>();
            now = map.Start;
            bool isLink = true;
            //
            while (JudgeDij(now, -1, 0) && JudgeDij(now, 0, -1) && JudgeDij(now, 1, 0) && JudgeDij(now, 0, 1)
             && JudgeDij(now, -1, 1) && JudgeDij(now, -1, -1) && JudgeDij(now, 1, -1) && JudgeDij(now, 1, 1))
            {
                if (queueDij.Count == 0)
                {
                    isLink = false;
                    break;
                }
                now = queueDij.Dequeue();
                await Delay(DelayTime);
                map[now].Rect.Fill = Map.clr_IsClose;
                await Delay(DelayTime);
            }
            now = map.End;
            if (isLink) await DrawPathAsync(map.End, map.Start);
        }


        //


        Heap HeapAs;
        PointInt Best;

        private int extraGap(int dx, int dy)
        {
            return Math.Abs(dx + dy) == 1 ? 10 : 14;
        }

        private int H(PointInt from, PointInt to)
        {
            int x = Math.Abs(from.X - to.X), y = Math.Abs(from.Y - to.Y);
            return Math.Abs(x - y) * 10 + (x < y ? x : y) * 14;
        }

        async Task JudgeAs(PointInt now, int dx, int dy)
        {
            PointInt next = new PointInt(now.X + dx, now.Y + dy);
            if (map.IsInRange(next) && map[next].Rect.Fill != Map.clr_Obstacle && map[next].Rect.Fill != Map.clr_Start)
            {
                if (map[now].G + extraGap(dx, dy) < map[next].G || map[next].G == 0)
                {
                    map[next].Rect.Fill = Map.clr_IsOpen;
                    map[next].Step = map[now].Step + 1;
                    map[next].From = now;
                    map[next].G = map[now].G + extraGap(dx, dy);
                    map[next].H = H(next, map.End);
                    HeapAs.InsNode(map[next]);
                    await Delay(DelayTime);
                }
            }
        }

        async Task JudgeAroundAs(PointInt now)
        {
            await JudgeAs(now, -1, -1);
            await JudgeAs(now, 0, -1);
            await JudgeAs(now, 1, -1);
            await JudgeAs(now, 1, 0);
            await JudgeAs(now, 1, 1);
            await JudgeAs(now, 0, 1);
            await JudgeAs(now, -1, 1);
            await JudgeAs(now, -1, 0);
        }

        public async Task Astar()
        {
            bool isLink = false;
            HeapAs = new Heap(map.Size * map.Size);
            Best = map.Start;
            map[Best].H = H(map.Start, map.End);
            await JudgeAroundAs(Best);

            while (HeapAs.Last > 0)
            {
                Best = HeapAs[1].Position;
                map[Best].Rect.Fill = Map.clr_IsClose;
                if (Best == map.End)
                {
                    isLink = true;
                    break;
                }
                HeapAs.DelNode();
                await JudgeAroundAs(Best);
            }
            if (isLink) await DrawPathAsync(map.End, map.Start);
        }


        //

        Heap HeapQp;

        async Task JudgeQp(PointInt now, int dx, int dy)
        {
            PointInt next = new PointInt(now.X + dx, now.Y + dy);
            if (map.IsInRange(next) && map[next].Rect.Fill != Map.clr_Obstacle && map[next].Rect.Fill != Map.clr_Start)
            {
                if (map[next].G == 0)
                {
                    map[next].Rect.Fill = Map.clr_IsOpen;
                    map[next].Step = map[now].Step + 1;
                    map[next].From = now;
                    map[next].G = map[now].G + extraGap(dx, dy);
                    map[next].H = H(next, map.End);
                    HeapQp.InsNodeQp(map[next]);
                    await Delay(DelayTime);
                }
            }
        }

        async Task JudgeAroundQp(PointInt now)
        {
            await JudgeQp(now, -1, -1);
            await JudgeQp(now, 0, -1);
            await JudgeQp(now, 1, -1);
            await JudgeQp(now, 1, 0);
            await JudgeQp(now, 1, 1);
            await JudgeQp(now, 0, 1);
            await JudgeQp(now, -1, 1);
            await JudgeQp(now, -1, 0);
        }

        async Task QuickPath()
        {
            bool isLink = false;
            HeapQp = new Heap(map.Size * map.Size);
            Best = map.Start;
            map[Best].H = H(map.Start, map.End);
            await JudgeAroundQp(Best);

            while (HeapQp.Last > 0)
            {
                Best = HeapQp[1].Position;
                map[Best].Rect.Fill = Map.clr_IsClose;
                if (Best == map.End)
                {
                    isLink = true;
                    break;
                }
                HeapQp.DelNodeQp();
                await JudgeAroundQp(Best);
            }
            if (isLink) await DrawPathAsync(map.End, map.Start);
        }
    }
}
