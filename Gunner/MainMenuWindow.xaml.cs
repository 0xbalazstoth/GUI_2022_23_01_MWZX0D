using Gunner.Controller;
using Repository.Classes;
using Repository.Exceptions;
using SFML.Audio;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Gunner
{
    /// <summary>
    /// Interaction logic for MainMenuWindow.xaml
    /// </summary>
    

    public partial class MainMenuWindow : Window
    {
        bool isMuted;
        SoundBuffer music = new ("Assets/Sounds/menuMusic.ogg");
        public Sound sound;
        int volume = 100;

        public MainMenuWindow()
        {
            InitializeComponent();
            Show();
            PlayMainMenuMusic(music,100);
            
        }

        private void PlayMainMenuMusic(SoundBuffer music,int milisec)
        {
            Thread.Sleep(milisec);
            sound = new Sound(music);
            sound.Volume = volume;
            sound.Loop = true;
            sound.Play();
        }

        private void btnNewGame_Click(object sender, RoutedEventArgs e)
        {
            //NewGameWindow newGameWindow = new NewGameWindow();
            //newGameWindow.ShowDialog();
        }

        private void btnLoadGame_Click(object sender, RoutedEventArgs e)
        {
            SaveHandler saveHandler = new SaveHandler();
            

            try
            {
                //IEnumerable<string[]> saves = saveHandler.LoadSaves();

                //LoadSavedGameWindow loadSavedGameWindow = new LoadSavedGameWindow(saves);
                //loadSavedGameWindow.ShowDialog();
            }
            catch (NoSaveException error)
            {
                lblErrorLoad.Visibility = Visibility.Visible;
                lblErrorLoad.Text = error.Message;
            }


        }

        private void btnHighscore_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnQuitGame_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            // Check if F11 is pressed
            if (e.Key == Key.F11)
            {
                // Check if window is in fullscreen mode
                if (WindowState == WindowState.Maximized)
                {
                    // Set window to normal mode
                    WindowState = WindowState.Normal;
                    WindowStyle = WindowStyle.None;
                }
                else
                {
                    // Set window to fullscreen mode
                    WindowState = WindowState.Maximized;
                    WindowStyle = WindowStyle.None;
                }
            }
        }

        private void btnMultiMediaControll_Click(object sender, RoutedEventArgs e)
        {
            
            var brush = new ImageBrush();
            if (isMuted == false)
            {
                brush.ImageSource = ConvertUriPNG("unmute");
                btnMultiMediaControll.Background = brush;
                sound.Volume = 0;
                isMuted = true;
            }
            else
            {
                brush.ImageSource = ConvertUriPNG("mute");
                btnMultiMediaControll.Background = brush;
                sound.Volume = 100;
                isMuted = false;
            }

        }

        public static BitmapImage ConvertUriPNG(string asset)
        {
            return new BitmapImage(new Uri($"../../../Assets/Textures/{asset}.png", UriKind.Relative));
        }
    }
}
