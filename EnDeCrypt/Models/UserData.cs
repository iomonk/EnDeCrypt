namespace EnDeCrypt.Models
{
    public class UserData
    {
        public string Input { get; set; }
        public byte[] Key { get; set; }
        public byte[] Iv { get; set; }
        public string DataToEncrypt { get; set; }
        public string DataToDecrypt { get; set; }
        public string ErrorMessage { get; set; }
    }
}