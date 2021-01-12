
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace System.Windows.Forms
{
    public class TextInputDialog : CommonDialog
    {

        Form form;
        Label label;
        TextBox textBox;
        Button buttonOK;
        Button buttonCancel;

        public string Text { get { return textBox.Text; } set { textBox.Text = value; } }
        public string Label { get { return label.Text; } set { label.Text = value; } }
        public string Title { get { return form.Text; } set { form.Text = value; } }

        public TextInputDialog()
		{
            form = new Form();
            label = new Label();
			textBox = new TextBox();
			buttonOK = new Button();
            buttonCancel = new Button();

            buttonOK.Text = "Save";
            buttonCancel.Text = "Cancel";
            buttonOK.DialogResult = DialogResult.OK;
            buttonCancel.DialogResult = DialogResult.Cancel;

			label.SetBounds(9, 20, 372, 13);
			textBox.SetBounds(12, 36, 372, 20);
			buttonOK.SetBounds(228, 72, 75, 23);
			buttonCancel.SetBounds(309, 72, 75, 23);

			label.AutoSize = true;
            textBox.Anchor = textBox.Anchor | AnchorStyles.Right;
            buttonOK.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            buttonCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

            form.ClientSize = new Size(396, 107);
            form.Controls.AddRange(new Control[] { label, textBox, buttonOK, buttonCancel });
            form.ClientSize = new Size(Math.Max(300, label.Right + 10), form.ClientSize.Height);
            form.FormBorderStyle = FormBorderStyle.FixedDialog;
            form.StartPosition = FormStartPosition.CenterScreen;
            form.MinimizeBox = false;
            form.MaximizeBox = false;
            form.AcceptButton = buttonOK;
            form.CancelButton = buttonCancel;
        }

		//public DialogResult ShowDialog()
		//{
		//	DialogResult dialogResult = form.ShowDialog();
		//	return dialogResult;
		//}

		public override void Reset()
		{
			throw new NotImplementedException();
		}

		protected override bool RunDialog(IntPtr hwndOwner)
		{
			throw new NotImplementedException();
		}
	}
}
