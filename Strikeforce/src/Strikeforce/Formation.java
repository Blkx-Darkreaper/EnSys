package Strikeforce;

import java.util.List;

public class Formation {

	private String type;
	private List<Bandit> members;
	private int nextMemberIndex = 0;
	
	public Formation (String inType, List<Bandit> inMembers) {
		type = inType;
		members = inMembers;
		
		for(Bandit aBandit : inMembers) {
			aBandit.setFormation(this);
		}
	}
	
	public String getType() {
		return type;
	}
	
	public List<Bandit> getAllMembers() {
		return members;
	}
	
	public Bandit getNextMember() {
		Bandit nextMember = members.get(nextMemberIndex);
		
		nextMemberIndex++;
		nextMemberIndex %= members.size();
		
		return nextMember;
	}
	
	public Bandit getFlightLead() {
		Bandit flightLead = members.get(0);
		return flightLead;
	}
	
	public int getFormationPosition(Bandit toFind) {
		int position = members.indexOf(toFind);
		return position;
	}
}
