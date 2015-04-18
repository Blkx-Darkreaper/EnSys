package Strikeforce;

import java.awt.Color;
import java.awt.image.BufferedImage;
import java.io.File;
import java.io.IOException;
import java.util.ArrayList;
import java.util.List;
import java.util.Random;
import javax.imageio.ImageIO;
import javax.swing.ImageIcon;

public class Global {
	public static ResLoader resLoader;
	public static final int FRAME_WIDTH = 800;
	public static final int FRAME_HEIGHT = 600;
	public static final int TIME_INTERVAL = 20;
	public static Menu mainMenu;
	public static final int MENU_WIDTH = (int) (FRAME_WIDTH * 0.8);
	public static final int MENU_HEIGHT = (int) (FRAME_HEIGHT * 0.8);
	public static int MENU_TITLE_HEIGHT;
	public static View view;
	public static final int VIEW_WIDTH = 250;
	public static final int VIEW_HEIGHT = 500;
	public static final int VIEW_POSITION_X = 100;
	public static final int VIEW_POSITION_Y = 50;
	public static final int CELL_SIZE = 16;
	public static int LEVEL_WIDTH;
	public static int LEVEL_HEIGHT;
	public static final Color selectionColour = Color.RED;
	public static final int selectionStroke = 2;
	public static final int UPPER_BOUNDS_X = LEVEL_WIDTH - 10;
	public static final int LOWER_BOUNDS_X = 10;
	public static final int UPPER_BOUNDS_Y = VIEW_HEIGHT - 20;
	public static final int LOWER_BOUNDS_Y = 20;
	public static int levelTop;
	public static final int MAX_ALTITUDE_SKY = 100;
	public static final int REVS_PER_ACCELERATION = 10;
	public static final int HARD_BANK_ANGLE = 1;
	public static final int MAX_BANK_ANGLE = 2;
	public static final int EXPLOSION_ANIMATION_FRAMES = 12;
	public static Random random = new Random();
	public static Weapon singleShot = new Weapon("Single shot", "Basic gun", 8,
			1, 20) {
		@Override
		public List<Projectile> openFire(int originX, int originY,
				int inDirection, int inAltitude) {
			if (reload > 0) {
				return null;
			}
			List<Projectile> allShots = new ArrayList<>();
			String bulletName = "bullet";
			ImageIcon bulletIcon = resLoader.getImageIcon(bulletName + ".png");
			int startX = originX;
			int startY = originY + bulletIcon.getIconHeight() / 2;
			boolean hitsAir = true;
			boolean hitsGround = false;
			boolean live = true;
			/*
			 * allShots.add(new Projectile(bulletIcon, startX, startY,
			 * muzzleVelocity, damage, inDirection, inAltitude));
			 */
			allShots.add(new Projectile(bulletName, startX, startY,
					inDirection, inAltitude, muzzleVelocity, damage, hitsAir,
					hitsGround, live));
			return allShots;
		}
	};
	public static Weapon splitShot = new Weapon("Split shot",
			"Fires two shots away at an angle", 8, 1, 20) {
		@Override
		public List<Projectile> openFire(int originX, int originY,
				int inDirection, int inAltitude) {
			if (reload > 0) {
				return null;
			}
			List<Projectile> allShots = new ArrayList<>();
			String bulletName = "bullet2";
			ImageIcon bulletIcon = resLoader.getImageIcon(bulletName + ".png");
			int startX = originX;
			int startY = originY + bulletIcon.getIconHeight() / 2;
			boolean hitsAir = true;
			boolean hitsGround = false;
			boolean live = true;
			/*
			 * allShots.add(new Projectile(bulletIcon, startX, startY,
			 * muzzleVelocity, damage, inDirection - 15, inAltitude));
			 * allShots.add(new Projectile(bulletIcon, startX, startY,
			 * muzzleVelocity, damage, inDirection + 15, inAltitude));
			 */
			allShots.add(new Projectile(bulletName, startX, startY,
					inDirection - 15, inAltitude, muzzleVelocity, damage,
					hitsAir, hitsGround, live));
			allShots.add(new Projectile(bulletName, startX, startY,
					inDirection + 15, inAltitude, muzzleVelocity, damage,
					hitsAir, hitsGround, live));
			return allShots;
		}
	};
	public static Weapon dumbBomb = new Weapon("Dumb bomb",
			"Drops a small unguided bomb", 2, 10, 50) {
		@Override
		public List<Projectile> openFire(int originX, int originY,
				int inDirection, int inAltitude) {
			if (reload > 0) {
				return null;
			}
			List<Projectile> allShots = new ArrayList<>();
			String bombName = "bomb";
			boolean hitsAir = false;
			boolean hitsGround = true;
			int fuseDelay = 100;
			boolean falls = true;
			int frames = 8;
			int frameSpeed = 2;
			String blastRadius = "huge";
			boolean live = false;
			allShots.add(new Bomb(bombName, originX, originY, inDirection,
					inAltitude, muzzleVelocity, damage, hitsAir, hitsGround,
					live, fuseDelay, falls, blastRadius, frames, frameSpeed));
			return allShots;
		}
	};

	public static String chooseExplosionAnimation(String size) {
		String explosionName = size + "explosion";
		int range;
		switch (size) {
		/*
		 * case "small": range = 1; break;
		 */
		case "big":
			range = 1;
			break;
		case "huge":
			range = 1;
			break;
		/*
		 * case "massive": range = 1; break;
		 */
		default:
			range = 4;
		}
		String choice;
		Random rand = new Random();
		int randomNumber = rand.nextInt(range);
		switch (randomNumber) {
		default:
			choice = "A";
			break;
		case 1:
			choice = "B";
			break;
		case 2:
			choice = "C";
			break;
		case 3:
			choice = "D";
			break;
		}
		explosionName += choice;
		return explosionName;
	}

	public static BufferedImage loadImage(String fileName) {
		File imageFile = resLoader.getFile(fileName);
		BufferedImage image;
		try {
			image = ImageIO.read(imageFile);
		} catch (IOException e) {
			System.out.println("Image could not be read");
			image = null;
		}
		return image;
	}
}