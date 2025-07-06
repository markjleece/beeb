// --------------------------------------------------------------
// An Adventure In Time - A Doctor Who fan game for the BBC Micro
// Model B
//
// Copyright (C) 2025  Mark John Leece
//
// This program is free software; you can redistribute it and/or
// modify it under the terms of the GNU General Public License
// as published by the Free Software Foundation; either version 2
// of the License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public
// License along with this program; if not, write to the Free
// Software Foundation, Inc., 51 Franklin Street, Fifth Floor,
// Boston, MA  02110-1301, USA.
// --------------------------------------------------------------

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
            enemyWeaponStrengthComboBox.Items.Add("512 (high)");
            enemyWeaponStrengthComboBox.Items.Add("256 (normal)");
            enemyWeaponStrengthComboBox.Items.Add("128 (low)");
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

            sampleSoundsCheckbox.Checked = (Settings.SampleSoundsEnabled != 0);
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            Settings.EnemyWeaponStrength = ToInteger(enemyWeaponStrengthComboBox.SelectedItem);
            Settings.LaserStrength = ToInteger(laserStrengthComboBox.SelectedItem);
            Settings.LaserHealthDrain = ToInteger(laserHealthDrainComboBox.SelectedItem);
            Settings.JewelHealthGain = ToInteger(jewelHealthGainComboBox.SelectedItem);
            Settings.SampleSoundsEnabled = sampleSoundsCheckbox.Checked ? 1 : 0;
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

        private readonly Settings Settings;
    }
}
