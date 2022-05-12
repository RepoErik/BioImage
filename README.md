![alt text](https://github.com/RepoErik/BioImage/blob/master/banner.bmp?raw=true)

# BioImage Library

A .NET Library for opening & annotating various microscopy imaging formats. Supports all bioformats supported images like .TIF, .CZI etc. 
Best for working with ROI's in OME format & CSV.

## Features


- Open & Save ImageJ Tiff files and embed ROI's in image Description tag.

- C# scripting with sample tool-script and other sample scripts in "/Scripts/" folder. [See samples](https://github.com/BioMicroscopy/BioImage-Scripts)

- RGB image viewing mode which automatically combines 3 channels into RGB image & shows ROI from each channel which can be configured in ROI Manager.

- Viewing image stacks with scroll wheel moving Z-plane and mouse side buttons scrolling C-planes.

- Editing & saving ROI's in images to OME format image stacks.

- Copy & Paste to quickly annotate images and name them easily by right click.

- Select multiple points by holding down control key, for delete & move tools. 

- Exporting ROI's from each OME image in a folder of images to CSV.

- Easy freeform annotation with magic select tool which selects based on blob detection. 

## Dependencies
- [BioFormats.Net](https://github.com/GDanovski/BioFormats.Net)
- [IKVM](http://www.ikvm.net/)
- [AForge](http://www.aforgenet.com/)
- [LibTiff.Net](https://bitmiracle.com/libtiff/)
- [Cs-script](https://github.com/oleg-shilo/cs-script/blob/master/LICENSE)

## Licenses
- BioImage [GPL3](https://www.gnu.org/licenses/gpl-3.0.en.html)
- AForge [LGPL](http://www.aforgenet.com/framework/license.html)
- BioFormats.Net [GPL3](https://www.gnu.org/licenses/gpl-3.0.en.html)
- [IKVM](https://github.com/gluck/ikvm/blob/master/LICENSE)
- LibTiff.Net [BSD](https://bitmiracle.com/libtiff/)
- Cs-script [MIT](https://github.com/oleg-shilo/cs-script/blob/master/LICENSE)

## Example usage.

ImageView imageview = new ImageView("16bitTestStack.ome.tif");

imageview.Dock = DockStyle.Fill;

mainTabControl.TabPages[3].Controls.Add(imageview);

//Another way of opening just image.

BioImage image = new BioImage(0,"16bitTestStack.ome.tif");

//Get RGB Bitmap of BioImage with coordinates (Series, Z-depth, Channel, Time)

Bitmap rgb = image.GetImageRGB(0,0,0,0);

//Get Filtered Bitmap of BioImage with coordinates (Series, Z-depth, Channel, Time)

Bitmap filt = image.GetImageFiltered(0,0,0,0);

image.SaveSeries("16bitTestSaveStack.ome.tif", 0);

## Scripting
-  Save scripts into "StartupPath/Scripts" with ".cs" ending.
-  Open script editor and recorder from menu.
-  Double click on script name in Script runner to run script.
-  Scripts saved in Scripts folder will be loaded into script runner.
-  Program installer include sample script "Sample.cs" which gets & sets pixels and saves resulting image.
-  Use Script recorder to record program function calls and script runner to turn recorder text into working scripts. (See sample [scripts](https://github.com/BioMicroscopy/BioImage-Scripts))
## Sample Script

//css_reference BioImage.dll;

using System;

using System.Windows.Forms;

using BioImage;

public class Loader
{

	public string Load()
	{	
		BioImage.BioImage b =  new BioImage.BioImage("E://TESTIMAGES//text.ome.tif",0);
		//We create a substack of BioImage b.
		BioImage.BioImage bio = new BioImage.BioImage(b,"subStack.ome.tif", 0, 0, 3, 0, 3, 0, 2);
		//SetValueRGB(int s, int z, int c, int t, int x, int y, int RGBindex, ushort value)
		b.SetValueRGB(0,0,0,0,0,0,0,15000);
		//GetValueRGB(int s, int z, int c, int t, int x, int y, int RGBindex);
		ushort val = b.GetValueRGB(0,0,0,0,0,0,1);
		b.SaveSeries("E://TESTIMAGES//save.ome.tif",0);
		bio.SaveSeries("E://TESTIMAGES//subStack.ome.tif",0);
		ImageViewer iv = new ImageViewer(bio);
		//We open the result in an ImageViewer.
		iv.ShowDialog();
		return val.ToString();
	}
	
}
## Sample Tool Script

//css_reference BioImage.dll;

using System;
using System.Windows.Forms;
using System.Drawing;
using BioImage;
using System.Threading;

public class Loader
{

	//Point ROI Tool Example
	public string Load()
	{
		int ind = 1;
		do
		{
			BioImage.Scripting.State s = BioImage.Scripting.GetState();
			if (s != null)
			{
				if (!s.processed)
				{
					if (s.type == BioImage.Scripting.Event.Up && s.buts == MouseButtons.Left)
					{
						ZCT cord = ImageView.viewer.GetCoordinate();
						Annotation an = Annotation.CreatePoint(cord, s.p.X, s.p.Y);
						ImageView.viewer.image.Annotations.Add(an);
						an.Text = "Point" + ind;
						ind++;
						BioImage.Scripting.LogLine(s.ToString());
						ImageView.viewer.UpdateOverlay();
					}
					else
					if (s.type == BioImage.Scripting.Event.Down)
					{
						BioImage.Scripting.LogLine(s.ToString());
					}
					else
					if (s.type == BioImage.Scripting.Event.Move)
					{
						BioImage.Scripting.LogLine(s.ToString());
					}
				}
				{
					s.processed = true;
				}
			}
		} while (true);
		return "Done";
	}
}



