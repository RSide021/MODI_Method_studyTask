using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace metodPotent
{
    public partial class Form1 : Form
    {
        int countClmn = 0;
        int countRow = 0;
        Dictionary<string, int> valueUV = new Dictionary<string, int>();
        Dictionary<string, int> foundC = new Dictionary<string, int>();
        Dictionary<string, int> foundDelta = new Dictionary<string, int>();
        public Form1()
        {
            InitializeComponent();
        }

        private void button_ok_Click(object sender, EventArgs e)
        {
            valueUV.Clear();
            foundC.Clear();
            foundDelta.Clear();
            textBox_info.Clear();
            countClmn = (int)numericUpDownClmn.Value;
            countRow = (int)numericUpDownRow.Value;
            if (countClmn == 0 || countRow == 0) return;
            while (dataGridView.Rows.Count > 0)
            {
                dataGridView.Rows.Clear();
            }
            while (dataGridView.Columns.Count > 0)
            {
                dataGridView.Columns.Clear();
            }
            for (int i = 0; i <= numericUpDownClmn.Value; i++)
            {
                if (i == 0)
                {
                    dataGridView.Columns.Add($"clmn{i}", "");
                }
                else
                {
                    dataGridView.Columns.Add($"clmn{i}", $"Заказчик {i}");
                }
            }
            for (int i = 0; i <= numericUpDownRow.Value; i++)
            {
                dataGridView.Rows.Add();
            }
            dataGridView[0, 0].Value = "Запасы\\Требуется";
        }

        private void button_solve_Click(object sender, EventArgs e)
        {
            addToMap();
            findC2();
            findDelta();
            needContinue();
        }

        private void button_color_Click(object sender, EventArgs e)
        {
            for(int i = 1; i <= countClmn; i++)
            {
                for(int j = 1; j <= countRow; j++)
                {
                    if(dataGridView[i, j].Selected)
                    {
                        dataGridView[i, j].Style.BackColor = Color.LightPink;

                    }
                }
            }
        }

        private void needContinue()
        {
            foreach (var el in foundDelta)
            {
                if (el.Value < 0)
                {
                    textBox_info.Text += "\r\n  !!!НЕОБХОДИМО ПРОДОЛЖИТЬ ПОИСК!!!\r\n";
                    return;
                }
            }
            textBox_info.Text += "\r\n  !!!ПОЛУЧЕН ОПТИМАЛЬНЫЙ ПЛАН!!!\r\n";
        }
        private void findDelta()
        {

            textBox_info.Text += "\r\nРАСЧЕТ ОЦЕНОК СОБОДНЫХ КЛЕТОК\r\n";
            for (int i = 1; i <= countRow; i++)
            {
                for (int j = 1; j <= countClmn; j++)
                {
                    if (dataGridView[j, i].Style.BackColor != Color.LightPink)
                    {
                        int c = Convert.ToInt32(dataGridView[j, i].Value);
                        int c2 = foundC[$"C'[{i},{j}]"];
                        int res = c - c2;
                        foundDelta.Add($"Δ[{i},{j}]", res);
                        textBox_info.Text += $"Δ[{i},{j}] = C[{i},{j}] - C'[{i},{j}] = {c} - {c2} = {res}\r\n";
                    }
                }
            }
        }
        private void findC2()
        {
            textBox_info.Text += "\r\nРАСЧЕТ КОСВЕННЫХ ТАРИФОВ\r\n";
            for (int i = 1; i <= countRow; i++)
            {
                for (int j = 1; j <= countClmn; j++)
                {
                    if (dataGridView[j, i].Style.BackColor != Color.LightPink)
                    {
                        int u = valueUV[$"U{i}"];
                        int v = valueUV[$"V{j}"];
                        int c = u + v;

                        foundC.Add($"C'[{i},{j}]", c);
                        textBox_info.Text += $"C'[{i},{j}] = U{i} + V{j} = {c}\r\n";
                    }
                }
            }
        }
        private void addToMap()
        {
            textBox_info.Text += "\r\nРАСЧЕТ ПОТЕНЦИАЛОВ ЗАНЯТЫХ КЛЕТОК\r\n";
            valueUV.Add("U1", 0);
            for (int i = 1; i <= countRow; i++)
            {
                for (int j = 1; j <= countClmn; j++)
                {
                    if (dataGridView[j, i].Style.BackColor == Color.LightPink)
                    {
                        bool cntInU = false;
                        bool cntInV = false;
                        if (valueUV.ContainsKey($"U{i}"))
                        {
                            cntInU = true;
                        }
                        if (valueUV.ContainsKey($"V{j}"))
                        {
                            cntInV = true;
                        }

                        if(cntInU & !cntInV) // есть U, но нет V
                        {
                            var cellValue = Convert.ToInt32(dataGridView[j, i].Value);
                            int v =  cellValue - valueUV[$"U{i}"];
                            valueUV.Add($"V{j}", v);
                            textBox_info.Text += $"U{i} = {valueUV[$"U{i}"]}    ";
                            textBox_info.Text += $"U{i} + V{j} = {cellValue}  =>  V{j} = {v}\r\n";
                        } else if(!cntInU & cntInV) // нет U, но есть V
                        {
                            var cellValue = Convert.ToInt32(dataGridView[j, i].Value);
                            int u = cellValue - valueUV[$"V{j}"];
                            valueUV.Add($"U{i}", u);
                            textBox_info.Text += $"V{j} = {valueUV[$"V{j}"]}    ";
                            textBox_info.Text += $"U{i} + V{j} = {cellValue}  =>  U{i} = {u}\r\n";
                        } else if(cntInU & cntInV) // есть оба
                        {
                            continue;
                        }else if(!cntInU & !cntInV) // оба отсутствуют
                        {
                            valueUV.Add($"U{i}", 0);
                            --j;
                            
                        }
                    }
                }
            }
        }
    }
}
