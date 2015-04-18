package Strikeforce;

import static Strikeforce.Global.*;
import java.awt.image.BufferedImage;

public class Building extends Entity {
	private int hitPoints;
	protected boolean covers;
	protected BufferedImage imageDestroyed;

	public Building(String inName, int inX, int inY, int inDirection,
			int inAltitude, int inHitPoints) {
		super(inName, inX, inY, inDirection, inAltitude);
		hitPoints = inHitPoints;
		String extension = "png";
		imageDestroyed = loadImage(inName + "destroyed" + "." + extension);
	}

	public boolean getCovers() {
		return covers;
	}

	public void setCovers(boolean condition) {
		covers = condition;
	}

	public void spawn() {
		return;
	}

	public void takeoffsAndLandings() {
		return;
	}

	public void dealDamage(int damageDealt) {
		hitPoints -= damageDealt;
	}

	public boolean criticalDamage() {
		if (hitPoints > 0) {
			return false;
		}
		currentImage = imageDestroyed;
		return true;
	}

	public void shiftLeft() {
		centerX -= CELL_SIZE;
		int leftEdge = CELL_SIZE / 2;
		if (centerX < leftEdge) {
			centerX = leftEdge;
		}
	}

	public void shiftRight() {
		centerX += CELL_SIZE;
		int rightEdge = LEVEL_WIDTH - CELL_SIZE / 2;
		if (centerX > rightEdge) {
			centerX = rightEdge;
		}
	}

	public void shiftUp() {
		centerY += CELL_SIZE;
		int topEdge = levelTop - CELL_SIZE / 2;
		if (centerY > topEdge) {
			centerY = topEdge;
		}
	}

	public void shiftDown() {
		centerY -= CELL_SIZE;
		int bottomEdge = CELL_SIZE / 2;
		if (centerY < bottomEdge) {
			centerY = bottomEdge;
		}
	}

	public void rotate() {
		direction += 90;
		direction %= 360;
	}
}