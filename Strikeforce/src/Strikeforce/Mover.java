package Strikeforce;

import static Strikeforce.Board.*;
import static Strikeforce.Global.*;

public class Mover extends Entity {
	
	protected int dx, dy;
	protected int speed = 0;
	
	public Mover(int startX, int startY, int inWidth, int inHeight) {
		super(startX, startY, inWidth, inHeight);
	}
	
	public Mover(String inName, int inX, int inY, int inDirection, int inAltitude, int inSpeed) {
		super(inName, inX, inY, inDirection, inAltitude);
		speed = inSpeed;
		updateVectors();
	}
	
	public int getDeltaX() {
		return dx;
	}
	
	public void setDeltaX(int inValue) {
		dx = inValue;
	}
	
	public int getDeltaY() {
		return dy;
	}
	
	@Override
	public void setDirection(int inDirection) {
		super.setDirection(inDirection);
		updateVectors();
	}
	
	public void updateVectors() {
		dx = (int) Math.round(speed * Math.sin(Math.toRadians(direction)));
		dy = (int) Math.round(speed * Math.cos(Math.toRadians(direction)));
	}
	
	public int getSpeed() {
		return speed;
	}
	
	public void setSpeed(int inValue) {
		speed = inValue;
		updateVectors();
	}

	public void update() {
		centerX += dx;
		centerY += dy;
		
/*		int lowerBoundsX = 0 + halfWidth;
		if(centerX < lowerBoundsX) {
			centerX = lowerBoundsX;
		}
		int upperBoundsX = currentLevel.getWidth() - halfWidth;
		if(centerX > upperBoundsX) {
			centerX = upperBoundsX;
		}

		int lowerBoundsY = 0 + halfHeight;
		if(centerY < lowerBoundsY) {
			centerY = lowerBoundsY;
		}*/
	}
	
	public void moveUp() {
		centerY += CELL_SIZE;
	}
	
	public void moveDown() {
		centerY -= CELL_SIZE;
	}
	
	public void moveLeft() {
		centerX -= CELL_SIZE;
	}
	
	public void moveRight() {
		centerX += CELL_SIZE;
	}
}
