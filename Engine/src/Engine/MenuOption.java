package Engine;
import java.awt.Color;
import java.awt.Font;
import java.awt.Graphics2D;
import java.awt.event.KeyAdapter;
import java.awt.event.KeyEvent;
import java.awt.event.KeyListener;
import java.awt.event.MouseEvent;
import java.awt.event.MouseListener;
import java.awt.image.BufferedImage;

import static Engine.Global.*;

import javax.swing.JPanel;

public class MenuOption extends JPanel implements MouseListener, KeyListener {
	private static final long serialVersionUID = 1L;
	//protected String name;
	protected BufferedImage currentImage;
	protected Font currentFont;
	protected boolean displayAsText = false;	//If true display name as text, if false display currentImage
	//protected int centerX, centerY;
	//protected int width, height;
	
	public MenuOption(String inName, Font inFont, int inCenterX, int inCenterY, int inWidth, int inHeight) {
		this(inName, inCenterX, inCenterY, inWidth, inHeight);
		currentFont = inFont;
		displayAsText = true;
	}
	
	public MenuOption(String inName, String inPath, String inAddon, String inExtension, int inCenterX, int inCenterY, 
			int inWidth, int inHeight) {
		this(inName, inCenterX, inCenterY, inWidth, inHeight);
		String filename = getFilename(inPath, inName, inAddon, inExtension);
		currentImage = loadImage(filename);
/*		int width = currentImage.getWidth();
		int height = currentImage.getHeight();
		setSize(width, height);*/
	}
	
	private MenuOption(String inName, int inCenterX, int inCenterY, int inWidth, int inHeight) {
		setName(inName);
		currentFont = DEFAULT_FONT;
		int x = inCenterX - inWidth / 2;
		int y = inCenterY - inHeight / 2;
		setLocation(x, y);
		setSize(inWidth, inHeight);
	}
	
	public BufferedImage getCurrentImage() {
		if(displayAsText == true) {
			setCurrentImageAsText();
		}
		
		if(currentImage == null) {
			throw new IllegalStateException("Image has not been set and not set to display as text");
		}
        
        return currentImage;
	}

	private void setCurrentImageAsText() {
		if(currentImage != null) {
			return;
		}
		
		String name = getName();
		
        if (currentFont == null) {
            currentFont = DEFAULT_FONT;
        }
        
		int height = currentFont.getSize() * 4 / 3;
		int width = name.length() * height;
		
		currentImage = new BufferedImage(width, height, BufferedImage.TYPE_INT_ARGB);
		Graphics2D g2d = currentImage.createGraphics();
        
        int bottomLeftCornerX = getX();
        int bottomLeftCornerY = getY() + height;
        
        g2d.setColor(Color.BLACK);
        g2d.drawString(name, bottomLeftCornerX, bottomLeftCornerY);
	}

	@Override
	public void mouseClicked(MouseEvent arg0) {
		// TODO Auto-generated method stub
		
	}

	@Override
	public void mouseEntered(MouseEvent arg0) {
		// TODO Auto-generated method stub
		
	}

	@Override
	public void mouseExited(MouseEvent arg0) {
		// TODO Auto-generated method stub
		
	}

	@Override
	public void mousePressed(MouseEvent arg0) {
		// TODO Auto-generated method stub
		
	}

	@Override
	public void mouseReleased(MouseEvent arg0) {
		// TODO Auto-generated method stub
		
	}

	@Override
	public void keyPressed(KeyEvent arg0) {
		// TODO Auto-generated method stub
		
	}

	@Override
	public void keyReleased(KeyEvent arg0) {
		// TODO Auto-generated method stub
		
	}

	@Override
	public void keyTyped(KeyEvent arg0) {
		// TODO Auto-generated method stub
		
	}
}
