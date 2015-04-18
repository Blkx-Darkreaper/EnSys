package Strikeforce;

import static Strikeforce.Board.checkWithinView;
import static Strikeforce.Board.getPlayer;
import static Strikeforce.Global.random;

import java.awt.Point;
import java.util.ArrayList;
import java.util.Collections;
import java.util.List;
import java.util.Queue;

public class Tank extends Vehicle {
	
	private Action currentAction;
	private Action attackManeuver;
	private int firingFrequency;
	
	private Hangar shelter;
	
	private Queue<Point> patrolPath;

	public Tank(String inName, int inX, int inY, int inDirection, int inSpeed, int inHitPoints) {
		super(inName, inX, inY, inDirection, 0, inSpeed, inHitPoints);
		firingFrequency = 30;
		firingDirection = inDirection;
		currentAction = deploy;
		attackManeuver = openFire;
	}

	@Override
	public void sortie() {
		currentAction.perform(this);
	}
	
	private int distanceToPoint(int pointX, int pointY) {
		int distanceX = centerX - pointX;
		int distanceY = centerY - pointY;
		
		int distanceToPoint = (int) Math.sqrt(distanceX * distanceX + distanceY * distanceY);
		return distanceToPoint;
	}
	
	private int headingToPoint(int pointX, int pointY) {
		int distanceX = pointX - centerX;
		int distanceY = pointY - centerY;
		
		int heading = 360;
		
		if(distanceY == 0) {
			if(distanceX < 0) {
				heading = 270;
				return heading;
			}
			
			heading = 90;
			return heading;
		}
		
		if(distanceY < 0) {
			heading = 180;
		}
		
		double value = distanceX;
		double value2 = distanceY;
		
		heading += (int) Math.toDegrees(Math.atan(value / value2));
		heading %= 360;
		
		return heading;
	}
	
	private void moveToPoint(int pointX, int pointY, int movementSpeed) {
		int distanceToPoint = distanceToPoint(pointX, pointY);
		
		if(distanceToPoint == 0) {
			return;
		}
		
		int headingToPoint = headingToPoint(pointX, pointY);
		
		int changeOfHeading = getHeadingChange(headingToPoint);
		
		if(speed < movementSpeed) {
			accelerate(movementSpeed);
		}
		
		if(speed > movementSpeed) {
			decelerate();
		}
		
		if(changeOfHeading > 0) {
			turnRight(changeOfHeading);
		}
		if(changeOfHeading < 0) {
			turnLeft(changeOfHeading);
		}
	}

	private int getHeadingChange(int desiredHeading) {		
		int changeOfHeading = desiredHeading - direction;
		
		List<Integer> headingChoices = new ArrayList<>();
		headingChoices.add(Math.abs(changeOfHeading));
		headingChoices.add(Math.abs(changeOfHeading + 360));
		headingChoices.add(Math.abs(changeOfHeading - 360));
		
		int bestChoice = Collections.min(headingChoices);
		
		if(bestChoice == Math.abs(changeOfHeading + 360)) {
			changeOfHeading += 360;
		}
		
		if(bestChoice == Math.abs(changeOfHeading - 360)) {
			changeOfHeading -= 360;
		}
		
		return changeOfHeading;
	}
	
	private boolean targetInRange(int targetX, int targetY, int range) {
		int distanceToTarget = distanceToPoint(targetX, targetY);
		
		if(distanceToTarget > range) {
			return false;
		}
		
		return true;
	}
	
	public Action deploy = new Action() {
		public void perform(Vehicle performer) {
			Point departurePoint = shelter.getDeparturePoint();
			int pointX = (int) departurePoint.getX();
			int pointY = (int) departurePoint.getY();
			
			int taxiSpeed = 1;
			moveToPoint(pointX, pointY, taxiSpeed);
			
			int range = 0;
			boolean atPoint = targetInRange(pointX, pointY, range);
			if(atPoint == false) {
				return;
			}
			
			currentAction = patrol;
			shelter.setBusySpawning(false);
		}
		
		public void perform(Bandit performer) {
			return;
		}
	};
	
	public Action patrol = new Action() {
		public void perform(Vehicle performer) {
			boolean banditInView = checkWithinView(performer);
			if(banditInView == true) {
				currentAction = attack;
				return;
			}
			
			if(patrolPath.size() == 0) {
				currentAction = returnToBase;
				return;
			}
			
			Point nextPatrolPoint = patrolPath.peek();
			int pointX = (int) nextPatrolPoint.getX();
			int pointY = (int) nextPatrolPoint.getY();
			
			moveToPoint(pointX, pointY, MAX_SPEED);
			
			int range = 20;
			boolean inRangeOfPoint = targetInRange(pointX, pointY, range);
			if(inRangeOfPoint == false) {
				return;
			}
			
			patrolPath.poll();
		}
		
		public void perform(Bandit performer) {
			return;
		}
	};
	
	public Action attack = new Action() {
		public void perform(Vehicle performer) {
			if(attackManeuver == null) {
				currentAction = returnToBase;
				return;
			}
			
			boolean vehicleInView = checkWithinView(performer);
			if(vehicleInView == true) {
				return;
			}
			
			attackManeuver.perform(performer);
		}

		public void perform(Bandit performer) {
			return;
		}
	};
	
	public Action returnToBase = new Action() {
		public void perform(Vehicle performer) {
			
		}
		
		public void perform(Bandit performer) {
			return;
		}
	};
	
	public Action returnToShelter = new Action() {
		public void perform(Vehicle performer) {
			return;
		}
		
		public void perform(Bandit performer) {
			boolean busySpawning = shelter.getBusySpawning();
			if(busySpawning == true) {
				stop();
				return;
			}
			
			Point spawnPoint = shelter.getSpawnPoint();
			int pointX = (int) spawnPoint.getX();
			int pointY = (int) spawnPoint.getY();

			int taxiSpeed = 1;
			moveToPoint(pointX, pointY, taxiSpeed);
			
			int range = 5;
			boolean atPoint = targetInRange(pointX, pointY, range);
			if(atPoint == false) {
				return;
			}

			stop();
			//shelter.despawn(performer);
		}
	};
	
	public Action openFire = new Action() {
		public void perform(Vehicle performer) {
			Aircraft player = getPlayer();
			int playerX = (int) player.getCenterX();
			int playerY = (int) player.getCenterY();
			
			int directionOfPlayer = headingToPoint(playerX, playerY);
			
			if(directionOfPlayer > firingDirection) {
				rotateTurretRight();
			}
			if(directionOfPlayer < firingDirection) {
				rotateTurretLeft();
			}
			
			int firingAngle = Math.abs(directionOfPlayer - firingDirection);
			if(firingAngle > 5) {
				return;
			}
			
			int randomNumber = random.nextInt(firingFrequency + 1);
			if(randomNumber != 0) {
				return;
			}
			
			fireWeaponSetA();
		}
		
		public void perform(Bandit performer) {
			return;
		}
	};
}
