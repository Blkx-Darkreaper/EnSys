package infantrySquadCombat;

public class Armament {

	private String name;
	private String description;
	
	private int accuracy;
	private int precision;
	private int damage;
	private int ROF;
	private int range;
	private int magazineSize;
	private int roundsRemaining;
	
	public Armament (String inName, String inDescription, 
			int inAccuracy, int inPrecision, int inDamage, int inROF, int inRange, int inAmmo) {
		
		name = inName;
		description = inDescription;
		accuracy = inAccuracy;
		precision = inPrecision;
		damage = inDamage;
		ROF = inROF;
		range = inRange;
		magazineSize = inAmmo;
		roundsRemaining = inAmmo;
	}
	
	public int getDamage () {
		return damage;
	}
	
	public int getROF () {
		return ROF;
	}
	
	public int getRange () {
		return range;
	}
	
	public int getRoundsRemaining () {
		return roundsRemaining;
	}
	
	public float getPenetration (String targetType) {
		float penetration;
		switch (targetType) {
			case "infantry":
				penetration = 1f;
				break;
			case "tank":
				penetration = 0.25f;
				break;
			default:
				penetration = 1f;
		}
		
		return penetration;
	}
	
	public int attack (Unit target) {
		return fireShot(target);
	}
	
	private int fireShot (Unit target) {
		if(roundsRemaining == 0) {
			return 0;
		}
		
		roundsRemaining--;
		
		// Determine if shot hits
		
		return damage;
	}
	
	public void reload () {
		roundsRemaining = magazineSize;
	}
}
