package Strikeforce;

import java.io.File;
import java.net.URL;
import javax.swing.*;

public class ResLoader {
	private ClassLoader allImageResources;

	public ResLoader(ClassLoader inClassLoader) {
		allImageResources = inClassLoader;
	}

	public ImageIcon getImageIcon(String filename) {
		return new ImageIcon(allImageResources.getResource(filename));
	}

	public File getFile(String filename) {
		URL filePath = allImageResources.getResource(filename);
		return new File(filePath.getPath());
	}
}