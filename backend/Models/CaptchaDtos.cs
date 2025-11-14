namespace backend.Models
{
    public class ImageCaptchaDto
    {
        public List<ImageDto> Images { get; set; } = [];
        public string Challenge { get; set; } = string.Empty;
        public string EncryptedKey { get; set; } = string.Empty;
    }

    public class ImageDto
    {
        public int Index { get; set; }
        public string Base64Data { get; set; } = string.Empty;
    }

    public class VerifyCaptchaDto
    {
        public List<int> SelectedIndices { get; set; } = [];
        public string EncryptedKey { get; set; } = string.Empty;
    }
}
