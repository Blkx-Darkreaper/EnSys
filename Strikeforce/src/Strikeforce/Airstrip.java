package Strikeforce;

import java.awt.Point;
import java.util.LinkedList;
import java.util.Queue;

public class Airstrip extends Building {
	
	private int runwayLength;
	private Point takeoffPoint;
	private int takeoffDirection;
	private Point landingPoint;
	private int landingDirection;
	private Queue<Bandit> landingPattern;
	private Queue<Bandit> takeoffPattern;
	private boolean runwayBusy = false;
	
	public Airstrip(String inName, int inX, int inY, int inDirection, int inAltitude, int inHitPoints) {
		super(inName, inX, inY, inDirection, inAltitude, inHitPoints);
		
		runwayLength = Math.max(getImage().getWidth(null), getImage().getHeight(null));
		
		int pointX = (int) (inX - 0.25 * runwayLength * Math.sin(Math.toRadians(inDirection)));
		int pointY = (int) (inY - 0.25 * runwayLength * Math.cos(Math.toRadians(inDirection)));
		
		takeoffPoint = new Point(pointX, pointY);
		takeoffDirection = inDirection;
		
		int offsetX = (int) (1.75 * runwayLength * Math.sin(Math.toRadians(inDirection)));
		int offsetY = (int) (1.75 * runwayLength * Math.cos(Math.toRadians(inDirection)));		
		landingPoint = new Point(pointX + offsetX, pointY + offsetY);
		landingDirection = (inDirection + 180) % 360;
		
		landingPattern = new LinkedList<>();
		takeoffPattern = new LinkedList<>();
	}
	
	public int getRunwayLength() {
		return runwayLength;
	}

	public Point getTakeoffPoint() {
		return takeoffPoint;
	}
	
	public int getTakeoffDirection() {
		return takeoffDirection;
	}

	public Point getLandingPoint() {
		return landingPoint;
	}
	
	public int getLandingDirection() {
		return landingDirection;
	}
	
	@Override
	public void takeoffsAndLandings() {
		if(runwayBusy == true) {
			return;
		}
		
		Bandit arrivingPlane = landingPattern.poll();
		
		if(arrivingPlane == null) {
			Bandit departingPlane = takeoffPattern.poll();
			
			if(departingPlane == null) {
				return;
			}
			
			departingPlane.setRequestGranted(true);
			runwayBusy = true;
			return;
		}
		
		arrivingPlane.setRequestGranted(true);
		runwayBusy = true;
	}
	
	public void requestTakeoff(Bandit departingPlane) {
		takeoffPattern.offer(departingPlane);
	}

	public void requestLanding(Bandit arrivingPlane) {
		landingPattern.offer(arrivingPlane);
	}
	
	public void clearOfRunway() {
		runwayBusy = false;
	}
}
