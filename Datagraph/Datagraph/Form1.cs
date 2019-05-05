﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using System.Web;

using Excel = Microsoft.Office.Interop.Excel;

namespace datagraph
{
  public partial class Form1 : Form
  {
    static string selectedPath;
    private static double i, j, k;
    Boolean isDefaultLoc = true;
    Boolean isRoot = true;
    DataTable dataTable = new DataTable();
    public Form1()
    {
      InitializeComponent();
      populateInitialValues();
    }

    private void populateInitialValues()
    {
      folderBrowserDialog2 = new FolderBrowserDialog();
      dataTable.Columns.Add("Radius(r)");
      dataTable.Columns.Add("Tangential Stress");
      dataTable.Columns.Add("Longitudinal Stress");
      dataTable.Columns.Add("Radial Stress");
      comboBox1.Items.Add("Yes");
      comboBox1.Items.Add("No");
      label10.Text = DateTime.Now.ToString();
      monthCalendar1.Visible = false;
      button3.Visible = false;
      toolTip1.SetToolTip(this.label10, "Click to display the calendar");
      toolTip2.SetToolTip(this.button1, "Click to compute the results");
      toolTip1.SetToolTip(this.button2, "Click to reset the textboxes");
      SetStyle(ControlStyles.AllPaintingInWmPaint |
               ControlStyles.DoubleBuffer |
               ControlStyles.ResizeRedraw |
               ControlStyles.UserPaint,
               true);
    }

    protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
    {
      if (keyData == (Keys.Control | Keys.C))
      {
        button1_Click(null, null);
      }
      if (keyData == (Keys.Control | Keys.N))
      {
        button2_Click(null, null);
      }
      if (keyData == (Keys.Control | Keys.D))
      {
        label10_Click(null, null);
      }
      if (keyData == (Keys.Control | Keys.H))
      {
        button3_Click(null, null);
      }
      if (keyData == (Keys.Control | Keys.F1))
      {
        toolStripMenuItem1.ShowDropDown();
      }
      if (keyData == (Keys.Control | Keys.V))
      {
        toolStripMenuItem5.ShowDropDown();
      }
      if (keyData == (Keys.Control | Keys.R))
      {
        toolStripMenuItem6.PerformClick();
      }
      if (keyData == (Keys.Control | Keys.S))
      {
        shortCutKeysToolStripMenuItem.PerformClick();
      }
      if (keyData == (Keys.Control | Keys.E))
      {
        toolStripMenuItem4.PerformClick();
      }
      return base.ProcessCmdKey(ref msg, keyData);
    }

    private void button1_Click(object sender, EventArgs e)
    {
      double steps;
      DataRow dr;
      double ri, ro, pi, po, r, ts, ls, rs, p1, p2, p3, p4, ls2, ts1, ts2, rs1, rs2;
      steps = Double.Parse(numericUpDown1.Value.ToString());

      if (string.IsNullOrEmpty(textBox1.Text) || string.IsNullOrEmpty(textBox2.Text) || string.IsNullOrEmpty(textBox3.Text) || string.IsNullOrEmpty(textBox4.Text) || string.IsNullOrEmpty(textBox5.Text) || comboBox1.Text == "Select" || numericUpDown1.Value == 0)
      {
        MessageBox.Show("Some fields are empty(See the rules in the View Menu)", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
      }
      else
      {
        ri = double.Parse(textBox1.Text);
        ro = double.Parse(textBox2.Text);
        pi = double.Parse(textBox3.Text);
        po = double.Parse(textBox4.Text);
        r = double.Parse(textBox5.Text);

        if (ri < 0 || ro < 0 || pi < 0 || po < 0 || r < 0)
        {
          MessageBox.Show("Values should be greater than zero(See the rules in the View Menu)", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        else
        {
          if (ri >= ro)
          {
            MessageBox.Show("Outside diameter shall be greater than or equal to inside diameter\n(See the rules in the View Menu)", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
          }
          else
          {
            double interval;
            interval = ro / steps;
            double intrvl = Math.Round(interval, 4);
            p1 = (pi * ri * ri) - (po * ro * ro);
            p2 = (ro * ro) - (ri * ri);
            p3 = (ri * ri * ro * ro) * (po - pi);
            p4 = (r * r) * (ro * ro - ri * ri);
            ts = (p1 / p2) - (p3 / p4);
            ts1 = Math.Round(ts, 4);

            textBox6.Text = ts1.ToString();
            if (comboBox1.Text == "Yes")
            {
              ls2 = p1 / p2;
              ls = Math.Round(ls2, 4);
              textBox7.Text = ls.ToString();
            }
            else
            {
              ls = 0;
              textBox7.Text = ls.ToString();
            }
            rs = (p1 / p2) + (p3 / p4);
            rs1 = Math.Round(rs, 4);

            textBox8.Text = rs1.ToString();
            for (i = ri; i <= ro; i += intrvl)
            {
              p1 = (pi * ri * ri) - (po * ro * ro);
              p2 = (ro * ro) - (ri * ri);
              p3 = (ri * ri * ro * ro) * (po - pi);
              p4 = (i * i) * (ro * ro - ri * ri);
              double tsn = (p1 / p2) - (p3 / p4);
              ts2 = Math.Round(tsn, 4);
              double rsn = (p1 / p2) + (p3 / p4);
              rs2 = Math.Round(rsn, 4);
              dr = this.dataTable.NewRow();
              this.dataTable.Rows.Add(dr);
              while (j <= k)
              {
                dr[0] = i;
                dr[1] = ts2;
                dr[2] = ls;
                dr[3] = rs2;
                dataGridView1.DataSource = dataTable;
                j++;
              }
              k = k + 4;
            }
          }
        }
      }
    }

    private void button2_Click(object sender, EventArgs e)
    {
      if (checkBox1.Checked)
      {
        reset();
      }
      else
      {
        comboBox1.Text = "Select";
        FormUtil.ClearTextBoxes(Controls);
        numericUpDown1.Value = 0;
      }
    }

    private void exitToolStripMenuItem_Click(object sender, EventArgs e)
    {
      MessageBox.Show("My software", "About", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private void exitToolStripMenuItem1_Click(object sender, EventArgs e)
    {
      try
      {
        System.Diagnostics.Process.Start("http://www.google.com");
      }
      catch (Exception ex)
      {
        MessageBox.Show(string.Format("An error occurred {0}", ex.Message), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    private void onlineHelpToolStripMenuItem_Click(object sender, EventArgs e)
    {
      Application.Exit();
    }

    private void textBox1_TextChanged(object sender, EventArgs e)
    {
      FormUtil.verifyTextBoxInputIsNum(textBox1);
    }

    private void textBox2_TextChanged(object sender, EventArgs e)
    {
      FormUtil.verifyTextBoxInputIsNum(textBox2);
    }

    private void textBox3_TextChanged(object sender, EventArgs e)
    {
      FormUtil.verifyTextBoxInputIsNum(textBox3);
    }

    private void textBox4_TextChanged(object sender, EventArgs e)
    {
      FormUtil.verifyTextBoxInputIsNum(textBox4);
    }

    private void textBox5_TextChanged(object sender, EventArgs e)
    {
      FormUtil.verifyTextBoxInputIsNum(textBox5);
    }

    private void button3_Click(object sender, EventArgs e)
    {
      monthCalendar1.Visible = false;
      button3.Visible = false;
    }
    protected override CreateParams CreateParams
    {
      get
      {
        CreateParams cp = base.CreateParams;
        cp.ExStyle = cp.ExStyle | 0x2000000;
        return cp;
      }
    }

    private void label10_Click(object sender, EventArgs e)
    {
      monthCalendar1.Visible = true;
      button3.Visible = true;
    }

    private void rulesToolStripMenuItem_Click(object sender, EventArgs e)
    {
      MessageBox.Show("All fields should be numerical.\nAll boxes should be field.\nAll numbers should be greater than Zero.\nOutside diameter should be greater than inside diameter.\n", "Rules", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private void contactToolStripMenuItem_Click(object sender, EventArgs e)
    {
      MessageBox.Show("Contact At: \n Email ID: soumya113157@nitp.ac.in", "Contact", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
    }

    private void button5_Click(object sender, EventArgs e)
    {

      object misValue = System.Reflection.Missing.Value;
      Microsoft.Office.Interop.Excel._Application app = new Microsoft.Office.Interop.Excel.Application();
      Microsoft.Office.Interop.Excel._Workbook workbook = app.Workbooks.Add(Type.Missing);
      Microsoft.Office.Interop.Excel._Worksheet worksheet = null;
      // see the excel sheet behind the program
      app.Visible = false;
      try
      {
        worksheet = (Microsoft.Office.Interop.Excel.Worksheet)workbook.Sheets["Sheet1"];
        worksheet = (Microsoft.Office.Interop.Excel.Worksheet)workbook.ActiveSheet;
        worksheet.Name = "Exported from gridview";

        for (int i = 1; i < dataGridView1.Columns.Count + 1; i++)
        {
          worksheet.Cells[1, i] = dataGridView1.Columns[i - 1].HeaderText;
        }

        for (int i = 0; i < dataGridView1.Rows.Count - 1; i++)
        {
          for (int j = 0; j < dataGridView1.Columns.Count; j++)
          {
            worksheet.Cells[i + 2, j + 1] = dataGridView1.Rows[i].Cells[j].Value.ToString();
          }
        }

        // save the application
        string fileName = String.Empty;
        SaveFileDialog saveFileExcel = new SaveFileDialog();

        saveFileExcel.Filter = "Excel files |*.xls|All files (*.*)|*.*";
        saveFileExcel.FilterIndex = 2;
        saveFileExcel.RestoreDirectory = true;

        if (saveFileExcel.ShowDialog() == DialogResult.OK)
        {
          fileName = saveFileExcel.FileName;
          workbook.SaveAs(fileName, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlExclusive, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);

        }
        else
          return;

        workbook = app.Workbooks.Open(fileName, 0, true, 5, "", "", true, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);
        worksheet = (Excel.Worksheet)workbook.Worksheets.get_Item(1);
        Excel.Range chartRange;

        Excel.ChartObjects xlCharts = (Excel.ChartObjects)worksheet.ChartObjects(Type.Missing);
        Excel.ChartObject myChart = (Excel.ChartObject)xlCharts.Add(10, 80, 300, 250);
        Excel.Chart chartPage = myChart.Chart;

        chartPage.ChartType = Excel.XlChartType.xlXYScatterLines;
        chartRange = worksheet.get_Range("A1", "D1000");
        chartPage.SetSourceData(chartRange, misValue);
        chartPage.Export("D:\\Image.jpeg", "JPEG", misValue);
        Image image = Image.FromFile("D:\\Image.jpeg");
        pictureBox1.Image = image;
      }
      catch (System.Exception ex)
      {

      }
      finally
      {
        app.Quit();
        workbook = null;
        app = null;
      }
    }

    private void graph()
    {
      Excel.Application xlApp;
      Excel.Workbook xlWorkBook;
      Excel.Worksheet xlWorkSheet;
      object misValue = System.Reflection.Missing.Value;

      xlApp = new Excel.Application();
      xlWorkBook = xlApp.Workbooks.Open("D:\\csharp.net-informations2.xls", 0, true, 5, "", "", true, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);
      xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);

      Excel.Range chartRange;

      Excel.ChartObjects xlCharts = (Excel.ChartObjects)xlWorkSheet.ChartObjects(Type.Missing);
      Excel.ChartObject myChart = (Excel.ChartObject)xlCharts.Add(10, 80, 300, 250);
      Excel.Chart chartPage = myChart.Chart;

      chartPage.ChartType = Excel.XlChartType.xlXYScatterLines;
      chartRange = xlWorkSheet.get_Range("A1", "D30");
      chartPage.SetSourceData(chartRange, misValue);
      chartPage.Export("D:\\Image.jpeg", "JPEG", misValue);
      Image image = Image.FromFile("D:\\Image.jpeg");
      pictureBox1.Image = image;
      xlWorkBook.Close(true, misValue, misValue);
      xlApp.Quit();

      releaseObject(xlWorkSheet);
      releaseObject(xlWorkBook);
      releaseObject(xlApp);
    }

    private void releaseObject(object obj)
    {
      try
      {
        System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
        obj = null;
      }
      catch (Exception ex)
      {
        obj = null;
        MessageBox.Show("Unable to release the Object " + ex.ToString());
      }
      finally
      {
        GC.Collect();
      }
    }
    private void button6_Click(object sender, EventArgs e)
    {
      dataTable.Clear();
    }

    private void toolStripMenuItem2_Click(object sender, EventArgs e)
    {
      string copyright = "\u00a9 Copyright 2014.";
      MessageBox.Show("Software Version:1.0.0.0\nGraph Automation\n\n\n" + copyright, "About", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private void toolStripMenuItem3_Click(object sender, EventArgs e)
    {
      try
      {
        System.Diagnostics.Process.Start("http://www.google.com");
      }
      catch (Exception ex)
      {
        MessageBox.Show(string.Format("An error occurred {0}", ex.Message), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    private void toolStripMenuItem4_Click(object sender, EventArgs e)
    {
      Application.Exit();
    }

    private void toolStripMenuItem6_Click(object sender, EventArgs e)
    {
      MessageBox.Show("All fields should be numerical.\nAll boxes should be field.\nAll numbers should be greater than Zero.\nOutside diameter should be greater than inside diameter.\n", "Rules", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private void toolStripMenuItem7_Click(object sender, EventArgs e)
    {
      MessageBox.Show("Contact At: \n Email ID:", "Contact", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
    }

    private void shortCutKeysToolStripMenuItem_Click(object sender, EventArgs e)
    {
      MessageBox.Show("Compute(CTRL+C)\nInsert(CTRL+I)\nReset(CTRL+N)\nShow Calendar(CTRL+D)\nHide Calendar(CTRL+H)\nEXIT(CTRL+E)\nRules(CTRL+R)\nShortCut Keys(CTRL+S)\nHelp Menu(CTRL+F1)\nView Menu(CTRL+V)", "Access Keys", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private void button7_Click_1(object sender, EventArgs e)
    {
      string folderdate = DateTime.Now.ToString("yyyy-MM-dd hh_mm_ss");
      string path = FormUtil.setFilePath(selectedPath, isDefaultLoc, isRoot);
      path = path + "\\" + folderdate;

      if (!Directory.Exists(path))
      {
        System.IO.Directory.CreateDirectory(path);
      }

      string filename = @path + "\\exprt.xls";
      string imagenew = @path + "\\Image.jpeg";
      string exclimage = @path + "\\exclimg.xls";

      object misValue = System.Reflection.Missing.Value;
      Microsoft.Office.Interop.Excel._Application app = new Microsoft.Office.Interop.Excel.Application();
      Microsoft.Office.Interop.Excel._Workbook workbook = app.Workbooks.Add(Type.Missing);
      Microsoft.Office.Interop.Excel._Worksheet worksheet = null;

      // see the excel sheet behind the program
      app.Visible = false;
      worksheet = (Microsoft.Office.Interop.Excel.Worksheet)workbook.Sheets["Sheet1"];
      worksheet = (Microsoft.Office.Interop.Excel.Worksheet)workbook.ActiveSheet;
      worksheet.Name = "Exported from gridview";

      for (int i = 1; i < dataGridView1.Columns.Count + 1; i++)
      {
        worksheet.Cells[1, i] = dataGridView1.Columns[i - 1].HeaderText;
      }

      for (int i = 0; i < dataGridView1.Rows.Count - 1; i++)
      {
        for (int j = 0; j < dataGridView1.Columns.Count; j++)
        {
          worksheet.Cells[i + 2, j + 1] = dataGridView1.Rows[i].Cells[j].Value.ToString();
        }
      }

      workbook.SaveAs(filename, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlExclusive, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
      workbook = app.Workbooks.Open(filename, 0, true, 5, "", "", true, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);
      worksheet = (Excel.Worksheet)workbook.Worksheets.get_Item(1);

      Excel.Range chartRange;

      Excel.ChartObjects xlCharts = (Excel.ChartObjects)worksheet.ChartObjects(Type.Missing);
      Excel.ChartObject myChart = (Excel.ChartObject)xlCharts.Add(10, 80, 300, 250);
      Excel.Chart chartPage = myChart.Chart;
      myChart.Height = 700;
      myChart.Width = 1000;
      chartPage.ChartType = Excel.XlChartType.xlXYScatterLines;
      chartRange = worksheet.get_Range("A1", "D2000");
      chartPage.SetSourceData(chartRange, misValue);
      //chartPage.ChartArea
      chartPage.Export(imagenew, "JPEG", misValue);
      Image image = Image.FromFile(imagenew);
      pictureBox1.Image = image;
      workbook.SaveAs(exclimage, Type.Missing,
        Type.Missing, Type.Missing,
        Type.Missing, Type.Missing,
        Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlExclusive,
        Type.Missing, Type.Missing,
        Type.Missing, Type.Missing,
        Type.Missing);
      workbook.Close(true, misValue, misValue);
      //  }
      //  catch (System.Exception ex)
      //  {
      //      MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
      //  }
      //  finally
      //  {
      app.Quit();
      releaseObject(worksheet);
      releaseObject(workbook);
      releaseObject(app);

      workbook = null;
      app = null;
      // workbook.Close(true, misValue, misValue);
      //}
    }
    private void button5_Click_1(object sender, EventArgs e)
    {
      graphext();
    }
    private void graphext()
    {
      string folderdate = DateTime.Now.ToString("yyyy-MM-dd hh_mm_ss");
      string path = FormUtil.setFilePath(selectedPath, isDefaultLoc, isRoot);
      path = path + "\\" + folderdate;

      if (!Directory.Exists(path))
      {
        System.IO.Directory.CreateDirectory(path);
      }

      string imagenew = path + "\\Image.jpeg";
      string exclimage = path + "\\exclimg.xls";

      Excel.Application xlApp;
      Excel.Workbook xlWorkBook;
      Excel.Worksheet xlWorkSheet;
      object misValue = System.Reflection.Missing.Value;

      openFileDialog1.FileName = String.Empty;
      openFileDialog1.Filter = "Excel Sheet(.xls)|*.xls|Microsoft Excel Sheets(.xlsx)|*.xlsx";

      if (openFileDialog1.ShowDialog() == DialogResult.OK)
      {

        xlApp = new Excel.Application();
        xlApp.DisplayAlerts = false;
        xlApp.Visible = false;
        xlWorkBook = xlApp.Workbooks.Open(openFileDialog1.FileName, 0, true, 5, "", "", true,
          Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);
        xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);
      }
      else
        return;

      Excel.Range chartRange;
      Excel.ChartObjects xlCharts = (Excel.ChartObjects)xlWorkSheet.ChartObjects(Type.Missing);
      Excel.ChartObject myChart = (Excel.ChartObject)xlCharts.Add(10, 80, 300, 250);
      Excel.Chart chartPage = myChart.Chart;


      chartPage.ChartType = Excel.XlChartType.xlXYScatterLines;
      chartRange = xlWorkSheet.get_Range("A1", "D2000");
      chartPage.SetSourceData(chartRange, misValue);
      myChart.Height = 700;
      myChart.Width = 1024;
      chartPage.Export(imagenew, "JPEG", misValue);
      Image image = Image.FromFile(imagenew);
      pictureBox1.Image = image;

      xlWorkBook.SaveAs(exclimage, Type.Missing,
        Type.Missing, Type.Missing,
        Type.Missing, Type.Missing,
        Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlExclusive,
        Type.Missing, Type.Missing,
        Type.Missing, Type.Missing,
        Type.Missing);
      xlWorkBook.Close(true, misValue, misValue);
      xlApp.Quit();

      releaseObject(xlWorkSheet);
      releaseObject(xlWorkBook);
      releaseObject(xlApp);
    }

    private void button8_Click(object sender, EventArgs e)
    {
      saveFileDialog1.Filter = "Bitmap Image (.bmp)|*.bmp|Gif Image (.gif)|*.gif|JPEG Image (.jpeg)|*.jpeg|Png Image (.png)|*.png|Tiff Image (.tiff)|*.tiff|Wmf Image (.wmf)|*.wmf";
      if (saveFileDialog1.ShowDialog() == DialogResult.OK)
      {
        pictureBox1.Image.Save(saveFileDialog1.FileName, ImageFormat.Jpeg);
      }
      else
        return;
    }

    private void goToFolderToolStripMenuItem_Click(object sender, EventArgs e)
    {
      string path = FormUtil.setFilePath(selectedPath, isDefaultLoc, isRoot);
      DirectoryInfo dir = new DirectoryInfo(path);
      if (Directory.Exists(path))
      {
        System.Diagnostics.Process.Start("explorer.exe", path);
      }
      else
      {
        MessageBox.Show("Directory doesnt exist", "Information", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
      }
    }

    private void manageSpaceToolStripMenuItem_Click(object sender, EventArgs e)
    {
      string path = FormUtil.setFilePath(selectedPath, isDefaultLoc, isRoot);
   
      if (MessageBox.Show("Delete all the files?", "Manage Space", MessageBoxButtons.OKCancel) == DialogResult.OK)
      {
        if (pictureBox1.Image != null)
        {
          pictureBox1.Image.Dispose();
          pictureBox1.Image = null;
          pictureBox1.ResetText();
        }

        DirectoryInfo dir = new DirectoryInfo(path);

        if (Directory.Exists(path))
        {

          DeleteDirectory(path);
          MessageBox.Show("Directory successfully deleted", "Information", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
        }
        else
        {
          MessageBox.Show("Folder Doesnt Exist", "Info", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
        }

      }
      else
        return;
    }

    private void DeleteDirectory(string path)
    {

      foreach (string filename in Directory.GetFiles(path))
      {
        File.Delete(filename);
      }
      foreach (string subfolder in Directory.GetDirectories(path))
      {
        DeleteDirectory(subfolder);
      }
      Directory.Delete(path);
    }

    private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
    {
      saveFileDialog1.Filter = "Bitmap Image (.bmp)|*.bmp|Gif Image (.gif)|*.gif|JPEG Image (.jpeg)|*.jpeg|Png Image (.png)|*.png|Tiff Image (.tiff)|*.tiff|Wmf Image (.wmf)|*.wmf";
      if (saveFileDialog1.ShowDialog() == DialogResult.OK)
      {
        pictureBox1.Image.Save(saveFileDialog1.FileName, ImageFormat.Jpeg);
      }
      else
        return;
    }

    private void openFolderToolStripMenuItem_Click(object sender, EventArgs e)
    {
      string path = FormUtil.setFilePath(selectedPath, isDefaultLoc, isRoot);
      DirectoryInfo dir = new DirectoryInfo(path);
      if (Directory.Exists(path))
      {
        System.Diagnostics.Process.Start("explorer.exe", @"C:\Export");
      }
      else
      {
        MessageBox.Show("Directory doesnt exist", "Information", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
      }
    }

    private void exitToolStripMenuItem2_Click(object sender, EventArgs e)
    {
      Application.Exit();
    }

    private void changeFolderDestinationToolStripMenuItem_Click(object sender, EventArgs e)
    {

      if (folderBrowserDialog2.ShowDialog() == DialogResult.OK)
      {
        selectedPath = folderBrowserDialog2.SelectedPath;
        DirectoryInfo d = new DirectoryInfo(selectedPath);
        if (d.Parent == null)
        {
          isDefaultLoc = false;
          MessageBox.Show("This is a root folder", "Info", MessageBoxButtons.OK);
        }
        else
        {
          MessageBox.Show("Path is:" + selectedPath, "Info", MessageBoxButtons.OK);
          isRoot = false;
        }
      }
      else return;
    }

    private void defaultFolderToolStripMenuItem_Click(object sender, EventArgs e)
    {
      MessageBox.Show("Default folder is set to C:", "Info", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
      isDefaultLoc = true;
      isRoot = true;
    }

    private void reset()
    {
      dataTable.Clear();
      comboBox1.Text = "Select";
      FormUtil.ClearTextBoxes(Controls);
      if (pictureBox1.Image != null)
      {
        pictureBox1.Image.Dispose();
        pictureBox1.Image = null;
        pictureBox1.ResetText();
      }
    }

    private void resetToolStripMenuItem_Click(object sender, EventArgs e)
    {
      if (pictureBox1.Image != null)
      {
        pictureBox1.Image.Dispose();
        pictureBox1.Image = null;
        pictureBox1.ResetText();
      }

    }
  }
}