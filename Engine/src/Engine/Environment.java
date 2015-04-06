package Engine;
import java.awt.Component;
import java.awt.Dimension;
import java.awt.Graphics2D;
import java.awt.Point;
import java.awt.Rectangle;
import java.awt.event.ActionEvent;
import java.awt.event.ActionListener;
import java.awt.image.BufferedImage;
import java.io.IOException;
import java.util.ArrayList;
import java.util.Arrays;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

import javax.swing.JComponent;
import javax.swing.Timer;

import static Engine.Global.*;

public class Environment extends JComponent implements ActionListener {

	private static final long serialVersionUID = 1L;
	protected int environmentId;
	protected static int nextId = 1;
	protected Timer time;
	protected Panel parentPanel;
	protected World world;
	protected Level level;
	protected enum State {MainMenu, Game, GameOver};
	protected State currentState;
	protected Menu mainMenu;
	protected BufferedImage currentImage;
	protected Map<Integer, Entity> allCurrentEntities = new HashMap<>();
	
	public Environment(User inUser, String levelName, String tilesetName, int inAbsCenterX, int inAbsCenterY, int inAbsWidth, int inAbsHeight, Panel inParent) {
		super();
		environmentId = nextId;
		nextId++;
		setSize(inAbsWidth, inAbsHeight);
		Dimension size = new Dimension(inAbsWidth, inAbsHeight);
		setPreferredSize(size);
		parentPanel = inParent;
		currentState = State.MainMenu;
		time = new Timer(DEFAULT_TIME_INTERVAL, this);
		time.start();
		currentImage = new BufferedImage(inAbsWidth, inAbsHeight, BufferedImage.TYPE_INT_ARGB);
		String menuName = "Main menu";
		int menuWidth = 100;
		int menuHeight = 50;
		int titleSpacing = 20;
		int optionSpacing = 10;
		mainMenu = new Menu(menuName, DEFAULT_FONT, inAbsCenterX, inAbsCenterY, menuWidth, menuHeight, 
				titleSpacing, optionSpacing);
		menuName = "Play";
		mainMenu.addMenuOption(menuName, DEFAULT_FONT, menuWidth, menuHeight);
		menuName = "Exit";
		mainMenu.addMenuOption(menuName, DEFAULT_FONT, menuWidth, menuHeight);
		
		//debug
		world = new World(this);
		try {
			level = new Level(levelName, tilesetName, 16);
		} catch (IOException e) {
			System.out.println("Could not load level");
			e.printStackTrace();
		}
/*		try {
			worldWidth = 800;
			worldHeight = 640;
			level = new Level(levelName, tilesetName, 16, worldWidth, worldHeight);
		} catch (IOException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}*/
		int levelWidth = level.getWidth();
		int levelHeight = level.getHeight();
		System.out.println("Width: " + levelWidth + " Height: " + levelHeight);	//debug
		
		Entity test = new Entity();
		addEntityToWorld(test);
		
		int centerX = 200;
		int centerY = 150;
		int width = 300;
		int height = 200;
		int absoluteCenterX = 150;
		int absoluteCenterY = 100;
		
		View main = new View("Main", absoluteCenterX, absoluteCenterY, centerX, centerY, width, height, this);
		main.setMaster(test);
		main.setLinkedToMaster(true);
		addView(main);
		
		View longView = new View("Long", absoluteCenterX + 325, absoluteCenterY + 100, centerX - 20, centerY + 150, 
				width - 100, height + 200, this);
		addView(longView);
		
		View mini = new View("Mini", absoluteCenterX - 50, absoluteCenterY + 200, centerX + 20, centerY + 20, width - 150, 
				height - 50, this);
		addView(mini);
		
		View zoom = new View("Zoom", absoluteCenterX + 125, absoluteCenterY + 200, centerX + 20, centerY + 20, 
				width - 150, height - 50, this);
		zoom.zoomIn(100);
		addView(zoom);
		
		world.addPlayer(inUser);
		inUser.setBehaviour(new Behaviour());
		inUser.setEntity(test);
		bindAllKeys(inUser);
		//end debug
		
		loadEntities();
		//updateViews();
	}
	
	public int getAbsoluteCenterX() {
		Point corner = getLocation();
		int absoluteWidth = getAbsoluteWidth();
		int absoluteCenterX = corner.x + absoluteWidth / 2;
		
		return absoluteCenterX;
	}
	
	public int getAbsoluteCenterY() {
		Point corner = getLocation();
		int absoluteHeight = getAbsoluteHeight();
		int absoluteCenterY = corner.y + absoluteHeight / 2;
		
		return absoluteCenterY;
	}
	
	public int getAbsoluteWidth() {
		Dimension size = getSize();
		int absoluteWidth = size.width;
		return absoluteWidth;
	}
	
	public int getAbsoluteHeight() {
		Dimension size = getSize();
		int absoluteHeight = size.height;
		return absoluteHeight;
	}
	
	public int getLevelWidth() {
		int levelWidth = level.getWidth();
		return levelWidth;
	}
	
	public int getLevelHeight() {
		int levelHeight = level.getHeight();
		return levelHeight;
	}
	
	public Graphics2D getGraphics() {
		if(currentImage == null) {
			int absoluteWidth = getAbsoluteWidth();
			int absoluteHeight = getAbsoluteHeight();
			
			currentImage = new BufferedImage(absoluteWidth, absoluteHeight, BufferedImage.TYPE_INT_ARGB);
		}
		
		Graphics2D g2d = currentImage.createGraphics();
		return g2d;
	}
	
	public BufferedImage getCurrentImage() {
		return currentImage;
	}

	public List<Sprite> getAllSpritesInBounds(Rectangle boundingBox) {
		List<Sprite> allSprites = level.getAllSpritesWithinBounds(boundingBox);
		
		return allSprites;
	}

	public List<Entity> getAllEntities() {
		List<Entity> allEntities = new ArrayList<>(allCurrentEntities.values());
		return allEntities;
	}
	
	public void addEntity(Entity toAdd) {
		int key = toAdd.hashCode();
		allCurrentEntities.put(key, toAdd);
	}
	
	public void removeEntity(Entity toRemove) {
		int key = toRemove.hashCode();
		allCurrentEntities.remove(key);
	}
	
	public void loadEntities() {
		List<Entity> allEntitiesToLoad = world.getAllEntities();
		for(Entity toAdd : allEntitiesToLoad) {
			addEntity(toAdd);
		}
	}
	
	public void updateEntities(List<Entity> updatedEntities, List<Entity> entitiesToRemove) {
		//allCurrentEntities.clear();	//debug
		for(Entity toAdd : updatedEntities) {
			addEntity(toAdd);
		}
		
		//updateSelection();
		
		for(Entity toRemove : entitiesToRemove) {
			removeEntity(toRemove);
		}
	}
	
	protected void mainMenu(Graphics2D g2d) {
		mainMenu.drawMenu(g2d);
	}
	
	protected void game() {}
	
	protected void gameOver() {}

	@Override
	public void actionPerformed(ActionEvent arg0) {
		int width = getWidth();
		int height = getHeight();
		//currentImage = new BufferedImage(width, height, BufferedImage.TYPE_INT_ARGB);
		currentImage = new BufferedImage(600, 400, BufferedImage.TYPE_INT_ARGB);
		Graphics2D g2d = currentImage.createGraphics();
		
		updateViews(g2d);
		
		switch (currentState) {
		case MainMenu:
			mainMenu(g2d);
			break;
		case Game:
			game();
			break;
		case GameOver:
			gameOver();
			break;
		}
		
		g2d.dispose();
		parentPanel.repaint();
	}
	
	public void updateViews(Graphics2D g2d) {
/*		Component[] array = getComponents();*/
		List<View> allViews = new ArrayList<>();
/*		for(Component toAdd : Arrays.asList(array)) {
			allViews.add((View) toAdd);
		}*/
		for(View view : allViews) {
			view.updateView();
			
			BufferedImage image = view.getCurrentImage();
			int absoluteCenterX = view.getAbsCenterX();
			int absoluteCenterY = view.getAbsCenterY();
			int absoluteWidth = view.getWidth();
			int absoluteHeight = view.getHeight();
			int absolutePositionX = absoluteCenterX - absoluteWidth / 2;
			int absolutePositionY = absoluteCenterY - absoluteHeight / 2;
			
			drawImage(g2d, image, absolutePositionX, absolutePositionY, absoluteWidth, absoluteHeight);
			view.setImageUpdated(false);
		}
	}
	
	public void addView(String inName, int inAbsoluteCenterX, int inAbsoluteCenterY, int inCenterX, int inCenterY, int inWidth, int inHeight) {
		View toAdd = new View(inName, inAbsoluteCenterX, inAbsoluteCenterY, inCenterX, inCenterY, inWidth, inHeight, this);

		addView(toAdd);
	}
	
	public void addView(int inAbsoluteCenterX, int inAbsoluteCenterY, int inCenterX, int inCenterY, int inWidth, int inHeight) {
		View toAdd = new View(inAbsoluteCenterX, inAbsoluteCenterY, inCenterX, inCenterY, inWidth, inHeight, this);
		
		addView(toAdd);
	}
	
	private void addView(View toAdd) {
		//allViews.add(toAdd);
		
		int centerX = toAdd.getCenterX();
		int centerY = toAdd.getCenterY();
		int width = toAdd.getWidth();
		int height = toAdd.getHeight();
		boolean withinLevel = checkWithinLevel(centerX, centerY, width, height);
		if(withinLevel == false) {
			throw new IllegalStateException("View is outside the bounds of the level");
		}
		
		add(toAdd);
	}
	
	public void addEntityToWorld() {
		Entity toAdd = new Entity();
		addEntityToWorld(toAdd);
	}
	
	private void addEntityToWorld(Entity toAdd) {
		int centerX = toAdd.getCenterX();
		int centerY = toAdd.getCenterY();
		int width = toAdd.getWidth();
		int height = toAdd.getHeight();
		
		boolean withinLevel = checkWithinLevel(centerX, centerY, width, height);
		if(withinLevel == false) {
			throw new IllegalStateException("Entity is outside the bounds of the level");
		}
		
		world.addEntity(toAdd);
	}
	
	private boolean checkWithinLevel(int inCenterX, int inCenterY, int inWidth,
			int inHeight) {
		boolean withinLevel = level.isWithinBounds(inCenterX, inCenterY, inWidth, inHeight);
		
		return withinLevel;
	}
	
	public void removeView(View toRemove) {
		toRemove.setEnvironment(null);
		remove(toRemove);
		//allViews.remove(toRemove);
	}
}
