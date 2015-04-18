package Strikeforce;

import java.awt.image.BufferedImage;
import java.util.ArrayList;
import java.util.List;
import static Strikeforce.Global.*;

public class Aircraft extends Vehicle {
	private BufferedImage levelFlight;
	private BufferedImage bankLeft;
	private BufferedImage bankLeftHard;
	private BufferedImage bankLeftMax;
	private BufferedImage bankRight;
	private BufferedImage bankRightHard;
	private BufferedImage bankRightMax;
	private BufferedImage inverted;
	private BufferedImage invBankLeft;
	private BufferedImage invBankLeftHard;
	private BufferedImage invBankRight;
	private BufferedImage invBankRightHard;
	private BufferedImage boostLevelFlight;
	private BufferedImage boostBankLeft;
	private BufferedImage boostBankLeftHard;
	private BufferedImage boostBankRight;
	private BufferedImage boostBankRightHard;
	private BufferedImage currentLoopImage;
	private List<BufferedImage> loopImages = new ArrayList<>();
	protected final int CRUISING_SPEED = 2;
	protected final int STALL_SPEED = 1;
	protected int climbRate = 10;
	protected final int MAX_ALTITUDE = 100;
	protected final int CRUISING_ALTITUDE = 50;
	protected final int BOOST_DURATION = 20;
	protected boolean airborne;
	protected int roll = 0;
	protected boolean looping = false;
	protected int loop = 0;
	protected boolean doingImmelmann = false;
	protected int immelmann = 0;
	protected boolean doingSplitS = false;
	protected int splitS = 0;
	protected boolean boosting = false;
	protected int boost = 0;

	public Aircraft(String inName, int inX, int inY, int inDirection,
			int inAltitude, int inSpeed, int inHitPoints) {
		super(inName, inX, inY, inDirection, inAltitude, inSpeed, inHitPoints);
		String extension = "png";
		levelFlight = loadImage(inName + "." + extension);
		bankLeft = loadImage(inName + "-bankleft" + "." + extension);
		bankLeftHard = loadImage(inName + "-banklefthard" + "." + extension);
		bankLeftMax = loadImage(inName + "-bankleftmax" + "." + extension);
		bankRight = loadImage(inName + "-bankright" + "." + extension);
		bankRightHard = loadImage(inName + "-bankrighthard" + "." + extension);
		bankRightMax = loadImage(inName + "-bankrightmax" + "." + extension);
		inverted = loadImage(inName + "-inv" + "." + extension);
		invBankLeft = loadImage(inName + "-invbankleft" + "." + extension);
		invBankLeftHard = loadImage(inName + "-invbanklefthard" + "."
				+ extension);
		invBankRight = loadImage(inName + "-invbankright" + "." + extension);
		invBankRightHard = loadImage(inName + "-invbankrighthard" + "."
				+ extension);
		boostLevelFlight = loadImage(inName + "-boost" + "." + extension);
		boostBankLeft = loadImage(inName + "-boostbankleft" + "." + extension);
		boostBankLeftHard = loadImage(inName + "-boostbanklefthard" + "."
				+ extension);
		boostBankRight = loadImage(inName + "-boostbankright" + "." + extension);
		boostBankRightHard = loadImage(inName + "-boostbankrighthard" + "."
				+ extension);
		for (int i = 1; i < 16; i++) {
			loopImages.add(loadImage(inName + "-loop" + i + "." + extension));
		}
		turnSpeed = 5;
		MAX_SPEED = 5;
	}

	public void setY(int position) {
		centerY = position;
	}

	@Override
	public BufferedImage getImage() {
		// System.out.println(bank); //debug
		if (roll == 0) {
			currentImage = levelFlight;
			if (boosting == true) {
				currentImage = boostLevelFlight;
			}
		}
		if (roll > 0) {
			currentImage = bankRight;
			if (boosting == true) {
				currentImage = boostBankRight;
			}
		}
		if (roll > HARD_BANK_ANGLE) {
			currentImage = bankRightHard;
			if (boosting == true) {
				currentImage = boostBankRightHard;
			}
		}
		if (roll < 0) {
			currentImage = bankLeft;
			if (boosting == true) {
				currentImage = boostBankLeft;
			}
		}
		if (roll < -HARD_BANK_ANGLE) {
			currentImage = bankLeftHard;
			if (boosting == true) {
				currentImage = boostBankLeftHard;
			}
		}
		if (looping == true) {
			currentImage = currentLoopImage;
		}
		return currentImage;
	}

	@Override
	public boolean getAirborne() {
		return airborne;
	}

	public void setAirborne(boolean condition) {
		airborne = condition;
	}

	public boolean getLooping() {
		return looping;
	}

	@Override
	public Effect getExplosionAnimation() {
		String explosionSize = "";
		String animationName = chooseExplosionAnimation(explosionSize);
		int frameSpeed = 2;
		Effect explosion = new Effect(animationName, centerX, centerY,
				direction, altitude, EXPLOSION_ANIMATION_FRAMES, frameSpeed);
		return explosion;
	}

	@Override
	public void update() {
		if (looping == true) {
			doLoop();
		}
		if (doingImmelmann == true) {
			doImmelmann();
		}
		if (boosting == true) {
			boost();
		}
		super.update();
	}

	public void doLoop() {
		if (looping == false) {
			if (speed == STALL_SPEED) {
				return;
			}
			loop = 0;
			looping = true;
			invulnerable = true;
			return;
		}
		switch (loop) {
		case 0:
			centerY -= 2;
			currentLoopImage = boostLevelFlight;
			break;
		case 2:
			centerY += 7;
			break;
		case 4:
			centerY += 18;
			currentLoopImage = loopImages.get(0);
			break;
		case 6:
			centerY += 5;
			break;
		case 8:
			centerY += 5;
			break;
		case 10:
			centerY += 7;
			currentLoopImage = loopImages.get(1);
			break;
		case 12:
			centerY -= 6;
			currentLoopImage = loopImages.get(2);
			break;
		case 14:
			centerY -= 5;
			currentLoopImage = loopImages.get(3);
			break;
		case 16:
			centerY -= 1;
			currentLoopImage = loopImages.get(4);
			break;
		case 18:
			centerY -= 19;
			currentLoopImage = loopImages.get(5);
			break;
		case 20:
			centerY -= 6;
			currentLoopImage = loopImages.get(6);
			break;
		case 22:
			centerY -= 9;
			break;
		case 24:
			centerY -= 24;
			currentLoopImage = loopImages.get(8);
			break;
		case 26:
			centerY += 2;
			currentLoopImage = loopImages.get(9);
			break;
		case 28:
			centerY -= 2;
			currentLoopImage = loopImages.get(10);
			break;
		case 30:
			centerY += 3;
			currentLoopImage = loopImages.get(11);
			break;
		case 32:
			centerY += 3;
			break;
		case 34:
			centerY += 9;
			currentLoopImage = loopImages.get(12);
			break;
		case 36:
			centerY += 3;
			currentLoopImage = loopImages.get(13);
			break;
		case 38:
			centerY += 4;
			break;
		case 40:
			centerY += 4;
			currentLoopImage = loopImages.get(14);
			break;
		case 42:
			centerY += 4;
			break;
		case 44:
			currentLoopImage = levelFlight;
			break;
		case 46:
			looping = false;
			invulnerable = false;
			speed--;
			return;
		}
		loop++;
	}

	public void doImmelmann() {
		if (doingImmelmann == false) {
			immelmann = 0;
			doingImmelmann = true;
			invulnerable = true;
			return;
		}
		switch (immelmann) {
		case 0:
			centerY -= 2;
			currentLoopImage = boostLevelFlight;
			break;
		case 2:
			centerY += 7;
			break;
		case 4:
			centerY += 18;
			currentLoopImage = loopImages.get(0);
			break;
		case 6:
			centerY += 5;
			break;
		case 8:
			centerY += 5;
			break;
		case 10:
			centerY += 7;
			currentLoopImage = loopImages.get(1);
			break;
		case 12:
			centerY -= 6;
			currentLoopImage = loopImages.get(2);
			break;
		case 14:
			centerY -= 5;
			currentLoopImage = loopImages.get(3);
			break;
		case 16:
			centerY -= 1;
			currentLoopImage = loopImages.get(4);
			break;
		case 18:
			centerY -= 19;
			currentLoopImage = loopImages.get(5);
			break;
		case 20:
			centerY -= 6;
			currentLoopImage = loopImages.get(6);
			break;
		case 22:
			altitude += 20;
			if (altitude > MAX_ALTITUDE) {
				altitude = MAX_ALTITUDE;
			}
			currentLoopImage = inverted;
			break;
		case 24:
			currentLoopImage = invBankRight;
			break;
		case 26:
			currentLoopImage = invBankRightHard;
			break;
		case 28:
			currentLoopImage = bankRightMax;
			break;
		case 30:
			currentLoopImage = bankRightHard;
			break;
		case 32:
			currentLoopImage = bankRight;
			break;
		case 44:
			currentLoopImage = levelFlight;
			break;
		case 46:
			doingImmelmann = false;
			invulnerable = false;
			return;
		}
		immelmann++;
	}

	public void doSplitS() {
	}

	public void boost() {
		if (boosting == false) {
			boost = 0;
			boostMultiplier = 1;
			int offsetX = (int) (Math.sin(Math.toRadians(direction)) * 3 / 2);
			int offsetY = (int) (Math.cos(Math.toRadians(direction)) * 3 / 2);
			centerX -= offsetX;
			centerY -= offsetY;
			boosting = true;
			return;
		}
		boost++;
		boostMultiplier = (int) Math.round(boost / REVS_PER_ACCELERATION) + 1;
		accelerate(MAX_SPEED);
		if (boost < BOOST_DURATION) {
			return;
		}
		int offsetX = (int) Math.sin(Math.toRadians(direction)) * 3 / 2;
		int offsetY = (int) Math.cos(Math.toRadians(direction)) * 3 / 2;
		centerX += offsetX;
		centerY += offsetY;
		boosting = false;
	}

	public void bankLeft() {
		dx = -speed;
		if (roll == -MAX_BANK_ANGLE) {
			return;
		}
		roll--;
	}

	public void bankRight() {
		dx = speed;
		if (roll == MAX_BANK_ANGLE) {
			return;
		}
		roll++;
	}

	public void levelOff() {
		roll = 0;
		updateVectors();
	}

	public void moveUp() {
		dy = speed * 2;
	}

	public void moveDown() {
		dy = 0;
	}

	public void cruise() {
		updateVectors();
	}

	@Override
	public void turnLeft(int desiredTurn) {
		for (int i = 0; i < Math.max(turnSpeed * speed, 1); i++) {
			if (i == desiredTurn) {
				break;
			}
			direction--;
		}
		direction %= 360;
		updateVectors();
	}

	@Override
	public void turnRight(int desiredTurn) {
		for (int i = 0; i < Math.max(turnSpeed * speed, 1); i++) {
			if (i == desiredTurn) {
				break;
			}
			direction++;
		}
		direction %= 360;
		updateVectors();
	}

	public void climb() {
		for (int i = 0; i < climbRate; i++) {
			altitude++;
		}
		if (altitude > MAX_ALTITUDE) {
			altitude = MAX_ALTITUDE;
		}
		if (altitude > 0) {
			airborne = true;
		}
	}

	public void descend() {
		altitude--;
		if (altitude < 0) {
			altitude = 0;
		}
		if (altitude == 0) {
			airborne = false;
		}
	}
}