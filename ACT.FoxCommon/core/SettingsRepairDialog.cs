using System;
using System.Windows.Forms;
using ACT.FoxCommon.localization;

namespace ACT.FoxCommon.core
{
    public partial class SettingsRepairDialog : Form
    {
        public SettingsRepairDialog()
        {
            InitializeComponent();
            Text = LocalizationBase.GetString("actPanelTitle");
            LocalizationBase.TranslateControls(this);
        }

        public bool HasBackup
        {
            get => buttonRestoreFromBackup.Visible;

            set => buttonRestoreFromBackup.Visible = value;
        }

        public string Message
        {
            get => labelMessage.Text;
            set => labelMessage.Text = value;
        }

        public SettingsRepairAction RepairAction
        {
            get
            {
                switch (DialogResult)
                {
                    case DialogResult.Retry:
                        return SettingsRepairAction.Restore;
                    case DialogResult.Yes:
                        return SettingsRepairAction.Reset;
                    case DialogResult.Cancel:
                        return SettingsRepairAction.Ignore;
                    default:
                        throw new Exception();
                }
            }
        }

        private void buttonRestoreFromBackup_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Retry;
        }

        private void buttonResetToDefault_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Yes;
        }

        private void buttonIgnore_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        public enum SettingsRepairAction
        {
            Ignore,
            Restore,
            Reset
        }
    }
}
