package Strikeforce;

import java.awt.Point;
import java.util.Deque;
import java.util.List;
import java.util.Queue;

public class Hangar extends Building {

	private Deque<Bandit> contents;
	private Board world;
	
	private Point spawnPoint;
	private boolean busySpawning;
	private Point departurePoint;
	private Airstrip nearestRunway;
	private Queue<Point> patrolPath;

	public Hangar(String inName, int inX, int inY, int inDirection, int inAltitude, int inHitPoints, Deque<Bandit> inContents, Board inWorld) {
		super(inName, inX, inY, inDirection, inAltitude, inHitPoints);
		
		contents = inContents;
		world = inWorld;
		spawnPoint = new Point(inX, inY);
		
		int distance = Math.max(currentImage.getWidth(null), currentImage.getHeight(null));
		int offsetX = (int) (.75 * distance * Math.sin(Math.toRadians(inDirection)));
		int offsetY = (int) (.75 * distance * Math.cos(Math.toRadians(inDirection)));
		departurePoint = new Point(inX + offsetX, inY + offsetY);
	}
	
	public Point getSpawnPoint() {
		return spawnPoint;
	}
	
	public boolean getBusySpawning() {
		return busySpawning;
	}
	
	public void setBusySpawning(boolean condition) {
		busySpawning = condition;
	}

	public Point getDeparturePoint() {
		return departurePoint;
	}
	
	public void setNearestRunway(Airstrip inNearbyRunway) {
		nearestRunway = inNearbyRunway;
	}
	
	public void setPatrolPath(Queue<Point> inPatrolPath) {
		patrolPath = inPatrolPath;
	}
	
	public void addFormationToContents(Formation formationToAdd) {
		List<Bandit> allBandits = formationToAdd.getAllMembers();
		contents.addAll(allBandits);
	}

	@Override
	public void spawn() {
		if(busySpawning == true) {
			return;
		}
		
		if(contents.size() == 0) {
			return;
		}
		
		Bandit spawnee = contents.pop();
		
		int pointX = (int) spawnPoint.getX();
		int pointY = (int) spawnPoint.getY();
		spawnee.setLocation(pointX, pointY);
		
		spawnee.setDirection(direction);
		
		spawnee.setShelter(this);
		spawnee.setRunway(nearestRunway);
		spawnee.setCircuit();
		spawnee.setPatrolPath(patrolPath);
		
		List<Vehicle> allGroundVehicles = world.getAllVehicles();
		allGroundVehicles.add(spawnee);
		busySpawning = true;
	}
	
	public void despawn(Bandit despawnee) {
		despawnee.stop();
		
		contents.push(despawnee);
		
		world.despawn(despawnee);
	}
}
