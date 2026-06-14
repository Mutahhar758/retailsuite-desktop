using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ERP
{
    class Validation
    {
        public static  string[] dateformats = {"dd/MM/yyyy h:mm:ss tt","dd/M/yyyy h:mm:ss tt","d/MM/yyyy h:mm:ss tt","d/M/yyyy h:mm:ss tt", "d/M/yyyy h:mm tt", 
                         "dd/MM/yyyy hh:mm:ss", "d/M/yyyy h:mm:ss", 
                         "d/M/yyyy hh:mm tt", "d/M/yyyy hh tt", 
                         "d/M/yyyy h:mm", "d/M/yyyy h:mm", 
                         "dd/MM/yyyy hh:mm", "dd/M/yyyy hh:mm",
                         "d/MM/yyyy HH:mm:ss.ffffff" ,"dd/MM/yyyy","d/M/yyyy","dd/M/yyyy","d/MM/yyyy","ddMMyyyy","ddMMyy",
                         "dd-MM-yy","d-MM-yy","dd-M-yy","dd-MM-yy","dd-MM-yyyy","dd-MMM-yyyy","dd-MMM-yyyy hh:mm:ss tt","dd/MMM/yyyy hh:mm:ss tt",
                         "dd","ddMM","dd-M","d-MM","dd-MM"};
        public static  void Clear(Control Parentctrl)
        {
            foreach (Control ctrl in Parentctrl.Controls)
            {
                if (ctrl.Tag != (object)"Lock")
                {
                    if (ctrl.GetType() == typeof(TextBox)) ctrl.Text = "";
                    else if (ctrl.GetType() == typeof(ComboBox))
                    {
                        (ctrl as ComboBox).SelectedIndex = -1;
                        ctrl.Text = "";
                    }
                    else if (ctrl.GetType() == typeof(DataGridView))
                    {
                        (ctrl as DataGridView).Rows.Clear();
                        if((ctrl as DataGridView).Rows .Count == 0)
                            (ctrl as DataGridView).Rows.Add();
                    }
                    else if (ctrl.HasChildren == true)
                        Clear(ctrl);
                }
            }

        }
        public static void Clear(Control Parentctrl,Label [] Otherlbl )
        {
            foreach (Control ctrl in Parentctrl.Controls)
            {
                if (ctrl.Tag != (object)"Lock")
                {
                    if (ctrl.GetType() == typeof(TextBox)) ctrl.Text = "";
                    else if (ctrl.GetType() == typeof(ComboBox))
                    {
                        (ctrl as ComboBox).SelectedIndex = -1;
                        ctrl.Text = "";
                    }
                    else if (ctrl.GetType() == typeof(DataGridView))
                    {
                        (ctrl as DataGridView).Rows.Clear();
                        if ((ctrl as DataGridView).Rows.Count == 0)
                            (ctrl as DataGridView).Rows.Add();
                         
                    }
                    else if ((from Label lb in Otherlbl
                              where lb == ctrl  select lb).Count()  > 0)
                    {
                        ctrl.Text = "";
                    }
                    else if (ctrl.HasChildren == true)
                        Clear(ctrl);
                }
            }

        }
        public static string  IsEmpty(string text,string  RetVal)
        {
            string  Retvalue = (text == "" ? RetVal  : text);
            return Retvalue;
        }
        public static string IsNull(string text, string Replace)
        {
            string Retvalue = (text == null ? Replace : text);
            return Retvalue;
        }
        public static string NullIfEmpty(string text)
        {
            text = (text == "" ? null : text);
            return text;
        }
        public static string NullIf(string text,string exptext)
        {
            string Retvalue = (text == exptext ? null  : text);
            return Retvalue;
        }
        public static string ToDate(string date)
        {
            string Retvalue = date.Replace(" ", "-").Replace("/", "-");
            return Retvalue;
        }
        public static object NullToDBNull(Object Value)
        {
            object Retvalue = Value == null ? DBNull .Value : Value;
            return Retvalue;
        }
        public static object NullOrEmptyToDBNull(string  Value)
        {
            object Retvalue = Value == null || Value == "" ? DBNull.Value : (object)Value;
            return Retvalue;
        }
        public static object DBNullToNull(Object Value)
        {
            object Retvalue = Value == DBNull.Value ? null : Value;
            return Retvalue;
        }
        public static object NullToZero(Object Value)
        {
            object Retvalue = Value == null ? "0" : Value;
            return Retvalue;
        }
        public static bool ToBool(string Value)
        {
            bool Retvalue = Value == null || Value == "0" || Value.ToLower()  == "false" ? false  : true;
            return Retvalue;
        }
        public static bool ToBool(object Value)
        {
            bool Retvalue = Value == null ? false : (bool)Value;
            return Retvalue;
        }  
        public static string  BoolToint(bool Value)
        {
            string  Retvalue = Value == true   ? "1" : "0";
            return Retvalue;
        }
        

    }
}
