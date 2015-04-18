package infantrySquadCombat;

public class Threat implements Comparable<Threat> {

	private Unit parent;
	private float effectiveness;
	private Unit target;
	private int threatLevel;
	private int adjustedThreatLevel;
	
	public Threat (Unit parent, Unit inTarget, int inThreatLevel, int inEffectiveness) {
		target = inTarget;
		threatLevel = inThreatLevel;
		effectiveness = inEffectiveness;
		adjustThreatLevel();
	}

	public void adjustThreatLevel () {
		float randomNumber = -0.25f + (float)(Math.random() * 0.5f);
		adjustedThreatLevel = (int) (threatLevel * effectiveness * (1 + randomNumber / parent.getJudgement()));
	}
	
	public Unit getTarget () {
		return target;
	}

	@Override
	public int compareTo(Threat other) {
		int difference = adjustedThreatLevel - other.adjustedThreatLevel;
		return difference;
	}
}
