using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Metadata.Profiles.Exif;
using Image = SixLabors.ImageSharp.Image;

namespace Image_Metadata_Removal
{
    public static class Program
    {
        private static void Main(string[] args)
        {
            string[] imageExtensions = [".jpg", ".jpeg", ".png", ".bmp", ".gif", ".tiff"];
            string currentDirectory = Directory.GetCurrentDirectory();

            if (args.Length > 0)
            {
                string fileToModify = args[0].Trim();

                if (imageExtensions.Contains(Path.GetExtension(fileToModify).ToLower()))
                {
                    if (File.Exists(fileToModify))
                    {
                        ProcessImage(fileToModify, currentDirectory);
                        MessageBox.Show("The image you set has been cleaned and is safe to be uploaded to the web.", "Data cleared up!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else MessageBox.Show($"The file you speak of does not exist!");
                }
                else MessageBox.Show($"That file is un-supported file format. Please upload an image with either the following extensions: \n\n" + string.Join(", ", imageExtensions));
            }
            else
            {
                DialogResult result = MessageBox.Show("This program will wipe all EXIF data from every image in this current directory.", "Do you wish to proceed?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (result == DialogResult.Yes)
                {
                    var imageFiles = Directory.GetFiles(currentDirectory, "*.*", SearchOption.TopDirectoryOnly)
                    .Where((file) => imageExtensions.Contains(Path.GetExtension(file).ToLower()));

                    foreach (string inputPath in imageFiles) ProcessImage(inputPath, currentDirectory);
                    MessageBox.Show("All images EXIF data cleaned up!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else MessageBox.Show("Ok, no changes were made to any images.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private static void ProcessImage(string inputPath, string currentDirectory)
        {
            string outputPath = Path.Combine(currentDirectory, $"{Path.GetFileNameWithoutExtension(inputPath)}_cleaned{Path.GetExtension(inputPath)}");

            try
            {
                using Image image = Image.Load(inputPath);
                image.Metadata.ExifProfile = null;
                image.Metadata.IptcProfile = null;
                image.Metadata.XmpProfile = null;
                ExifProfile exifProfile = new();
                exifProfile.SetValue(ExifTag.XPTitle, "EXIF data cleaned for security and privacy purposes.");
                image.Metadata.ExifProfile = exifProfile;
                image.Save(outputPath);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while processing {Path.GetFileNameWithoutExtension(inputPath)}\n{ex.Message}");
            }
        }
    }
};