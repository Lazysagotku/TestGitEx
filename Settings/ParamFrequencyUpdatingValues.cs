using System;
using System.Windows.Forms;

namespace TimeReportV3
{
    internal sealed class ParamFrequencyUpdatingValues : IParamSettings
    {
        //private int FrequencyUpdatingValues = 1;

        private readonly TextBox TbxFrequencyUpdatingValues;
        public ParamFrequencyUpdatingValues(TextBox tbxFrequencyUpdatingValues)
        {
            TbxFrequencyUpdatingValues = tbxFrequencyUpdatingValues;

            TbxFrequencyUpdatingValues.MaxLength = 2;
            TbxFrequencyUpdatingValues.TextChanged += new System.EventHandler(TbxFrequencyUpdatingValues_TextChanged);
            TbxFrequencyUpdatingValues.KeyPress += new System.Windows.Forms.KeyPressEventHandler(TbxFrequencyUpdatingValues_KeyPress);
        }

        private void TbxFrequencyUpdatingValues_TextChanged(object sender, EventArgs e)
        {
            if (!IsCorrectValue(TbxFrequencyUpdatingValues.Text))
            {
                TbxFrequencyUpdatingValues.Text = Properties.Settings.Default.FrequencyUpdatingValues.ToString();
                TbxFrequencyUpdatingValues.SelectAll();
            }
        }

        private void TbxFrequencyUpdatingValues_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Также свойство MaxLenght установлено равным 2 
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        public void SaveValue()
        {
            string curValue = TbxFrequencyUpdatingValues.Text;
            if (IsCorrectValue(curValue))
            {
                Properties.Settings.Default.FrequencyUpdatingValues = Byte.Parse(curValue); ;
            }
        }

        private bool IsCorrectValue(string curValue)
        {
            if (Byte.TryParse(curValue, out byte num))
            {
                return (num > 0 & num <= 30);
            }

            return false;
        }

        public void SetStartValue()
        {
            TbxFrequencyUpdatingValues.Text = Properties.Settings.Default.FrequencyUpdatingValues.ToString();
        }
    }
}
