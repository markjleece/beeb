// This file is Copyright © 2025 - Mark John Leece - All rights reserved
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LevelEditor.ux
{
    public partial class EditCoefficientsDialog : Form
    {
        internal EditCoefficientsDialog(Coefficients coefficients)
        {
            Coefficients = coefficients;
            InitializeComponent();
        }

        private void EditCoefficients_Load(object sender, EventArgs e)
        {
            enemyWeaponStrengthComboBox.Items.Add("128 (high)");
            enemyWeaponStrengthComboBox.Items.Add("64 (normal)");
            enemyWeaponStrengthComboBox.Items.Add("32 (low)");
            SetSelectedValue(enemyWeaponStrengthComboBox, Coefficients.EnemyWeaponStrength);

            laserStrengthComboBox.Items.Add("64 (high)");
            laserStrengthComboBox.Items.Add("32 (normal)");
            laserStrengthComboBox.Items.Add("16 (low)");
            SetSelectedValue(laserStrengthComboBox, Coefficients.LaserStrength);

            laserHealthDrainComboBox.Items.Add("16 (high)");
            laserHealthDrainComboBox.Items.Add("8 (normal)");
            laserHealthDrainComboBox.Items.Add("4 (low)");
            SetSelectedValue(laserHealthDrainComboBox, Coefficients.LaserHealthDrain);

            jewelHealthGainComboBox.Items.Add("512 (high)");
            jewelHealthGainComboBox.Items.Add("256 (normal)");
            jewelHealthGainComboBox.Items.Add("128 (low)");
            SetSelectedValue(jewelHealthGainComboBox, Coefficients.JewelHealthGain);
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            Coefficients.EnemyWeaponStrength = ToInteger(enemyWeaponStrengthComboBox.SelectedItem);
            Coefficients.LaserStrength = ToInteger(laserStrengthComboBox.SelectedItem);
            Coefficients.LaserHealthDrain = ToInteger(laserHealthDrainComboBox.SelectedItem);
            Coefficients.JewelHealthGain = ToInteger(jewelHealthGainComboBox.SelectedItem);
        }
        private static void SetSelectedValue(ComboBox comboBox, int value)
        {
            foreach (object item in comboBox.Items)
            {
                if (ToInteger(item) == value)
                {
                    comboBox.SelectedItem = item;
                    break;
                }
            }
        }

        private static int ToInteger(object? obj)
        {
            if (obj == null) return 0;
            return Int32.Parse(((string)obj).Split(' ')[0]);
        }

        private Coefficients Coefficients;
    }
}
