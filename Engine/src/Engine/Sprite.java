package Engine;
import java.awt.image.BufferedImage;
import javax.swing.JComponent;

public class Sprite extends JComponent {

	private static final long serialVersionUID = 1L;
	protected int centerX, centerY;
	protected double scale = 1;
	protected BufferedImage currentImage;
	
	public Sprite() {}
	
	public Sprite(int inCenterX, int inCenterY, BufferedImage inImage) {
		super();
		centerX = inCenterX;
		centerY = inCenterY;

		currentImage = inImage;
		
		int width = getScaledWidth();
		int height = getScaledHeight();
		setSize(width, height);
		
		int cornerFourX = inCenterX - width / 2;
		int cornerFourY = inCenterY - height / 2;
		setLocation(cornerFourX, cornerFourY);
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
