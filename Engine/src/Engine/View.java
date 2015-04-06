package Engine;
import java.awt.AlphaComposite;
import java.awt.BasicStroke;
import java.awt.Color;
import java.awt.Dimension;
import java.awt.Font;
import java.awt.Graphics;
import java.awt.Graphics2D;
import java.awt.Point;
import java.awt.Rectangle;
import java.awt.geom.AffineTransform;
import java.awt.image.BufferedImage;
import java.awt.image.WritableRaster;
import java.util.ArrayList;
import java.util.List;

import static Engine.Global.*;

import javax.swing.JPanel;
import javax.swing.text.BadLocationException;

import Engine.Entity.EntityIcon;
import Engine.Entity.EntityLabel;
import Engine.Global.Outline;

public class View extends JPanel {

	private static final long serialVersionUID = 1L;
	protected static int viewId = 1;
	protected int centerX, centerY;
	protected Rectangle viewBox;
	protected BufferedImage currentImage;
	protected boolean imageUpdated;
	protected Environment parentEnvironment;
	protected double viewScale = 1.0;
	protected boolean visible = true;
	protected Rectangle selectionBox;
	protected Color selectionColour;
	protected Entity master;
	protected boolean linkedToMaster;
	private boolean debug = false;
	
	public View(int inAbsCenterX, int inAbsCenterY,
			int inCenterX, int inCenterY, int inWidth, int inHeight, Environment inParent) {
		this("View" + viewId++, inAbsCenterX, inAbsCenterY, inCenterX, inCenterY, inWidth, inHeight, inParent);
	}
	
	public View(String inName, int inAbsCenterX, int inAbsCenterY, int inCenterX, int inCenterY, 
			int inWidth, int inHeight, Environment inParent) {
		super();
		setName(inName);
		int cornerX = inAbsCenterX - inWidth / 2;
		int cornerY = inAbsCenterY - inHeight / 2;
		setLocation(cornerX, cornerY);
		setSize(inWidth, inHeight);
		Dimension size = new Dimension(inWidth, inHeight);
		setPreferredSize(size);
		centerX = inCenterX;
		centerY = inCenterY;
		parentEnvironment = inParent;
		
		updateViewBox();
	}

	public int getAbsCenterX() {
		Point corner = getLocation();
		int absWidth = getWidth();
		int absCenterX = corner.x + absWidth / 2;
		
		return absCenterX;
	}
	
	public int getAbsCenterY() {
		Point corner = getLocation();
		int absHeight = getHeight();
		int absCenterY = corner.y + absHeight / 2;
		
		return absCenterY;
	}
	
	public int getCenterX() {
		if(linkedToMaster == true) {
			return master.getCenterX();
		}
		
		return centerX;
	}
	
	public int getCenterY() {
		if(linkedToMaster == true) {
			return master.getCenterY();
		}
		
		return centerY;
	}

	public int getScaledWidth() {
		int width = getWidth();
		return (int) (width / viewScale);
	}
	
	public int getScaledHeight() {
		int height = getHeight();
		return (int) (height / viewScale);
	}
	
	public BufferedImage getCurrentImage() {
		return currentImage;
	}
	
	public boolean getImageUpdated() {
		return imageUpdated;
	}
	
	public void setImageUpdated(boolean condition) {
		imageUpdated = condition;
	}

	public Environment getEnvironment() {
		return parentEnvironment;
	}
	
	public void setEnvironment(Environment inParent) {
		parentEnvironment = inParent;
	}
	
	public boolean getVisible() {
		return visible;
	}
	
	public void setSelectionBox(Rectangle inSelection) {
		if(inSelection == null) {
			selectionBox = null;
			return;
		}
		
		int absCornerX = inSelection.x;
		int absCornerY = inSelection.y;
		
		int width = inSelection.width;
		int height = inSelection.height;
		
		int viewCenterX = getCenterX();
		int viewCenterY = getCenterY();
		
		int viewAbsWidth = getWidth();
		int viewAbsHeight = getHeight();
		
		int viewScaledWidth = getScaledWidth();
		int viewScaledHeight = getScaledHeight();
		
		int viewAbsCenterX = getAbsCenterX();
		int viewAbsCenterY = getAbsCenterY();
		
		int offsetX = -8;
		int offsetY = -33;
		
		int viewCornerX = viewCenterX - viewScaledWidth / 2;
		int viewCornerY = viewCenterY + viewScaledHeight / 2;
		
		int viewAbsCornerX = viewAbsCenterX - viewAbsWidth / 2;
		int viewAbsCornerY = viewAbsCenterY - viewAbsHeight / 2;
		
		//int levelHeight = parentEnvironment.getLevelHeight();
		
		int cornerX = (int) ((absCornerX + offsetX - viewAbsCornerX) / viewScale) + viewCornerX;
		int cornerY = viewCornerY - (int) ((absCornerY + offsetY - viewAbsCornerY) / viewScale);	//invert y
		
		width = (int) (inSelection.width / viewScale);
		height = (int) (inSelection.height / viewScale);
		
		selectionBox = new Rectangle(cornerX, cornerY, width, height);
	}
	
	public Entity getMaster() {
		return master;
	}
	
	public void setMaster(Entity inMaster) {
		master = inMaster;
	}
	
	public void setLinkedToMaster(boolean condition) {
		linkedToMaster = condition;
	}
		
	public void moveUp() {
		if(linkedToMaster == true) {
			return;
		}
		
		centerY++;
		updateViewBox();
	}
	
	public void moveDown() {
		if(linkedToMaster == true) {
			return;
		}
		
		centerY--;
		updateViewBox();
	}
	
	public void moveLeft() {
		if(linkedToMaster == true) {
			return;
		}
		
		centerX--;
		updateViewBox();
	}
	
	public void moveRight() {
		if(linkedToMaster == true) {
			return;
		}
		
		centerX++;
		updateViewBox();
	}
	
	public void zoomIn(int zoomPercent) {
		viewScale += zoomPercent / 100;
		
		updateViewBox();
	}
	
	public void zoomOut(int zoomPercent) {
		viewScale -= zoomPercent / 100;
		
		updateViewBox();
	}
	
	public boolean checkWithinView(Sprite toCheck) {
		int scaledWidth = toCheck.getScaledWidth();
		int scaledHeight = toCheck.getScaledHeight();

		int centerX = toCheck.getCenterX();
		int centerY = toCheck.getCenterY();

		if(debug == true) {
			Graphics2D g2d = (Graphics2D) currentImage.getGraphics();
			
			int viewCenterX = getCenterX();
			int viewCenterY = getCenterY();
			
			int viewScaledWidth = viewBox.width;
			int viewScaledHeight = viewBox.height;
			
			int toDrawCornerXRelativeToViewCorner = centerX - scaledWidth / 2 - viewCenterX + viewScaledWidth / 2;
			int toDrawCornerYRelativeToViewCorner = -centerY - scaledHeight / 2 + viewCenterY + viewScaledHeight / 2;	//invert position in view
			
			g2d.setColor(Color.GREEN);
			g2d.drawRect(toDrawCornerXRelativeToViewCorner, toDrawCornerYRelativeToViewCorner, scaledWidth, scaledHeight);
			
			g2d.setColor(Color.BLUE);
			g2d.drawRect(0, 0, viewScaledWidth - 1, viewScaledHeight - 1);
		}
		
		boolean inView = checkWithinBounds(toCheck, viewBox);
		
		return inView;
	}
	
	public boolean checkWithinBounds(Sprite toCheck, Rectangle bounds) {
		if(bounds == null) {
			return false;
		}
		
		int scaledWidth = toCheck.getScaledWidth();
		int scaledHeight = toCheck.getScaledHeight();

		int centerX = toCheck.getCenterX();
		int centerY = toCheck.getCenterY();
		
		int worldHeight = parentEnvironment.getLevelHeight();
		
		int cornerX = centerX - scaledWidth / 2;
		int cornerY = worldHeight - (centerY + scaledHeight / 2);	//invert y
		
		Rectangle boxToCheck = new Rectangle(cornerX, cornerY, scaledWidth, scaledHeight);
		
		boolean inBounds = boxToCheck.intersects(bounds);
		
		return inBounds;
	}
	
	public void drawSprite(Graphics2D g2d, Sprite toDraw) {
		if (toDraw == null) {
			return;
		}

		BufferedImage image = (BufferedImage) toDraw.getCurrentImage();
		
		int scaledWidth = toDraw.getScaledWidth();
		int scaledHeight = toDraw.getScaledHeight();
		
		int viewCenterX = getCenterX();
		int viewCenterY = getCenterY();
		
		int toDrawCenterX = toDraw.getCenterX();
		int toDrawCenterY = toDraw.getCenterY();
		
		int cornerXRelativeToViewCenter = (toDrawCenterX - scaledWidth / 2) - viewCenterX;
		int cornerYRelativeToViewCenter = (toDrawCenterY + scaledHeight / 2) - viewCenterY;	// Invert position in view
		
		int viewWidth = getScaledWidth();
		int viewHeight = getScaledHeight();
		
		int cornerXRelativeToViewCornerX = viewWidth / 2 + cornerXRelativeToViewCenter;
		int cornerYRelativeToViewCornerY = viewHeight / 2 - cornerYRelativeToViewCenter;
		
		drawImage(g2d, image, cornerXRelativeToViewCornerX,
				cornerYRelativeToViewCornerY, scaledWidth, scaledHeight);
	}
	
	public void drawEntity(Graphics2D g2d, Entity toDraw) {
		if (toDraw == null) {
			return;
		}
		
		AffineTransform defaultOrientation = g2d.getTransform();
		
		BufferedImage image = (BufferedImage) toDraw.getCurrentImage();

		int toDrawWidth = toDraw.getScaledWidth();
		int toDrawHeight = toDraw.getScaledHeight();
		
		int toDrawCenterX = toDraw.getCenterX();
		int toDrawCenterY = toDraw.getCenterY();
		
		int viewCenterX = getCenterX();
		int viewCenterY = getCenterY();
		
		int viewWidth = getScaledWidth();
		int viewHeight = getScaledHeight();
		
		int toDrawCornerXRelativeToViewCorner = toDrawCenterX - toDrawWidth / 2 - viewCenterX + viewWidth / 2;
		int toDrawCornerYRelativeToViewCorner = viewHeight / 2 - toDrawCenterY - toDrawHeight / 2 + viewCenterY;	//invert position in view
		
		setTransparency(g2d, toDraw);
		
		setRotation(g2d, toDraw, toDrawWidth, toDrawHeight, toDrawCornerXRelativeToViewCorner,
				toDrawCornerYRelativeToViewCorner);
		
		drawShadow(g2d, toDraw, toDrawWidth, toDrawHeight, toDrawCornerXRelativeToViewCorner,
				toDrawCornerYRelativeToViewCorner);
		
		drawImage(g2d, image, toDrawCornerXRelativeToViewCorner,
				toDrawCornerYRelativeToViewCorner, toDrawWidth, toDrawHeight);
		
		drawFill(g2d, toDraw, image, toDrawWidth, toDrawHeight, toDrawCornerXRelativeToViewCorner,
				toDrawCornerYRelativeToViewCorner);
		
		drawStroke(g2d, toDraw, image, toDrawWidth, toDrawHeight);
		
		drawFlash(g2d, toDraw, toDrawWidth, toDrawHeight, toDrawCornerXRelativeToViewCorner, 
				toDrawCornerYRelativeToViewCorner);
		
		drawLabel(g2d, toDraw, toDrawCornerXRelativeToViewCorner, toDrawCornerYRelativeToViewCorner, 
				toDrawWidth, toDrawHeight);
		
		drawIcons(g2d, toDraw, toDrawCornerXRelativeToViewCorner, toDrawCornerYRelativeToViewCorner, 
				toDrawWidth, toDrawHeight);
		
		g2d.setTransform(defaultOrientation);
	}
	
	private void drawShadow(Graphics2D g2d, Entity toDraw, int scaledWidth, int scaledHeight, 
			int absolutePositionX, int absolutePositionY) {
		boolean hasShadow = toDraw.getHasShadow();
		if (hasShadow == false) {
			return;
		}
		
		BufferedImage image = toDraw.getCurrentImage();
		int width = image.getWidth();
		int height = image.getHeight();
		BufferedImage shadow = new BufferedImage(width, height, BufferedImage.TYPE_INT_ARGB);
		
		int opacity = 50;
		int offset = 5;
		int shadowColour = 0;
		
		int alphaLoss = (toDraw.getOpacity() - opacity);
		if(alphaLoss < 0) {
			alphaLoss = 0;
		}
				
		alphaLoss = alphaLoss * 255 / 100;

		for (int x = 0; x < width; x++) {
			for (int y = 0; y < height; y++) {
				int colour = image.getRGB(x, y);

				int alpha = (colour>>24) & 0xff;
				alpha -= alphaLoss;
				if (alpha < 0) {
					alpha = 0;
				}
				Color newColour = new Color(shadowColour, shadowColour,
						shadowColour, alpha);
				int rgb = newColour.getRGB();
				shadow.setRGB(x, y, rgb);
			}
		}
		
		double scale = toDraw.getScale();
		int shadowWidth = (int) (scale * scaledWidth);
		int shadowHeight = (int) (scale * scaledHeight);
		
		g2d.drawImage(shadow, absolutePositionX + offset, absolutePositionY
				+ offset, shadowWidth, shadowHeight, null);
	}
    
	private void drawFill(Graphics2D g2d, Entity toDraw, BufferedImage image, int scaledWidth, int scaledHeight, 
			int absolutePositionX, int absolutePositionY) {
		boolean hasFill = toDraw.getHasFill();
		if(hasFill == false) {
			return;
		}
		
		Color fillColour = toDraw.getFillColour();
		int fillOpacity = toDraw.getFillOpacity();
		
		BufferedImage fill = getOverlay(image, fillColour, fillOpacity);
		
		if (fill == null) {
			return;
		}
		
		drawImage(g2d, fill, absolutePositionX, absolutePositionY, scaledWidth, scaledHeight);
	}

	private void drawStroke(Graphics2D g2d, Entity toDraw, BufferedImage image, int scaledWidth, int scaledHeight) {
		boolean hasStroke = toDraw.getHasStroke();
		if(hasStroke == false) {
			return;
		}
		
		Color strokeColour = toDraw.getStrokeColour();
		BasicStroke stroke = toDraw.getStroke();
		
		g2d.setStroke(stroke);
		g2d.setColor(strokeColour);
		
		int shapeApprox = toDraw.getOutlineShape();
		int toDrawCenterX = toDraw.getCenterX();
		int toDrawCenterY = toDraw.getCenterY();
		
		//Outline strokeOutline = new Outline(toDrawCenterX, toDrawCenterY, image, shapeApprox);
		Outline strokeOutline = new Outline(toDrawCenterX, toDrawCenterY, scaledWidth, scaledHeight, shapeApprox);
		
		int viewWidth = getScaledWidth();
		int viewHeight = getScaledHeight();
		
		int viewCenterX = getCenterX();
		int viewCenterY = getCenterY();
		
		strokeOutline.drawOutline(g2d, viewCenterX, viewCenterY, viewWidth, viewHeight);
	}
    
	private void drawFlash(Graphics2D g2d, Entity toDraw, int scaledWidth, int scaledHeight, 
			int absolutePositionX, int absolutePositionY) {
		boolean isFlashing = toDraw.getIsFlashing();
		if(isFlashing == false) {
			return;
		}
		
		Color fillColour = toDraw.getFlashColour();
		BufferedImage image = toDraw.getCurrentImage();
		int maxOpacity = toDraw.getMaxFlashOpacity();
		int nextFlashStep = toDraw.getNextFlashStep();
		int flashPeriod = toDraw.getFlashPeriod();
		
		int fillOpacity = 2 * maxOpacity / flashPeriod * (flashPeriod / 2 - Math.abs(flashPeriod / 2 - nextFlashStep));
		
		BufferedImage fill = getOverlay(image, fillColour, fillOpacity);
		
		if (fill == null) {
			return;
		}
		
		Global.drawImage(g2d, fill, absolutePositionX, absolutePositionY, scaledWidth, scaledHeight);
	}
	
	private void drawLabel(Graphics2D g2d, Entity toDraw, int cornerX, int cornerY, int scaledWidth, int scaledHeight) {
		boolean hasLabel = toDraw.getHasLabel();
		if(hasLabel == false) {
			return;
		}
		
		EntityLabel label = toDraw.getLabel();
		
		String text = label.getText();
		
		Color defaultColour = g2d.getColor();
		Color colour = label.getColour();
		g2d.setColor(colour);
		
		Font defaultFont = g2d.getFont();
		Font font = label.getFont();
		g2d.setFont(font);
		
		int offsetX = label.getOffsetX();
		int offsetY = label.getOffsetY();
		
		int width = font.getSize() * text.length() / 2;
		int height = font.getSize();
		
		int bottomCornerX = cornerX + scaledWidth / 2 - width / 2 + offsetX;
		int bottomCornerY = cornerY + scaledHeight / 2 + height / 2 - offsetY;
		
		g2d.drawString(text, bottomCornerX, bottomCornerY);
		
		g2d.setColor(defaultColour);
		g2d.setFont(defaultFont);
	}
	
	private void drawIcons(Graphics2D g2d, Entity toDraw, int cornerX, int cornerY, 
			int scaledWidth, int scaledHeight) {
		boolean hasIcons = toDraw.getHasIcons();
		if(hasIcons == false) {
			return;
		}
		
		List<EntityIcon> allIcons = toDraw.getIcons();
		for(EntityIcon icon : allIcons) {
			BufferedImage image = icon.getImage();
			int width = image.getWidth();
			int height = image.getHeight();
			
			int offsetX = icon.getOffsetX();
			int offsetY = icon.getOffsetY();
			
			int positionX = cornerX + scaledWidth / 2 - width / 2 + offsetX;
			int positionY = cornerY + scaledHeight / 2 + height / 2 - offsetY;
			
			Global.drawImage(g2d, image, positionX, positionY, width, height);
		}
	}
	
	private void setTransparency(Graphics2D g2d, Entity toDraw) {
		float opacity = (float) toDraw.getOpacity() / 100;
		g2d.setComposite(AlphaComposite.getInstance(AlphaComposite.SRC_OVER,
				opacity));
	}

	private void setRotation(Graphics2D g2d, Entity toDraw, int scaledWidth,
			int scaledHeight, int absolutePositionX, int absolutePositionY) {
		int direction = toDraw.getDirection();
		int rotationPointX = absolutePositionX + scaledWidth / 2;
		int rotationPointY = absolutePositionY + scaledHeight / 2;
		g2d.rotate(Math.toRadians(direction), rotationPointX, rotationPointY);
	}
	
	private BufferedImage getOverlay(BufferedImage image, Color colour, int opacity) {
		if (opacity == 0) {
			return null;
		}
		int red = colour.getRed();
		int green = colour.getGreen();
		int blue = colour.getBlue();
		int width = image.getWidth();
		int height = image.getHeight();
		WritableRaster raster = image.getRaster();
		// int imageType = image.getType();
		boolean hasAlpha = image.getColorModel().hasAlpha();
		BufferedImage fill = new BufferedImage(width, height,
				BufferedImage.TYPE_INT_ARGB);
		WritableRaster fillRaster = fill.getRaster();
		for (int x = 0; x < width; x++) {
			for (int y = 0; y < height; y++) {
				int[] rgb = new int[4];
				if (hasAlpha == false) {
					rgb[3] = 255;
				}
				raster.getPixel(x, y, rgb);
				rgb[0] = red;
				rgb[1] = green;
				rgb[2] = blue;
				int oldAlpha = rgb[3];
				int newAlpha = oldAlpha * opacity / 100;
				rgb[3] = newAlpha;
				fillRaster.setPixel(x, y, rgb);
			}
		}
		return fill;
	}
    
	public void updateView() {
		if(visible == false) {
			return;
		}
		updateViewBox();

		Graphics g = currentImage.getGraphics();
		
		paintComponent(g);
	}

	private void updateViewBox() {
		int centerX = getCenterX();
		int centerY = getCenterY();
		
		int scaledWidth = getScaledWidth();
		int scaledHeight = getScaledHeight();
		
		int levelHeight = parentEnvironment.getLevelHeight();
		
		int positionX = centerX - scaledWidth / 2;
		int positionY = levelHeight - (centerY + scaledHeight / 2);	//invert y
		
		viewBox = new Rectangle(positionX, positionY, scaledWidth, scaledHeight);
		currentImage = new BufferedImage(scaledWidth, scaledHeight, BufferedImage.TYPE_INT_ARGB);
	}

	public void paintComponent(Graphics g) {
		Graphics2D g2d = (Graphics2D) g;

		List<Sprite> allSprites = parentEnvironment.getAllSpritesInBounds(viewBox);
		for(Sprite toDraw : allSprites) {
			boolean inView = checkWithinView(toDraw);
			if(inView == false) {
				continue;
			}
			
			drawSprite(g2d, toDraw);
		}

		// debug
		int scaledWidth = getScaledWidth();
		int scaledHeight = getScaledHeight();
		
		g2d.setColor(Color.BLACK);
		g2d.drawRect(0, 0, scaledWidth-1, scaledHeight-1);
		
/*		g2d.setColor(Color.WHITE);
		g2d.fillRect(0, 0, scaledWidth, scaledHeight);*/
		
		g2d.setColor(Color.RED);
		g2d.drawLine(scaledWidth / 2, 0, scaledWidth / 2, scaledHeight);
		g2d.drawLine(0, scaledHeight / 2, scaledWidth, scaledHeight / 2);
		// end debug

		List<Entity> allEntities = parentEnvironment.getAllEntities();
		for(Entity toDraw : allEntities) {
			boolean inView = checkWithinView(toDraw);
			if(inView == false) {
				continue;
			}
			
			drawEntity(g2d, toDraw);
		}
		
		drawSelection(g2d);
		
		imageUpdated = true;
		
		g2d.dispose();
	}
	
	public void drawSelection(Graphics2D g2d) {
		if(selectionBox == null) {
			return;
		}
		
		int cornerX = selectionBox.x;
		int cornerY = selectionBox.y;

		int width = selectionBox.width;
		int height = selectionBox.height;
		
		int viewCenterX = getCenterX();
		int viewCenterY = getCenterY();
		
		int viewWidth = getScaledWidth();
		int viewHeight = getScaledHeight();
		
		//int levelHeight = parentEnvironment.getLevelHeight();
		
		int cornerXRelativeToViewCorner = (int) viewScale * (cornerX - (viewCenterX - viewWidth / 2));
		int cornerYRelativeToViewCorner = (int) viewScale * ((viewCenterY + viewHeight / 2) - cornerY);
		
		g2d.setColor(selectionColour);
		g2d.drawRect(cornerXRelativeToViewCorner, cornerYRelativeToViewCorner, width, height);
	}
	
	public List<Entity> getSelectedEntities() {
		List<Entity> selectedEntities = new ArrayList<>();
		
		List<Entity> allEntities = parentEnvironment.getAllEntities();
		for(Entity toCheck : allEntities) {
			boolean isSelectable = toCheck.getIsSelectable();
			if(isSelectable == false) {
				continue;
			}
			
			boolean inSelection = checkWithinBounds(toCheck, selectionBox);
			if(inSelection == false) {
				continue;
			}
			
			selectedEntities.add(toCheck);
			toCheck.setIsSelected(true);
		}
		
		return selectedEntities;
	}
	
	public void deselect(List<Entity> selectedEntities) {
    	//setSelectionBox(null);
    	if(selectedEntities == null) {
    		return;
    	}
    	
		for(Entity selected : selectedEntities) {
			selected.setIsSelected(false);
		}
	}
}
