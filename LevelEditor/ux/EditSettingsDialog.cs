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
    public partial class EditSettingsDialog : Form
    {
        internal EditSettingsDialog(Settings coefficients)
        {
            Settings = coefficients;
            InitializeComponent();
        }

        private void EditCoefficients_Load(object sender, EventArgs e)
        {
            enemyWeaponStrengthComboBox.Items.Add("128 (high)");
            enemyWeaponStrengthComboBox.Items.Add("64 (normal)");
            enemyWeaponStrengthComboBox.Items.Add("32 (low)");
            SetSelectedValue(enemyWeaponStrengthComboBox, Settings.EnemyWeaponStrength);

            laserStrengthComboBox.Items.Add("64 (high)");
            laserStrengthComboBox.Items.Add("32 (normal)");
            laserStrengthComboBox.Items.Add("16 (low)");
            SetSelectedValue(laserStrengthComboBox, Settings.LaserStrength);

            laserHealthDrainComboBox.Items.Add("16 (high)");
            laserHealthDrainComboBox.Items.Add("8 (normal)");
            laserHealthDrainComboBox.Items.Add("4 (low)");
            SetSelectedValue(laserHealthDrainComboBox, Settings.LaserHealthDrain);

            jewelHealthGainComboBox.Items.Add("1024 (high)");
            jewelHealthGainComboBox.Items.Add("512 (normal)");
            jewelHealthGainComboBox.Items.Add("256 (low)");
            SetSelectedValue(jewelHealthGainComboBox, Settings.JewelHealthGain);

            dalekSamplesEnabled.Checked = (Settings.DalekSamplesEnabled != 0);
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            Settings.EnemyWeaponStrength = ToInteger(enemyWeaponStrengthComboBox.SelectedItem);
            Settings.LaserStrength = ToInteger(laserStrengthComboBox.SelectedItem);
            Settings.LaserHealthDrain = ToInteger(laserHealthDrainComboBox.SelectedItem);
            Settings.JewelHealthGain = ToInteger(jewelHealthGainComboBox.SelectedItem);
            Settings.DalekSamplesEnabled = dalekSamplesEnabled.Checked ? 1 : 0;
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

        private Settings Settings;

        private void sampleBasedSoundCheckBox_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}
