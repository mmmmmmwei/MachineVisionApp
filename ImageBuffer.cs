namespace ConsoleApp1;

public class ImageBuffer
{
    private int _currentImage;

    // Store a copy of the image
    public void SetImage(int image)
    {
        // _currentImage?.Dispose();
        // _currentImage = (int)image.Clone();

        _currentImage=image;
    }

    // Retrieve a copy for processing
    public int GetImage()
    {
        // return _currentImage != null ? (int)_currentImage.Clone() : null;
        return _currentImage;
    }

    public bool HasImage => _currentImage != null;
}