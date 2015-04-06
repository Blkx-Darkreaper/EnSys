package Engine;
import java.awt.BasicStroke;
import java.awt.Color;
import java.awt.Font;
import java.awt.Rectangle;
import java.awt.Shape;
import java.awt.geom.AffineTransform;
import java.awt.geom.PathIterator;
import java.awt.geom.Point2D;
import java.awt.geom.Rectangle2D;
import java.awt.image.BufferedImage;
import java.util.ArrayList;
import java.util.Date;
import java.util.Deque;
import java.util.HashMap;
import java.util.LinkedList;
import java.util.List;
import java.util.Map;

import Engine.Global.Outline;
import static Engine.Global.*;

public class Entity extends Sprite implements Comparable<Entity> {

	protected static int entityId;
	protected static int nextId = 1;
	protected Date lastModified;
	protected boolean modified = true;
	protected String name;
	protected String path;
	protected String addon;
	protected String extension;
	protected int hitpoints;
	protected HitBox hitBox;
	protected int direction = 0;
	protected int speed = 0;
	protected MediaClip currentClip;
	protected Deque<Animation> animationQueue = new LinkedList<>();
	protected Map<String, Animation> allAnimations = new HashMap<>();
	protected int opacity = 100;
	protected boolean hasShadow = false;
	protected boolean hasFill = false;
	protected Color fillColour;
	protected int fillOpacity;
	protected boolean hasStroke = false;
	protected Color strokeColour;
	protected int outlineShape;
	protected BasicStroke stroke;
	protected boolean isFlashing = false;
	protected Color flashColour;
	protected int maxFlashOpacity;
	protected int flashPeriod;
	protected int currentFlashStep = 0;
	protected boolean hasLabel = false;
	protected EntityLabel label;
	protected boolean hasIcons = false;
	protected List<EntityIcon> allIcons = new LinkedList<>();
	protected Entity master;
	protected boolean linkedToMaster;
	protected boolean isSelectable = true;
	protected boolean isSelected = false;
	
	public Entity() {
		entityId = nextId;
		nextId++;
		name = "test";
		//path = "image/";
		extension = "png";
		centerX = 200;
		centerY = 150;
		
		String filename = getFilename(path, name, addon, extension);
		currentImage = loadImage(filename);
		lastModified = new Date();
		MediaClip sound = new MediaClip("sound", "", "", "wav", false);
		//addAnimation("idle", currentImage, sound);
		addAnimation("idle", currentImage, null);
		
		Font font = new Font("ComicSans", Font.PLAIN, 20);
		setLabel("Test", Color.RED, font, 0, -30);
		setHasLabel(true);
		
		addIcon("bar", null, null, "png");
		setHasIcons(true);
		
		int lineWidth = 2;
		int cap = BasicStroke.CAP_SQUARE;
		int join = BasicStroke.JOIN_BEVEL;
		Color inColour = Color.RED;
		setStroke(lineWidth, cap, join, inColour);
		setOutlineShape(Outline.RECTANGLE);
		//setHasStroke(true);
	}
	
	public Entity(int inCenterX, int inCenterY, int inWidth, int inHeight) {
		this("", "", "", "", false, inCenterX, inCenterY, inWidth, inHeight);
	}
	
	public Entity(String inName, int inCenterX, int inCenterY, int inWidth, int inHeight) {
		this(inName, "", "", "", false, inCenterX, inCenterY, inWidth, inHeight);
	}
	
	public Entity(String inName, String inPath, String inAddon, String inExtension, boolean loadImage, int inCenterX, int inCenterY, 
			int inWidth, int inHeight) {
		entityId = nextId;
		nextId++;
		name = inName;
		path = inPath;
		addon = inAddon;
		extension = inExtension;
		centerX = inCenterX;
		centerY = inCenterY;
		currentImage = new BufferedImage(inWidth, inHeight, BufferedImage.TYPE_INT_ARGB);
		if(loadImage == true) {
			String filename = getFilename(path, name, addon, extension);
			currentImage = loadImage(filename);
		}
		lastModified = new Date();
		addAnimation("idle", currentImage, null);
	}
	
	public Entity(String inName, String inPath, String inAddon, String inExtension) {
		name = inName;
		path = inPath;
		addon = inAddon;
		extension = inExtension;
		String filename = getFilename(path, name, addon, extension);
		currentImage = loadImage(filename);
	}

	public boolean getModified() {
		return modified;
	}
	
	public int getCenterX() {
		if(linkedToMaster == true) {
			return master.getCenterX();
		}
		
		return centerX;
	}
	
	public void setCenterX(int inCenterX) {
		centerX = inCenterX;
	}
	
	public int getCenterY() {
		if(linkedToMaster == true) {
			return master.getCenterY();
		}
		
		return centerY;
	}
	
	public void setCenterY(int inCenterY) {
		centerY = inCenterY;
	}
	
	public void setWidth(int inWidth) {
		int height = currentImage.getHeight();
		
		currentImage = new BufferedImage(inWidth, height, BufferedImage.TYPE_INT_ARGB);
	}
	
	public void setHeight(int inHeight) {
		int width = currentImage.getWidth();
		
		currentImage = new BufferedImage(width, inHeight, BufferedImage.TYPE_INT_ARGB);
	}
	
	public int getHitpoints() {
		return hitpoints;
	}
	
	public void setHitpoints(int inHitpoints) {
		hitpoints = inHitpoints;
	}
	
	public int getDirection() {
		return direction;
	}
	
	public void setDirection(int inDirection) {
		direction = inDirection;
    	direction %= 360;
	}
	
	public int getSpeed() {
		return speed;
	}
	
	public void setSpeed(int inSpeed) {
		speed = inSpeed;
	}
	
	public MediaClip getCurrentClip() {
		return currentClip;
	}
	
	public void setCurrentImage(BufferedImage image) {
		currentImage = image;
	}

	public void addAnimation(String animationName, int totalFrames, Map<Integer, MediaClip> inMedia) {
		Animation toAdd = new Animation(animationName);
		for(int i = 1; i < totalFrames; i++) {
			String animationAddon = addon + animationName + i;
			String filename = getFilename(name, path, animationAddon, extension);
			BufferedImage imageToAdd = loadImage(filename);
			MediaClip clipToAdd = inMedia.get(i);
			
			toAdd.addFrame(imageToAdd, clipToAdd);
		}

		allAnimations.put(animationName, toAdd);
	}
	
	public void addAnimation(String animationName, BufferedImage imageToAdd, MediaClip clipToAdd) {
		List<BufferedImage> inImages = new ArrayList<>();
		inImages.add(imageToAdd);
		Animation toAdd = new Animation(animationName);
		toAdd.addFrame(imageToAdd, clipToAdd);
		
		allAnimations.put(animationName, toAdd);
	}
	
	public double getScale() {
		return scale;
	}

	public int getOpacity() {
		return opacity;
	}
	
	public boolean getHasShadow() {
		return hasShadow;
	}
	
	public boolean getHasFill() {
		return hasFill;
	}
	
	public Color getFillColour() {
		return fillColour;
	}
	
	public int getFillOpacity() {
		return fillOpacity;
	}
	
	public BasicStroke getStroke() {
		return stroke;
	}
	
	public void setStroke(int lineWidth, int cap, int join, Color inColour) {
		strokeColour = inColour;
		stroke = new BasicStroke(lineWidth, cap, join);
	}
	
	public void setStroke(int lineWidth, int cap, int join, int dashLength, int spaceLength, Color inColour) {
		strokeColour = inColour;
		
		float[] dash = {dashLength, spaceLength};
		stroke = new BasicStroke(lineWidth, cap, join, 1.0f, dash, 0.0f);
	}
	
	public boolean getHasStroke() {
		return hasStroke;
	}
	
	public void setHasStroke(boolean condition) {
		if(stroke == null) {
			hasStroke = false;
		}
		
		hasStroke = condition;
	}
	
	public Color getStrokeColour() {
		return strokeColour;
	}
	
	public int getOutlineShape() {
		return outlineShape;
	}
	
	public void setOutlineShape(int inShape) {
		outlineShape = inShape;
	}
	
	public boolean getIsFlashing() {
		return isFlashing;
	}
	
	public Color getFlashColour() {
		return flashColour;
	}
	
	public int getMaxFlashOpacity() {
		return maxFlashOpacity;
	}
	
	public int getFlashPeriod() {
		return flashPeriod;
	}
	
	public int getNextFlashStep() {
		int nextFlashStep = currentFlashStep;
		
		currentFlashStep++;
		
		if(currentFlashStep > flashPeriod) {
			currentFlashStep = 0;
		}
		
		return nextFlashStep;
	}
	
	public boolean getHasLabel() {
		return hasLabel;
	}
	
	public void setHasLabel(boolean condition) {
		hasLabel = condition;
	}
	
	public EntityLabel getLabel() {
		return label;
	}
	
	public void setLabel(String inText, Color inColour, Font inFont, int inOffsetX, int inOffsetY) {
		if(inFont == null) {
			inFont = DEFAULT_FONT;
		}
		
		if(inColour == null) {
			inColour = Color.BLACK;
		}
		
		label = new EntityLabel(inText, inColour, inFont, inOffsetX, inOffsetY);
	}
	
	public boolean getHasIcons() {
		return hasIcons;
	}
	
	public void setHasIcons(boolean condition) {
		hasIcons = condition;
	}
	
	public List<EntityIcon> getIcons() {
		return allIcons;
	}
	
	public EntityIcon getIcon(int index) {
		EntityIcon toGet = allIcons.get(index);
		return toGet;
	}
	
	public void addIcon(String name, String path, String addon, String extension) {
		int offsetX = 0;
		
		int factor = allIcons.size() + 1;
		int offsetY = EntityIcon.DEFAULT_OFFSET * factor;
		
		addIcon(name, path, addon, extension, offsetX, offsetY);
	}
	
	public void addIcon(String name, String path, String addon, String extension, int offsetX, int offsetY) {
		EntityIcon toAdd = new EntityIcon(name, path, addon, extension, offsetX, offsetY);
		
		allIcons.add(toAdd);
	}
	
	public Entity getMaster() {
		return master;
	}
	
	public HitBox getHitBox() {
		if(hitBox == null) {
			setHitBox();
		}
		
		return hitBox;
	}
	
	public void setHitBox() {
		int width = currentImage.getWidth(null);
		int height = currentImage.getHeight(null);
		int radius = Math.round(Math.min(width, height) / 2);
		hitBox = new HitBox(centerX, centerY, radius);
	}
	
	public void setHitBox(HitBox inHitBox) {
		hitBox = inHitBox;
	}
	
	public boolean getIsSelectable() {
		return isSelectable;
	}
	
	public boolean getIsSelected() {
		return isSelected;
	}
	
	public void setIsSelected(boolean condition) {
		isSelected = condition;
		setSelection();
	}
	
	public void setSelection() {
		boolean selected = getIsSelected();
		if(selected == false) {
			setHasStroke(false);
			return;
		}
		
		int lineWidth = 2;
		int cap = BasicStroke.CAP_SQUARE;
		int join = BasicStroke.JOIN_BEVEL;
		Color inColour = Color.RED;
		
		setStroke(lineWidth, cap, join, inColour);
		setOutlineShape(Outline.RECTANGLE);
		setHasStroke(true);
	}
	
	public void animate() {
		nextFrame();
	}
	
	public void nextFrame() {
		if(animationQueue.size() == 0) {
			Animation idle = allAnimations.get("idle");
			if(idle == null) {
				return;
			}
			
			animationQueue.add(idle);
		}
		Animation currentAnimation = animationQueue.peekFirst();
		
		BufferedImage nextImage = currentAnimation.getImage();
		currentImage = nextImage;
		
		MediaClip nextClip = currentAnimation.getClip();
		currentClip = nextClip;
		
		currentAnimation.next();
		
		boolean hasNext = currentAnimation.hasNext();
		if(hasNext == false) {
			animationQueue.poll();
		}
	}
	
	public void previousFrame() {
		if(animationQueue.size() == 0) {
			return;
		}
		
		Animation currentAnimation = animationQueue.peekFirst();
		
		currentAnimation.previous();
		
		BufferedImage nextImage = currentAnimation.getImage();
		currentImage = nextImage;
		
		MediaClip nextClip = currentAnimation.getClip();
		currentClip = nextClip;
	}
	
	public void playClip() {
		if(currentClip == null) {
			return;
		}
		
		currentClip.play();
	}
	
	public void update() {
		modified = false;
		
		animate();
		playClip();
		//If no change return
		
		lastModified = new Date();
		modified = true;
	}
	
	@Override
	public boolean equals(Object object) {
		Entity other = (Entity) object;
		
		if(entityId != other.entityId) {
			return false;
		}
		
		return true;
	}
	
	@Override
	public int hashCode() {
		return entityId;
	}
	
	public int compareTo(Entity other) {
		Date otherDate = other.lastModified;
		int comparison = lastModified.compareTo(otherDate);
		
		return comparison;
	}
	
	public boolean checkForCollision(Entity other) {
		boolean collision = other.getHitBox().intersects(getHitBox());
		return collision;
	}
	
	public Entity spawn() {
		Entity child = null;
		
		return child;
	}
	
	public void despawn(Entity toDespawn) {
		
	}

	public class HitBox extends Rectangle {
		private int centerX, centerY;
		private int radius;

		public HitBox(int inCenterX, int inCenterY, int inRadius) {
			super(inCenterX - inRadius / 2, inCenterY - inRadius / 2, inRadius, inRadius);
			centerX = inCenterX;
			centerY = inCenterY;
			radius = inRadius;
		}

		public int getRadius() {
			return radius;
		}

		public boolean overlap (HitBox other) {
			int rise = (int) (centerY - other.getCenterY());
			int run = (int) (centerX - other.getCenterX());
			int distanceBetweenFoci = (int) Math.sqrt(rise * rise + run * run);
			if (distanceBetweenFoci >= (radius + other.getRadius())) {
				return false;
			}
			return true;
		}
	}
	
	public class EntityLabel {
		
		protected String text;
		protected Color colour;
		protected Font font;
		protected int offsetX, offsetY;
		
		public EntityLabel (String inText, Color inColour, Font inFont, int inOffsetX, int inOffsetY) {
			text = inText;
			colour = inColour;
			font = inFont;
			offsetX = inOffsetX;
			offsetY = inOffsetY;
		}
		
		public String getText() {
			return text;
		}
		
		public Color getColour() {
			return colour;
		}
		
		public Font getFont() {
			return font;
		}
		
		public int getOffsetX() {
			return offsetX;
		}
		
		public int getOffsetY() {
			return offsetY;
		}
	}
	
	public class EntityIcon {
		
		//protected BufferedImage image;
		protected Entity icon;
		protected boolean isAnimated = false;
		protected int offsetX, offsetY;
		protected static final int DEFAULT_OFFSET = 50;
		
		public EntityIcon(String inName, String inPath, String inAddon, String inExtension, 
				int inOffsetX, int inOffsetY) {
			icon = new Entity(inName, inPath, inAddon, inExtension);
			offsetX = inOffsetX;
			offsetY = inOffsetY;
		}
		
		public BufferedImage getImage() {
			if(isAnimated == true) {
				icon.animate();
			}
			
			BufferedImage image = icon.getCurrentImage();
			return image;
		}
		
		public boolean getIsAnimated() {
			return isAnimated;
		}
		
		public void setIsAnimated(boolean condition) {
			isAnimated = condition;
		}
		
		public int getOffsetX() {
			return offsetX;
		}
		
		public int getOffsetY() {
			return offsetY;
		}
		
		public void addAnimation(String animationName, int totalFrames) {
			icon.addAnimation(animationName, totalFrames, null);
		}
		
		public void nextFrame() {
			icon.nextFrame();
		}
		
		public void previousFrame() {
			icon.previousFrame();
		}
	}
}
