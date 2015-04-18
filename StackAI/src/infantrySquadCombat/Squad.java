package infantrySquadCombat;
import java.util.ArrayList;
import java.util.List;

public class Squad {

	private String name;
	private Battlefield currentBattlefield;
	private List<Unit> allUnits;
	private List<Unit> allEnemies;
	private int morale;
	private boolean unitsWounded;
	public boolean squadScattered;
	public boolean tankSupport;
	public boolean artillerySupport;
	public boolean closeAirSupport;
	
	public Squad (String inName, List<Unit> startingUnits) {
		name = inName;
		allUnits = startingUnits;
		for(Unit aUnit : allUnits) {
			aUnit.joinSquad(this);
		}
		allEnemies = new ArrayList<>();
		morale = 10;
		unitsWounded = false;
		squadScattered = false;
		tankSupport = false;
		artillerySupport = false;
		closeAirSupport = false;
	}
	
	public String getName() {
		return name;
	}
	
	public List<Unit> getAllFriendlies() {
		return allUnits;
	}
	
	public List<Unit> getAllEnemies() {
		return allEnemies;
	}
	
	public int getMorale() {
		return morale;
	}
	
	public boolean getUnitsWounded () {
		return unitsWounded;
	}
	
	public void setUnitsWounded (boolean woundedStatus) {
		unitsWounded = woundedStatus;
	}
	
	public void reinforce (List<Unit> reinforcements) {
		allUnits.addAll(reinforcements);
		for(Unit aUnit : reinforcements) {
			aUnit.joinSquad(this);
		}
		refreshAllUnits();
	}
	
	public void enemyReinforces (List<Unit> enemyReinforcements) {
		allEnemies.addAll(enemyReinforcements);
		refreshAllUnits();
	}
	
	public void refreshAllUnits () {
		Unit commander = allUnits.get(0);
		int commanderPosition = commander.getPosition();
		for(Unit aUnit : allUnits) {
			aUnit.refresh(allUnits, allEnemies);
		}
	}
	
	public void unitLost (Unit kia) {
		allUnits.remove(kia);
		int moraleLoss = morale / allUnits.size();
		
		morale -= moraleLoss;
	}
	
	public void enemyKilled (Unit kia) {
		int moraleBoost = 1;
		
		morale += moraleBoost;
	}
	
	public List<Integer> getCoverAvailable() {
		List<Integer> availableCover = currentBattlefield.getCoverAvailable();
		
		return availableCover;
	}
}
