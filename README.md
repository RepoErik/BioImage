![alt text](https://github.com/RepoErik/BioImage/blob/master/banner.bmp?raw=true)

# BioImage Library

A .NET Library for opening & annotating various microscopy imaging formats. Supports all bioformats supported images like .TIF, .CZI etc. 
Best for working with ROI's in OME format & CSV.

## Features

-Exporting ROI's to CSV files for easy analysis in statistics programs.

-Great at viewing image stacks with scroll wheel moving Z-plane and mouse side buttons scrolling C-planes.

-RGB image viewing mode which automatically combines 3 channels into RGB image.

-Editing & saving ROI's in images to OME format image stacks.

-Exporting ROI's from each image in a folder of images to CSV. For further quick & easy analysis of images.

-Exports bounding boxes of ROI's in CSV format.

-Select multiple points by holding down left control key for move & delete tools.

Example usage.

ImageView imageview = new ImageView("16bitTestStack.ome.tif");

imageview.Dock = DockStyle.Fill;

mainTabControl.TabPages[3].Controls.Add(imageview);

//Another way of opening just image.

BioImage image = new BioImage("16bitTestStack.ome.tif");

image.SaveSeries("16bitTestSaveStack.ome.tif", 0);

