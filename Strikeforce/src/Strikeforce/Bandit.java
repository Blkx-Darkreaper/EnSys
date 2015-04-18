package Strikeforce;

import java.awt.Point;
import java.util.ArrayList;
import java.util.Collections;
import java.util.List;
import java.util.Queue;

import static Strikeforce.Global.*;
import static Strikeforce.Board.*;

public class Bandit extends Aircraft {
	
	private Action currentAction;
	private Action attackManeuver;
	private int firingFrequency;
	
	private Hangar shelter;
	private Airstrip runway;
	private boolean requestGranted = false;
	
	private Formation formation;
	private boolean holdingFormation = false;
	
	private int nextPointIndex = 0;
	private List<Point> circuit;
	private Queue<Point> patrolPath;
	private boolean performOnce;
	
	private Point nextWayPoint;
	private int rangeToWayPoint;
	private Action nextAction;
	
	public Bandit(String inName, int inX, int inY, int inDirection, int inAltitude, int inSpeed, int inHitPoints) {
		super(inName, inX, inY, inDirection, inAltitude, inSpeed, inHitPoints);
		firingFrequency = 30;
		firingDirection = inDirection;
		currentAction = deploy;
		attackManeuver = firingPass;
	}
	
	public void setAttackManeuver(Action inAttackManeuver) {
		attackManeuver = inAttackManeuver;
	}
	
	public void setShelter(Hangar inShelter) {
		shelter = inShelter;
	}
	
	public void setRunway(Airstrip inRunway) {
		runway = inRunway;
	}

	public void setRequestGranted (boolean condition) {
		requestGranted = condition;
	}
	
	public void setFormation(Formation inFormation) {
		formation = inFormation;
	}
	
	public void setCircuit() {
		circuit = new ArrayList<>();
		Point point1 = runway.getLandingPoint();
		circuit.add(point1);
		int pointX = (int) point1.getX();
		int pointY = (int) point1.getY();
		
		Point point2 = new Point(pointX + 50, pointY - 100);
		circuit.add(point2);
	}
	
	public void setPatrolPath(Queue<Point> inPatrolPath) {
		patrolPath = inPatrolPath;
	}
	
	@Override
	public void sortie() {
		currentAction.perform(this);
	}
	
	public void headToWayPoint() {
		int pointX = (int) nextWayPoint.getX();
		int pointY = (int) nextWayPoint.getY();
		
		moveToPoint(pointX, pointY, speed);
		
		boolean atPoint = targetInRange(pointX, pointY, rangeToWayPoint);
		if(atPoint == false) {
			return;
		}
		
		nextAction.perform(this);
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
	
	private void flyCircuit(List<Point> allPoints, int airSpeed) {
		Point nextPoint = allPoints.get(nextPointIndex);
		int pointX = (int) nextPoint.getX();
		int pointY = (int) nextPoint.getY();
		moveToPoint(pointX, pointY, airSpeed);
		
		int range = 10;
		boolean atPoint = targetInRange(pointX, pointY, range);
		if(atPoint == false) {
			return;
		}
		
		nextPointIndex++;
		
		nextPointIndex %= allPoints.size();
	}
	
	private void flyFormation(int spread) {
		int formationPosition = formation.getFormationPosition(this);
		
		if(formationPosition == 0) {
			return;
		}
		
		Aircraft flightLead = formation.getFlightLead();
		int leadX = flightLead.getCenterX();
		int leadY = flightLead.getCenterY();
		int leadDirection = flightLead.getDirection();
		
		int offsetX;
		int offsetY;
		int distance = spread * formationPosition;
		String formationType = formation.getType();
		
		switch (formationType) {
		case "line": // spreads out to the right of the lead
			offsetX = (int) (distance * Math.cos(Math.toRadians(leadDirection)));
			offsetY = (int) (distance * Math.sin(Math.toRadians(leadDirection)));
			break;
		case "column":
			offsetX = (int) (distance * Math.sin(Math.toRadians(leadDirection)));
			offsetY = (int) (distance * Math.cos(Math.toRadians(leadDirection)));
			break;
		case "vee":
			distance = (int) (spread * Math.ceil(formationPosition / 2));
			if((formationPosition % 2) != 0) {
				offsetX = (int) (-distance * Math.sin(Math.toRadians(leadDirection + 45)));
				offsetY = (int) (-distance * Math.cos(Math.toRadians(leadDirection + 45)));
			}
			else {
				offsetX = (int) (-distance * Math.sin(Math.toRadians(leadDirection - 45)));
				offsetY = (int) (-distance * Math.cos(Math.toRadians(leadDirection - 45)));
			}
			break;
		default:
			offsetX = 0;
			offsetY = 0;
		}
		
		int distanceToFormationPoint = distanceToPoint(leadX + offsetX, leadY + offsetY);
		
		if(distanceToFormationPoint > spread) {
			moveToPoint(leadX + offsetX, leadY + offsetY, MAX_SPEED);
		} else {
			int formationSpeed = flightLead.getSpeed();
			moveToPoint(leadX + offsetX, leadY + offsetY, formationSpeed);
		}
		
		int range = 5;
		boolean atFormationPoint = targetInRange(leadX + offsetX, leadY + offsetY, range);
		if(atFormationPoint == false) {
			return;
		}
		int changeOfDirection = getHeadingChange(leadDirection);
		
		if(changeOfDirection > 0) {
			turnRight(changeOfDirection);
		}
		if(changeOfDirection < 0) {
			turnLeft(changeOfDirection);
		}
		
		holdingFormation = true;
	}
	
	public void despawn() {
		
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
			
			currentAction = taxiToRunway;
			shelter.setBusySpawning(false);
		}
		
		public void perform(Bandit performer) {
			perform((Vehicle) performer);
		}
	};
	
	public Action taxiToRunway = new Action() {
		public void perform(Bandit performer) {
			Point takeoffPoint = runway.getTakeoffPoint();
			int pointX = (int) takeoffPoint.getX();
			int pointY = (int) takeoffPoint.getY();
			
			int taxiSpeed = 1;
			moveToPoint(pointX, pointY, taxiSpeed);
			
			int range = 50;
			boolean nearPoint = targetInRange(pointX, pointY, range);
			if(nearPoint == false) {
				return;
			}
			
			currentAction = waitForRunway;
			runway.requestTakeoff(performer);
		}
		
		public void perform(Vehicle performer) {
			return;
		}
	};
	
	public Action waitForRunway = new Action() {
		public void perform(Bandit performer) {
			if(requestGranted == false) {
				stop();
				return;
			}
			
			currentAction = prepareForTakeoff;
			requestGranted = false;
		}
		
		public void perform(Vehicle performer) {
			return;
		}
	};
	
	public Action prepareForTakeoff = new Action() {
		public void perform(Bandit performer) {
			Point takeoffPoint = runway.getTakeoffPoint();
			int pointX = (int) takeoffPoint.getX();
			int pointY = (int) takeoffPoint.getY();
			
			int taxiSpeed = 1;
			moveToPoint(pointX, pointY, taxiSpeed);
			
			int range = 0;
			boolean atPoint = targetInRange(pointX, pointY, range);
			if(atPoint == false) {
				return;
			}
			
			stop();
			
			int takeoffDirection = runway.getTakeoffDirection();
			int changeOfDirection = takeoffDirection - direction;
			changeOfDirection %= 360;
			
			if(changeOfDirection > 0) {
				turnRight(changeOfDirection);
				return;
			}
			if(changeOfDirection < 0) {
				turnLeft(changeOfDirection);
				return;
			}
			
			currentAction = takeoff;
		}
		
		public void perform(Vehicle performer) {
			return;
		}
	};
	
	public Action takeoff = new Action() {
		public void perform(Bandit performer) {
			if(speed < CRUISING_SPEED) {
				if(boosting == true) {
					return;
				}
				
				if(performOnce == false) {
					boost();
					performOnce = true;
					return;
				}
				
				accelerate(CRUISING_SPEED);
				return;
			}
			
			Point takeoffPoint = runway.getTakeoffPoint();
			int pointX = (int) takeoffPoint.getX();
			int pointY = (int) takeoffPoint.getY();
			int distanceFromTakeoff = distanceToPoint(pointX, pointY);
			int runwayLength = runway.getRunwayLength();
			
			if(distanceFromTakeoff < (0.6 * runwayLength)) {
				return;
			}
			
			if(altitude < CRUISING_ALTITUDE) {
				climb();
				return;
			}
			
			currentAction = formUp;
			runway.clearOfRunway();
			performOnce = false;
		}
		
		public void perform(Vehicle performer) {
			return;
		}
	};
	
	public Action formUp = new Action() {
		public void perform(Vehicle performer) {
			return;
		}

		public void perform(Bandit performer) {
			if(formation == null) {
				currentAction = patrol;
				return;
			}
			
			int formationPosition = formation.getFormationPosition(performer);
			if(formationPosition != 0) {
				int spread = 20;
				flyFormation(spread);
				return;
			}
			
			holdingFormation = true;
			
			List<Bandit> allFormationMembers = formation.getAllMembers();
			for(Bandit anAircraft : allFormationMembers) {
				boolean aircraftHoldingFormation = anAircraft.holdingFormation;
				
				if(aircraftHoldingFormation == false) {
					flyCircuit(circuit, STALL_SPEED);
					return;
				}
			}
			
			currentAction = patrol;
		}
	};
	
	public Action patrol = new Action() {
		public void perform(Vehicle performer) {
			return;
		}

		public void perform(Bandit performer) {
			boolean banditInView = checkWithinView(performer);
			if(banditInView == true) {
				currentAction = attack;
				return;
			}
			
			int formationPosition = formation.getFormationPosition(performer);
			if(formationPosition != 0) {
				int spread = 20;
				flyFormation(spread);
				return;
			}
			
			if(patrolPath.size() == 0) {
				currentAction = returnToBase;
				return;
			}
			
			Point nextPatrolPoint = patrolPath.peek();
			int pointX = (int) nextPatrolPoint.getX();
			int pointY = (int) nextPatrolPoint.getY();
			
			moveToPoint(pointX, pointY, CRUISING_SPEED);
			
			int range = 20;
			boolean inRangeOfPoint = targetInRange(pointX, pointY, range);
			if(inRangeOfPoint == false) {
				return;
			}
			
			patrolPath.poll();
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
			if(attackManeuver == null) {
				currentAction = returnToBase;
				return;
			}
			
			attackManeuver.perform(performer);
			
			boolean banditInView = checkWithinView(performer);
			if(banditInView == true) {
				return;
			}
			
			currentAction = returnToBase;
		}
	};
	
	public Action returnToBase = new Action() {
		public void perform(Vehicle performer) {
			return;
		}
		
		public void perform(Bandit performer) {
			Point landingPoint = runway.getLandingPoint();
			int pointX = (int) landingPoint.getX();
			int pointY = (int) landingPoint.getY();
			
			moveToPoint(pointX, pointY, CRUISING_SPEED);
			
			int range = 50;
			boolean nearPoint = targetInRange(pointX, pointY, range);
			if(nearPoint == false) {
				return;
			}
			
			currentAction = prepareForLanding;
			runway.requestLanding(performer);
		}
	};
	
	public Action prepareForLanding = new Action() {
		public void perform(Vehicle performer) {
			return;
		}
		
		public void perform(Bandit perfomer) {
			Point landingPoint = runway.getLandingPoint();
			int pointX = (int) landingPoint.getX();
			int pointY = (int) landingPoint.getY();
			
			moveToPoint(pointX, pointY, STALL_SPEED + 1);
			
			int range = 10;
			boolean atPoint = targetInRange(pointX, pointY, range);
			if(atPoint == false) {		
				return;
			}
			
			if(requestGranted == false) {
				currentAction = holdingPattern;
				return;
			}
			
			currentAction = land;
			requestGranted = false;
		}
	};
	
	public Action holdingPattern = new Action() {
		public void perform(Vehicle performer) {
			return;
		}
		
		public void perform(Bandit performer) {
			if(requestGranted == true) {
				currentAction = prepareForLanding;
			}
			
			flyCircuit(circuit, STALL_SPEED + 1);
		}
	};
	
	public Action land = new Action() {
		public void perform(Vehicle performer) {
			return;
		}
		
		public void perform(Bandit performer) {
			if(speed != 1) {
				decelerate();
			}
			
			int landingDirection = runway.getLandingDirection();
			int changeOfDirection = landingDirection - direction;
			changeOfDirection %= 360;
			
			if(changeOfDirection > 0) {
				turnRight(changeOfDirection);
			}
			if(changeOfDirection < 0) {
				turnLeft(changeOfDirection);
			}
			
			Point touchdownPoint = runway.getTakeoffPoint();
			int pointX = (int) touchdownPoint.getX();
			int pointY = (int) touchdownPoint.getY();
			
			int range = (int) (0.4 * runway.getRunwayLength());
			boolean nearPoint = targetInRange(pointX, pointY, range);
			if(nearPoint == false) {
				return;
			}
			
			if(altitude != 0) {
				descend();
			}
			
			range = 5;
			nearPoint = targetInRange(pointX, pointY, range);
			if(nearPoint == false) {
				return;
			}
			
			stop();
			currentAction = clearRunway;
		}
	};
	
	public Action clearRunway = new Action() {
		public void perform(Vehicle performer) {
			return;
		}
		
		public void perform(Bandit performer) {
			int newDirection = runway.getLandingDirection() + 90;
			int changeOfDirection = newDirection - direction;
			changeOfDirection %= 360;
			
			if(changeOfDirection > 0) {
				turnRight(changeOfDirection);
			}
			if(changeOfDirection < 0) {
				turnLeft(changeOfDirection);
			}
			
			int taxiSpeed = 1;
			if(speed < taxiSpeed) {
				accelerate(taxiSpeed);
			}
			if(speed > taxiSpeed) {
				decelerate();
			}
			
			Point takeoffPoint = runway.getTakeoffPoint();
			int pointX = (int) takeoffPoint.getX();
			int pointY = (int) takeoffPoint.getY();
			
			int distanceFromPoint = distanceToPoint(pointX, pointY);
			if(distanceFromPoint < 50) {
				return;
			}
		
			currentAction = taxiToShelter;
			runway.clearOfRunway();
		}
	};
	
	public Action taxiToShelter = new Action() {
		public void perform(Vehicle performer) {
			return;
		}
		
		public void perform(Bandit performer) {
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
			
			currentAction = returnToShelter;
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
	
	public Action firingPass = new Action() {
		public void perform(Vehicle performer) {
			return;
		}
		
		public void perform(Bandit performer) {
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
	};
}
