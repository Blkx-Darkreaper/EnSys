package infantrySquadCombat;
import java.util.ArrayList;
import java.util.List;

public class Response implements Comparable<Response> {
	
	private String name;
	private String description;
	private int priority;
	private int attempts;
	private int successes;
	private List<Condition> allConditions;
	private List<Action> allActions;
	
	public Response (String inName, String inDescription) {
		name = inName;
		description = inDescription;
		priority = 0;
		attempts = 0;
		successes = 0;
		allConditions = new ArrayList<>();
		allActions = new ArrayList<>();
	}
	
	public String getName () {
		return name;
	}
	
	public String getDescription () {
		return description;
	}
	
	public int getPriority () {
		return priority;
	}
	
	public void setPriority (int inPriority) {
		priority = inPriority;
	}
	
	public int getAttempts () {
		return attempts;
	}
	
	public int getSuccesses () {
		return successes;
	}
	
	public void addCondition (Condition conditionToAdd) {		
		allConditions.add(conditionToAdd);
	}
	
	public boolean checkAllConditionsMet (Unit parent) {
		for(Condition aCondition : allConditions) {
			boolean outcome = aCondition.test(parent);
			if(outcome == false) {
				return false;
			}
		}
		
		attempts++;
		return true;
	}
	
	public boolean attemptResponse(Unit parent) throws InterruptedException {
		performActions(parent);
		successes++;
		return true;
	}
	
	public void addAction (Action actionToAdd) {
		allActions.add(actionToAdd);
	}
	
	public void performActions (Unit parent) throws InterruptedException {
		//System.out.println(description);
		for(Action anAction : allActions) {
			//System.out.println(anAction); //debug
			boolean performed = anAction.perform(parent);
			if(performed == true) {
				System.out.println(parent.getUnitName() + ": " + anAction.getName()); //debug
			}
		}
		System.out.println("");
	}

	public int compareTo(Response other) {
		int difference;
		
		if(attempts == 0) {
			difference = priority - other.priority;
			return difference;
		}
		
		int successPercentage = successes/attempts;
		int otherSuccessPercentage = 0;
		
		if(other.attempts != 0) {
			otherSuccessPercentage = other.successes/other.attempts;
		}
		
		difference = successPercentage - otherSuccessPercentage;
		
		if(difference != 0) {
			return difference;
		}
		
		difference = attempts - other.attempts;
		return difference;
	}
	
	public boolean equals(Response other) {
		boolean match = name.equals(other.name);
		
		return match;
	}
}
