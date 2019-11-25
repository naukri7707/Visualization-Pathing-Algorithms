using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace VisualizationPathingAlgorithms
{
    class MapIO
    {
        Map map;
        private String MapExt(string Name)
        {
            string mapTxt = "Name : " + Name + "\nSize : " + map.Size + "\n\n<<Map>>";
            for (int i = 0; i < map.Size; i++)
            {
                mapTxt += '\n';
                for (int j = 0; j < map.Size; j++)
                {
                    if (i == map.End.X && j == map.End.Y) mapTxt += '3';
                    else if (i == map.Start.X && j == map.Start.Y) mapTxt += '2';
                    else if (map.Index[i, j].Rect.Fill == Map.clr_Obstacle) mapTxt += '1';
                    else mapTxt += '0';
                }
            }
            return mapTxt;
        }

        public void MapOut(Map map)
        {
            SaveFileDialog sf = new SaveFileDialog();
            sf.Filter = "MAP檔案|*.map";
            sf.Title = "儲存地圖設定檔";
            sf.FileName = "Map000";
            sf.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;//當前應用程序域路徑
            if (sf.ShowDialog() == true)
            {
                this.map = map;
                StreamWriter sw = new StreamWriter(sf.FileName);
                sw.Write(MapExt(Path.GetFileNameWithoutExtension(sf.FileName)));
                sw.Close();
            }
        }

        //

        private bool MapImp(string path, double cvsSize)
        {
            StreamReader sr = new StreamReader(path);
            sr.ReadLine();
            int s, p = 0;
            if (!int.TryParse(Regex.Replace(sr.ReadLine(), "[^0-9]", ""), out s))
            {
                MessageBox.Show("地圖檔錯誤");
                return false;
            }
            map = new Map(s, cvsSize);
            sr.Read();
            sr.ReadLine();
            string reader = sr.ReadToEnd();
            try
            {

                for (int i = 0; i < map.Size; i++)
                {
                    for (int j = 0; j < map.Size; j++)
                    {
                        switch (reader[p++])
                        {
                            case '0':
                                map[i, j].Rect.Fill = Map.clr_Space;
                                break;
                            case '1':
                                map[i, j].Rect.Fill = Map.clr_Obstacle;
                                break;
                            case '2':
                                map[i, j].Rect.Fill = Map.clr_Start;
                                map.Start = new PointInt(i, j);
                                break;
                            case '3':
                                map[i, j].Rect.Fill = Map.clr_End;
                                map.End = new PointInt(i, j);
                                break;
                        }
                    }
                    p++;
                }
                return true;
            }
            catch
            {
                MessageBox.Show("地圖檔錯誤");
                return false;
            }
        }

        public bool MapIn(double cvsSize, ref Map map)
        {
            OpenFileDialog of = new OpenFileDialog();
            of.Filter = "MAP檔案|*.map";
            of.Title = "載入地圖設定檔";
            of.FileName = "Map000.map";
            of.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;//當前應用程序域路徑
            if (of.ShowDialog() == true)
            {
                if (MapImp(of.FileName, cvsSize))
                    map = this.map;
                return true;
            }
            else return false;
        }
    }
}
