using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Security;
using System.Runtime.InteropServices;
using Microsoft.Samples;

namespace FormPlay
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.ReturnedButtonLabel.Text = string.Empty;
            if (!TaskDialog.IsAvailableOnThisOS)
            {
                this.TaskDialogButton.Enabled = false;
                this.ReturnedButtonLabel.Text = "Requires OS version " + TaskDialog.RequiredOSVersion + " or later.";
            }
            this.progressWithTimerCheckBox.Checked = true;
            this.UpdateEnabledState();
        }

        private void TaskDialogButton_Click(object sender, EventArgs e)
        {
            TaskDialog taskDialog = new TaskDialog();

            taskDialog.WindowTitle = this.windowTitle.Text;
            taskDialog.MainInstruction = this.mainInstructionTextBox.Text;
            taskDialog.Content = this.contentTextBox.Text;
            taskDialog.Footer = this.footerTextBox.Text;
            taskDialog.ExpandedInformation = this.expandedInfoTextBox.Text;

            // Common buttons
            TaskDialogCommonButtons commonButtons = 0;
            if (this.okCheckBox.Checked) { commonButtons |= TaskDialogCommonButtons.Ok; }
            if (this.yesCheckBox.Checked) { commonButtons |= TaskDialogCommonButtons.Yes; }
            if (this.NoCheckBox.Checked) { commonButtons |= TaskDialogCommonButtons.No; }
            if (this.cancelCheckBox.Checked) { commonButtons |= TaskDialogCommonButtons.Cancel; }
            if (this.retryCheckBox.Checked) { commonButtons |= TaskDialogCommonButtons.Retry; }
            if (this.closeCheckBox.Checked) { commonButtons |= TaskDialogCommonButtons.Close; }
            taskDialog.CommonButtons = commonButtons;

            // Custom Buttons
            List<TaskDialogButton> customButtons = new List<TaskDialogButton>();
            if (!string.IsNullOrEmpty(this.buttonIDTextBox1.Text) &&
                !string.IsNullOrEmpty(this.buttonTextBox1.Text))
            {
                try
                {
                    TaskDialogButton button = new TaskDialogButton();
                    button.ButtonId = Convert.ToInt32(this.buttonIDTextBox1.Text, 10);
                    button.ButtonText = this.buttonTextBox1.Text;
                    customButtons.Add(button);
                }
                catch (FormatException)
                {
                }
            }

            if (!string.IsNullOrEmpty(this.buttonIDTextBox2.Text) &&
                !string.IsNullOrEmpty(this.buttonTextBox2.Text))
            {
                try
                {
                    TaskDialogButton button = new TaskDialogButton();
                    button.ButtonId = Convert.ToInt32(this.buttonIDTextBox2.Text, 10);
                    button.ButtonText = this.buttonTextBox2.Text;
                    customButtons.Add(button);
                }
                catch (FormatException)
                {
                }
            }

            if (!string.IsNullOrEmpty(this.buttonIDTextBox3.Text) &&
                !string.IsNullOrEmpty(this.buttonTextBox3.Text))
            {
                try
                {
                    TaskDialogButton button = new TaskDialogButton();
                    button.ButtonId = Convert.ToInt32(this.buttonIDTextBox3.Text, 10);
                    button.ButtonText = this.buttonTextBox3.Text;
                    customButtons.Add(button);
                }
                catch (FormatException)
                {
                }
            }

            if (customButtons.Count > 0)
            {
                taskDialog.Buttons = customButtons.ToArray();
            }

            // DefaultButton
            if (!string.IsNullOrEmpty(this.defaultButtonTextBox.Text))
            {
                try
                {
                    taskDialog.DefaultButton = Convert.ToInt32(this.defaultButtonTextBox.Text, 10);
                }
                catch (FormatException)
                {
                }
            }

            // Radio Buttons
            List<TaskDialogButton> customRadioButtons = new List<TaskDialogButton>();
            if (!string.IsNullOrEmpty(this.radioButtonIDTextBox1.Text) &&
                !string.IsNullOrEmpty(this.radioButtonTextBox1.Text))
            {
                try
                {
                    TaskDialogButton button = new TaskDialogButton();
                    button.ButtonId = Convert.ToInt32(this.radioButtonIDTextBox1.Text, 10);
                    button.ButtonText = this.radioButtonTextBox1.Text;
                    customRadioButtons.Add(button);
                }
                catch (FormatException)
                {
                }
            }

            if (!string.IsNullOrEmpty(this.radioButtonIDTextBox2.Text) &&
                !string.IsNullOrEmpty(this.radioButtonTextBox2.Text))
            {
                try
                {
                    TaskDialogButton button = new TaskDialogButton();
                    button.ButtonId = Convert.ToInt32(this.radioButtonIDTextBox2.Text, 10);
                    button.ButtonText = this.radioButtonTextBox2.Text;
                    customRadioButtons.Add(button);
                }
                catch (FormatException)
                {
                }
            }

            if (!string.IsNullOrEmpty(this.radioButtonIDTextBox3.Text) &&
                !string.IsNullOrEmpty(this.radioButtonTextBox3.Text))
            {
                try
                {
                    TaskDialogButton button = new TaskDialogButton();
                    button.ButtonId = Convert.ToInt32(this.radioButtonIDTextBox3.Text, 10);
                    button.ButtonText = this.radioButtonTextBox3.Text;
                    customRadioButtons.Add(button);
                }
                catch (FormatException)
                {
                }
            }

            if (customRadioButtons.Count > 0)
            {
                taskDialog.RadioButtons = customRadioButtons.ToArray();
            }

            // DefaultRadioButton
            if (!string.IsNullOrEmpty(this.defaultRadioButtonTextBox.Text))
            {
                try
                {
                    taskDialog.DefaultRadioButton = Convert.ToInt32(this.defaultRadioButtonTextBox.Text, 10);
                }
                catch (FormatException)
                {
                }
            }

            // Main Icon
            if (this.informationRadioButton.Checked)
            {
                taskDialog.MainIcon = TaskDialogIcon.Information;
            }
            if (this.warningRadioButton.Checked)
            {
                taskDialog.MainIcon = TaskDialogIcon.Warning;
            }
            if (this.errorRadioButton.Checked)
            {
                taskDialog.MainIcon = TaskDialogIcon.Error;
            }
            if (this.shieldRadioButton.Checked)
            {
                taskDialog.MainIcon = TaskDialogIcon.Shield;
            }

            // Footer Icon
            if (this.footerIconInfo.Checked)
            {
                taskDialog.FooterIcon = TaskDialogIcon.Information;
            }
            if (this.footerIconWarning.Checked)
            {
                taskDialog.FooterIcon = TaskDialogIcon.Warning;
            }
            if (this.footerIconError.Checked)
            {
                taskDialog.FooterIcon = TaskDialogIcon.Error;
            }
            if (this.footerIconShield.Checked)
            {
                taskDialog.FooterIcon = TaskDialogIcon.Shield;
            }

            taskDialog.EnableHyperlinks = this.enableHyperlinksCheckBox.Checked;
            taskDialog.ShowProgressBar = this.showProgressBarCheckBox.Checked;
            taskDialog.AllowDialogCancellation = this.allowCancelCheckBox.Checked;
            taskDialog.CallbackTimer = this.progressWithTimerCheckBox.Checked;
            taskDialog.ExpandedByDefault = this.expandedByDefaultCheckBox.Checked;
            taskDialog.ExpandFooterArea = this.expandedFooterCheckBox.Checked;
            taskDialog.PositionRelativeToWindow = this.positionRelativeToWindowCheckBox.Checked;
            taskDialog.RightToLeftLayout = this.RightToLeftLayoutCheckbox.Checked;
            taskDialog.NoDefaultRadioButton = this.NoDefaultRadioButtonCheckBox.Checked;
            taskDialog.CanBeMinimized = this.CanBeMinimizedCheckBox.Checked;
            taskDialog.ShowMarqueeProgressBar = this.showMarqueeCheckBox.Checked;
            taskDialog.UseCommandLinks = this.UseCommandLinksCheckBox.Checked;
            taskDialog.UseCommandLinksNoIcon = this.useCommandLinksNoIconCheckBox.Checked;
            taskDialog.VerificationText = this.verficationFlagTextBox.Text;
            taskDialog.VerificationFlagChecked = this.verifyFlagCheckBox.Checked;
            taskDialog.ExpandedControlText = this.expandedControlTextBox.Text;
            taskDialog.CollapsedControlText = this.collapsedControlTextBox.Text;

            taskDialog.Callback = new TaskDialogCallback(this.TaskDialogCallback);

            //
            // Show the Dialog
            //
            bool verification = false;
            int radioButtonResult;
            DialogResult result = (DialogResult)taskDialog.Show((taskDialog.CanBeMinimized ? null : this), out verification, out radioButtonResult);

            this.ReturnedButtonLabel.Text = "Button Selected: " + result.ToString() +
                "   Verification: " + (verification ? "checked" : "clear") +
                "   Radio Button: " + radioButtonResult.ToString();
        }

        private bool TaskDialogCallback(ActiveTaskDialog taskDialog, TaskDialogNotificationArgs args, object callbackData)
        {
            if (args.Notification == TaskDialogNotification.Created &&
                this.showMarqueeCheckBox.Checked)
            {
                taskDialog.SetProgressBarMarquee(true, 0);
                return false;
            }

            if (args.Notification == TaskDialogNotification.Timer)
            {                
                if (this.showProgressBarCheckBox.Checked)
                {
                    if (args.TimerTickCount < 10000)
                    {
                        taskDialog.SetProgressBarPosition(((int)args.TimerTickCount) / 100);
                    }
                    else if (args.TimerTickCount < 11000)
                    {
                        // Done
                        taskDialog.SetProgressBarPosition(100);
                    }
                    else if (args.TimerTickCount < 12000 && this.autoCancelCheckBox.Checked)
                    {
                        taskDialog.ClickButton((int)DialogResult.Cancel);
                    }
                    else
                    {
                        return true; // reset timer
                    }
                }
                return false;
            }

            if (args.Notification == TaskDialogNotification.HyperlinkClicked)
            {
                if (args.Hyperlink.StartsWith("http:", StringComparison.InvariantCultureIgnoreCase))
                {
                    System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo();
                    psi.FileName = args.Hyperlink;
                    psi.UseShellExecute = true;
                    System.Diagnostics.Process.Start(psi);
                }
                else
                {
                    MessageBox.Show(
                        (IWin32Window)taskDialog,
                        args.Notification.ToString() + "\n" +
                        "Hyperlink: " + args.Hyperlink,
                        "Got callback");
                }
            }

            return false;
        }

        private void SampleUsage()
        {
            TaskDialog taskDialog = new TaskDialog();
            taskDialog.WindowTitle = "My Application";
            taskDialog.MainInstruction = "Do you want to do this?";
            taskDialog.CommonButtons = TaskDialogCommonButtons.Yes | TaskDialogCommonButtons.No;
            int result = taskDialog.Show();
            if (result == (int)DialogResult.Yes)
            {
                // Do it.
            }
        }

        private void SampleUsageComplex()
        {
            TaskDialog taskDialog = new TaskDialog();
            taskDialog.WindowTitle = "My Application";
            taskDialog.MainInstruction = "Do you want to do this?";

            taskDialog.EnableHyperlinks = true;
            taskDialog.Content = "If you do this there could be all sorts of consequnces. " +
                "If you don't there will be other consequences. " +
                "You can <A HREF=\"Learn\">learn more about those consequences </A> or more " +
                "about <A HREF=\"blah\">blah blah blah</A>.";
            taskDialog.Callback = new TaskDialogCallback(this.MyTaskDialogCallback);
            taskDialog.VerificationText = "Don't ask me this ever again.";
            taskDialog.VerificationFlagChecked = false;

            TaskDialogButton doItButton = new TaskDialogButton();
            doItButton.ButtonId = 101;
            doItButton.ButtonText = "Do It";

            TaskDialogButton dontDoItButton = new TaskDialogButton();
            dontDoItButton.ButtonId = 102;
            dontDoItButton.ButtonText = "Don't Do It";

            taskDialog.Buttons = new TaskDialogButton[] { doItButton, dontDoItButton };

            bool dontShowAgain;
            int result = taskDialog.Show(null, out dontShowAgain);
            if (dontShowAgain)
            {
                // Suppress future asks.
            }
            if (result == doItButton.ButtonId)
            {
                // Do it.
            }
        }

        private bool MyTaskDialogCallback(
            ActiveTaskDialog taskDialog, 
            TaskDialogNotificationArgs args, 
            object callbackData)
        {
            if (args.Notification == TaskDialogNotification.HyperlinkClicked)
            {
                if (args.Hyperlink.Equals("Learn", StringComparison.Ordinal))
                {
                    // Show a help topic.
                }
                else if (args.Hyperlink.Equals("blah", StringComparison.Ordinal))
                {
                    // Show a different help topic.
                }
            }
            return false;
        }



        private void UpdateEnabledState()
        {
            this.progressWithTimerCheckBox.Enabled = this.showProgressBarCheckBox.Checked;
            this.autoCancelCheckBox.Enabled = this.showProgressBarCheckBox.Checked &&
                this.progressWithTimerCheckBox.Checked;
        }

        private void showProgressBarCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            this.UpdateEnabledState();
        }

        private void progressWithTimerCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            this.UpdateEnabledState();
        }

    }
}