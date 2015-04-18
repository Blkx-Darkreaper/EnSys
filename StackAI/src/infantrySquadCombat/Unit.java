package infantrySquadCombat;
import java.util.ArrayList;
import java.util.Collections;
import java.util.List;

import static infantrySquadCombat.Global.*;

public class Unit extends Thread implements Comparable<Unit> {
	private String name;
	private String type;
	private String rank;
	private int rankValue;
	private Squad squad;

	private int position;
	private boolean movesRight;
	
	// Battlefield situation
	public boolean underFire;
	public boolean firing;
	public int suppression;
	public boolean suppressed;
	public int cover;
	public int inCover;
	public int threatLevel;
	public int courage;
	public int panic;
	public boolean alliesWounded;
	
	// Unit stats
	private int maxHealth;
	private int health;
	private int stance;
	private int judgement;
	private int discipline;
	private boolean wounded;
	private boolean killed;
	private int experience;
	
	// Combat stats
	public Armament equippedWeapon;
	public List<Threat> allThreats;
	private Unit enemyTarget;
	public boolean aiming;
	
	public Unit (String inName, String inType, String inRank) {
		name = inName;
		type = inType;
		rank = inRank;
		switch (rank) {
		case "Private":
			rank = "Pvt.";
		case "Pvt.":
			rankValue = 0;
			break;
		case "Corporal":
			rank = "Cpl.";
		case "Cpl.":
			rankValue = 1;
			break;
		case "Sergeant":
			rank = "Sgt.";
		case "Sgt.":
			rankValue = 2;
			break;
		case "Lieutenant":
			rank = "Lt.";
		case "Lt.":
			rankValue = 3;
			break;
		case "Captain":	
			rank = "Cpt.";
		case "Cpt.":
			rankValue = 4;
			break;
		default:
			rank = "Gen.";
			rankValue = 10;
		}
		
		squad = null;
		
		position = 0;
		movesRight = true;
		
		underFire = false;
		firing = false;
		suppression = 0;
		suppressed = false;
		cover = 0;
		inCover = 0;
		threatLevel = 0;
		courage = 10;
		panic = 0;
		alliesWounded = false;
		
		maxHealth = 10;
		health = maxHealth;
		stance = 2;
		judgement = 1;
		discipline = 5;
		wounded = false;
		killed = false;
		
		equippedWeapon = new Armament("test rifle", "", 0, 0, 2, 1, 5, 30);
		allThreats = new ArrayList<>();
		enemyTarget = null;
		aiming = false;
	}
	
	public String getUnitName () {
		return name;
	}
	
	public String getRank () {
		return rank;
	}
	
	public Squad getSquad () {
		return squad;
	}
	
	public int getPosition () {
		return position;
	}
	
	public void setStartPosition (int startPosition) {
		position = startPosition;
		
		if(position != 0) {
			movesRight = false;
		}
	}
	
	public void movePosition (int distanceMoved) {
		if(movesRight == true) {
			position += distanceMoved;
		} else {
			position -= distanceMoved;
		}
	}
	
	public int getStance () {
		return stance;
	}
	
	public void changeStance (int newStance) {
		if(newStance < 0) {
			return;
		}
		if(newStance > 2) {
			return;
		}
		
		stance = newStance;
	}
	
	public int getJudgement () {
		return judgement;
	}
	
	public Unit getEnemyTarget () {
		return enemyTarget;
	}
	
	public void chooseEnemyTarget (Unit chosenTarget) {
		enemyTarget = chosenTarget;
	}

	public int compareTo(Unit other) {
		int difference = rankValue - other.rankValue;
		
		if(difference != 0) {
			return difference;
		}
		
		difference = experience - other.experience;
		
		return difference;
	}
	
	public void run() {
		if(killed == true) {
			return;
		}
		
		try {
			status();
			squad.refreshAllUnits();
			respond();
			run();
		} catch (InterruptedException e) {
			e.printStackTrace();
		}
	}
	
	public void respond () {
		
	}
	
	public boolean isWounded() {
		return wounded;
	}
	
	public void armUnit (Armament weaponToAdd) {
		equippedWeapon = weaponToAdd;
	}
	
	public void joinSquad (Squad groupToJoin) {
		squad = groupToJoin;
	}

	private void status() {
		String outputString = name;
		outputString += " is at position: " + position;
		outputString += ", with " + health + "hp";
		outputString += ", ThreatLevel: " + threatLevel;
		
		System.out.println(outputString);
	}
	
	public void refresh (List<Unit> allFriendlies, List<Unit> allEnemies) {
		determineCourage(allFriendlies);
		determineThreatLevel(allEnemies);
		
		if(underFire == false) {
			suppression-= 2;
			
			if(suppression < 0) {
				suppression = 0;
			}
		}
	}
	
	public void determineCourage (List<Unit> allFriendlies) {
		int courageBonus = squad.getMorale();
		
		if(allFriendlies.size() > 1) {
			for(Unit aFriendly : allFriendlies) {
				int distance = getDistanceToTarget(aFriendly);
				int friendlyWoundedBonus = 0;
				
				if(aFriendly.wounded == true) {
					friendlyWoundedBonus = WOUNDED_FRIENDLIES_COURAGE_BONUS;
				}
				
				courageBonus += (FRIENDLY_COURAGE_BONUS + friendlyWoundedBonus) / distance;
			}
		}
		
		courage = courageBonus;
	}
	
	private int determineEffectiveness(Unit targetEnemy) {
		int range = equippedWeapon.getRange();
		int distance = getDistanceToTarget(targetEnemy);
		int enemyProtection = targetEnemy.getProtection();
		int damage = equippedWeapon.getDamage();
		int rateOfFire = equippedWeapon.getROF();
		String targetType = enemyTarget.type;
		float penetration = equippedWeapon.getPenetration(targetType);
		
		int effectiveness = (int)(range / distance * damage * rateOfFire * penetration * (MAX_PROTECTION - enemyProtection)/MAX_PROTECTION);
		
		return effectiveness;
	}

	private int getProtection() {
		return cover * inCover;
	}
	
	public void determineThreatLevel (List<Unit> allEnemies) {
		if(allEnemies.size() == 0) {
			threatLevel = 0;
		}
		
		int totalThreatLevel = 0;
		allThreats.clear();
		
		for(Unit anEnemy : allEnemies) {
			int enemyRange = anEnemy.equippedWeapon.getRange() + 1;
			int distance = getDistanceToTarget(anEnemy);
			int enemyDamage = anEnemy.equippedWeapon.getDamage();
			int enemyROF = anEnemy.equippedWeapon.getROF();
			int enemyHealth = anEnemy.health / anEnemy.maxHealth;
			
			int threat = enemyRange / distance * enemyDamage * enemyROF * enemyHealth;
			int effectiveness = determineEffectiveness(anEnemy);
			allThreats.add(new Threat(this, anEnemy, threat, effectiveness));
			
			totalThreatLevel += threat;
		}
		
		threatLevel = totalThreatLevel;
	}

	private int getDistanceToTarget(Unit target) {
		return Math.abs(position - target.position) + 1;
	}
	
	public int attackTarget () {
		int damage = equippedWeapon.attack(enemyTarget);
		
		if(damage > 0) {
			enemyTarget.unitAttacked(this, damage);
		}
		
		return damage;
	}
	
	public void reload() {
		equippedWeapon.reload();
	}
	
	public void takingFire (int ROF) {
		underFire = true;
		suppression++;
		
		if(suppression > courage) {
			suppressed = true;
		}
	}
	
	public void unitAttacked (Unit attacker, int damage) {
		health -= damage;
		unitWounded();
		
		if(health > 0) {
			return;
		}
		
		unitKilled();
		attacker.killConfirmed(this);
	}
	
	public void unitWounded () {
		wounded = true;
		squad.setUnitsWounded(true);
	}
	
	public void unitKilled () {
		killed = true;
		squad.unitLost(this);
	}
	
	public void killConfirmed (Unit enemy) {
		squad.enemyKilled(enemy);
		System.out.println(name + " has killed " + enemy.name);
	}
	
	public int makeAssessment () {
		int assessment = 0;
		
		int range = equippedWeapon.getRange();
		int distance = getDistanceToTarget(enemyTarget);
		assessment += 2 * range / (distance + 1);
		
		int protection = getProtection();
		assessment += protection / 2;
		
		int nearbyCover = (getCoverAhead() + getCoverBehind()) / 2;
		assessment += nearbyCover * 1;
		
		assessment += courage / 2;
		
		assessment -= panic;
		
		assessment -= threatLevel / 5;
		
		return assessment;
	}
	
	private int getCover() {
		int cover;
		List<Integer> availableCover = squad.getCoverAvailable();
		
		cover = availableCover.get(position);
		
		return cover;
	}

	private int getCoverAhead() {
		int coverAhead;
		List<Integer> availableCover = squad.getCoverAvailable();
		int forwardPosition;
		if(movesRight == true) {
			forwardPosition = position++;
		} else {
			forwardPosition = position--;
		}
		
		coverAhead = availableCover.get(forwardPosition);
		
		return coverAhead;
	}
	
	private int getCoverBehind() {
		int coverBehind;
		List<Integer> availableCover = squad.getCoverAvailable();
		int rearPosition;
		if(movesRight == true) {
			rearPosition = position--;
		} else {
			rearPosition = position++;
		}
		
		coverBehind = availableCover.get(rearPosition);
		
		return coverBehind;
	}

	public void addResponse(Response takeCover) {
		// TODO Auto-generated method stub
		
	}
}
