﻿using System.Windows;
using System.Windows.Input;

namespace WPFForm
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void MouseHeader(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed) DragMove();
        }
        private void MainWindowsMinimals(object sender, RoutedEventArgs e) => Application.Current.MainWindow.WindowState = WindowState.Minimized;
        private void MainWindowExit(object sender, RoutedEventArgs e) => Application.Current.Shutdown();

        Program_DES des;
        private void ButtonEnc(object sender , RoutedEventArgs e)
        {
            des = new Program_DES(EncKey.Text);
            des.MetodEnc(EncText.Text);
        }

        private void ButtonDec(object sender , RoutedEventArgs e)
        {
            des = new Program_DES(DecKey.Text);
            des.MetodDec(DecText.Text);
        }
        
    }
}
