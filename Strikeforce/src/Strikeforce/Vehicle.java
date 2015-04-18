package Strikeforce;

import static Strikeforce.Global.*;

import java.awt.image.BufferedImage;
import java.util.ArrayList;
import java.util.List;

public class Vehicle extends Mover {

	protected int hitPoints;
	protected int accelerationRate;
	protected int boostMultiplier = 1;
	protected int revs = 0;
	protected int turnSpeed;
	protected int MAX_SPEED = 3;
	protected boolean invulnerable = false;
	
	protected int firingDirection = 0;
	protected int turretTurnSpeed;
	protected Entity turret;
	protected List<Weapon> weaponSetA;
	protected List<Weapon> weaponSetB;
	
	protected List<Projectile> allProjectiles;
	
	protected BufferedImage imageDestroyed;
	
	public Vehicle(String inName, int inX, int inY, int inDirection, int inAltitude, int inSpeed, int inHitPoints) {
		super(inName, inX, inY, inDirection, inAltitude, inSpeed);
		hitPoints = inHitPoints;
		accelerationRate = 1;
		turnSpeed = 10;
		turretTurnSpeed = 5;
		allProjectiles = new ArrayList<>();
		
		String extension = "png";
		imageDestroyed = loadImage(inName + "destroyed" + "." + extension);
	}
	
	public int getHitPoints() {
		return hitPoints;
	}
	
	public int getFiringDirection() {
		return firingDirection;
	}
	
	public void setFiringDirection(int inDirection) {
		firingDirection = inDirection;
	}
	
	public Entity getTurret() {
		if(turret == null) {
			return null;
		}
		
		turret.setDirection(firingDirection);
		return turret;
	}
	
	public void setTurret(Entity turretToAdd) {
		turret = turretToAdd;
	}

	public void setWeaponSetA (List<Weapon> allWeaponsToAdd) {
		weaponSetA = allWeaponsToAdd;
	}
	
	public void setWeaponSetB (List<Weapon> allWeaponsToAdd) {
		weaponSetB = allWeaponsToAdd;
	}
	
	public List<Projectile> getAllProjectiles() {
		return allProjectiles;
	}
	
	public boolean getInvulnerable() {
		return invulnerable;
	}
	
	public boolean getAirborne() {
		return false;
	}
	
	@Override
	public double getScale() {
		return 0.5 + (double) altitude / MAX_ALTITUDE_SKY;
	}
	
	@Override
	public Effect getExplosionAnimation() {
		String explosionSize = "";
		String animationName = chooseExplosionAnimation(explosionSize);
		int frameSpeed = 2;
		Effect explosion = new Effect(animationName, centerX, centerY, direction, altitude, 
				EXPLOSION_ANIMATION_FRAMES, frameSpeed);
		return explosion;
	}
	
	public void setLocation(int pointX, int pointY) {
		centerX = pointX;
		centerY = pointY;
	}
	
	@Override
	public void update() {
		super.update();
		
		if(weaponSetA != null) {
			for(Weapon aWeapon : weaponSetA) {
				aWeapon.reload();
			}
		}
		
		if(weaponSetB != null) {
			for(Weapon aWeapon : weaponSetB) {
				aWeapon.reload();
			}
		}
	}
	
	public void accelerate(int desiredSpeed) {
		for(int i = 0; i < (accelerationRate * boostMultiplier); i++) {
			int revsNeeded = (desiredSpeed - speed) * REVS_PER_ACCELERATION;
			if(revs == revsNeeded) {
				break;
			}
			
			revs++;
		}
		
		if(revs < REVS_PER_ACCELERATION) {
			return;
		}
		
		speed += revs / REVS_PER_ACCELERATION;
		revs %= REVS_PER_ACCELERATION;

		if(speed > MAX_SPEED) {
			speed = MAX_SPEED;
		}
		
		updateVectors();
	}

	public void decelerate() {
		speed--;
		
		if(speed < 0) {
			speed = 0;
		}
		
		updateVectors();
	}
	
	public void stop() {
		speed = 0;
		updateVectors();
	}
	
	public void turnLeft(int desiredTurn) {
		for(int i = 0; i < turnSpeed; i++) {
			if(i == desiredTurn) {
				break;
			}
			
			direction--;
		}
		
		direction %= 360;
		
		updateVectors();
	}
	
	public void turnRight(int desiredTurn) {
		for(int i = 0; i < turnSpeed; i++) {
			if(i == desiredTurn) {
				break;
			}
			
			direction++;
		}
		
		direction %= 360;
		
		updateVectors();
	}
	
	public void rotateTurretLeft() {
		for(int i = 0; i < turretTurnSpeed; i++) {
			firingDirection--;
		}
		
		firingDirection %= 360;
	}
	
	public void rotateTurretRight() {
		for(int i = 0; i < turretTurnSpeed; i++) {
			firingDirection++;
		}
		
		firingDirection %= 360;
	}
	
	public void fireWeaponSetA() {
		if(weaponSetA.isEmpty() == true) {
			return;
		}
		
		int originX = getCenterX();
		int originY = getCenterY();
		for(Weapon aWeapon : weaponSetA) {
			List<Projectile> firedShots = aWeapon.openFire(originX, originY, firingDirection, altitude);
			
			if(firedShots == null) {
				continue;
			}
			
			allProjectiles.addAll(firedShots);
		}
	}
	
	public void fireWeaponSetB() {
		if(weaponSetB.isEmpty() == true) {
			return;
		}
		
		int originX = getCenterX();
		int originY = getCenterY();
		for(Weapon aWeapon : weaponSetB) {
			List<Projectile> firedShots = aWeapon.openFire(originX, originY, firingDirection, altitude);
			allProjectiles.addAll(firedShots);
		}
	}
	
	public void dealDamage(int damageDealt) {
		hitPoints -= damageDealt;
	}
	
	public boolean criticalDamage() {
		if(hitPoints > 0) {
			return false;
		}
		
		currentImage = imageDestroyed;
		return true;
	}
	
	public void sortie() {
		return;
	}
}
