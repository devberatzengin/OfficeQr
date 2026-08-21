using QRCoder;

namespace OfficeQr.Helpers;



public class QrCodeGenerator
{
    public string CreateQr<T>(T type) where T : class
    {
        
        if (type is null)
            throw new ArgumentNullException($"{nameof(type)} object is null");

        string className = typeof(T).Name;
        var idProperty = typeof(T).GetProperty("Id")!.GetValue(type)!.ToString();
        if (idProperty == null)
            throw new InvalidOperationException($"Not found id property in {className} class ");

        using var qrGenerator = new QRCodeGenerator();
    
        using var qrCodeData = qrGenerator.CreateQrCode($"WMS:{className.ToUpper()}:{idProperty}", QRCodeGenerator.ECCLevel.Q);
        
        using var qrCode = new PngByteQRCode(qrCodeData);
        byte[] qrCodeBytes = qrCode.GetGraphic(20);
        
        string base64Image = Convert.ToBase64String(qrCodeBytes);

        return base64Image;
    }
}