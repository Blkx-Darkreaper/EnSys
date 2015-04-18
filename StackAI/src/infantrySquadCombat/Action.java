package infantrySquadCombat;

public class Action {
	
	private String name;
	private String description;
	private int timeToComplete; // milliseconds
	
	public Action (String inName, String inDescription, int inTime) {
		name = inName;
		description = inDescription;
		timeToComplete = inTime;
	}
	
	public String getName () {
		return name;
	}
	
	public int getTimeToComplete () {
		return timeToComplete;
	}
	
	public String toString() {
		String outputString = description;
		return outputString;
	}
	
	public boolean perform(Unit parent) throws InterruptedException {
		return false;
	}
}
