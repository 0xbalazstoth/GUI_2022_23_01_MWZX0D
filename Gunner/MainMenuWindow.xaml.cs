using Gunner.Controller;
using Repository.Classes;
using Repository.Exceptions;
using SFML.Audio;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
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
        private SoundBuffer MainMenuSound = new(SoundBufferBase.mainMenuMusic);
        private SoundBuffer btnHoverSound = new(SoundBufferBase.buttonEnterSound);

        bool isMuted;
        public Sound sound;
        public Sound btnSound;
        int mainMenuVolume = 10;
        int mainMenuBtnVolume = 10;
        int resetVolume;

        public MainMenuWindow()
        {
            InitializeComponent();
            Show();
            PlayMainMenuMusic(MainMenuSound,true,100);
            
        }

        public static BitmapImage ConvertUriPNG(string asset)
        {
            return new BitmapImage(new Uri($"../../../Assets/Textures/{asset}.png", UriKind.Relative));
        }

        private void PlayMainMenuMusic(SoundBuffer music, bool loop,int milisec = 0)
        {
            Task.Delay(milisec);
            sound = new Sound(music);
            sound.Volume = mainMenuVolume;
            sound.Loop = loop;
            sound.Play();
        }

        private void PlayMouseEnterSound(SoundBuffer music)
        {
            btnSound = new Sound(music);
            btnSound.Volume = mainMenuBtnVolume;
            btnSound.Play();
        }

        private void btnNewGame_Click(object sender, RoutedEventArgs e)
        {
            lblErrorLoad.Visibility = Visibility.Hidden;
            NewGameWindow newGameWindow = new NewGameWindow();
            newGameWindow.ShowDialog();
        }

        private void btnLoadGame_Click(object sender, RoutedEventArgs e)
        {
            lblErrorLoad.Visibility = Visibility.Hidden;
            SaveHandler saveHandler = new SaveHandler();

            try
            {
                string[] saves = saveHandler.LoadSaves();

                LoadSavedGameWindow loadSavedGameWindow = new LoadSavedGameWindow(saves);
                loadSavedGameWindow.ShowDialog();
            }
            catch (NoSaveException error)
            {
                lblErrorLoad.Visibility = Visibility.Visible;
                lblErrorLoad.Text = error.Message;

            }


        }
        private void btnOptions_Click(object sender, RoutedEventArgs e)
        {
            lblErrorLoad.Visibility = Visibility.Hidden;
            GameOptionsWindow gameOptionsWindow = new GameOptionsWindow();
            gameOptionsWindow.ShowDialog();
        }

        private void btnHighscore_Click(object sender, RoutedEventArgs e)
        {
            lblErrorLoad.Visibility = Visibility.Hidden;
        }

        private void btnQuitGame_Click(object sender, RoutedEventArgs e)
        {
            lblErrorLoad.Visibility = Visibility.Hidden;
            Environment.Exit(0);
        }

        private void btnMultiMediaControll_Click(object sender, RoutedEventArgs e)
        {
            lblErrorLoad.Visibility = Visibility.Hidden;
            var brush = new ImageBrush();
            if (isMuted == false)
            {
                brush.ImageSource = ConvertUriPNG("unmute");
                btnMultiMediaControll.Background = brush;
                resetVolume = (int)sound.Volume;
                sound.Volume = 0;
                isMuted = true;
            }
            else
            {
                brush.ImageSource = ConvertUriPNG("mute");
                btnMultiMediaControll.Background = brush;
                sound.Volume = resetVolume;
                isMuted = false;
            }

        }

        private void gridMainMenu_MouseEnter(object sender, MouseEventArgs e)
        {
            PlayMouseEnterSound(btnHoverSound);
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            lblErrorLoad.Visibility = Visibility.Hidden;
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
    }
}
