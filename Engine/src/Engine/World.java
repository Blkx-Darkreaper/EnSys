package Engine;
import java.awt.BasicStroke;
import java.awt.Color;
import java.awt.event.ActionEvent;
import java.awt.event.ActionListener;
import java.util.ArrayList;
import java.util.LinkedList;
import java.util.List;

import javax.swing.Timer;

import static Engine.Global.*;
import Engine.Global.Outline;

public class World implements ActionListener {

	protected int worldId;
	protected static int nextId = 1;
	protected Timer time;
	protected int timeInterval;
	protected boolean active;
	protected Level currentLevel;
	protected List<User> allPlayers = new LinkedList<>();
	protected List<Entity> allEntities = new LinkedList<>();
	protected Environment parentEnvironment;
	
	public World(Environment inParent) {
		worldId = nextId;
		nextId++;
		parentEnvironment = inParent;
		time = new Timer(DEFAULT_TIME_INTERVAL, this);
		time.start();
		serverLogger.getLogger("Server log");
		active = true;
	}
	
	public void addPlayer(User toAdd) {
		allPlayers.add(toAdd);
	}
	
	public void removePlayer(User toRemove) {
		allPlayers.remove(toRemove);
	}
	
	public List<Entity> getAllEntities() {
		return allEntities;
	}

	public void addEntity(Entity toAdd) {
		allEntities.add(toAdd);
	}
	
	@Override
	public void actionPerformed(ActionEvent arg0) {
		update();
	}
	
	public void update() {
		List<Entity> updatedEntities = new ArrayList<>();
		List<Entity> toRemove = new ArrayList<>();
		for(Entity entity : allEntities) {
			entity.update();
			boolean modified = entity.getModified();
			if(modified == false) {
				continue;
			}
			
			updatedEntities.add(entity);
		}
		
		parentEnvironment.updateEntities(updatedEntities, toRemove);
	}
	
	public void detectCollisions() {}
}
