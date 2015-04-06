package Engine;
import static Engine.Global.*;
import java.awt.Font;
import java.awt.Graphics2D;
import java.awt.image.BufferedImage;
import java.util.ArrayList;
import java.util.List;

public class Menu extends MenuOption {
	private static final long serialVersionUID = 1L;
	protected List<MenuOption> menuOptions = new ArrayList<>();
	protected int menuSpacing;	//debug
	protected int optionSpacing;
	protected int nextOptionY;
	
	public Menu(String inName, Font inFont, int inCenterX, int inCenterY, int inWidth, int inHeight, int inMenuSpacing, 
			int inOptionSpacing) {
		super(inName, inFont, inCenterX, inCenterY, inWidth, inHeight);
		menuSpacing = inMenuSpacing;
		optionSpacing = inOptionSpacing;
		nextOptionY = inCenterY + inMenuSpacing;
	}

	public List<MenuOption> getMenuOptions() {
		return menuOptions;
	}
	
	public void addMenuOption(String inName, Font inFont, int inWidth, int inHeight) {
		int titleCornerX = getX();
		int titleWidth = getWidth();
		
		int centerX = titleCornerX + titleWidth - inWidth / 2;
		int centerY = getNextOptionY();
		
		MenuOption toAdd = new MenuOption(inName, inFont, centerX, centerY, inWidth, inHeight);
		
		menuOptions.add(toAdd);
	}
	
	public void addMenuOption(String inName, String inPath, String inAddon, String inExtension) {
		int titleCornerX = getX();
		int titleWidth = getWidth();
		int titleHeight = getHeight();
		
		int centerX = titleCornerX;
		int centerY = getNextOptionY();
		
		MenuOption toAdd = new MenuOption(inName, inPath, inAddon, inExtension, centerX, centerY, titleWidth, titleHeight);
		
		menuOptions.add(toAdd);
	}

	private int getNextOptionY() {
		int centerY = nextOptionY;
		
		nextOptionY += optionSpacing;
		return centerY;
	}
	
	public void drawMenu(Graphics2D g2d) {
		BufferedImage image;
		int absCornerX;
		int absCornerY;
		int width;
		int height;
		List<MenuOption> allMenuOptions = getMenuOptions();
		//BufferedImage image = new BufferedImage(width, height, BufferedImage.TYPE_INT_ARGB);
		//Draw all menu options and title to image
		for(MenuOption toDraw : allMenuOptions) {
			image = toDraw.getCurrentImage();
			absCornerX = toDraw.getX();
			absCornerY = toDraw.getY();
			width = toDraw.getWidth();
			height = toDraw.getHeight();
			drawImage(g2d, image, absCornerX, absCornerY, width, height);
		}
		
		//Draw title
		image = getCurrentImage();
		absCornerX = getX();
		absCornerY = getY();
		width = getWidth();
		height = getHeight();
		drawImage(g2d, image, absCornerX, absCornerY, width, height);
	}
}
