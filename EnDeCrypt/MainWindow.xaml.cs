using System;
using System.Text;
using System.Windows;
using System.Windows.Media;
using EnDeCrypt.Services;

namespace EnDeCrypt;

/// <summary>
///     Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private const string KeyIvErrorMessage = "Key or IV is not the required amount of characters";
    private const string InputBoxErrorMessage = "Input box can not be empty";
    private const string DataCopiedMessage = "Data copied to your clipboard";
    private const string DataToEncryptLabel = "Data to Encrypt";
    private const string DataToDecryptLabel = "Data to Decrypt";
    
    public MainWindow()
    {
        InitializeComponent();
    }

    private void GoBtn(object sender, RoutedEventArgs e)
    {
        var validInputs = CheckInputs();
        if (!validInputs) return;
        var (key, iv, input) = ProcessInputs();

        // Encrypt
        if (EncryptRb.IsChecked == true)
            Clipboard.SetText(Convert.ToBase64String(Encryption.EncryptStringToBytes(input, key, iv)));

        // Decrypt
        if (DecryptRb.IsChecked == true)
            Clipboard.SetText(Decryption.DecryptStringFromBytes(Convert.FromBase64String(input), key, iv));

        MessageBox.Show(DataCopiedMessage);
    }

    private bool CheckInputs()
    {
        if (KeyPassBox.Password.Length != 32 || IvPassBox.Password.Length != 16)
        {
            MessageBox.Show(KeyIvErrorMessage);
            return false;
        }

        if (!string.IsNullOrEmpty(InputBox.Text)) return true;
        MessageBox.Show(InputBoxErrorMessage);
        return false;
    }

    private (byte[] key, byte[] iv, string input) ProcessInputs()
    {
        var key = Encoding.ASCII.GetBytes(KeyPassBox.Password);
        var iv = Encoding.ASCII.GetBytes(IvPassBox.Password);
        var input = InputBox.Text;
        return (key, iv, input);
    }

    private void EncryptRbChecked(object sender, RoutedEventArgs e)
    {
        InputLabel.Content = DataToEncryptLabel;
        InputBox.IsEnabled = true;
        InputBox.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
    }

    private void DecryptRbChecked(object sender, RoutedEventArgs e)
    {
        InputLabel.Content = DataToDecryptLabel;
        InputBox.IsEnabled = true;
        InputBox.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
    }
}
