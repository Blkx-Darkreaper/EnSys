package Strikeforce;

import java.awt.event.KeyEvent;

import static Strikeforce.Global.*;

public class Fighter extends Aircraft {
	
	private boolean playerFiringA = false;
	private boolean playerFiringB = false;
	private boolean playerBoosting = false;
	
	private final int moveUpKey = KeyEvent.VK_UP;
	private final int moveDownKey = KeyEvent.VK_DOWN;
	private final int moveLeftKey = KeyEvent.VK_LEFT;
	private final int moveRightKey = KeyEvent.VK_RIGHT;
	private final int fireAKey = KeyEvent.VK_A;
	private final int fireBKey = KeyEvent.VK_S;
	private final int boostKey = KeyEvent.VK_D;
	private final int doLoopKey = KeyEvent.VK_F;
	
	public Fighter(String inName, int inX, int inY, int inDirection, int inAltitude, int inSpeed, int inHitPoints) {
		super(inName, inX, inY, inDirection, inAltitude, inSpeed, inHitPoints);
	}
	
	public boolean getPlayerFiringA () {
		return playerFiringA;
	}
	
	public void setPlayerFiringA(boolean inCondition) {
		playerFiringA = inCondition;
	}
	
	public boolean getPlayerFiringB () {
		return playerFiringB;
	}
	
	public void setPlayerFiringB (boolean inCondition) {
		playerFiringB = inCondition;
	}
	
	public boolean getPlayerBoosting() {
		return playerBoosting;
	}
	
	public void setPlayerBoosting (boolean condition) {
		playerBoosting = condition;
	}

	public void keyPressed(KeyEvent e) {
		int key = e.getKeyCode();

		switch (key) {
/*		case KeyEvent.VK_F:
			craft.doLoop();
			break;*/
		case moveLeftKey:
			bankLeft();
			break;
		case moveRightKey:
			bankRight();
			break;
		case moveUpKey:
			moveUp();
			break;
		case moveDownKey:
			moveDown();
			break;
		case fireAKey:
			if(getPlayerFiringA() == true) {
				break;
			}
			
			fireWeaponSetA();
			playerFiringA = true;
			break;
		case fireBKey:
			if(getPlayerFiringB() == true) {
				break;
			}
			
			fireWeaponSetB();
			playerFiringB = true;
			break;
		case boostKey:
			if(getPlayerBoosting() == true) {
				break;
			}
			
			boost();
			playerBoosting = true;
			break;
		}
	}
	
	public void keyReleased(KeyEvent e) {
		int key = e.getKeyCode();

		switch (key) {
		case doLoopKey:
			doLoop();
			break;
		case moveLeftKey:
		case moveRightKey:
			levelOff();
			break;
		case moveUpKey:
		case moveDownKey:
			cruise();
			break;
		case fireAKey:
			setPlayerFiringA(false);
			break;
		case fireBKey:
			setPlayerFiringB(false);
			break;
		case boostKey:
			setPlayerBoosting(false);
			break;
		}
	}

	public void gameover() {
		
	}
}
