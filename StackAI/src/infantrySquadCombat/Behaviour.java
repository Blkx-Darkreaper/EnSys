package infantrySquadCombat;
import static infantrySquadCombat.Global.allConditions;
import static infantrySquadCombat.Global.allResponses;

import java.util.ArrayList;
import java.util.Collections;

public class Behaviour extends Thread {
	
	private String name;
	private Unit parent;
	private ArrayList<Response> allResponses;
	
	public Behaviour (String inName, Unit inParent) {
		name = inName;
		parent = inParent;
		allResponses = new ArrayList<>();
	}

	public void addResponse (Response inResponse) {
		allResponses.add(inResponse);
		int responseIndex = allResponses.indexOf(inResponse);
		inResponse.setPriority(responseIndex);
		Collections.sort(allResponses);
	}
	
	public void displayResponseOrder () {
		String name = parent.getName();
		System.out.println(name + "'s current response order:");
		for(Response aResponse : allResponses) {
			
			int index = allResponses.indexOf(aResponse);
			System.out.println(index + " - " + aResponse.getName() + " Successes: " + aResponse.getSuccesses() + " Attempts: " + aResponse.getAttempts());
		}
		System.out.println("");
	}
	
	public Response respond () throws InterruptedException {
		String name = parent.getName();
		for(Response aResponse : allResponses) {
			System.out.println(name + " - " + aResponse.getName() + "..."); //debug
			boolean success = aResponse.checkAllConditionsMet(parent);
			
			if(success == true) {
				System.out.println(name + " chose to " + aResponse.getDescription() + "."); //debug
				aResponse.attemptResponse(parent);
				//improveResponse(aResponse);
				return aResponse;
			}
		}
		
		System.out.println(name + " doesn't know what to do.\n");
		return null;
	}
	
	public void improveResponse (Response looksLike) {
		int responseIndex = allResponses.indexOf(looksLike);
		
		if (responseIndex == 0) {
			return;
		}
		
		Response toImprove = allResponses.get(responseIndex);
		Response responseAbove = allResponses.get(responseIndex - 1);
		
		int comparison = toImprove.compareTo(responseAbove);
		
		if(comparison <= 0) {
			return;
		}
		
		Collections.swap(allResponses, responseIndex, responseIndex - 1);
	}
	
	public void improveAllResponses () {
		Collections.sort(allResponses);
	}
	
	public int saveAllConditionsIdCode () {
		int sumOfAllConditions = 0;
		
		for(Condition aCondition : allConditions) {
			if(aCondition.test(parent) == true) {
				int id = aCondition.getId();
				sumOfAllConditions += Math.pow(id, 2.0);
			}
		}
		
		return sumOfAllConditions;
	}
}
