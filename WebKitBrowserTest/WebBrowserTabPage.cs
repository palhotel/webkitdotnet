/*
 * Copyright (c) 2009, Peter Nelson (charn.opcode@gmail.com)
 * All rights reserved.
 * 
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions are met:
 * 
 * * Redistributions of source code must retain the above copyright notice, 
 *   this list of conditions and the following disclaimer.
 * * Redistributions in binary form must reproduce the above copyright notice, 
 *   this list of conditions and the following disclaimer in the documentation 
 *   and/or other materials provided with the distribution.
 *   
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
 * AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE 
 * IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE 
 * ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE 
 * LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR 
 * CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF 
 * SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS 
 * INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN 
 * CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) 
 * ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE 
 * POSSIBILITY OF SUCH DAMAGE.
*/

using System;                           /* {@@} */
using System.ComponentModel;            /* {@@} */
using System.Runtime.InteropServices;   /* {@@} */
using System.Windows.Forms;
using WebKit;

namespace WebKitBrowserTest
{
    public class TestScriptObject
    {
        public void f()
        {
            MessageBox.Show("Hey!");
        }
    }

    public partial class WebBrowserTabPage : TabPage
    {
        public WebKitBrowser browser;
        
        private StatusStrip statusStrip;
        private ToolStripLabel statusLabel;
        private ToolStripLabel iconLabel;
        private ToolStripProgressBar progressBar;
        private ToolStripContainer container;
        
        public WebBrowserTabPage() : this(new WebKitBrowser(), true)
        {
        }
        
        public WebBrowserTabPage(WebKitBrowser browserControl, bool goHome)
        {
            InitializeComponent();

            statusStrip = new StatusStrip();
            statusStrip.Name = "statusStrip";
            statusStrip.Visible = true;
            statusStrip.SizingGrip = false;

            container = new ToolStripContainer();
            container.Name = "container";
            container.Visible = true;
            container.Dock = DockStyle.Fill;

            statusLabel = new ToolStripLabel();
            statusLabel.Name = "statusLabel";
            statusLabel.Text = "Done";
            statusLabel.Visible = true;

            iconLabel = new ToolStripLabel();
            iconLabel.Name = "iconLabel";
            iconLabel.Text = "No Icon";
            iconLabel.Visible = true;

            progressBar = new ToolStripProgressBar();
            progressBar.Name = "progressBar";
            progressBar.Visible = true;

            statusStrip.Items.Add(statusLabel);
            statusStrip.Items.Add(iconLabel);
            statusStrip.Items.Add(progressBar);

            container.BottomToolStripPanel.Controls.Add(statusStrip);

            // create webbrowser control
            browser = browserControl;
            browser.Visible = true;
            browser.Dock = DockStyle.Fill;
            browser.Name = "browser";
            browser.IsWebBrowserContextMenuEnabled = false; /* {@@} */
            //browser.IsScriptingEnabled = false;
            container.ContentPanel.Controls.Add(browser);

            browser.ObjectForScripting = new TestScriptObject();

            // context menu

            this.Controls.Add(container);
            this.Text = "<New Tab>";

            // events
            browser.DocumentTitleChanged += (s, e) => this.Text = browser.DocumentTitle;
            browser.Navigating += (s, e) => statusLabel.Text = "Loading...";
            browser.Navigated += (s, e) => { statusLabel.Text = "Downloading..."; };
            browser.DocumentCompleted += (s, e) => { statusLabel.Text = "Done"; };
            browser.ProgressStarted += (s, e) => { progressBar.Visible = true; };
            browser.ProgressChanged += (s, e) => { progressBar.Value = e.ProgressPercentage; };
            browser.ProgressFinished += (s, e) => { progressBar.Visible = false; };
            if (goHome)
                browser.Navigate("http://www.google.com");

            browser.ShowJavaScriptAlertPanel += (s, e) => MessageBox.Show(e.Message, "[JavaScript Alert]");
            browser.ShowJavaScriptConfirmPanel += (s, e) =>
            {
                e.ReturnValue = MessageBox.Show(e.Message, "[JavaScript Confirm]", MessageBoxButtons.YesNo) == DialogResult.Yes;
            };
            browser.ShowJavaScriptPromptPanel += (s, e) =>
            {
                var frm = new JSPromptForm(e.Message, e.DefaultValue);
                if (frm.ShowDialog() == DialogResult.OK)
                    e.ReturnValue = frm.Value;
            };

            /* {@@} */
            if (!browser.IsWebBrowserContextMenuEnabled)
            {
                CreateContextMenu();
             // ContextMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(ContextMenuStrip_Opening);
                browser.ContextMenuOpen += new EventHandler(ContextMenuStrip_Opening2);
            }
            /* {@@} */
        }

        /* {@@} */
        #region Original ContextMenu of this browser
        #region CreateContextMenu
        private void AddContextMenu(
            string textString, string targetString, Keys shortCutKey)
        {
            ToolStripMenuItem item = new ToolStripMenuItem();
            item.Text = textString;
            item.Tag = targetString;
            item.ShortcutKeys = shortCutKey;
            item.Click += new EventHandler(ContextMenu_Click);
            ContextMenuStrip.Items.Add(item);
        }

        private void AddSeparator()
        {
            ContextMenuStrip.Items.Add(new ToolStripSeparator());
        }

        public void CreateContextMenu()
        {
            if (ContextMenuStrip != null)
                ContextMenuStrip.Dispose();

            ContextMenuStrip = new ContextMenuStrip();
            ContextMenuStrip.AllowMerge = true;

            AddContextMenu(
                "test",
                "Test",
                0);
            AddSeparator();
            AddContextMenu(
                "Close tab",
                "CloseTab",
                0);
            AddContextMenu(
                "Forward",
                "GoForward",
                0);
            AddContextMenu(
                "Back",
                "GoBack",
                0);
            AddContextMenu(
                "Reload",
                "Reload",
                0);
            AddSeparator();
            AddContextMenu(
                "Properties",
                "ShowPropertiesDialog",
                Keys.Control | Keys.P);
            AddContextMenu(
                "View source",
                "ViewSource",
                Keys.Control | Keys.S);
            AddContextMenu(
                "Print",
                "Print",
                Keys.Control | Keys.I);
            AddContextMenu(
                "Preview print",
                "PrintPreview",
                Keys.Control | Keys.N);
        }
        #endregion

        #region DisposeContextMenu
        public void DisposeContextMenu()
        {
            if (browser.IsWebBrowserContextMenuEnabled)
            {
             // ContextMenuStrip.Opening -= new CancelEventHandler(ContextMenuStrip_Opening);
                browser.ContextMenuOpen -= new EventHandler(ContextMenuStrip_Opening2);
            }
        }
        #endregion

        #region Event handler for ContextMenu
        void ContextMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            int count = ContextMenuStrip.Items.Count;
            for (int i = 0; i < count; i++)
            {
                ToolStripItem item = ContextMenuStrip.Items[i];

                item.Enabled = true;
                item.Visible = true;

                if (item.Text == "Properties")
                {
                    item.Enabled = false;
                    item.Visible = true;
                }

                if (browser.DocumentText == string.Empty)
                {
                    if (item.Text == "Reload" ||
                        item.Text == "View source" ||
                        item.Text == "Print" ||
                        item.Text == "Preview print")
                    {
                        item.Enabled = false;
                        item.Visible = false;
                    }
                }

                if (!browser.CanGoBack)
                {
                    if (item.Text == "Back")
                    {
                        item.Enabled = false;
                        item.Visible = false;
                    }
                }

                if (!browser.CanGoForward)
                {
                    if (item.Text == "Forward")
                    {
                        item.Enabled = false;
                        item.Visible = false;
                    }
                }
            }
        }

        void ContextMenuStrip_Opening2(object sender, EventArgs e)
        {
            CancelEventArgs ce = new CancelEventArgs(false);
            ContextMenuStrip_Opening(sender, ce);
            if (ce.Cancel == false)
            {
                int x = Cursor.Position.X;
                int y = Cursor.Position.Y;
                ContextMenuStrip.Show(x + 10, y + 10);
            }
        }

        void ContextMenu_Click(object sender, EventArgs e)
        {
            string value = (string)(((ToolStripMenuItem)sender).Tag);
            if (value == "Test")
                MessageBox.Show("test!!!", "via Original ContextMenu");
            else if (value == "ViewSource")
            {
                if (browser.DocumentText.Length > 0)
                {
                    SourceViewForm dlg = new SourceViewForm(browser.DocumentText, browser);
                    dlg.Show();
                }
            }
            else if (value == "Print")
            {
                if (browser.DocumentText.Length > 0)
                    browser.Print();
            }
            else if (value == "PrintPreview")
            {
                if (browser.DocumentText.Length > 0)
                    browser.ShowPrintPreviewDialog();
            }
            else if (value == "GoBack")
                browser.GoBack();
            else if (value == "GoForward")
                browser.GoForward();
            else if (value == "Reload")
                browser.Reload();
            else if (value == "CloseTab")
                OnWindowClosing(new EventArgs());
        }
        #endregion
        #endregion

        #region window.close()
        public enum GetWindowCmd
        {
            GW_HWNDFIRST = 0,
            GW_HWNDLAST = 1,
            GW_HWNDNEXT = 2,
            GW_HWNDPREV = 3,
            GW_OWNER = 4,
            GW_CHILD = 5,
            GW_ENABLEDPOPUP = 6
        }

        [DllImport("user32.dll")]
        extern public static IntPtr GetWindow(IntPtr hWnd, GetWindowCmd cmd);
        protected override void WndProc(ref Message m)
        {
            const uint WM_PARENTNOTIFY = 0x0210;
            const uint WM_DESTROY = 0x0002;

            if (m.Msg == WM_PARENTNOTIFY)
            {
                if (m.WParam.ToInt32() == WM_DESTROY)
                {
                    if (m.LParam == GetWindow(Handle, GetWindowCmd.GW_CHILD))
                    {
                        OnWindowClosing(new EventArgs());
                    }
                }
            }

            base.WndProc(ref m);
        }

        public event EventHandler WindowClosing;
        protected virtual void OnWindowClosing(EventArgs e)
        {
            if (WindowClosing != null)
                WindowClosing(this, e);
        }
        #endregion
        /* {@@} */

        public void Stop()
        {
            browser.Stop();
            statusLabel.Text = "Stopped";
        }
    }
}
