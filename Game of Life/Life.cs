﻿using System;
using System.Drawing;
using System.Text;
using System.IO;
using System.Collections.Generic;

namespace Game_of_Life
{
    class Life
    {
        int x, y;
        Random rnd = new Random();
        List<bool[,]> states;
        private bool[,] currentState {
            get { return states[states.Count-1]; }
            set { states[states.Count - 1] = value; }
        }
        public Life(int x, int y)
        {
            Reset(x, y);
        }
        public void Reset(int x, int y)
        {
            this.x = x;
            this.y = y;
            states = new List<bool[,]>();
            states.Add(new bool[x,y]);
            Random();
        }
        public void Reset()
        {
            states = new List<bool[,]>();
            states.Add(new bool[x, y]);
        }
        public void Random()
        {
            for (int i = 0; i < x; i++)
                for (int j = 0; j < y; j++)
                    currentState[i, j] = rnd.Next(0, 6) == 0;
        }
        private int aliveNeighbors(int i, int j)
        {
            int s = 0;
            if (i > 0 && j > 0 && currentState[i - 1, j - 1]) s++;
            if (i > 0 && currentState[i - 1, j]) s++;
            if (j > 0 && currentState[i, j - 1]) s++;
            if (i > 0 && j < y - 1 && currentState[i - 1, j + 1]) s++;
            if (j < y - 1 && currentState[i, j + 1]) s++;
            if (i < x - 1 && j > 0 && currentState[i + 1, j - 1]) s++;
            if (i < x - 1 && currentState[i + 1, j]) s++;
            if (i < x - 1 && j < y - 1 && currentState[i + 1, j + 1]) s++;
            return s;
        }
        public void Next()
        {
            bool[,] b = new bool[x, y];
            for (int i = 0; i < x; i++)
                for (int j = 0; j < y; j++)
                    if ((!currentState[i, j] && aliveNeighbors(i, j) == 3) ||
                        (currentState[i, j] && aliveNeighbors(i, j) >= 2 && aliveNeighbors(i, j) <= 3)) b[i, j] = true;
            states.Add(b);
        }
        public bool Reversible { get { return states.Count > 1; } }
        public void Previous()
        {
            if (!Reversible) return;
            states.RemoveAt(states.Count - 1);
        }
        public bool MultiColor { get; set; }
        private Brush color(int i,int j)
        {
            if(!MultiColor)
            {
                if (currentState[i, j]) return new Pen(Color.RoyalBlue).Brush;
                else return new Pen(Color.White).Brush;
            }
            if (currentState[i, j] && (aliveNeighbors(i, j) == 2 || aliveNeighbors(i, j) == 3)) return new Pen(Color.RoyalBlue).Brush;
            if (currentState[i, j]) return new Pen(Color.FromArgb(90, 0, 200)).Brush;
            if (aliveNeighbors(i, j) == 3) return new Pen(Color.LightBlue).Brush;
            return new Pen(Color.White).Brush;
        }
        public void Paint(Graphics g, int width, int height)
        {
            float w = (float)width / x;
            float h = (float)height / y;
            for (int i = 0; i < x; i++)
                for (int j = 0; j < y; j++)
                    g.FillRectangle(color(i, j), i * w, j * h, i * w + w, j * h + h);
            for (int i = 1; i < x; i++)
                g.DrawLine(Pens.LightGray, i * w, 0, i * w, height);
            for (int i = 1; i < y; i++)
                g.DrawLine(Pens.LightGray, 0, i * h, width, i * h);
        }
        public int X { get { return x; } }
        public int Y { get { return y; } }
        public void Change(int i,int j)
        {
            if (i < 0 || j < 0 || i > x || j > y) throw new Exception();
            currentState[i, j] ^= true;
        }
        public void Export(StreamWriter sw)
        {
            StringBuilder sb = new StringBuilder(x + "\n" + y + "\n");
            for (int k = 0; k < states.Count; k++)
                for (int i = 0; i < x; i++)
                    for (int j = 0; j < y; j++)
                        sb.Append((states[k][i, j] ? 1 : 0) + "\n");
            sw.Write(Convert.ToBase64String(Encoding.UTF8.GetBytes(sb.ToString())));
            sw.Close();
        }
        public void Import(StreamReader sr)
        {
            string[] data = Encoding.UTF8.GetString(Convert.FromBase64String(sr.ReadToEnd())).Split('\n');
            Reset(int.Parse(data[0]), int.Parse(data[1]));
            for (int i = 1; i < (data.Length - 3) / (x * y); i++) states.Add(new bool[x, y]);
            for (int i = 2; i < data.Length - 1; i++) states[(i - 2) / (x * y)][((i - 2) % (x * y)) / x, (i - 2) % x] = (int.Parse(data[i]) == 1);
        }
    }
}