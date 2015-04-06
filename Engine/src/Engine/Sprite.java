package Engine;

import java.awt.image.BufferedImage;

public class Sprite {
	
	protected int centerX, centerY;
	protected double scale = 1;
	protected BufferedImage currentImage;
	
	public Sprite() {}
	
	public Sprite(int inCenterX, int inCenterY, BufferedImage inImage) {
		centerX = inCenterX;
		centerY = inCenterY;
		currentImage = inImage;
	}
	
	public int getCenterX() {
		return centerX;
	}
	
	public int getCenterY() {
		return centerY;
	}
	
	public int getWidth() {
		int width = currentImage.getWidth();
		return width;
	}
	
	public int getHeight() {
		int height = currentImage.getHeight();
		return height;
	}

	public int getScaledWidth() {
		int width = getWidth();
		int scaledWidth = (int) Math.round(width * scale);
		
		return scaledWidth;
	}
	
	public int getScaledHeight() {
		int height = getHeight();
		int scaledHeight = (int) Math.round(height * scale);
		
		return scaledHeight;
	}
	
	public double getScale() {
		return scale;
	}
	
	public void setScale(double inScale) {
		scale = inScale;
	}
	
	public BufferedImage getCurrentImage() {
		return currentImage;
	}
}
