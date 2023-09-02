using System.IO;
using System.Windows.Forms;

namespace SFSE
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void OpenFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog.ShowDialog(this);
        }

        private void OpenFileDialog_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {
            FileStream saveFileStream = File.OpenRead(OpenFileDialog.FileName);
            Program.LoadedData = new SaveData(new SaveFile(saveFileStream).DecompressedChunks[0].Data);

            ValidateAvatarButton.Enabled = true;

            AvatarNameContentLabel.Text = new String(Program.LoadedData.AvatarName);
            AvatarSexContentLabel.Text = Program.LoadedData.AvatarSex == 1 ? "Female" : "Male";
            AvatarModelContentLabel.Text = Program.LoadedData.AvatarModel.ToString();
            DateTime SaveTimestamp = new DateTime(1970, 1, 1).AddSeconds(Program.LoadedData.Timestamp);
            SaveDateContentLabel.Text = SaveTimestamp.ToString();

            LevelTextBox.Enabled = true;
            LevelTextBox.Text = Convert.ToString(Program.LoadedData.AvatarLevel);

            LevelProgressTrackBar.Enabled = true;
            LevelProgressTrackBar.Value = Experience.ToPercent(Program.LoadedData.AvatarLevel, Program.LoadedData.AvatarExperience);

            AgilityTextBox.Enabled = true;
            AgilityTextBox.Text = Convert.ToString(Program.LoadedData.AvatarAgility);

            CharismaTextBox.Enabled = true;
            CharismaTextBox.Text = Convert.ToString(Program.LoadedData.AvatarCharisma);

            DexterityTextBox.Enabled = true;
            DexterityTextBox.Text = Convert.ToString(Program.LoadedData.AvatarDexterity);

            IntelligenceTextBox.Enabled = true;
            IntelligenceTextBox.Text = Convert.ToString(Program.LoadedData.AvatarIntelligence);

            StaminaTextBox.Enabled = true;
            StaminaTextBox.Text = Convert.ToString(Program.LoadedData.AvatarStamina);

            StrengthTextBox.Enabled = true;
            StrengthTextBox.Text = Convert.ToString(Program.LoadedData.AvatarStrength);

            WisdomTextBox.Enabled = true;
            WisdomTextBox.Text = Convert.ToString(Program.LoadedData.AvatarWisdom);

            FreeStatPointsTextBox.Enabled = true;
            FreeStatPointsTextBox.Text = Program.LoadedData.AvatarFreeStatPoints.ToString();

            AbilitySlotListBox.Enabled = true;
            AbilitySlotListBox.Items.AddRange(new object[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" });
            AbilitySlotListBox.SelectedIndex = 0;

            AbilityTypeListBox.Enabled = true;
            AbilityTypeListBox.DataSource = Enum.GetValues(typeof(Program.AbilityType));
            AbilityTypeListBox.SelectedItem = (Program.AbilityType)Program.LoadedData.AvatarAbilities[AbilitySlotListBox.SelectedIndex].Type;

            AbilitySubtypeListBox.Enabled = true;
            Program.UpdateSubtypeValue(AbilityTypeListBox, AbilitySubtypeListBox, Program.LoadedData.AvatarAbilities[0].SubType);

            AbilityLevelListBox.Enabled = true;
            AbilityLevelListBox.DataSource = Enum.GetValues(typeof(Program.AbilityLevel));
            AbilityLevelListBox.SelectedItem = (Program.AbilityLevel)Program.LoadedData.AvatarAbilities[0].Level;

            FreeAbilityPointsTextBox.Enabled = true;
            FreeAbilityPointsTextBox.Text = Program.LoadedData.AvatarFreeAbilityPoints.ToString();

            GoldTextBox.Enabled = true;
            GoldTextBox.Text = Program.LoadedData.AvatarGold.ToString();

            SilverTextBox.Enabled = true;
            SilverTextBox.Text = Program.LoadedData.AvatarSilver.ToString();

            CopperTextBox.Enabled = true;
            CopperTextBox.Text = Program.LoadedData.AvatarCopper.ToString();
        }

        private void AbilitySlotListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            var slot = AbilitySlotListBox.SelectedIndex;
            AbilityTypeListBox.SelectedItem = (Program.AbilityType)Program.LoadedData.AvatarAbilities[slot].Type;
            Program.UpdateSubtypeValue(AbilityTypeListBox, AbilitySubtypeListBox, Program.LoadedData.AvatarAbilities[slot].SubType);
            AbilityLevelListBox.SelectedItem = (Program.AbilityLevel)Program.LoadedData.AvatarAbilities[slot].Level;
        }

        private void AbilityTypeListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            Program.UpdateSubtypeChoices(AbilityTypeListBox, AbilitySubtypeListBox);
        }

        private void ValidateAvatarButton_Click(object sender, EventArgs e)
        {
            var level = Program.LoadedData.AvatarLevel;

            var stats = Program.LoadedData.AvatarAgility +
                Program.LoadedData.AvatarCharisma +
                Program.LoadedData.AvatarDexterity +
                Program.LoadedData.AvatarIntelligence +
                Program.LoadedData.AvatarStamina +
                Program.LoadedData.AvatarStrength +
                Program.LoadedData.AvatarWisdom +
                Program.LoadedData.AvatarFreeStatPoints;

            var abilities = 0;
            for (var i = 0; i < 10; i++)
            {
                var abilityLevel = Program.LoadedData.AvatarAbilities[i].Level;
                abilities += (abilityLevel == 255) ? 0 : abilityLevel;
            }
            abilities += Program.LoadedData.AvatarFreeAbilityPoints;

            var experience = Program.LoadedData.AvatarExperience;

            if (experience < Experience.ForLevel[level] || experience > Experience.ForLevel[(byte)(level + 1)])
            {
                MessageBox.Show(
                    "Wrong experience value for current level! Play with the level progress slide; if the problem persists, report a bug!",
                    "Wrong experience value!",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                    );
            }
            else if (abilities != (2 * level))
            {
                MessageBox.Show(
                    "Wrong ability points count for current level!\nExpected: " + (2 * level).ToString() + ", Got: " + abilities.ToString() + ".",
                    "Wrong ability points count!",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                    );
            }
            else if (stats != (5 * (level - 1) + 205))
            {
                MessageBox.Show(
                    "Wrong stat points count for current level!\nExpected: " + (5 * (level - 1) + 205).ToString() + ", Got: " + stats.ToString() + ".",
                    "Wrong stat points count!",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                    );
            }
            else
            {
                MessageBox.Show(
                    "All good!",
                    "All good!",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                    );
            }
        }

        private void LevelTextBox_Leave(object sender, EventArgs e)
        {
            try
            {
                Byte level = Convert.ToByte(LevelTextBox.Text);
                if (level > 50 || level < 1) throw new Exception();
                Program.LoadedData.AvatarLevel = level;
                Program.LoadedData.AvatarExperience = Experience.FromPercent(level, (byte)LevelProgressTrackBar.Value);
            }
            catch
            {
                MessageBox.Show("Please enter a number in a range [1, 50].", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LevelProgressTrackBar_Leave(object sender, EventArgs e)
        {
            Program.LoadedData.AvatarExperience = Experience.FromPercent(Program.LoadedData.AvatarLevel, (byte)LevelProgressTrackBar.Value);
        }

        private void GoldTextBox_Leave(object sender, EventArgs e)
        {
            try
            {
                Program.LoadedData.AvatarGold = Convert.ToInt32(GoldTextBox.Text);
            }
            catch
            {
                MessageBox.Show("Please enter a number in a range [0, 2147483647].", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SilverTextBox_Leave(object sender, EventArgs e)
        {
            try
            {
                Program.LoadedData.AvatarSilver = Convert.ToInt32(SilverTextBox.Text);
            }
            catch
            {
                MessageBox.Show("Please enter a number in a range [0, 2147483647].", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CopperTextBox_Leave(object sender, EventArgs e)
        {
            try
            {
                Program.LoadedData.AvatarCopper = Convert.ToInt32(CopperTextBox.Text);
            }
            catch
            {
                MessageBox.Show("Please enter a number in a range [0, 2147483647].", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void StrengthTextBox_Leave(object sender, EventArgs e)
        {
            try
            {
                Program.LoadedData.AvatarStrength = Convert.ToInt32(StrengthTextBox.Text);
            }
            catch
            {
                MessageBox.Show("Please enter a number in a range [0, 2147483647].", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void StaminaTextBox_Leave(object sender, EventArgs e)
        {
            try
            {
                Program.LoadedData.AvatarStamina = Convert.ToInt32(StaminaTextBox.Text);
            }
            catch
            {
                MessageBox.Show("Please enter a number in a range [0, 2147483647].", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DexterityTextBox_Leave(object sender, EventArgs e)
        {
            try
            {
                Program.LoadedData.AvatarDexterity = Convert.ToInt32(DexterityTextBox.Text);
            }
            catch
            {
                MessageBox.Show("Please enter a number in a range [0, 2147483647].", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AgilityTextBox_Leave(object sender, EventArgs e)
        {
            try
            {
                Program.LoadedData.AvatarAgility = Convert.ToInt32(AgilityTextBox.Text);
            }
            catch
            {
                MessageBox.Show("Please enter a number in a range [0, 2147483647].", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void IntelligenceTextBox_Leave(object sender, EventArgs e)
        {
            try
            {
                Program.LoadedData.AvatarIntelligence = Convert.ToInt32(IntelligenceTextBox.Text);
            }
            catch
            {
                MessageBox.Show("Please enter a number in a range [0, 2147483647].", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void WisdomTextBox_Leave(object sender, EventArgs e)
        {
            try
            {
                Program.LoadedData.AvatarWisdom = Convert.ToInt32(WisdomTextBox.Text);
            }
            catch
            {
                MessageBox.Show("Please enter a number in a range [0, 2147483647].", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CharismaTextBox_Leave(object sender, EventArgs e)
        {
            try
            {
                Program.LoadedData.AvatarCharisma = Convert.ToInt32(CharismaTextBox.Text);
            }
            catch
            {
                MessageBox.Show("Please enter a number in a range [0, 2147483647].", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void FreeStatPointsTextBox_Leave(object sender, EventArgs e)
        {
            try
            {
                Program.LoadedData.AvatarFreeStatPoints = Convert.ToInt16(FreeStatPointsTextBox.Text);
            }
            catch
            {
                MessageBox.Show("Please enter a number in a range [0, 32767].", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AbilityTypeListBox_Leave(object sender, EventArgs e)
        {
            var slot = AbilitySlotListBox.SelectedIndex;
            var value = (AbilityTypeListBox.SelectedValue == null) ? (byte)255 : (byte)AbilityTypeListBox.SelectedValue;
            Program.LoadedData.AvatarAbilities[slot].Type = value;
            if (value == 255)
            {
                Program.LoadedData.AvatarAbilities[slot].SubType = value;
                Program.LoadedData.AvatarAbilities[slot].Level = value;
            }
        }

        private void AbilitySubtypeListBox_Leave(object sender, EventArgs e)
        {
            var slot = AbilitySlotListBox.SelectedIndex;
            var value = (AbilitySubtypeListBox.SelectedValue == null) ? (byte)255 : (byte)AbilitySubtypeListBox.SelectedValue;
            Program.LoadedData.AvatarAbilities[slot].SubType = value;
            if (value == 255)
            {
                Program.LoadedData.AvatarAbilities[slot].Type = value;
                Program.LoadedData.AvatarAbilities[slot].Level = value;
            }
        }

        private void AbilityLevelListBox_Leave(object sender, EventArgs e)
        {
            var slot = AbilitySlotListBox.SelectedIndex;
            var value = (AbilityLevelListBox.SelectedValue == null) ? (byte)255 : (byte)AbilityLevelListBox.SelectedValue;
            Program.LoadedData.AvatarAbilities[slot].Level = value;
            if (value == 255)
            {
                Program.LoadedData.AvatarAbilities[slot].Type = value;
                Program.LoadedData.AvatarAbilities[slot].SubType = value;
            }
        }

        private void FreeAbilityPointsTextBox_Leave(object sender, EventArgs e)
        {
            try
            {
                Program.LoadedData.AvatarFreeAbilityPoints = Convert.ToInt16(FreeAbilityPointsTextBox.Text);
            }
            catch
            {
                MessageBox.Show("Please enter a number in a range [0, 32767].", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
