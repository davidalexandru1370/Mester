namespace Registry.utils
{
    public class Base64Stream
    {
        byte[] data;

        public Base64Stream(string base64Data)
        {
            this.data = Convert.FromBase64String(base64Data);
        }

        public Stream AsStream()
        {
            return new MemoryStream(data);
        }
    }
}
