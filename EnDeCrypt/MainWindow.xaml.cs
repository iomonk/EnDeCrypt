using System;
using System.Text;
using System.Windows;
using EnDeCrypt.Services;

namespace EnDeCrypt;

/// <summary>
///     Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private const string ErrorMessage =
        "Key or IV is not the required amount of characters. Input box can not be empty.";
    
    public MainWindow()
    {
        InitializeComponent();
    }

    private void EncryptBtn(object sender, RoutedEventArgs e)
    {
        var validInputs = CheckInputs();
        if (!validInputs) return;
        var (key, iv, input) = ProcessInputs();

        OutputBox.Text = Convert.ToBase64String(Encryption.EncryptStringToBytes(input, key, iv));
    }

    private void DecryptBtn(object sender, RoutedEventArgs e)
    {
        var validInputs = CheckInputs();
        if (!validInputs) return;
        var (key, iv, input) = ProcessInputs();

        OutputBox.Text = Decryption.DecryptStringFromBytes(Convert.FromBase64String(input), key, iv);
    }

    private bool CheckInputs()
    {
        if (KeyPassBox.Password.Length == 32 || IvPassBox.Password.Length == 16 ||
            !string.IsNullOrEmpty(InputBox.Text)) return true;
        MessageBox.Show(ErrorMessage);
        return false;
    }

    private (byte[] key, byte[] iv, string input) ProcessInputs()
    {
        var key = Encoding.ASCII.GetBytes(KeyPassBox.Password);
        var iv = Encoding.ASCII.GetBytes(IvPassBox.Password);
        var input = InputBox.Text;
        return (key, iv, input);
    }
}