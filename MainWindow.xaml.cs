using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using NHotkey;
using NHotkey.Wpf;

namespace WWAutoClicker
{
    public partial class MainWindow : Window
    {
        private readonly Dictionary<string, KeyTimer> keyTimers;
        private readonly KeySimulator keySimulator;
        private CancellationTokenSource cts;
        private bool isrunning;

        public MainWindow()
        {
            InitializeComponent();
            keySimulator = new KeySimulator();
            keyTimers = new Dictionary<string, KeyTimer>();
            InitializeKeyTimers();
            cts = new CancellationTokenSource();

            // Register the hotkeys (numeric keypad minus key for stopping and multiply key for starting)
            HotkeyManager.Current.AddOrReplace("StopAll", Key.Subtract, ModifierKeys.None, OnStopAllHotkey);
            HotkeyManager.Current.AddOrReplace("StartAll", Key.Multiply, ModifierKeys.None, OnStartAllHotkey);
        }


        private void InitializeKeyTimers()
        {
            string[] keys = { "1", "2", "3", "Q", "E", "R", "F", "LeftMouse" };
            foreach (var key in keys)
            {
                keyTimers[key] = null;
            }
        }

        private void OnStopAllHotkey(object sender, HotkeyEventArgs e)
        {
            StopAllTimers();
            e.Handled = true;
            isrunning = false;
        }

        private void OnStartAllHotkey(object sender, HotkeyEventArgs e)
        {
            if (isrunning)
            {
                return;
            }

            StartAllTimers();
            e.Handled = true;
            isrunning = true;
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            if (isrunning)
            {
                return;
            }
            StartAllTimers();
            isrunning = true;
        }

        private void StartTimer(string key, string startText, string intervalText, string repeatText, bool isChecked)
        {
            if (isChecked && int.TryParse(startText, out int startTime) && int.TryParse(intervalText, out int intervalSeconds))
            {
                int repeat = string.IsNullOrEmpty(repeatText) ? int.MaxValue : int.Parse(repeatText);

                keyTimers[key] = new KeyTimer(key, repeat, intervalSeconds, keySimulator);
                keyTimers[key].Start(startTime);
            }
            else if (keyTimers[key] != null)
            {
                keyTimers[key].Stop();
                keyTimers[key] = null;
            }
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            StopAllTimers();
            isrunning = false;
        }

        private void StopAllTimers()
        {
            cts.Cancel();
            foreach (var key in keyTimers.Keys)
            {
                keyTimers[key]?.Stop();
            }
            cts = new CancellationTokenSource();
            Debug.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}: All timers stopped");
            isrunning = false;
        }

        private void StartAllTimers()
        {
            StartTimer("1", textBox1Start.Text, textBox1Interval.Text, textBox1Repeat.Text, checkBox1.IsChecked ?? false);
            StartTimer("2", textBox2Start.Text, textBox2Interval.Text, textBox2Repeat.Text, checkBox2.IsChecked ?? false);
            StartTimer("3", textBox3Start.Text, textBox3Interval.Text, textBox3Repeat.Text, checkBox3.IsChecked ?? false);
            StartTimer("Q", textBoxQStart.Text, textBoxQInterval.Text, textBoxQRepeat.Text, checkBoxQ.IsChecked ?? false);
            StartTimer("E", textBoxEStart.Text, textBoxEInterval.Text, textBoxERepeat.Text, checkBoxE.IsChecked ?? false);
            StartTimer("R", textBoxRStart.Text, textBoxRInterval.Text, textBoxRRepeat.Text, checkBoxR.IsChecked ?? false);
            StartTimer("F", textBoxFStart.Text, textBoxFInterval.Text, textBoxFRepeat.Text, checkBoxF.IsChecked ?? false);
            StartTimer("LeftMouse", textBoxLeftMouseStart.Text, textBoxLeftMouseInterval.Text, textBoxLeftMouseRepeat.Text, checkBoxLeftMouse.IsChecked ?? false);

            Debug.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}: All timers started");
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            // Logic for handling checkbox checked state if needed
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            // Logic for handling checkbox unchecked state if needed
        }

        private void CheckBoxSwap_Checked(object sender, RoutedEventArgs e)
        {
            checkBox1.IsChecked = true;
            checkBox2.IsChecked = true;
            checkBox3.IsChecked = true;
        }

        private void CheckBoxSwap_Unchecked(object sender, RoutedEventArgs e)
        {
            checkBox1.IsChecked = false;
            checkBox2.IsChecked = false;
            checkBox3.IsChecked = false;
        }
        
        private void CheckBoxBattle_Checked(object sender, RoutedEventArgs e)
        {
            checkBoxQ.IsChecked = true;
            checkBoxE.IsChecked = true;
            checkBoxR.IsChecked = true;
            checkBoxF.IsChecked = true;
            checkBoxLeftMouse.IsChecked = true;
        }

        private void CheckBoxBattle_Unchecked(object sender, RoutedEventArgs e)
        {
            checkBoxQ.IsChecked = false;
            checkBoxE.IsChecked = false;
            checkBoxR.IsChecked = false;
            checkBoxF.IsChecked = false;
            checkBoxLeftMouse.IsChecked = false;
        }
    }
}