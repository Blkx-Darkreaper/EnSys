package Strikeforce;

import java.awt.*;
import java.awt.event.*;
import java.awt.geom.AffineTransform;
import java.awt.image.BufferedImage;
import java.awt.image.WritableRaster;
import java.io.BufferedReader;
import java.io.FileReader;
import java.io.IOException;
import java.util.Collections;
import java.util.Deque;
import java.util.Iterator;
import java.util.LinkedList;
import java.util.List;
import java.util.ArrayList;
import java.util.Queue;
import static Strikeforce.Global.*;
import javax.swing.*;

@SuppressWarnings("serial")
public class Board extends JPanel implements ActionListener {
	private enum State {
		Menu, Planning, Build, Raid
	};

	private State currentState = State.Raid;
	private State previousState;
	private final int menuKey = KeyEvent.VK_ESCAPE;
	private int windowScale = 1;
	private static Level currentLevel;
	private int levelTop;
	private static Fighter playerFighter;
	private static Cursor playerCursor;
	private List<Effect> allEffects;
	private List<Vehicle> allVehicles;
	private static List<Building> allBuildings;
	private static View view;
	private Timer time;

	public Board() {
		resLoader = new ResLoader(this.getClass().getClassLoader());
		String levelName = "germany";
		String tileSetName = "Germany";
		currentLevel = new Level(levelName, tileSetName);
		
		String menuName = "menu";
		List<String> menuOptions = new ArrayList<>();
		menuOptions.add("Planning");
		menuOptions.add("Build");
		menuOptions.add("Raid");
		menuOptions.add("Exit");
		mainMenu = new Menu(menuName, 150, menuOptions);
		
		String viewName = "view";
		int viewX = LEVEL_WIDTH / 2;
		int viewY = VIEW_HEIGHT / 2;
		int direction = 0;
		int altitude = 0;
		view = new View(viewName, viewX, viewY, direction, altitude);
		// view = new View(BACKGROUND_WIDTH / 2, VIEW_HEIGHT / 2, VIEW_WIDTH,
		// VIEW_HEIGHT);
		
		String name = "cursor";
		int cursorX = CELL_SIZE / 2;
		int cursorY = CELL_SIZE / 2;
		levelTop = LEVEL_HEIGHT;
		playerCursor = new Cursor(name, cursorX, cursorY);
		
		String playerName = "f18";
		int startX = 120;
		int startY = 100;
		direction = 0;
		altitude = 50;
		int speed = 1;
		int hitPoints = 1;
		playerFighter = new Fighter(playerName, startX, startY, direction,
				altitude, speed, hitPoints);
		List<Weapon> basicWeaponSetup = new ArrayList<>();
		List<Weapon> otherWeaponSetup = new ArrayList<>();
		basicWeaponSetup.add(singleShot);
		basicWeaponSetup.add(splitShot);
		otherWeaponSetup.add(dumbBomb);
		playerFighter.setWeaponSetA(basicWeaponSetup);
		playerFighter.setWeaponSetB(otherWeaponSetup);
		playerFighter.setHasShadow(true);
		
		allEffects = new ArrayList<>();
		allVehicles = new ArrayList<>();
		allBuildings = new ArrayList<>();
		addEntities();
		
		addKeyListener(new KeyActionListener());
		setFocusable(true);
		time = new Timer(TIME_INTERVAL, this);
		time.start();
	}

	public State getState() {
		return currentState;
	}

	public void setState(State inState) {
		currentState = inState;
	}

	public View getView() {
		return view;
	}

	public static Fighter getPlayer() {
		return playerFighter;
	}

	public List<Vehicle> getAllVehicles() {
		return allVehicles;
	}

	public Timer getTime() {
		return time;
	}

	public void despawn(Vehicle despawnee) {
		for (Iterator<Vehicle> vehicleIter = allVehicles.iterator(); vehicleIter
				.hasNext();) {
			Vehicle aVehicle = vehicleIter.next();
			if (aVehicle != despawnee) {
				continue;
			}
			vehicleIter.remove();
		}
	}

	public void actionPerformed(ActionEvent e) {
		switch (currentState) {
		case Planning:
			planningMode();
			break;
		case Build:
			buildMode();
			break;
		case Raid:
			raidMode();
			break;
		case Menu:
			menu();
			break;
		}
		repaint();
	}

	private class KeyActionListener extends KeyAdapter {
		public void keyReleased(KeyEvent e) {
			State currentState = getState();
			switch (currentState) {
			case Planning:
			case Build:
				playerCursor.keyReleased(e);
				break;
			case Raid:
				playerFighter.keyReleased(e);
				break;
			case Menu:
				break;
			}
		}

		public void keyPressed(KeyEvent e) {
			State currentState = getState();
			int key = e.getKeyCode();
			if (key == menuKey) {
				if (currentState != State.Menu) {
					previousState = currentState;
					setState(State.Menu);
					return;
				}
				if (previousState == null) {
					return;
				}
				setState(previousState);
				return;
			}
			switch (currentState) {
			case Planning:
			case Build:
				playerCursor.keyPressed(e);
				break;
			case Raid:
				playerFighter.keyPressed(e);
				break;
			case Menu:
				break;
			}
		}
	}

	private void updatePlayer(Graphics2D g2d) {
		switch (currentState) {
		case Planning:
		case Build:
			drawEntity(g2d, playerCursor);
			break;
		case Raid:
			drawEntity(g2d, playerFighter);
			break;
		case Menu:
			break;
		}
	}

	private void updateMenu(Graphics2D g2d) {
		drawEntity(g2d, mainMenu);
		for (Iterator<Entity> optionIter = mainMenu.getMenuOptions().iterator(); optionIter
				.hasNext();) {
			Entity menuOption = optionIter.next();
			drawEntity(g2d, menuOption);
		}
	}

	private void updateGroundVehicles(Graphics2D g2d) {
		for (Iterator<Vehicle> groundIter = allVehicles.iterator(); groundIter
				.hasNext();) {
			Vehicle aVehicle = groundIter.next();
			boolean airborne = aVehicle.getAirborne();
			if (airborne == true) {
				continue;
			}
			for (Iterator<Projectile> bulletIter = aVehicle.getAllProjectiles()
					.iterator(); bulletIter.hasNext();) {
				Projectile aProjectile = bulletIter.next();
				if (checkWithinView(aProjectile) == false) {
					bulletIter.remove();
					continue;
				}
				drawEntity(g2d, aProjectile);
			}
			if (checkWithinView(aVehicle) == false) {
				continue;
			}
			drawEntity(g2d, aVehicle);
			Entity turret = aVehicle.getTurret();
			if (turret == null) {
				continue;
			}
			drawEntity(g2d, turret);
		}
	}

	private void updateBuildings(Graphics2D g2d) {
		for (Iterator<Building> buildingIter = allBuildings.iterator(); buildingIter
				.hasNext();) {
			Building aBuilding = buildingIter.next();
			boolean cover = aBuilding.getCovers();
			if (cover == true) {
				continue;
			}
			if (checkWithinView(aBuilding) == false) {
				continue;
			}
			drawEntity(g2d, aBuilding);
		}
	}

	private void updateBuildingCover(Graphics2D g2d) {
		for (Iterator<Building> buildingIter = allBuildings.iterator(); buildingIter
				.hasNext();) {
			Building aBuilding = buildingIter.next();
			boolean cover = aBuilding.getCovers();
			if (cover == false) {
				continue;
			}
			if (checkWithinView(aBuilding) == false) {
				continue;
			}
			drawEntity(g2d, aBuilding);
		}
	}

	private void updateAirEnemies(Graphics2D g2d) {
		/*
		 * if(currentPhase != "Raid") { return; }
		 */
		// allVehicles.addAll(currentLevel.spawnLine(playerFighter.getCenterY()));
		for (Iterator<Vehicle> airborneIter = allVehicles.iterator(); airborneIter
				.hasNext();) {
			Vehicle aBandit = airborneIter.next();
			boolean airborne = aBandit.getAirborne();
			if (airborne == false) {
				continue;
			}
			for (Iterator<Projectile> bulletIter = aBandit.getAllProjectiles()
					.iterator(); bulletIter.hasNext();) {
				Projectile aProjectile = bulletIter.next();
				if (checkWithinView(aProjectile) == false) {
					bulletIter.remove();
					continue;
				}
				drawEntity(g2d, aProjectile);
			}
			if (checkWithinView(aBandit) == false) {
				continue;
			}
			drawEntity(g2d, aBandit);
		}
	}

	private void updateEffects(Graphics2D g2d) {
		if (currentState != State.Raid) {
			return;
		}
		for (Iterator<Effect> effectIter = allEffects.iterator(); effectIter
				.hasNext();) {
			Effect anEffect = effectIter.next();
			if (anEffect.getAnimationOver() == true) {
				continue;
			}
			drawEntity(g2d, anEffect);
		}
	}

	private void updateBackground(Graphics2D g2d) {
		for (Entity aSector : currentLevel.getAllSectors()) {
			if (checkWithinView(aSector) == false) {
				continue;
			}
			drawEntity(g2d, aSector);
		}
	}

	private void planningMode() {
		view.update();
		playerCursor.update();
		keepPlayerInView();
	}

	private void buildMode() {
		view.update();
		playerCursor.update();
		keepPlayerInView();
		for (Building aBuilding : allBuildings) {
			aBuilding.spawn();
			aBuilding.takeoffsAndLandings();
		}
		for (Vehicle aVehicle : allVehicles) {
			aVehicle.sortie();
			aVehicle.update();
			for (Iterator<Projectile> projectileIter = aVehicle
					.getAllProjectiles().iterator(); projectileIter.hasNext();) {
				Projectile aProjectile = projectileIter.next();
				aProjectile.update();
				boolean detonate = aProjectile.getDetonate();
				if (detonate == false) {
					continue;
				}
				allEffects.add(aProjectile.getExplosionAnimation());
				projectileIter.remove();
			}
		}
		for (Iterator<Effect> effectIter = allEffects.iterator(); effectIter
				.hasNext();) {
			Effect anEffect = effectIter.next();
			anEffect.animate();
			boolean animationOver = anEffect.getAnimationOver();
			if (animationOver == false) {
				continue;
			}
			effectIter.remove();
		}
		for (Iterator<Projectile> projectileIter = playerFighter
				.getAllProjectiles().iterator(); projectileIter.hasNext();) {
			Projectile aProjectile = projectileIter.next();
			aProjectile.update();
			boolean detonate = aProjectile.getDetonate();
			if (detonate == false) {
				continue;
			}
			allEffects.add(aProjectile.getExplosionAnimation());
			projectileIter.remove();
		}
	}

	private void raidMode() {
		setViewScrollRate();
		setViewPanRate();
		view.update();
		playerFighter.update();
		keepPlayerInView();
		// System.out.println("X: " + player.getPlayerCraft().getX() + ", Y: " +
		// player.getPlayerCraft().getY()); //debug
		for (Building aBuilding : allBuildings) {
			aBuilding.spawn();
			aBuilding.takeoffsAndLandings();
		}
		for (Vehicle aVehicle : allVehicles) {
			aVehicle.sortie();
			aVehicle.update();
			for (Iterator<Projectile> projectileIter = aVehicle
					.getAllProjectiles().iterator(); projectileIter.hasNext();) {
				Projectile aProjectile = projectileIter.next();
				aProjectile.update();
				boolean detonate = aProjectile.getDetonate();
				if (detonate == false) {
					continue;
				}
				allEffects.add(aProjectile.getExplosionAnimation());
				projectileIter.remove();
			}
		}
		for (Iterator<Effect> effectIter = allEffects.iterator(); effectIter
				.hasNext();) {
			Effect anEffect = effectIter.next();
			anEffect.animate();
			boolean animationOver = anEffect.getAnimationOver();
			if (animationOver == false) {
				continue;
			}
			effectIter.remove();
		}
		for (Iterator<Projectile> projectileIter = playerFighter
				.getAllProjectiles().iterator(); projectileIter.hasNext();) {
			Projectile aProjectile = projectileIter.next();
			aProjectile.update();
			boolean detonate = aProjectile.getDetonate();
			if (detonate == false) {
				continue;
			}
			allEffects.add(aProjectile.getExplosionAnimation());
			projectileIter.remove();
		}
	}

	private void menu() {
	}

	private void addEntities() {
		String name = "airstrip";
		int startX = 150;
		int startY = 2000;
		int direction = 180;
		int altitude = 0;
		int hitPoints = 1;
		Airstrip runway = new Airstrip(name, startX, startY, direction,
				altitude, hitPoints);
		allBuildings.add(runway);
		String testJetName = "f18";
		Deque<Bandit> hangarAircraft = new LinkedList<>();
		startX = 0;
		startY = 0;
		direction = 0;
		altitude = 0;
		int speed = 0;
		hitPoints = 1;
		Bandit bandit1 = new Bandit(testJetName, startX, startY, direction,
				altitude, speed, hitPoints);
		Bandit bandit2 = new Bandit(testJetName, startX, startY, direction,
				altitude, speed, hitPoints);
		List<Weapon> basicWeaponSetup = new ArrayList<>();
		List<Weapon> otherWeaponSetup = new ArrayList<>();
		basicWeaponSetup.add(singleShot);
		basicWeaponSetup.add(splitShot);
		otherWeaponSetup.add(dumbBomb);
		bandit1.setWeaponSetA(basicWeaponSetup);
		List<Bandit> formationMembers = new ArrayList<>();
		formationMembers.add(bandit1);
		formationMembers.add(bandit2);
		String formationType = "line";
		Formation formation = new Formation(formationType, formationMembers);
		bandit1.setFormation(formation);
		bandit2.setFormation(formation);
		hangarAircraft.push(bandit1);
		hangarAircraft.push(bandit2);
		String hangarName = "hangar";
		startX = 60;
		startY = 2150;
		direction = 90;
		altitude = 0;
		hitPoints = 10;
		Hangar hangar = new Hangar(hangarName, startX, startY, direction,
				altitude, hitPoints, hangarAircraft, this);
		Queue<Point> patrolPath = new LinkedList<>();
		patrolPath.offer(new Point(150, 800));
		patrolPath.offer(new Point(50, 600));
		hangar.setPatrolPath(patrolPath);
		hangar.setNearestRunway(runway);
		hangar.setCovers(true);
		allBuildings.add(hangar);
		/*
		 * ImageIcon tankIcon = resLoader.getImageIcon("tank-body.png"); Vehicle
		 * tank = new Vehicle(tankIcon, currentLevel.getWidth() / 2, 300);
		 * ImageIcon turretIcon = resLoader.getImageIcon("tank-turret.png");
		 * Entity turret = new Entity(turretIcon, currentLevel.getWidth() / 2,
		 * 300); tank.setTurret(turret); tank.setDirection(180);
		 * tank.setFiringDirection(180); allGroundVehicles.add(tank);
		 */
	}

	private void keepPlayerInView() {
		int upperBoundsY;
		int lowerBoundsY;
		int playerX;
		int playerY;
		switch (currentState) {
		case Raid:
			boolean looping = playerFighter.getLooping();
			if (looping == true) {
				return;
			}
			upperBoundsY = view.getCenterY() + VIEW_HEIGHT / 2;
			lowerBoundsY = view.getCenterY() - VIEW_HEIGHT / 2;
			playerY = playerFighter.getCenterY();
			int altitude = playerFighter.getAltitude();
			double scale = windowScale + (double) altitude / MAX_ALTITUDE_SKY;
			int halfPlayerHeight = (int) Math.round(playerFighter.getImage()
					.getHeight(null) * scale / 2);
			if ((playerY - halfPlayerHeight) < lowerBoundsY) {
				playerFighter.setY(lowerBoundsY + halfPlayerHeight);
			}
			if ((playerY + halfPlayerHeight) > upperBoundsY) {
				playerFighter.setY(upperBoundsY - halfPlayerHeight);
			}
			break;
		case Planning:
		case Build:
			upperBoundsY = view.getCenterY() + VIEW_HEIGHT / 2;
			lowerBoundsY = view.getCenterY() - VIEW_HEIGHT / 2;
			playerX = playerCursor.getCenterX();
			playerY = playerCursor.getCenterY();
			if ((playerY - CELL_SIZE / 2) < lowerBoundsY) {
				view.moveDown();
			}
			if ((playerY + CELL_SIZE / 2) > upperBoundsY) {
				view.moveUp();
			}
			break;
		}
	}

	private void setViewPanRate() {
		int panRate = playerFighter.getSpeed();
		int panDirection = 0;
		int playerX = playerFighter.getCenterX();
		int viewX = view.getCenterX();
		int thirdViewWidth = VIEW_WIDTH / 3;
		int displacement = playerX - viewX;
		int distance = Math.abs(displacement);
		if (distance > thirdViewWidth) {
			panDirection = distance / displacement;
		}
		panRate *= panDirection;
		view.setDeltaX(panRate);
	}

	private void setViewScrollRate() {
		int scrollRate = playerFighter.getSpeed();
		view.setSpeed(scrollRate);
	}

	public void paintComponent(Graphics g) {
		super.paintComponent(g);
		Graphics2D g2d = (Graphics2D) g;
		updateBackground(g2d);
		detectCollisions();
		updateBuildings(g2d);
		updateGroundVehicles(g2d);
		updateBuildingCover(g2d);
		updatePlayerProjectiles(g2d);
		updateAirEnemies(g2d);
		updatePlayer(g2d);
		updateEffects(g2d);
		// View bounds
		drawEntity(g2d, view); // debug
		if (currentState == State.Menu) {
			updateMenu(g2d);
		}
		g2d.dispose();
	}

	public static boolean checkWithinView(Entity toCheck) {
		return view.checkWithinBounds(toCheck);
	}

	private void detectCollisions() {
		Aircraft playerCraft = playerFighter;
		List<Projectile> allFriendlyProjectiles = playerCraft
				.getAllProjectiles();
		for (Iterator<Vehicle> banditIter = allVehicles.iterator(); banditIter
				.hasNext();) {
			Vehicle aBandit = banditIter.next();
			boolean airborne = aBandit.getAirborne();
			for (Iterator<Projectile> projectileIter = allFriendlyProjectiles
					.iterator(); projectileIter.hasNext();) {
				Projectile aProjectile = projectileIter.next();
				boolean live = aProjectile.getLive();
				if (live == false) {
					continue;
				}
				boolean hitsGround = aProjectile.getHitsGround();
				boolean hitsAir = aProjectile.getHitsAir();
				if (airborne == true) {
					if (hitsAir == false) {
						continue;
					}
				} else {
					if (hitsGround == false) {
						continue;
					}
				}
				boolean collision = aBandit.checkForCollision(aProjectile);
				if (collision == false) {
					continue;
				}
				int damage = aProjectile.getDamage();
				aBandit.dealDamage(damage);
				if (aBandit.criticalDamage() == true) {
					allEffects.add(aBandit.getExplosionAnimation());
					if (airborne == true) {
						banditIter.remove();
					}
				}
				projectileIter.remove();
			}
			List<Effect> effectsToAdd = new ArrayList<>();
			for (Iterator<Effect> effectIter = allEffects.iterator(); effectIter
					.hasNext();) {
				Effect anEffect = effectIter.next();
				boolean hitsAir = anEffect.getHitsAir();
				boolean hitsGround = anEffect.getHitsGround();
				if (airborne == true) {
					if (hitsAir == false) {
						continue;
					}
				} else {
					if (hitsGround == false) {
						continue;
					}
				}
				boolean collision = aBandit.checkForCollision(anEffect);
				if (collision == false) {
					continue;
				}
				int damage = anEffect.getDamage();
				aBandit.dealDamage(damage);
				if (aBandit.criticalDamage() == true) {
					effectsToAdd.add(aBandit.getExplosionAnimation());
					if (airborne == true) {
						banditIter.remove();
					}
				}
			}
			allEffects.addAll(effectsToAdd);
			if (playerCraft.getInvulnerable() == true) {
				continue;
			}
			List<Projectile> allEnemyProjectiles = aBandit.allProjectiles;
			for (Iterator<Projectile> projectileIter = allEnemyProjectiles
					.iterator(); projectileIter.hasNext();) {
				Projectile aProjectile = projectileIter.next();
				boolean collision = playerCraft.checkForCollision(aProjectile);
				if (collision == false) {
					continue;
				}
				int damage = aProjectile.getDamage();
				playerFighter.dealDamage(damage);
				if (playerFighter.criticalDamage() == true) {
					allEffects.add(playerFighter.getExplosionAnimation());
					// gameover();
					return;
				}
				projectileIter.remove();
			}
			if (airborne == false) {
				continue;
			}
			boolean collision = playerCraft.checkForCollision(aBandit);
			if (collision == true) {
				int damage = 5;
				aBandit.dealDamage(damage);
				if (aBandit.criticalDamage() == true) {
					allEffects.add(aBandit.getExplosionAnimation());
					if (airborne == true) {
						banditIter.remove();
					}
				}
				// gameover();
				return;
			}
		}
		for (Iterator<Building> buildingIter = allBuildings.iterator(); buildingIter
				.hasNext();) {
			Building aBuilding = buildingIter.next();
			for (Iterator<Projectile> projectileIter = allFriendlyProjectiles
					.iterator(); projectileIter.hasNext();) {
				Projectile aProjectile = projectileIter.next();
				boolean live = aProjectile.getLive();
				if (live == false) {
					continue;
				}
				boolean hitsGround = aProjectile.getHitsGround();
				if (hitsGround == false) {
					continue;
				}
				boolean collision = aBuilding.checkForCollision(aProjectile);
				if (collision == false) {
					continue;
				}
				int damage = aProjectile.getDamage();
				aBuilding.dealDamage(damage);
				if (aBuilding.criticalDamage() == true) {
					allEffects.add(aBuilding.getExplosionAnimation());
				}
				projectileIter.remove();
			}
			List<Effect> effectsToAdd = new ArrayList<>();
			for (Iterator<Effect> effectIter = allEffects.iterator(); effectIter
					.hasNext();) {
				Effect anEffect = effectIter.next();
				boolean hitsGround = anEffect.getHitsGround();
				if (hitsGround == false) {
					continue;
				}
				boolean collision = aBuilding.checkForCollision(anEffect);
				if (collision == false) {
					continue;
				}
				int damage = anEffect.getDamage();
				aBuilding.dealDamage(damage);
				if (aBuilding.criticalDamage() == true) {
					effectsToAdd.add(aBuilding.getExplosionAnimation());
				}
			}
			allEffects.addAll(effectsToAdd);
		}
	}

	private void drawEntity(Graphics2D g2d, Entity toDraw) {
		if (toDraw == null) {
			return;
		}
		AffineTransform defaultOrientation = g2d.getTransform();
		BufferedImage image = (BufferedImage) toDraw.getImage();
		int width = image.getWidth(null);
		int height = image.getHeight(null);
		double scale = windowScale * toDraw.getScale();
		int scaledWidth = (int) Math.round(width * scale);
		int scaledHeight = (int) Math.round(height * scale);
		int positionXRelativeToViewCenter = (toDraw.getCenterX() - scaledWidth / 2)
				- view.getCenterX();
		int positionYRelativeToViewCenter = (toDraw.getCenterY() + scaledHeight / 2)
				- view.getCenterY();
		int absolutePositionX = VIEW_POSITION_X + VIEW_WIDTH / 2
				+ positionXRelativeToViewCenter;
		int absolutePositionY = VIEW_POSITION_Y + VIEW_HEIGHT / 2
				- positionYRelativeToViewCenter;
		setOpacity(g2d, toDraw);
		setRotation(g2d, toDraw, scaledWidth, scaledHeight, absolutePositionX,
				absolutePositionY);
		drawShadow(g2d, toDraw, scaledWidth, scaledHeight, absolutePositionX,
				absolutePositionY);
		BufferedImage fill = getFill(image, Color.RED, 0);
		// g2d.drawImage(image, absolutePositionX, absolutePositionY, null);
		g2d.drawImage(image, absolutePositionX, absolutePositionY, scaledWidth,
				scaledHeight, null);
		if (fill != null) {
			g2d.drawImage(fill, absolutePositionX, absolutePositionY,
					scaledWidth, scaledHeight, null);
		}
		boolean selected = toDraw.getIsSelected();
		if (selected == false) {
			g2d.setTransform(defaultOrientation);
			return;
		}
		drawBorder(g2d, scaledWidth, scaledHeight, absolutePositionX,
				absolutePositionY);
		g2d.setTransform(defaultOrientation);
	}

	private void drawShadow(Graphics2D g2d, Entity toDraw, int scaledWidth, 
			int scaledHeight, int absolutePositionX, int absolutePositionY) {
		boolean hasShadow = toDraw.getHasShadow();
		if (hasShadow == false) {
			return;
		}
		
		BufferedImage image = toDraw.getImage();
		int width = image.getWidth();
		int height = image.getHeight();
		BufferedImage shadow = new BufferedImage(width, height,
				BufferedImage.TYPE_INT_ARGB);
		int shadowColour = 0;
		int opacity = 50;
		int alphaLoss = (100 - opacity) * 255 / 100;

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
		int altitude = toDraw.getAltitude();
		int offset = (int) (altitude / 2.5) + 2;
		double scale = 0.9 - 0.65 * (double) altitude / MAX_ALTITUDE_SKY;
		int shadowWidth = (int) (scale * scaledWidth);
		int shadowHeight = (int) (scale * scaledHeight);
		g2d.drawImage(shadow, absolutePositionX + offset, absolutePositionY
				+ offset, shadowWidth, shadowHeight, null);
	}

	private void setOpacity(Graphics2D g2d, Entity toDraw) {
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

	private BufferedImage getFill(BufferedImage image, Color colour, int opacity) {
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
		/*
		 * int red = colour.getRed(); int green = colour.getGreen(); int blue =
		 * colour.getBlue(); int width = image.getWidth(); int height =
		 * image.getHeight(); BufferedImage fill = new BufferedImage(width,
		 * height, BufferedImage.TYPE_INT_ARGB); for (int x = 0; x < width; x++)
		 * { for (int y = 0; y < height; y++) { Color pixel = new
		 * Color(image.getRGB(x, y)); int alpha = pixel.getAlpha(); Color
		 * fillPixel = new Color(red, green, blue, alpha); int rgb =
		 * fillPixel.getRGB(); fill.setRGB(x, y, rgb); } }
		 */
		return fill;
	}

	private void drawBorder(Graphics2D g2d, int scaledWidth, int scaledHeight,
			int absolutePositionX, int absolutePositionY) {
		g2d.setStroke(new BasicStroke(selectionStroke));
		g2d.setColor(selectionColour);
		// g2d.setPaint(Color.RED);
		Rectangle border = new Rectangle(absolutePositionX, absolutePositionY,
				scaledWidth, scaledHeight);
		g2d.draw(border);
	}

	private void updatePlayerProjectiles(Graphics2D g2d) {
		if (currentState != State.Raid) {
			return;
		}
		for (Iterator<Projectile> bulletIter = playerFighter
				.getAllProjectiles().iterator(); bulletIter.hasNext();) {
			Projectile aProjectile = bulletIter.next();
			if (checkWithinView(aProjectile) == false) {
				bulletIter.remove();
				continue;
			}
			drawEntity(g2d, aProjectile);
		}
	}

	public static boolean buildingInArea(Rectangle areaToCheck) {
		for (Building aBuilding : allBuildings) {
			Rectangle hitBox = aBuilding.getBounds();
			boolean inArea = areaToCheck.intersects(hitBox);
			if (inArea == false) {
				continue;
			}
			return true;
		}
		return false;
	}

	public static Building selectBuildingInArea(Rectangle areaToCheck,
			Building toIgnore) {
		for (Building aBuilding : allBuildings) {
			if (aBuilding == toIgnore) {
				continue;
			}
			Rectangle hitBox = aBuilding.getBounds();
			boolean inArea = areaToCheck.intersects(hitBox);
			if (inArea == false) {
				continue;
			}
			return aBuilding;
		}
		return null;
	}

	public void gameover() {
		JOptionPane.showMessageDialog(null, "You have been shot down!",
				"Game Over", JOptionPane.INFORMATION_MESSAGE);
		playerFighter.gameover();
	}
}

class View extends Mover {
	public View(int startX, int startY, int inWidth, int inHeight) {
		super(startX, startY, inWidth, inHeight);
	}

	public View(String inName, int startX, int startY, int inDirection,
			int inAltitude) {
		super(inName, startX, startY, inDirection, inAltitude, 0);
	}

	public boolean checkWithinBounds(Entity toCheck) {
		Rectangle viewBox = getBounds();
		return toCheck.getBounds().intersects(viewBox);
	}

	@Override
	public void moveUp() {
		super.moveUp();
		/*
		 * int topEdge = LEVEL_HEIGHT - VIEW_HEIGHT / 2; if(centerY > topEdge) {
		 * centerY = topEdge; }
		 */
	}

	@Override
	public void moveDown() {
		super.moveDown();
		int bottomEdge = VIEW_HEIGHT / 2;
		if (centerY < bottomEdge) {
			centerY = bottomEdge;
		}
	}

	@Override
	public void moveLeft() {
		super.moveLeft();
		int leftEdge = VIEW_WIDTH / 2;
		if (centerX < leftEdge) {
			centerX = leftEdge;
		}
	}

	@Override
	public void moveRight() {
		super.moveRight();
		int rightEdge = LEVEL_WIDTH - VIEW_WIDTH / 2;
		if (centerX > rightEdge) {
			centerX = rightEdge;
		}
	}
}

class Level {
	private static final int CELL_HEIGHT = CELL_SIZE;
	private static final int CELL_WIDTH = CELL_SIZE;
	private List<Entity> allSectors = new ArrayList<>();
	private List<String> mapLines = new ArrayList<>();
	private List<BufferedImage> allTiles = new ArrayList<>();
	// private List<String> spawnLines = new ArrayList<>();
	// private boolean[] spawnedLines;
	private boolean[] renderedTiles;
	private int BACKGROUND_HEIGHT;
	private int BACKGROUND_WIDTH;

	public Level(String levelName, String tileSetName) {
		readLevelFile(levelName);
		loadTileSet(tileSetName);
		generateLevel(2);
	}

	public Entity getSectorAtIndex(int index) {
		if (index < 0) {
			return null;
		}
		if (index >= allSectors.size()) {
			return null;
		}
		return allSectors.get(index);
	}

	public List<Entity> getAllSectors() {
		return allSectors;
	}

	public int getWidth() {
		return BACKGROUND_WIDTH;
	}

	public int getHeight() {
		return BACKGROUND_HEIGHT;
	}

	private void readLevelFile(String levelName) {
		try (BufferedReader br = new BufferedReader(new FileReader("levels/"
				+ levelName + ".lvl"))) {
			for (String line; (line = br.readLine()) != null;) {
				if (line == null) {
					continue;
				}
				mapLines.add(line);
				// spawnLines.add(line);
				int width = line.length() * CELL_WIDTH;
				if (width > LEVEL_WIDTH) {
					LEVEL_WIDTH = width;
				}
			}
		} catch (IOException e) {
			System.out.println("Level load error: " + e);
		}
		// Collections.reverse(spawnLines);
		// spawnedLines = new boolean[spawnLines.size()];
		Collections.reverse(mapLines);
		renderedTiles = new boolean[mapLines.size()];
		LEVEL_HEIGHT = mapLines.size() * CELL_HEIGHT;
	}

	private void loadTileSet(String tileSetName) {
		String extension = "png";
		String fileName = "tileset" + tileSetName + "." + extension;
		BufferedImage tileSet = loadImage(fileName);
		int cellsWide = tileSet.getWidth() / CELL_WIDTH;
		int cellsHigh = tileSet.getHeight() / CELL_HEIGHT;
		for (int y = 0; y < cellsHigh; y++) {
			int offsetY = y * CELL_HEIGHT;
			for (int x = 0; x < cellsWide; x++) {
				int offsetX = x * CELL_WIDTH;
				allTiles.add(tileSet.getSubimage(offsetX, offsetY, CELL_WIDTH,
						CELL_HEIGHT));
			}
		}
	}

	private void generateLevel(int totalSectors) {
		int sectorWidth = LEVEL_WIDTH;
		int sectorHeight = Math.max(LEVEL_HEIGHT / totalSectors, 16);
		int linesPerSector = Math.max(mapLines.size() / totalSectors, 1);
		int startLine = 0;
		int startPositionX = sectorWidth / 2;
		int startPositionY = sectorHeight / 2;
		for (int i = 0; i < totalSectors; i++) {
			generateSector(sectorWidth, sectorHeight, startLine,
					linesPerSector, startPositionX, startPositionY);
			startLine += linesPerSector;
			startPositionY += linesPerSector * CELL_HEIGHT;
		}
		/*
		 * String backgroundName = "Germany1"; Entity background = new
		 * Entity(backgroundName, 0, 0, 0, 0); BufferedImage backgroundImage =
		 * background.getImage(); BACKGROUND_HEIGHT =
		 * backgroundImage.getHeight(null); BACKGROUND_WIDTH =
		 * backgroundImage.getWidth(null); int startPositionX = BACKGROUND_WIDTH
		 * / 2; int startPositionY = BACKGROUND_HEIGHT / 2; int direction = 0;
		 * int altitude = 0; int levelHeight = BACKGROUND_HEIGHT; for(int i = 1;
		 * i < 9; i++) { backgroundName = "Germany" + i; Entity sector = new
		 * Entity(backgroundName, startPositionX, startPositionY, direction,
		 * altitude); allSectors.add(sector); startPositionY +=
		 * sector.getImage().getHeight(); levelHeight +=
		 * sector.getImage().getHeight(); } LEVEL_WIDTH = BACKGROUND_WIDTH;
		 * LEVEL_HEIGHT = levelHeight;
		 */
	}

	private void generateSector(int sectorWidth, int sectorHeight,
			int startLine, int linesPerSector, int startPositionX,
			int startPositionY) {
		BufferedImage sectorBackground = new BufferedImage(sectorWidth,
				sectorHeight, BufferedImage.TYPE_INT_ARGB);
		Graphics2D g2d = sectorBackground.createGraphics();
		for (int y = 0; y < linesPerSector; y++) {
			String line = mapLines.get(y + startLine);
			int cornerY = sectorHeight - (y + 1) * CELL_HEIGHT;
			for (int x = 0; x < line.length(); x++) {
				char character = line.charAt(x);
				int tileValue = (int) character - 65; // start with A being 0
				BufferedImage tile = allTiles.get(tileValue);
				int cornerX = x * CELL_WIDTH;
				g2d.drawImage(tile, cornerX, cornerY, null);
			}
		}
		Entity sector = new Entity(sectorBackground, startPositionX,
				startPositionY);
		allSectors.add(sector);
		g2d.dispose();
	}
	/*
	 * public List<Aircraft> spawnLine(int playerY) { List<Aircraft> bandits =
	 * new ArrayList<>(); int index = playerY / CELL_HEIGHT; if(index <
	 * spawnLines.size() && !spawnedLines[index]) { spawnedLines[index] = true;
	 * // Set so we don't spawn these guys again String banditName = "f18"; //
	 * Look through the chars of the level file's current line int x = 20;
	 * for(char c : spawnLines.get(index).toCharArray()) { switch(c) { case '1':
	 * { // Basic bandit int y = VIEW_HEIGHT + playerY; int direction = 180; int
	 * altitude = 25; int speed = 1; int hitpoints = 1; Aircraft bandit = new
	 * Aircraft(banditName, x, y, direction, altitude, speed, hitpoints);
	 * //System.out.println("Spawning basic bandit at: " + bandit.getX() + ", "
	 * + bandit.getY()); //bandit.setSpeed(-1); bandit.setAirborne(true);
	 * bandits.add(bandit); break; } } x += CELL_WIDTH; } } return bandits; }
	 */
}