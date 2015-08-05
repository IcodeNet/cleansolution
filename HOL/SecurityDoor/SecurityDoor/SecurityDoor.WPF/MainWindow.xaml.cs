// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="">
//   
// </copyright>
// <summary>
//   Interaction logic for MainWindow.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SecurityDoor.WPF
{
    using System;
    using System.ComponentModel;
    using System.Media;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;

    using SecurityDoor.WPF.SecurityWeb;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        #region Constants and Fields

        /// <summary>
        ///   The card key.
        /// </summary>
        private string cardKey;

        /// <summary>
        ///   The locked flag.
        /// </summary>
        private bool locked;

        /// <summary>
        ///   The door open flag.
        /// </summary>
        private bool open;

        /// <summary>
        ///   The room number.
        /// </summary>
        private string roomNumber;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "MainWindow" /> class.
        /// </summary>
        public MainWindow()
        {
            this.InitializeComponent();
            this.DataContext = this;
            this.locked = true;
            var r = new Random();
            this.RoomNumber = r.Next(100).ToString();
            this.CardKey = Guid.NewGuid().ToString();
        }

        #endregion

        #region Events

        /// <summary>
        ///   The property changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Properties

        /// <summary>
        ///   Gets a value indicating whether CanOpen.
        /// </summary>
        public bool CanOpen
        {
            get
            {
                return this.DoorClosed && this.Unlocked;
            }
        }

        /// <summary>
        ///   Gets or sets CardKey.
        /// </summary>
        public string CardKey
        {
            get
            {
                return this.cardKey;
            }

            set
            {
                this.cardKey = value;
                this.NotifyChanged("CardKey");
            }
        }

        /// <summary>
        ///   Gets a value indicating whether DoorClosed.
        /// </summary>
        public bool DoorClosed
        {
            get
            {
                return !this.DoorOpened;
            }
        }

        /// <summary>
        ///   Gets or sets a value indicating whether Open.
        /// </summary>
        public bool DoorOpened
        {
            get
            {
                return this.open;
            }

            set
            {
                this.open = value;
                if (this.open)
                {
                    this.doorImage.Source =
                        new BitmapImage(
                            new Uri(@"pack://application:,,,/SecurityDoor.WPF;component/Images/OpenDoor.png"));
                    var soundPlayer = new SoundPlayer(Properties.Resources.Open);
                    soundPlayer.Play();
                }
                else
                {
                    this.doorImage.Source =
                        new BitmapImage(
                            new Uri(@"pack://application:,,,/SecurityDoor.WPF;component/Images/ClosedDoor.gif"));
                    var soundPlayer = new SoundPlayer(Properties.Resources.Close);
                    soundPlayer.Play();
                }

                this.NotifyChanged("DoorOpened");
                this.NotifyChanged("DoorClosed");
                this.NotifyChanged("CanOpen");
            }
        }

        /// <summary>
        ///   Gets or sets a value indicating whether Locked.
        /// </summary>
        public bool Locked
        {
            get
            {
                return this.locked;
            }

            set
            {
                this.locked = value;

                if (this.Locked)
                {
                    this.LED.Fill = Brushes.Red;
                }
                else
                {
                    var soundPlayer = new SoundPlayer(Properties.Resources.Unlock);
                    soundPlayer.Play();
                    this.LED.Fill = Brushes.GreenYellow;
                }

                this.NotifyChanged("Locked");
                this.NotifyChanged("Unlocked");
                this.NotifyChanged("CanOpen");
            }
        }

        /// <summary>
        ///   Gets or sets RoomNumber.
        /// </summary>
        public string RoomNumber
        {
            get
            {
                return this.roomNumber;
            }

            set
            {
                this.roomNumber = value;
                this.NotifyChanged("RoomNumber");
            }
        }

        /// <summary>
        ///   Gets a value indicating whether Unlocked.
        /// </summary>
        public bool Unlocked
        {
            get
            {
                return !this.Locked;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// The notify changed.
        /// </summary>
        /// <param name="property">
        /// The property.
        /// </param>
        public void NotifyChanged(string property)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Authorizes a card key
        /// </summary>
        private void AuthorizeCard()
        {
            var proxy = new ServiceClient();
            try
            {
                proxy.OpenDoorCompleted += this.OnOpenDoorCompleted;
                proxy.OpenDoorAsync(
                    new OpenDoor
                        {
                           CardKey = Guid.Parse(this.textKey.Text), RoomNumber = Int32.Parse(this.textRoom.Text) 
                        });

                proxy.Close();
            }
            catch (Exception ex)
            {
                proxy.Abort();
                MessageBox.Show("Unable to authorize card " + ex.Message);
            }
        }

        /// <summary>
        /// The button close click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The event args.
        /// </param>
        private void ButtonCloseClick(object sender, RoutedEventArgs e)
        {
            this.DoorOpened = false;
            this.Locked = true;
        }

        /// <summary>
        /// The button empty_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The event args.
        /// </param>
        private void ButtonEmptyClick(object sender, RoutedEventArgs e)
        {
            this.textKey.Text = Guid.Empty.ToString();
        }

        /// <summary>
        /// The button open_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The event args.
        /// </param>
        private void ButtonOpenClick(object sender, RoutedEventArgs e)
        {
            this.DoorOpened = true;
        }

        /// <summary>
        /// The button unlock_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The event args.
        /// </param>
        private void ButtonUnlockClick(object sender, RoutedEventArgs e)
        {
            this.AuthorizeCard();
        }

        /// <summary>
        /// The on open door completed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The event args.
        /// </param>
        private void OnOpenDoorCompleted(object sender, OpenDoorCompletedEventArgs e)
        {
            var result = e.Result.GetValueOrDefault(false);

            if (!result)
            {
                var soundPlayer = new SoundPlayer(Properties.Resources.Buzz);
                soundPlayer.Play();
                this.Locked = true;
            }
            else
            {
                this.locked = false;
            }
        }

        #endregion
    }
}